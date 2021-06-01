using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerSend
    {
        private static void SendTCPData(int toRoom, int toClient, Packet packet)
        {
            if (toRoom < 0)
                return;

            packet.WriteLength();
            foreach (Client client in SocketServer.Rooms[toRoom].Clients)
            {
                if (client.Id == toClient)
                {
                    client.tcp.SendData(packet);
                    return;
                }
            }
        }

        private static void SendTCPDataToRoom(int toRoom, Packet packet)
        {
            if (toRoom < 0)
                return;

            packet.WriteLength();
            foreach (Client client in SocketServer.Rooms[toRoom].Clients)
            {
                client.tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToRoom(int toRoom, int exceptClient, Packet packet)
        {
            if (toRoom < 0)
                return;

            packet.WriteLength();
            foreach (Client client in SocketServer.Rooms[toRoom].Clients)
            {
                if (client.Id == exceptClient)
                    continue;
                client.tcp.SendData(packet);
            }
        }

        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();
            foreach (Room room in SocketServer.Rooms)
            {
                foreach (Client client in room.Clients)
                {
                    client.tcp.SendData(packet);
                }
            }
        }

        private static void SendTCPDataToAll(int exceptClient, Packet packet)
        {
            packet.WriteLength();
            foreach (Room room in SocketServer.Rooms)
            {
                foreach (Client client in room.Clients)
                {
                    if (client.Id == exceptClient)
                        continue;
                    client.tcp.SendData(packet);
                }
            }
        }
        #region Packets
        public static void Welcome(int toRoom, int toClient, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(message);
                packet.Write(toClient);
                packet.Write(toRoom);
                SendTCPData(toRoom, toClient, packet);
            }
        }

        public static void RoomChange(int toRoom, string message)
        {
            using (Packet packet = new Packet((int)ServerPackets.roomChange))
            {
                packet.Write(message);
                packet.Write(toRoom);
                SendTCPDataToRoom(toRoom, packet);
            }
        }
        #endregion
    }
}
