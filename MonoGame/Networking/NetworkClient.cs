using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;
using MonoGame.Output;

namespace MonoGame.Networking;

internal class NetworkClient
{
    private readonly UdpClient _udpClient;
    private readonly IPEndPoint _remoteEndPoint;
    private readonly Stopwatch _stopwatch;
    private readonly SortedDictionary<long, Controls> _controlQueue;
    private readonly SortedDictionary<long, IRenderable> _renderableQueue;

    internal NetworkClient(string ipAddress, int port)
    {
        _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        _udpClient = new UdpClient();
        _stopwatch = Stopwatch.StartNew();
        _controlQueue = new SortedDictionary<long, Controls>();
        _renderableQueue = new SortedDictionary<long, IRenderable>();
    }

    internal void Connect()
    {
        SendInitialPacket();
    }

    private void SendInitialPacket()
    {
        var initialPacket = BitConverter.GetBytes(_stopwatch.ElapsedMilliseconds);
        _udpClient.Send(initialPacket, initialPacket.Length, _remoteEndPoint);
        StartListening();
    }

    internal void StartListening()
    {
        _udpClient.BeginReceive(ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        IPEndPoint remoteEp = null;
        var receivedData = _udpClient.EndReceive(ar, ref remoteEp);

        ProcessReceivedData(receivedData);

        _udpClient.BeginReceive(ReceiveCallback, null);
    }

    internal void SendControlData(Controls controlData)
    {
        var data = new[] { (byte)controlData };
        SendData(data, 0); // Assuming '0' is the data type for Controls
    }

    internal void SendRenderableData(IRenderable renderableData)
    {
        var data = SerializeRenderableData(renderableData);
        SendData(data, 1); // Assuming '1' is the data type for Renderables
    }


    private void SendData(byte[] data, byte dataType)
    {
        var packet = PrependHeaders(data, dataType);
        _udpClient.Send(packet, packet.Length, _remoteEndPoint);
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
        lock (_controlQueue)
        {
            _controlQueue.Add(timestamp, controlData);
        }
    }

    private void ProcessRenderableData(byte[] payload, long timestamp)
    {
        var renderableData = DeserializeRenderableData(payload);
        lock (_renderableQueue)
        {
            _renderableQueue.Add(timestamp, renderableData);
        }
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


    internal void Disconnect()
    {
        _udpClient.Close();
    }
}