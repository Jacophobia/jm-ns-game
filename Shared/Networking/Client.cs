using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Shared.DataStructures;
using Shared.Extensions;
using Shared.Input;
using Shared.Output;

namespace Shared.Networking;

public class Client : IDisposable
{
    private const byte ControlDataType = 0;
    private const int MaxQueueSize = 15;
    private const int MaxBufferSize = 65_536;
    private const byte RenderableDataType = 1;
    
    private readonly string _serverIp;
    private readonly int _serverTcpPort;
    private UdpClient _udpClient;
    private readonly ConcurrentPriorityQueue<IEnumerable<Renderable>, long> _incomingRenderableQueue;
    private readonly byte[] _receiveBuffer;
    private bool _isConnected;
    private readonly Stopwatch _stopwatch;
    private readonly ObjectPool<Renderable> _renderablePool;

    public Client(string serverIp, int serverTcpPort)
    {
        _serverIp = serverIp;
        _serverTcpPort = serverTcpPort;
        _incomingRenderableQueue = new ConcurrentPriorityQueue<IEnumerable<Renderable>, long>();
        _receiveBuffer = new byte[MaxBufferSize];
        _isConnected = false;
        _stopwatch = Stopwatch.StartNew();
        _renderablePool = new ObjectPool<Renderable>();
    }

    public void Connect()
    {
        using var tcpClient = new TcpClient(_serverIp, _serverTcpPort);
        
        var stream = tcpClient.GetStream();
        var buffer = new byte[4];
        var bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead != buffer.Length)
            throw new Exception("Error reading from the server.");

        var udpPort = BitConverter.ToInt32(buffer, 0);
        _udpClient = new UdpClient();
        _udpClient.Connect(_serverIp, udpPort);

        _isConnected = true;

        Console.WriteLine("Connected to server's UDP port: " + udpPort);

        var receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }
    
    private void AddHeaders(byte dataType, BinaryWriter writer)
    {
        writer.Write(_stopwatch.ElapsedMilliseconds);
        writer.Write(dataType);
    }

    public void Send(Controls data)
    {
        if (!_isConnected) 
            return;
        
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);
        
        AddHeaders(ControlDataType, writer);
        
        writer.Write((int)data);

        _udpClient.Send(ms.GetBuffer(), (int)ms.Position);
    }
    
    private IEnumerable<Renderable> DeserializeRenderableData(ArraySegment<byte> data)
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

            yield return renderable;
            
            _renderablePool.Return(renderable);
        }
    }

    private void ReceiveData()
    {
        while (_isConnected)
        {
            try
            {
                // Use the same buffer for each receive operation
                var receivedBytes = _udpClient.Client.Receive(_receiveBuffer);
                
                if (receivedBytes <= 9)
                    continue;

                var data = new ArraySegment<byte>(_receiveBuffer, 0, receivedBytes);

                Debug.Assert(data.Array != null, "segment.Array should not be null");

                var timestamp = BitConverter.ToInt64(data.Array ?? Array.Empty<byte>(), data.Offset);
                var dataType = data.Array[data.Offset + 8];
                var payload = new ArraySegment<byte>(data.Array, data.Offset + 9, data.Count - (data.Offset + 9));

                switch (dataType)
                {
                    case RenderableDataType:
                        if (_incomingRenderableQueue.Count >= MaxQueueSize)
                        {
                            _incomingRenderableQueue.TryDequeue(out _, out _); // Remove oldest data if queue is full
                        }

                        _incomingRenderableQueue.Enqueue(DeserializeRenderableData(payload), timestamp);
                        break;
                }
            }
            catch (SocketException)
            {
                // do nothing when the other client closes the connection
            }
        }
    }

    public IEnumerable<Renderable> DequeueRenderable()
    {
        return _incomingRenderableQueue.Dequeue();
    }

    public void Disconnect()
    {
        _isConnected = false;
        _udpClient.Close();
    }

    public void Dispose()
    {
        _udpClient?.Dispose();

        GC.SuppressFinalize(this);
    }
}
