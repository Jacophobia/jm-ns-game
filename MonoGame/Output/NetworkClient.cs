using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.DataStructures;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Output;

public class NetworkClient : IDisposable, IControlListener
{
    private const int ReceiveTimeout = 2_000; // Timeout in milliseconds
    private const byte ControlDataType = 0;
    private const byte RenderableDataType = 1;
    private const int MaxBufferSize = 65536;
    
    private readonly UdpClient _udpClient;
    private IPEndPoint _remoteEndPoint; // TODO: Implement a system for more than two players and make it based on a player class
    private readonly Stopwatch _stopwatch;
    private readonly PriorityQueue<Controls> _controlQueue;
    private readonly PriorityQueue<IEnumerable<IRenderable>> _renderableQueue;
    private Thread _listeningThread;
    private bool _listening;
    private readonly byte[] _receiveBuffer;
    private readonly ObjectPool<Renderable> _renderablePool;

    private NetworkClient()
    {
        _stopwatch = Stopwatch.StartNew();
        _renderableQueue = new PriorityQueue<IEnumerable<IRenderable>>();
        _controlQueue = new PriorityQueue<Controls>();
        _receiveBuffer = new byte[MaxBufferSize]; // Adjust size as needed
        _renderablePool = new ObjectPool<Renderable>();
    }

    public NetworkClient(int port, string ipAddress) : this()
    {
        _udpClient = new UdpClient();
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
    }

    public NetworkClient(int port) : this()
    {
        _udpClient = new UdpClient(port);
        _remoteEndPoint = null;
    }

    public void Connect()
    {
        SendInitialPacket();
    }

    private void SendInitialPacket()
    {
        var initialPacket = BitConverter.GetBytes(_stopwatch.ElapsedMilliseconds);
        _udpClient.Send(initialPacket, initialPacket.Length, _remoteEndPoint);
        StartListening();
    }

    public void StartListening()
    {
        _listening = true;
        _udpClient.Client.ReceiveTimeout = ReceiveTimeout;
        _listeningThread = new Thread(_remoteEndPoint != null ? ListenLoop : HostListenLoop);
        _listeningThread.Start();
    }

    private void HostListenLoop()
    {
        while (_listening)
        {
            try
            {
                // Use the same buffer for each receive operation
                var receivedBytes = _udpClient.Receive(ref _remoteEndPoint);

                ProcessReceivedData(receivedBytes);
            }
            catch (SocketException)
            {
                // If the exception is due to the socket being closed, exit the loop
            }
        }
    }
    
    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
    private void ListenLoop()
    {
        while (_listening)
        {
            try
            {
                // Create an endpoint for any IP. This will be populated with the sender's info.
                EndPoint senderEndPoint = _remoteEndPoint;

                // Use the same buffer for each receive operation
                var receivedBytes = _udpClient.Client.ReceiveFrom(_receiveBuffer, ref senderEndPoint);
                
                // The senderEndPoint is now populated with the sender's address and port
                if (senderEndPoint is IPEndPoint senderIp)
                {
                    // You can now use senderIP.Address and senderIP.Port
                    _remoteEndPoint = senderIp;
                }

                ProcessReceivedData(new ArraySegment<byte>(_receiveBuffer, 0, receivedBytes));
            }
            catch (SocketException)
            {
                // If the exception is due to the socket being closed, exit the loop
            }
        }
    }

    public void SendControlData(Controls controlData)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        AddHeaders(ControlDataType, writer);
        
        writer.Write((byte)controlData);
        
        _udpClient.Send(ms.GetBuffer(), (int)ms.Length, _remoteEndPoint);
    }

    // ReSharper disable once LoopCanBeConvertedToQuery
    public Controls GetControls()
    {
        var controls = Controls.None;
        foreach (var control in _controlQueue.GetAll())
            controls |= control;
        return controls;
    }

    private MemoryStream _memoryStream;
    private BinaryWriter _binaryWriter;
    private bool _connected;

    public void PrepareRenderableBatch()
    {
        _connected = true;
        if (_remoteEndPoint == null)
        {
            _connected = false;
            return;
        }

        _memoryStream = new MemoryStream(MaxBufferSize);
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
        
        _udpClient.Send(_memoryStream.GetBuffer(), (int)_memoryStream.Length, _remoteEndPoint);
        _memoryStream?.Dispose();
        _binaryWriter?.Dispose();
    } 

    public IEnumerable<IRenderable> GetRenderableData()
    {  
        return _renderableQueue.Get();
    }

    private void AddHeaders(byte dataType, BinaryWriter writer)
    {
        writer.Write(_stopwatch.ElapsedMilliseconds);
        writer.Write(dataType);
    }

    private void ProcessReceivedData(ArraySegment<byte> segment)
    {
        var timestamp = BitConverter.ToInt64(segment.Array ?? Array.Empty<byte>(), segment.Offset);
        if (segment.Count <= 8)
        {
            _stopwatch.Restart();
            return;
        }

        Debug.Assert(segment.Array != null, "segment.Array != null");
        var dataType = segment.Array[segment.Offset + 8];
        var payload = new ArraySegment<byte>(segment.Array, segment.Offset + 9, segment.Count - (segment.Offset + 9));

        switch (dataType)
        {
            case ControlDataType:
                ProcessControlData(payload, timestamp);
                break;
            case RenderableDataType:
                ProcessRenderableData(payload, timestamp);
                break;
        }
    }

    private void ProcessControlData(ArraySegment<byte> payload, long timestamp)
    {
        try
        {
            Debug.Assert(payload.Array != null, "payload.Array != null");
            var controlData = (Controls)payload.Array[payload.Offset];
            _controlQueue.Put(controlData, timestamp);
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

    private void ProcessRenderableData(ArraySegment<byte> payload, long timestamp)
    {
            var renderableData = DeserializeRenderableData(payload);
            _renderableQueue.Put(renderableData, timestamp);
    }

    private IEnumerable<IRenderable> DeserializeRenderableData(ArraySegment<byte> data)
    {
        using var ms = new MemoryStream(data.Array ?? Array.Empty<byte>(), data.Offset, data.Count);
        var reader = new BinaryReader(ms); // Using BinaryReader for more efficient reads

        while (ms.Position < ms.Length)
        {
            var renderable = _renderablePool.Get();

            try
            {
                renderable.TextureName = reader.ReadUtf8String();
                renderable.Destination = reader.ReadRectangle();
                renderable.Source = reader.ReadRectangle();
                renderable.Color = reader.ReadColor();
                renderable.Rotation = reader.ReadSingle();
                renderable.Origin = reader.ReadVector2();
                renderable.Effect = (SpriteEffects)reader.ReadInt32();
                renderable.Depth = reader.ReadSingle();
            }
            catch (ContentLoadException e)
            {
                Debug.WriteLine(e.Message);
                yield break;
            }
            catch (SystemException e)
            {
                Debug.WriteLine(e.Message);
                yield break;
            }
            finally
            {
                _renderablePool.Return(renderable);
            }

            yield return renderable;
        }
    }

    public void Disconnect()
    {
        _listening = false;
        _listeningThread?.Join();
        _udpClient.Close();
    }

    public void Dispose()
    {
        Disconnect();
        _udpClient?.Dispose();
    }
}