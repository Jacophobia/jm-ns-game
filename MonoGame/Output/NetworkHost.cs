using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Players;

namespace MonoGame.Output;

public class NetworkHost : UdpNetwork, IControlSource
{
    private readonly ConcurrentDictionary<IPEndPoint, Controls> _connectedPlayers;
    private readonly ConcurrentQueue<External> _newPlayers;
    private readonly byte[] _sendBuffer;
        
    public NetworkHost(int port) : base(new UdpClient(port))
    {
        _connectedPlayers = new ConcurrentDictionary<IPEndPoint, Controls>();
        _newPlayers = new ConcurrentQueue<External>();
        _sendBuffer = new byte[MaxBufferSize];
    }
        
    // ReSharper disable once LoopCanBeConvertedToQuery
    public Controls GetControls(IPlayer player)
    {
        if (player.Id.Key is IPEndPoint endPoint)
        {
            return _connectedPlayers[endPoint];
        }

        return Controls.None;
    }

    public bool TryGetNewPlayer(out External newPlayer)
    {
        return _newPlayers.TryDequeue(out newPlayer);
    }
        
    private MemoryStream _memoryStream;
    private BinaryWriter _binaryWriter;
    private bool _connected;

    public void PrepareRenderableBatch()
    {
        _connected = true;
        if (!_connectedPlayers.Any())
        {
            _connected = false;
            return;
        }
        _memoryStream = new MemoryStream(_sendBuffer);
        _binaryWriter = new BinaryWriter(_memoryStream);
        
        AddHeaders(RenderableDataType, _binaryWriter);
    }

    public void Enqueue(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        float? depth = null)
    {
        if (!_connected)
            return;
        
        _binaryWriter.WriteString(texture?.Name ?? renderable.Texture.Name);
        _binaryWriter.WriteRectangle(destination ?? renderable.Destination);
        _binaryWriter.WriteRectangle(source ?? renderable.Source);
        _binaryWriter.WriteColor(color ?? renderable.Color);
        _binaryWriter.Write(rotation ?? renderable.Rotation);
        _binaryWriter.WriteVector2(origin ?? renderable.Origin);
        _binaryWriter.Write((int)(effect == SpriteEffects.None ? renderable.Effect : effect));
        _binaryWriter.Write(depth ?? renderable.Depth);
    }

    public void SendRenderableBatch()
    {
        if (!_connected)
        {
            return;
        }

        foreach (var player in _connectedPlayers)
        {
            Send(_memoryStream, player.Key);
        }
            
        _memoryStream?.Dispose();
        _binaryWriter?.Dispose();
    }

    protected override void ProcessData(IPEndPoint endPoint, byte dataType, long timestamp, ArraySegment<byte> data)
    {
        if (!_connectedPlayers.ContainsKey(endPoint))
        {
            _connectedPlayers[endPoint] = Controls.None;
            _newPlayers.Enqueue(new External(endPoint, new Camera(), this));
        }
            
        Debug.Assert(dataType == ControlDataType, "The wrong data type was sent");
        try
        {
            Debug.Assert(data.Array != null, "data.Array != null");
            var controlData = (Controls)data.Array[data.Offset];
            _connectedPlayers[endPoint] |= controlData;
        }
        catch (ContentLoadException e)
        {
            Debug.WriteLine(e.Message);
        }
        catch (SystemException e)
        {
            Debug.WriteLine(e.Message);
        }
    }

    protected override bool Listen(out IPEndPoint endPoint, out ArraySegment<byte> data)
    {
        endPoint = null;
        data = Client.Receive(ref endPoint);

        return data != null && data.Count > 0;
    }
}