using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{


    class Program
    {
        public Program()
        {
            
        }
        // State object for receiving data from remote device.  
        public static void Main(String[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            Client.Instance.ConnectToServer();

            await Task.Delay(-1);
        }
    }
}
