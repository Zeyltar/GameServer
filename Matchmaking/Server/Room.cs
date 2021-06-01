using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Room
    {
        private List<Client> _clients;

        public Room(IEnumerable<Client> clients = null)
        {
            _clients = new List<Client>();
            if (clients != null)
            {
                _clients.AddRange(clients);
                
            }
        }


        public List<Client> Clients { get => _clients; }
    }
}
