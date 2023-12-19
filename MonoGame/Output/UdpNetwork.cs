using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MonoGame.Interfaces;

namespace MonoGame.Output
{
    public abstract class UdpNetwork : IDisposable
    {
        protected const int MaxBufferSize = 65_536;
        protected const byte ControlDataType = 0;
        protected const byte RenderableDataType = 1;
        protected const byte InitialConnectionDataType = 3;
        protected const byte WritableDataType = 4;
        private const int ReceiveTimeout = 2_000; // Timeout in milliseconds
        protected readonly UdpClient Client;
        private readonly Thread _listeningThread;
        private readonly Stopwatch _stopwatch;
        private bool _listening;

        protected UdpNetwork(UdpClient client)
        {
            Client = client;
            _listeningThread = new Thread(ListenLoop);
            _stopwatch = Stopwatch.StartNew();
        }

        protected void AddHeaders(byte dataType, BinaryWriter writer)
        {
            writer.Write(_stopwatch.ElapsedMilliseconds);
            writer.Write(dataType);
        }
        
        protected void Send(byte dataType, IEnumerable<ISerializable> serializables, IPEndPoint endPoint)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
        
            AddHeaders(dataType, writer);

            foreach (var serializable in serializables)
            {
                serializable.Serialize(writer);
            }
            
            Send(ms, endPoint);
        }
        
        protected void Send(byte dataType, ISerializable serializable, IPEndPoint endPoint)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
        
            AddHeaders(dataType, writer);

            serializable.Serialize(writer);
            
            Send(ms, endPoint);
        }
        
        protected void Send(byte dataType, ArraySegment<byte> data, IPEndPoint endPoint)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
        
            AddHeaders(dataType, writer);

            writer.Write(data);
            
            Send(ms, endPoint);
        }
        
        protected void Send(byte dataType, byte data, IPEndPoint endPoint)
        {
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
        
            AddHeaders(dataType, writer);

            writer.Write(data);
            
            Send(ms, endPoint);
        }

        protected void Send(MemoryStream ms, IPEndPoint endPoint)
        {
            Client.Send(ms.GetBuffer(), (int)ms.Position, endPoint);
        }

        private void ProcessReceivedData(IPEndPoint endPoint, ArraySegment<byte> data)
        {
            Debug.Assert(data.Array != null, "segment.Array should not be null");
            
            if (data.Count <= data.Offset + 8 || data.Array[data.Offset + 8] is InitialConnectionDataType)
            {
                return;
            }

            
            var timestamp = BitConverter.ToInt64(data.Array ?? Array.Empty<byte>(), data.Offset);
            var dataType = data.Array[data.Offset + 8];
            var payload = new ArraySegment<byte>(data.Array, data.Offset + 9, data.Count - (data.Offset + 9));

            ProcessData(endPoint, dataType, timestamp, payload);
        }

        protected abstract void ProcessData(IPEndPoint endPoint, byte dataType, long timestamp, ArraySegment<byte> data);

        public void Start()
        {
            OnStart();
            StartListening();
        }

        protected abstract void OnStart();

        private void StartListening()
        {
            _listening = true;
            Client.Client.ReceiveTimeout = ReceiveTimeout;
            _listeningThread.Start();
        }

        private void ListenLoop()
        {
            while (_listening)
            {
                try
                {
                    if (Listen(out var endpoint, out var data))
                        ProcessReceivedData(endpoint, data);
                }
                catch (SocketException)
                {
                    // If the exception is due to the socket being closed, exit the loop
                }
            }
        }

        protected abstract bool Listen(out IPEndPoint endPoint, out ArraySegment<byte> data);

        public void Disconnect()
        {
            _listening = false;
            _listeningThread.Join();
        }

        public void Dispose()
        {
            Client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}