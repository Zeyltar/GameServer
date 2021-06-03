using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Server
{
    class Program
    {
        private static bool isRunning = false;
        public Program()
        {

        }
        
        public static void Main(String[] args)
        {
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();

            MainAsync().GetAwaiter().GetResult();            
        }

        public static async Task MainAsync()
        {
            SocketServer server = new SocketServer(IPAddress.Loopback, 26950);
            server.Listen();
            await Task.Delay(-1);
        }

        public static void MainThread()
        {
            Console.WriteLine($"Main thread started. Running @ {Constants.TICKS_PER_SECOND} ticks per seconds");

            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
