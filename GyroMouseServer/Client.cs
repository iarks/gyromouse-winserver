using System.Net.Sockets;

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
