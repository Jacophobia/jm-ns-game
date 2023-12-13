namespace MonoGame.Networking;

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Client
{
    private readonly string _serverIp;
    private readonly int _serverTcpPort;
    private UdpClient _udpClient;
    private readonly ConcurrentQueue<string> _incomingDataQueue;
    private bool _isConnected;
    private const int MaxQueueSize = 15;

    public Client(string serverIp, int serverTcpPort)
    {
        _serverIp = serverIp;
        _serverTcpPort = serverTcpPort;
        _incomingDataQueue = new ConcurrentQueue<string>();
        _isConnected = false;
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

    public void SendData(string data)
    {
        if (!_isConnected) 
            return;
        
        var bytesToSend = System.Text.Encoding.UTF8.GetBytes(data);
        _udpClient.Send(bytesToSend, bytesToSend.Length);
    }

    private void ReceiveData()
    {
        while (_isConnected)
        {
            var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            var receivedBytes = _udpClient.Receive(ref remoteEndPoint);
            var receivedData = System.Text.Encoding.UTF8.GetString(receivedBytes);

            lock (_incomingDataQueue)
            {
                if (_incomingDataQueue.Count >= MaxQueueSize)
                {
                    _incomingDataQueue.TryDequeue(out _); // Remove oldest data if queue is full
                }
                _incomingDataQueue.Enqueue(receivedData);
            }
        }
    }

    public bool TryDequeueData(out string data)
    {
        lock (_incomingDataQueue)
        {
            return _incomingDataQueue.TryDequeue(out data);
        }
    }

    public void Disconnect()
    {
        _isConnected = false;
        _udpClient.Close();
    }
}
