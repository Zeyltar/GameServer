using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ClientHandle
    {

        public static void Welcome(Packet packet)
        {
            string message = packet.ReadString();
            int id = packet.ReadInt();
            int roomIndex = packet.ReadInt();
            Console.WriteLine($"Message from server: {message} ({packet.Length()} bytes)");

            Client.Instance.id = id;
            Client.Instance.currentRoom = roomIndex;
            ClientSend.WelcomeReceived();
        }

        public static void RoomChange(Packet packet)
        {
            string message = packet.ReadString();
            int roomIndex = packet.ReadInt();

            Console.WriteLine($"Message from server: {message} ({packet.Length()} bytes)");

            Client.Instance.currentRoom = roomIndex;
            ClientSend.RoomChangedReceived();
        }
    }
}
