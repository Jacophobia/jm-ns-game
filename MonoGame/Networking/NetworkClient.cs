using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.DataStructures;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Networking;

public class NetworkClient : IDisposable
{
    private readonly UdpClient _udpClient;
    private IPEndPoint _remoteEndPoint;
    private Stopwatch _stopwatch;
    private readonly PriorityQueue<Controls> _controlQueue;
    private readonly PriorityQueue<IEnumerable<IRenderable>> _renderableQueue;
    private Thread _listeningThread;
    private bool _listening;
    private readonly int _port;
    private readonly bool _isHosting;

    private NetworkClient()
    {
        _stopwatch = Stopwatch.StartNew();
        _renderableQueue = new PriorityQueue<IEnumerable<IRenderable>>();
        _controlQueue = new PriorityQueue<Controls>();
    }

    public NetworkClient(int port, string ipAddress) : this()
    {
        _udpClient = new UdpClient();
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        _port = port;
        _isHosting = false;
    }

    public NetworkClient(int port) : this()
    {
        _udpClient = new UdpClient(port);
        _remoteEndPoint = null; // new IPEndPoint(IPAddress.Any, _port);
        _port = port;
        _isHosting = true;
    }

    public long TotalMilliseconds => _stopwatch.ElapsedMilliseconds;

    public void Connect()
    {
        if (_isHosting)
            return;
        
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
        _listeningThread = new Thread(ListenLoop);
        _listeningThread.Start();
    }
    
    private void ListenLoop()
    {
        while (_listening)
        {
            var receivedData = _udpClient.Receive(ref _remoteEndPoint);
            ProcessReceivedData(receivedData);
        }
    }

    public void SendControlData(Controls controlData)
    {
        var data = new[] { (byte)controlData };
        var packet = PrependHeaders(data, 0);
        _udpClient.Send(packet, packet.Length, _remoteEndPoint);
    }

    // ReSharper disable once LoopCanBeConvertedToQuery
    public Controls GetControlData()
    {
        var controls = Controls.None;
        foreach (var control in _controlQueue.GetAll())
            controls |= control;
        return controls;
    }

    public void SendRenderableData(IEnumerable<IRenderable> renderableData)
    {
        var data = SerializeRenderableData(renderableData);
        var packet = PrependHeaders(data, 1);
        if (_remoteEndPoint != null)
            _udpClient.Send(packet, packet.Length, _remoteEndPoint);
    }

    // ReSharper disable once LoopCanBeConvertedToQuery
    public IEnumerable<IEnumerable<IRenderable>> GetRenderableData()
    {
        foreach (var renderable in _renderableQueue.GetAll())
            yield return renderable;
    }

    private byte[] PrependHeaders(byte[] data, byte dataType)
    {
        var timestamp = _stopwatch.ElapsedMilliseconds;
        var timestampBytes = BitConverter.GetBytes(timestamp);
        var packet = new byte[timestampBytes.Length + 1 + data.Length];
        Buffer.BlockCopy(timestampBytes, 0, packet, 0, timestampBytes.Length);
        packet[timestampBytes.Length] = dataType;
        Buffer.BlockCopy(data, 0, packet, timestampBytes.Length + 1, data.Length);
        return packet;
    }

    private void ProcessReceivedData(byte[] data)
    {
        var timestamp = BitConverter.ToInt64(data, 0);
        if (data.Length <= 8)
        {
            _stopwatch.Restart();
            return;
        }
        
        var dataType = data[8];
        var payload = new byte[data.Length - 9];
        Buffer.BlockCopy(data, 9, payload, 0, data.Length - 9);

        switch (dataType)
        {
            case 0: // Control Data
                ProcessControlData(payload, timestamp);
                break;
            case 1: // Renderable Data
                ProcessRenderableData(payload, timestamp);
                break;
        }
    }

    private void ProcessControlData(IReadOnlyList<byte> payload, long timestamp)
    {
        var controlData = (Controls)payload[0];
        _controlQueue.Put(controlData, timestamp);
    }

    private void ProcessRenderableData(byte[] payload, long timestamp)
    {
        var renderableData = DeserializeRenderableData(payload);
        _renderableQueue.Put(renderableData, timestamp);
    }
    
    private static byte[] SerializeRenderableData(IEnumerable<IRenderable> renderables)
    {
        using var ms = new MemoryStream();
        var writer = new BinaryWriter(ms); // Using BinaryWriter for more efficient writes

        foreach (var renderable in renderables)
        {
            writer.Write(renderable.Texture.Name);
            writer.Write(renderable.Destination.X);
            writer.Write(renderable.Destination.Y);
            writer.Write(renderable.Destination.Width);
            writer.Write(renderable.Destination.Height);
            writer.Write(renderable.Source.X);
            writer.Write(renderable.Source.Y);
            writer.Write(renderable.Source.Width);
            writer.Write(renderable.Source.Height);
            writer.Write(renderable.Color.PackedValue); // Storing color as a single integer
            writer.Write(renderable.Rotation);
            writer.Write(renderable.Origin.X);
            writer.Write(renderable.Origin.Y);
            writer.Write((int)renderable.Effect);
            writer.Write(renderable.Depth);
        }

        return ms.ToArray();
    }

    private static IEnumerable<IRenderable> DeserializeRenderableData(byte[] data)
    {
        using var ms = new MemoryStream(data);
        var reader = new BinaryReader(ms); // Using BinaryReader for more efficient reads

        while (ms.Position < ms.Length)
        {
            var textureName = reader.ReadString();
            var destination = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            var source = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            var color = new Color(reader.ReadUInt32()); // Reading color as a single integer
            var rotation = reader.ReadSingle();
            var origin = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            var effect = (SpriteEffects)reader.ReadInt32();
            var depth = reader.ReadInt32();

            yield return new Renderable(textureName, destination, source, color, rotation, origin, effect, depth);
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