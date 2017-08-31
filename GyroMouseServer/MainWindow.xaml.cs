using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object remoteIPEndpoint, RoutedEventArgs e)
        {
            MainThread = SynchronizationContext.Current;
            //if (MainThread == null)
            //    MainThread = new SynchronizationContext();
            ipep = new IPEndPoint(IPAddress.Any, 9050);
            newsock = new UdpClient(ipep);
            
            label_ipAddress.Content = "Server started at IP : "+ getLocalHost()+" Listeneing on port : "+ "9050";
            label_messages.Content = "Waiting for a client...";

            remoteIPEndpoint = new IPEndPoint(IPAddress.Any, 0);

            ThreadStart clientConnectThreadStart = new ThreadStart(clientConnect);
            Thread clientConnectThread = new Thread(clientConnectThreadStart);
            clientConnectThread.Start();
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
            string welcome = "Welcome to my test server";
            data = Encoding.ASCII.GetBytes(welcome);
            newsock.Send(data, data.Length, this.remoteIPEndpoint);
        }
    }
}