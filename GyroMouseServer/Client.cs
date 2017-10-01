using System.Threading;
using System.Net.Sockets;

namespace GyroMouseServer
{
    public sealed class Client
    {

        private bool isConnected = false;
        private TcpClient tcpClient;
        private NetworkStream tcpStream;
        private string ssKey;
        static public string ssKeyGlobal;
        private static Client instance = null;

        private Client()
        {
            //prevents initialisation
        }

        public static Client getNewInstance()
        {
            instance = null;
            instance = new Client();
            return instance;
        }

        public static Client getInstance()
        {
            return instance;
        }

        public static void reset()
        {
            instance = null;
        }

        public void setProperties(bool isConnected,TcpClient tcpClient,NetworkStream tcpStream, string ssKey)
        {
            this.isConnected = isConnected;
            this.tcpClient = tcpClient;
            this.tcpStream = tcpStream;
            this.ssKey = ssKey;
            ssKeyGlobal = ssKey;
        }

        public bool getConnected()
        {
            return this.isConnected;
        }

        public TcpClient getSessionTcpClient()
        {
            return this.tcpClient;
        }

        public NetworkStream getStream()
        {
            return this.tcpStream;
        }

        public string getKey()
        {
            return this.ssKey;
        }
        
        public void sendMessage(string message)
        {
            byte[] msgn = System.Text.Encoding.ASCII.GetBytes(message);
            this.tcpStream.WriteTimeout = 3000;
            this.tcpStream.Write(msgn, 0, msgn.Length);
            this.tcpStream.Flush();
        }

        public void closeConnection()
        {
            this.tcpClient.Close();
            reset();
        }
    }
}
