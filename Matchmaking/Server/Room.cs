using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Room
    {

        private static List<Room> _rooms = new List<Room>();

        private List<Client> _clients;

        public Room(IEnumerable<Client> clients = null)
        {
            _clients = new List<Client>();
            if (clients != null)
            {
                _clients.AddRange(clients);
                
            }
        }

        public static void CreateRoom()
        {
            if (_rooms[Constants.WAITING_ROOM_ID].Clients.Count >= Constants.ROOM_MINIMUM_CLIENTS)
            {
                Room room = new Room(_rooms[Constants.WAITING_ROOM_ID].Clients.GetRange(0, 2));
                _rooms[Constants.WAITING_ROOM_ID].Clients.RemoveRange(0, 2);
                _rooms.Add(room);
                Console.WriteLine("New room created");
                int index = _rooms.LastIndexOf(room);

                foreach (Client client in _rooms[index].Clients)
                {
                    client.CurrentRoom = index;
                }

                ServerSend.RoomChange(index, $"Your room has been changed. You are now in room {index}");
            }
        }

        public List<Client> Clients { get => _clients; }
        public static List<Room> Rooms { get => _rooms;}
    }
}