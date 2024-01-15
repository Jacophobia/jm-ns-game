using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;
using MonoGame.Players;

namespace MonoGame.Networking;

public class Server : IControlSource, IDisposable
{
    private const int MaxBufferSize = 65_536;
    private const byte ControlDataType = 0;
    private const byte RenderableDataType = 1;
    private const byte InitialConnectionDataType = 3;
    private const byte WritableDataType = 4;
    private const int ReceiveTimeout = 2_000; // Timeout in milliseconds
    
    private readonly TcpListener _tcpListener;
    private readonly ConcurrentDictionary<Guid, UdpClient> _clients;
    private readonly ConcurrentDictionary<Guid, Controls> _playerControls;
    private readonly ConcurrentDictionary<Guid, IPEndPoint> _endPoints;
    private readonly ConcurrentQueue<External> _newPlayers;
    private readonly ConcurrentQueue<Guid> _disconnectedPlayers;
    private readonly List<Thread> _activeThreads;
    private readonly int _tcpPort;
    private bool _isRunning;
    private readonly Stopwatch _stopwatch;
    
    private readonly ConcurrentDictionary<Guid, MemoryStream> _renderableSendBuffers;
    private readonly ConcurrentDictionary<Guid, MemoryStream> _writableSendBuffers;

    public Server(int tcpPort)
    {
        _tcpPort = tcpPort;
        _tcpListener = new TcpListener(IPAddress.Any, tcpPort);
        _clients = new ConcurrentDictionary<Guid, UdpClient>();
        _playerControls = new ConcurrentDictionary<Guid, Controls>();
        _newPlayers = new ConcurrentQueue<External>();
        _disconnectedPlayers = new ConcurrentQueue<Guid>();
        _activeThreads = new List<Thread>();
        _isRunning = true;
        _stopwatch = Stopwatch.StartNew();
        _renderableSendBuffers = new ConcurrentDictionary<Guid, MemoryStream>();
        _writableSendBuffers = new ConcurrentDictionary<Guid, MemoryStream>();
        _endPoints = new ConcurrentDictionary<Guid, IPEndPoint>();
    }
    
    public Controls GetControls(IPlayer player)
    {
        if (!_playerControls.TryGetValue(player.Id, out var controls))
        {
            controls = Controls.None;
        }

        _playerControls[player.Id] = Controls.None;
        
        return controls;
    }

    private void Remove(Guid clientId)
    {
        _clients.TryRemove(clientId, out _);
        _endPoints.TryRemove(clientId, out _);
        _playerControls.TryRemove(clientId, out _);
        _disconnectedPlayers.Enqueue(clientId);
    }
    
    public bool TryGetNewPlayer(out External newPlayer)
    {
        return _newPlayers.TryDequeue(out newPlayer);
    }

    public bool TryGetDisconnectedPlayerId(out Guid playerId)
    {
        return _disconnectedPlayers.TryDequeue(out playerId);
    }
    
    private void AddHeaders(byte dataType, BinaryWriter writer)
    {
        writer.Write(_stopwatch.ElapsedMilliseconds);
        writer.Write(dataType);
    }
    
    public void PrepareRenderableBatch(IPlayer player)
    {
        if (!_clients.ContainsKey(player.Id))
        {
            return;
        }

        _renderableSendBuffers.TryAdd(player.Id, new MemoryStream(new byte[MaxBufferSize], 0, MaxBufferSize, true, true));
        _writableSendBuffers.TryAdd(player.Id, new MemoryStream(new byte[MaxBufferSize], 0, MaxBufferSize, true, true));

        _renderableSendBuffers[player.Id].Position = 0;
        _writableSendBuffers[player.Id].Position = 0;
    }

    public void Enqueue(IPlayer player, IRenderable renderable, Texture2D texture = null, 
        Rectangle? destination = null, Rectangle? source = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, SpriteEffects effect = SpriteEffects.None, 
        float? depth = null)
    {
        if (!_clients.ContainsKey(player.Id)|| _renderableSendBuffers[player.Id].Length - _renderableSendBuffers[player.Id].Position < 50) // TODO: Find out how big a renderable actually is
        {
            return;
        }

        var writer = new BinaryWriter(_renderableSendBuffers[player.Id]);

        if (_renderableSendBuffers[player.Id].Position == 0)
        {
            AddHeaders(RenderableDataType, writer);
        }
        
        writer.WriteString(texture?.Name ?? renderable.Texture.Name);
        writer.WriteRectangle(destination ?? renderable.Destination);
        writer.WriteRectangle(source ?? renderable.Source);
        writer.WriteColor(color ?? renderable.Color);
        writer.Write(rotation ?? renderable.Rotation);
        writer.WriteVector2(origin ?? renderable.Origin);
        writer.Write((int)(effect == SpriteEffects.None ? renderable.Effect : effect));
        writer.Write(depth ?? renderable.Depth);
    }

    public void Enqueue(IPlayer player, IWritable writable, SpriteFont font = null, 
        string text = null, Vector2? position = null, Color? color = null, 
        float? rotation = null, Vector2? origin = null, Vector2? scale = null, SpriteEffects effect = SpriteEffects.None, 
        float? depth = null)
    {
        if (!_clients.ContainsKey(player.Id)|| _writableSendBuffers[player.Id].Length - _writableSendBuffers[player.Id].Position < 50) // TODO: Find out how big a renderable actually is
        {
            return;
        }

        using var writer = new BinaryWriter(_writableSendBuffers[player.Id]);

        if (_writableSendBuffers[player.Id].Position == 0)
        {
            AddHeaders(WritableDataType, writer);
        }
        
        writer.WriteString(font?.Texture.Name ?? writable.Font.Texture.Name);
        writer.WriteString(text ?? writable.Text);
        writer.WriteVector2(position ?? writable.Position);
        writer.WriteColor(color ?? writable.TextColor);
        writer.WriteFloat(rotation ?? writable.Rotation);
        writer.WriteVector2(origin ?? writable.Origin);
        writer.WriteVector2(scale ?? writable.Scale);
        writer.Write((int)(effect == SpriteEffects.None ? writable.Effects : effect));
        writer.WriteFloat(depth ?? writable.LayerDepth);
    }

    public void SendRenderableBatch(IPlayer player)
    {
        if (!_clients.TryGetValue(player.Id, out var client))
        {
            return;
        }
        
        client.Send(_renderableSendBuffers[player.Id].GetBuffer(), (int)_renderableSendBuffers[player.Id].Position, _endPoints[player.Id]);
        client.Send(_writableSendBuffers[player.Id].GetBuffer(), (int)_writableSendBuffers[player.Id].Position, _endPoints[player.Id]);
    }

    public void Start()
    {
        _tcpListener.Start();
        Console.WriteLine($"TCP Server started on port {_tcpPort}.");
        var tcpThread = new Thread(HandleTcpConnections);
        tcpThread.Start();
        _activeThreads.Add(tcpThread);
    }

    public void Stop()
    {
        _isRunning = false;
        _tcpListener.Stop();
        
        foreach (var udpClient in _clients.Values)
        {
            udpClient.Close();
        }
        _clients.Clear();

        foreach (var thread in _activeThreads.Where(thread => thread.IsAlive))
        {
            thread.Join();
        }
        
        _activeThreads.Clear();
    }

    private void HandleTcpConnections()
    {
        while (_isRunning)
        {
            try
            {
                var tcpClient = _tcpListener.AcceptTcpClient();
                var udpThread = new Thread(() => HandleUdpCommunication(tcpClient));
                udpThread.Start();
                
                _activeThreads.Add(udpThread);
            }
            catch (SocketException)
            {
                // Ignore connection related errors and just retry
            }
        }
    }

    private void HandleUdpCommunication(TcpClient tcpClient)
    {
        var clientId = Guid.NewGuid();
        var udpClient = new UdpClient(0); // using 0 dynamically allocates a port
        
        if (udpClient.Client.LocalEndPoint is not IPEndPoint endPoint)
            return;
        
        var udpPort = endPoint.Port;

        var stream = tcpClient.GetStream();
        var portBytes = BitConverter.GetBytes(udpPort);
        stream.Write(portBytes, 0, portBytes.Length);

        _clients[clientId] = udpClient;
        
        Console.WriteLine($"Client {clientId} is connected on port {udpPort}");

        IPEndPoint remoteEndPoint = null;
        try
        {
            while (_isRunning)
            {
                var receivedBytes = udpClient.Receive(ref remoteEndPoint);

                _endPoints[clientId] = remoteEndPoint;

                ProcessReceivedData(clientId, receivedBytes);
            }
        }
        catch (SocketException)
        {
            // this exception occurs when the connected client closes 
            //  the connection
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            Debug.WriteLine($"An error occurred: {e.Message}");
        }
        finally
        {
            Remove(clientId);
            
            udpClient.Close();
            tcpClient.Close();
            
            Console.WriteLine($"Client {clientId} has disconnected");
            Console.WriteLine($"Port {udpPort} has been closed");
        }
    }

    private void ProcessReceivedData(Guid userId, ArraySegment<byte> data)
    {
        Debug.Assert(data.Array != null, "segment.Array should not be null");
            
        if (data.Count <= data.Offset + 8 || data.Array[data.Offset + 8] is InitialConnectionDataType)
        {
            return;
        }

        var dataType = data.Array[data.Offset + 8];
        var payload = new ArraySegment<byte>(data.Array, data.Offset + 9, data.Count - (data.Offset + 9));
        
        if (!_playerControls.ContainsKey(userId))
        {
            _playerControls[userId] = Controls.None;
            _newPlayers.Enqueue(new External(userId, new Camera(), this));
        }
            
        Debug.Assert(dataType == ControlDataType, "The wrong data type was sent");
        
        try
        {
            Debug.Assert(payload.Array != null, "data.Array != null");
            var controlData = (Controls)BitConverter.ToInt32(payload.Array, payload.Offset);
            _playerControls[userId] |= controlData;
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

    public void Dispose()
    {
        if (_isRunning)
            Stop();
        
        foreach (var client in _clients.Values)
        {
            client?.Dispose();
        }

        foreach (var buffer in _renderableSendBuffers.Values)
        {
            buffer?.Dispose();
        }

        foreach (var buffer in _writableSendBuffers.Values)
        {
            buffer?.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}
