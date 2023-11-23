﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
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
    private readonly PriorityQueue<IRenderable> _renderableQueue;
    private Thread _listeningThread;
    private bool _listening;
    private readonly int _port;
    private readonly bool _isHosting;

    private NetworkClient()
    {
        _stopwatch = Stopwatch.StartNew();
        _renderableQueue = new PriorityQueue<IRenderable>();
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
            if (true)
            {
                var receivedData = _udpClient.Receive(ref _remoteEndPoint);
                ProcessReceivedData(receivedData);
            }
            else
            {
                // Sleep for a short period to avoid spinning.
                Thread.Sleep(10);
            }
        }
    }

    public void SendControlData(Controls controlData)
    {
        var data = new[] { (byte)controlData };
        var packet = PrependHeaders(data, 0);
        _udpClient.Send(packet, packet.Length, _remoteEndPoint);
    }

    // ReSharper disable once LoopCanBeConvertedToQuery
    public Controls GetControlData(long currentTime)
    {
        var controls = Controls.None;
        foreach (var control in _controlQueue.GetAll())
            controls |= control;
        return controls;
    }

    public void SendRenderableData(IRenderable renderableData)
    {
        var data = SerializeRenderableData(renderableData);
        var packet = PrependHeaders(data, 1);
        if (_remoteEndPoint != null)
            _udpClient.Send(packet, packet.Length, _remoteEndPoint);
    }

    // ReSharper disable once LoopCanBeConvertedToQuery
    public IEnumerable<IRenderable> GetRenderableData(long currentTime)
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
            // TODO: Sync up stopwatches
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

    private static byte[] SerializeRenderableData(IRenderable data)
    {
        using var ms = new MemoryStream();

        // Serialize Texture.Name (as string)
        ms.WriteString(data.Texture.Name);

        // Serialize Rectangle (as four integers)
        ms.WriteRectangle(data.Destination);
        ms.WriteRectangle(data.Source);

        // Serialize Color (as four bytes)
        ms.WriteColor(data.Color);

        // Serialize Rotation (as float)
        ms.WriteFloat(data.Rotation);

        // Serialize Origin (as two floats)
        ms.WriteVector2(data.Origin);

        // Serialize Effect (as integer)
        ms.WriteInt((int)data.Effect);

        // Serialize Depth (as integer)
        ms.WriteInt(data.Depth);

        return ms.ToArray();
    }

    private static IRenderable DeserializeRenderableData(byte[] data)
    {
        using var ms = new MemoryStream(data);

        // Deserialize Texture.Name
        var textureName = ms.ReadString();

        // Deserialize Rectangle
        var destination = ms.ReadRectangle();
        var source = ms.ReadRectangle();

        // Deserialize Color
        var color = ms.ReadColor();

        // Deserialize Rotation
        var rotation = ms.ReadFloat();

        // Deserialize Origin
        var origin = ms.ReadVector2();

        // Deserialize Effect
        var effect = (SpriteEffects)ms.ReadInt();

        // Deserialize Depth
        var depth = ms.ReadInt();

        return new Renderable(textureName, destination, source, color, rotation, origin, effect, depth);
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