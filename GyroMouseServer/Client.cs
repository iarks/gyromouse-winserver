using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GyroMouseServer
{
    public static class Client
    {
        
        public static bool isConnected = false;
        public static TcpClient tcpClient;
        public static NetworkStream tcpStream;
        public static string ssKey;
    }
}
