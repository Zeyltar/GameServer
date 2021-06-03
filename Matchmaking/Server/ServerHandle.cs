using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, int fromRoom, Packet packet)
        {
            int clientIdCheck = packet.ReadInt();


            foreach (Client client in Room.Rooms[fromRoom].Clients)
            {
                if (client.Id == fromClient)
                {
                    if (fromClient != clientIdCheck)
                    {
                        Console.WriteLine($"Client ID is different from packet client ID");
                    }
                    else
                    {
                        Console.WriteLine($"{client.tcp.Socket.Client.RemoteEndPoint} connected successfully and is now client {fromClient}");
                        client.isValidated = true;
                    }
                    return;
                }
            }
        }

        public static void RoomChangedReceived(int fromClient, int fromRoom, Packet packet)
        {
            int cltCrtRoomCheck = packet.ReadInt();

            foreach (Client client in Room.Rooms[fromRoom].Clients)
            {
                if (client.Id == fromClient)
                {
                    if (fromRoom != cltCrtRoomCheck)
                    {
                        Console.WriteLine($"Client {fromClient} in room {fromRoom} has assumed the wrong room ({cltCrtRoomCheck})");
                    }
                    else
                    {
                        Console.WriteLine($"Client {fromClient} successfully joined room {fromRoom}");
                    }
                    return;
                }
            }

        }
    }
}
