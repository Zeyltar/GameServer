using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();

            foreach (Client client in SocketServer.Rooms[SocketServer.WAITING_ROOM_ID].Clients)
            {
                if (client.Id == fromClient)
                {
                    Console.WriteLine($"{client.tcp.Socket.Client.RemoteEndPoint} connected successfully and is now client {fromClient}");
                    if (fromClient != clientIdCheck)
                    {
                        Console.WriteLine($"Client ID is different from packet client ID");
                    }
                    return;
                }
            }
        }
    }
}
