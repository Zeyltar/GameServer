using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public const int DATA_BUFFER_SIZE = 4096;

        private readonly int _id;
        private int _currentRoom;
        public TCP tcp;
        public bool isValidated;

        public Client(int id)
        {
            _id = id;
            tcp = new TCP(id);
            _currentRoom = Constants.WAITING_ROOM_ID;
            isValidated = false;
        }
        public int Id { get => _id; }
        public int CurrentRoom
        {
            get => _currentRoom;
            set
            {
                _currentRoom = value;
                tcp.CurrentRoom = value;
            }
        }

        public class TCP 
        {
            private readonly int _id;
            private int _currentRoom;
            private TcpClient _socket;
            private NetworkStream _stream;
            private Packet _receivedData;
            private byte[] _receivedBuffer;

            public TCP(int id)
            {
                _id = id;
                _currentRoom = 0;
            }

            public int CurrentRoom { get => _currentRoom; set => _currentRoom = value; }
            public TcpClient Socket { get => _socket;}

            public void Connect(TcpClient socket)
            {
                _socket = socket;
                _socket.ReceiveBufferSize = DATA_BUFFER_SIZE;
                _socket.SendBufferSize = DATA_BUFFER_SIZE;

                _stream = _socket.GetStream();

                _receivedData = new Packet();
                _receivedBuffer = new byte[DATA_BUFFER_SIZE];

                _stream.BeginRead(_receivedBuffer, 0, DATA_BUFFER_SIZE, ReceiveCallback, null);

                ServerSend.Welcome(_currentRoom, _id, $"Welcome client {_id}, you joined the waiting room.\nThere is currently {Room.Rooms[_currentRoom].Clients.Count} client(s) waiting");
            }

            public void SendData(Packet packet)
            {
                try
                {
                    if  (_socket != null)
                    {
                        _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                        Console.WriteLine($"{packet.Length()} bytes sent to {_socket.Client.RemoteEndPoint}...");
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine($"Error sending data to client {_id} via TCP: {e}");
                }
            }

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    int byteLength = _stream.EndRead(result);
                    if (byteLength <= 0)
                    {
                        // TODO déconnexion
                        return;
                    }

                    byte[] data = new byte[byteLength];
                    Array.Copy(_receivedBuffer, data, byteLength);

                    Console.WriteLine($"{byteLength} bytes received from {_socket.Client.RemoteEndPoint}...");
                    _receivedData.Reset(HandleData(data));
                    
                    _stream.BeginRead(_receivedBuffer, 0, DATA_BUFFER_SIZE, ReceiveCallback, null);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    // TODO déconnexion
                }
            }

            private bool HandleData(byte[] data)
            {
                int packetLength = 0;
                _receivedData.SetBytes(data);

                if (_receivedData.UnreadLength() >= 4)
                {
                    packetLength = _receivedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }

                while (packetLength > 0 && packetLength <= _receivedData.UnreadLength())
                {
                    byte[] packetBytes = _receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() => 
                    {
                        using (Packet packet = new Packet(packetBytes))
                        {
                            int packetId = packet.ReadInt();
                            SocketServer.packetHandlers[packetId](_id, _currentRoom, packet);
                        }
                    });
                    
                    packetLength = 0;
                    if (_receivedData.UnreadLength() >= 4)
                    {
                        packetLength = _receivedData.ReadInt();
                        if (packetLength <= 0)
                        {
                            return true;
                        }
                    }
                }

                if (packetLength <= 1)
                {
                    return true;
                }

                return false;
            }

        }
    }
}
