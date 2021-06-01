using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        public const int DATA_BUFFER_SIZE = 4096;
        public const int WAITING_ROOM_ID = 0;

        private static Client _instance = new Client();

        public int currentRoom;
        public IPAddress _ip;
        public int _port;
        public int id;
        public TCP tcp;

        private delegate void PacketHandler(Packet packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        public static Client Instance { get => _instance; }

        public Client()
        {
            tcp = new TCP();
            _ip = IPAddress.Loopback;
            _port = 26950;
            currentRoom = WAITING_ROOM_ID;
        }

        public void ConnectToServer()
        {
            InitializeClientData();
            tcp.Connect();
        }

        public class TCP
        {
            private TcpClient _socket;
            private NetworkStream _stream;
            private Packet _receivedData;
            private byte[] _receivedBuffer;

            public void Connect()
            {
                _socket = new TcpClient();
                _socket.ReceiveBufferSize = DATA_BUFFER_SIZE;
                _socket.SendBufferSize = DATA_BUFFER_SIZE;

                _receivedBuffer = new byte[DATA_BUFFER_SIZE];

                _socket.BeginConnect(_instance._ip, _instance._port, ConnectCallback, _socket);

            }

            private void ConnectCallback(IAsyncResult result)
            {
                _socket.EndConnect(result);

                if (!_socket.Connected)
                {
                    return;
                }

                Console.WriteLine($"Connected to server at {_socket.Client.RemoteEndPoint}...");

                _receivedData = new Packet();

                _stream = _socket.GetStream();
                _stream.BeginRead(_receivedBuffer, 0, DATA_BUFFER_SIZE, ReceiveCallback, null);
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

                    Console.WriteLine($"{byteLength} bytes received from server...");
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


                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();
                        packetHandlers[packetId](packet);

                    }
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

            public void SendData(Packet packet)
            {
                try
                {
                    if (_socket != null)
                    {
                        _stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine($"Error sending data to server via TCP: {e.ToString()}");
                }
            }
        }
        private void InitializeClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
                {
                    {(int)ServerPackets.welcome, ClientHandle.Welcome }
                };
        }

    }
}
