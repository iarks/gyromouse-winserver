using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using GyroMouseServer_LocalHost;
using GyroMouseServer_ClientRequestHandler;
using System.Collections.Concurrent;

namespace GyroMouseServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        private IPEndPoint serverEndPoint;
        private UdpClient listeningPort;
        private IPEndPoint clientEndpoint;
        private SynchronizationContext MainThread;
        
        private ThreadStart clientRequestHandleThreadStart;
        private Thread clientRequestHandleThread;

        private BlockingCollection<string> blockingCollections;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object remoteIPEndpoint, RoutedEventArgs e)
        {
            MainThread = SynchronizationContext.Current;

            serverEndPoint = new IPEndPoint(IPAddress.Any, 9050);
            listeningPort = new UdpClient(serverEndPoint);

            label_ipAddress.Content = "Server started at IP : " + LocalHost.getLocalHost() + " Listeneing on port : " + "9050";
            label_messages.Content = "Waiting for a client...";

            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

            blockingCollections = new BlockingCollection<string> { };
            
            ClientRequestParser clientRequestHandler = new ClientRequestParser(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, MainThread);
            clientRequestHandler.setUIElements(label_messages,label_ipAddress);

            clientRequestHandleThreadStart = new ThreadStart(clientRequestHandler.parseRequests);
            clientRequestHandleThread = new Thread(clientRequestHandleThreadStart);
            clientRequestHandleThread.Start();

            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;

        }
        
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            clientRequestHandleThread.Abort();
            listeningPort.Close();

            string message = "Server Stopped";
            MainThread.Send((object state) =>
            {
                textBlock_ipAddress.Text = "";
               

                label_messages.Content = message;
                label_ipAddress.Content = "";
            }, null);

            button_startServer.IsEnabled = true;
            button_stopServer.IsEnabled = false;
        }
    }
}