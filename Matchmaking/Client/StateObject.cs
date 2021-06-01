using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 4094;

        // Receive buffer.  
        public byte[] buffer;

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket;

        public StateObject(Socket socket = null)
        {
            buffer = new byte[BufferSize];
            sb = new StringBuilder();
            workSocket = socket;
        }
    }
}
