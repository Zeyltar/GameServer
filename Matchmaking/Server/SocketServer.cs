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
        private int _nClients;
        private int _port;
        private IPAddress _ip;
        private TcpListener _listener;
        public delegate void PacketHandler(int fromClient, int fromRoom, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        public SocketServer(IPAddress ip, int port)
        {
            // Room 0 will be the waiting room for clients
            Room.Rooms.Add(new Room());
            _nClients = 0;
            _ip = ip;
            _port = port;
            _listener = new TcpListener(_ip, _port);
        }
        public static List<Room> Rooms { get => Room.Rooms; }

        public void Listen()
        {
            InitializeServerData();
            _listener.Start();
            Console.WriteLine($"Server started on port {_port}.");
            _listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

        }        

        private void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = _listener.EndAcceptTcpClient(result);
            _listener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            _nClients++;

            Console.WriteLine($"Connection from {client.Client.RemoteEndPoint}...");

            Client waitingClient = new Client(_nClients);

            Room.Rooms[Constants.WAITING_ROOM_ID].Clients.Add(waitingClient);
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
