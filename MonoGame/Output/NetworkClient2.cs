using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.DataStructures;
using MonoGame.Extensions;
using MonoGame.Input;
using MonoGame.Interfaces;

namespace MonoGame.Output
{
    public class NetworkClient2 : UdpNetwork
    {
        private readonly byte[] _receiveBuffer;
        private readonly byte[] _sendBuffer;
        private IPEndPoint _remoteEndPoint;
        private readonly PriorityQueue<IEnumerable<IRenderable>, long> _renderableQueue;
        private readonly ObjectPool<Renderable> _renderablePool;


        public NetworkClient2(int port, string ipAddress) : base(new UdpClient())
        {
            _receiveBuffer = new byte[MaxBufferSize];
            _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            
        }
        
        public void Connect()
        {
            SendInitialPacket();
        }
        
        private void SendInitialPacket()
        {
            Send(0, ArraySegment<ISerializable>.Empty, _remoteEndPoint);
            StartListening();
        }
        
        public IEnumerable<IRenderable> GetRenderableData()
        {  
            return _renderableQueue.Dequeue();
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

        public void SendControlData(Controls controlData)
        {
            Send(ControlDataType, (byte)controlData, _remoteEndPoint);
        }

        protected override void ProcessData(IPEndPoint endPoint, byte dataType, long timestamp, ArraySegment<byte> data)
        {
            Debug.Assert(dataType == RenderableDataType, "The wrong data type was sent");
            var renderableData = DeserializeRenderableData(data);
            _renderableQueue.Enqueue(renderableData, timestamp);
        }

        protected override bool Listen(out IPEndPoint endPoint, out ArraySegment<byte> data)
        {
            EndPoint senderEndPoint = endPoint = _remoteEndPoint;
                        
            // Use the same buffer for each receive operation
            var receivedBytes = Client.Client.ReceiveFrom(_receiveBuffer, ref senderEndPoint);

            data = new ArraySegment<byte>(_receiveBuffer, 0, receivedBytes);
                    
            // The senderEndPoint is now populated with the sender's address and port
            if (senderEndPoint is not IPEndPoint senderIp) 
                return false;
            
            endPoint = senderIp;
            return true;

        }
    }
}
