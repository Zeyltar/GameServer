using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class SocketServer
    {
        public const int WAITING_ROOM_ID = 0;
        public const int ROOM_MINIMUM_CLIENTS = 2;
        private static List<Room> _rooms = new List<Room>();
        private int _nClients;

        private int _port;
        private IPAddress _ip;
        private TcpListener _listener;
        public delegate void PacketHandler(int fromClient, int fromRoom, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public SocketServer(IPAddress ip, int port)
        {
            // Room 0 will be the waiting room for clients
            _rooms.Add(new Room());
            _nClients = 0;
            _ip = ip;
            _port = port;
            _listener = new TcpListener(_ip, _port);
        }
        public static List<Room> Rooms { get => _rooms; }

        public void Listen()
        {
            InitializeServerData();
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}.");
            _listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Thread createRoomThread = new Thread(new ThreadStart(CreateRoom));
            createRoomThread.Start();
        }

        private void CreateRoom()
        {
            while (true)
            {
                if (_rooms[WAITING_ROOM_ID].Clients.Count >= ROOM_MINIMUM_CLIENTS)
                {
                    Room room = new Room(_rooms[WAITING_ROOM_ID].Clients.GetRange(0, 2));
                    _rooms[WAITING_ROOM_ID].Clients.RemoveRange(0, 2);
                    _rooms.Add(room);
                    Console.WriteLine("New room created");
                    int index = _rooms.LastIndexOf(room);

                    foreach (Client client in _rooms[index].Clients)
                    {
                        client.CurrentRoom = index;
                    }

                    ServerSend.RoomChange(index, $"Your room has been changed. You are now in room {index}");
                }
                Thread.Sleep(1000);
            }
            
        }

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = _listener.EndAcceptTcpClient(result);
            _listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            _nClients++;

            Console.WriteLine($"Connection from {client.Client.RemoteEndPoint}...");

            Client waitingClient = new Client(_nClients);

            _rooms[WAITING_ROOM_ID].Clients.Add(waitingClient);
            waitingClient.tcp.Connect(client);
        }

        private void InitializeServerData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.roomChangedReceived, ServerHandle.RoomChangedReceived }
            };
        }
    }
}
