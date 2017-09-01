using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms; 

namespace GyroMouseServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        byte[] data = new byte[1024];
        IPEndPoint ipep;
        UdpClient newsock;
        IPEndPoint remoteIPEndpoint;
        private SynchronizationContext MainThread;
        KeyboardInput kbi = new KeyboardInput();
        bool serverSwitch = false;
        ThreadStart clientConnectThreadStart;
        Thread clientConnectThread;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object remoteIPEndpoint, RoutedEventArgs e)
        {
            MainThread = SynchronizationContext.Current;

            ipep = new IPEndPoint(IPAddress.Any, 9050);
            newsock = new UdpClient(ipep);

            label_ipAddress.Content = "Server started at IP : " + getLocalHost() + " Listeneing on port : " + "9050";
            label_messages.Content = "Waiting for a client...";

            remoteIPEndpoint = new IPEndPoint(IPAddress.Any, 0);

            clientConnectThreadStart = new ThreadStart(clientConnect);
            clientConnectThread = new Thread(clientConnectThreadStart);
            clientConnectThread.Start();
            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;
        }

        public static string getLocalHost()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        void clientConnect()
        {
            // receiving message
            data = newsock.Receive(ref this.remoteIPEndpoint);
            MainThread.Send((object state) =>
            {
                label_messages.Content = Encoding.ASCII.GetString(data, 0, data.Length);
            }, null);

            // sending a message
            string connectionInitiateMessage = "Connected To " + System.Environment.MachineName;
            data = Encoding.ASCII.GetBytes(connectionInitiateMessage);
            newsock.Send(data, data.Length, this.remoteIPEndpoint);
        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            clientConnectThread.Abort();
            newsock.Close();
            string message = "Server Stopped";
            MainThread.Send((object state) =>
            {
                label_messages.Content = message;
                label_ipAddress.Content = "";
            }, null);
            button_startServer.IsEnabled = true;
            button_stopServer.IsEnabled = false;
        }
    }
}