using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace MonoGame.Networking;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server
{
    private readonly TcpListener _tcpListener;
    private readonly ConcurrentDictionary<Guid, UdpClient> _clientUdpMap;
    private readonly List<Thread> _activeThreads;
    private readonly int _tcpPort;
    private bool _isRunning;

    protected Server(int tcpPort)
    {
        _tcpPort = tcpPort;
        _tcpListener = new TcpListener(IPAddress.Any, tcpPort);
        _clientUdpMap = new ConcurrentDictionary<Guid, UdpClient>();
        _activeThreads = new List<Thread>();
        _isRunning = true;
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
        
        foreach (var udpClient in _clientUdpMap.Values)
        {
            udpClient.Close();
        }
        _clientUdpMap.Clear();

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
                // Handle the exception as needed
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

        _clientUdpMap[clientId] = udpClient;

        IPEndPoint remoteEndPoint = null;
        try
        {
            while (_isRunning)
            {
                var receivedBytes = udpClient.Receive(ref remoteEndPoint);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
            Debug.WriteLine($"An error occurred: {e.Message}");
        }
        finally
        {
            _clientUdpMap.Remove(clientId, out _);
            
            udpClient.Close();
            tcpClient.Close();
        }
    }

    protected void ProcessReceivedData(Guid userId, ArraySegment<byte> receivedData)
    {
        
    }
}
