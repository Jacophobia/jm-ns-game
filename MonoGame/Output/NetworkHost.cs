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
    private readonly byte[] _renderableSendBuffer;
    private readonly byte[] _writableSendBuffer;
        
    public NetworkHost(int port) : base(new UdpClient(port))
    {
        _connectedPlayers = new ConcurrentDictionary<IPEndPoint, Controls>();
        _newPlayers = new ConcurrentQueue<External>();
        _renderableSendBuffer = new byte[MaxBufferSize];
        _writableSendBuffer = new byte[MaxBufferSize];
    }
        
    public Controls GetControls(IPlayer player)
    {
        if (player.Id.Key is not IPEndPoint endPoint) 
            return Controls.None;
        
        var controls = _connectedPlayers[endPoint];
        _connectedPlayers[endPoint] = Controls.None;
        return controls;

    }

    public bool TryGetNewPlayer(out External newPlayer)
    {
        return _newPlayers.TryDequeue(out newPlayer);
    }
        
    private MemoryStream _renderableMemoryStream;
    private BinaryWriter _renderableBinaryWriter;
    private MemoryStream _writableMemoryStream;
    private BinaryWriter _writableBinaryWriter;
    private bool _connected;

    public void PrepareRenderableBatch()
    {
        _connected = true;
        if (!_connectedPlayers.Any())
        {
            _connected = false;
            return;
        }
        _renderableMemoryStream = new MemoryStream(_renderableSendBuffer, 0, _renderableSendBuffer.Length, true, true);
        _renderableBinaryWriter = new BinaryWriter(_renderableMemoryStream);
        
        _writableMemoryStream = new MemoryStream(_renderableSendBuffer, 0, _renderableSendBuffer.Length, true, true);
        _writableBinaryWriter = new BinaryWriter(_writableMemoryStream);
        
        AddHeaders(RenderableDataType, _renderableBinaryWriter);
        AddHeaders(WritableDataType, _writableBinaryWriter);
    }

    public void Enqueue(IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        float? depth = null)
    {
        if (!_connected || _renderableMemoryStream.Length - _renderableMemoryStream.Position < 50)
            return;
        
        _renderableBinaryWriter.WriteString(texture?.Name ?? renderable.Texture.Name);
        _renderableBinaryWriter.WriteRectangle(destination ?? renderable.Destination);
        _renderableBinaryWriter.WriteRectangle(source ?? renderable.Source);
        _renderableBinaryWriter.WriteColor(color ?? renderable.Color);
        _renderableBinaryWriter.Write(rotation ?? renderable.Rotation);
        _renderableBinaryWriter.WriteVector2(origin ?? renderable.Origin);
        _renderableBinaryWriter.Write((int)(effect == SpriteEffects.None ? renderable.Effect : effect));
        _renderableBinaryWriter.Write(depth ?? renderable.Depth);
    }

    public void SendRenderableBatch()
    {
        if (!_connected)
        {
            return;
        }

        foreach (var player in _connectedPlayers)
        {
            Send(_renderableMemoryStream, player.Key);
        }
            
        _renderableMemoryStream?.Dispose();
        _renderableBinaryWriter?.Dispose();
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

    protected override void OnStart()
    {
        
    }

    protected override bool Listen(out IPEndPoint endPoint, out ArraySegment<byte> data)
    {
        endPoint = null;
        data = Client.Receive(ref endPoint);

        return data != null && data.Count > 0;
    }
}