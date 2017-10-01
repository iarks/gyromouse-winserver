using System.Drawing;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using GyroMouseServer_LocalHost;
using GyroMouseServer_ClientRequestHandler;
using System.Collections.Concurrent;
using System.Windows.Forms;
using Windows.UI.Notifications;

namespace GyroMouseServer
{
    public partial class MainWindow : Window
    {
        // UDP variables --
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndpoint;
        private UdpClient listeningPort;

        // TCP variables --
        private IPAddress serverIPAddr = IPAddress.Parse(LocalHost.getLocalHost());
        private TcpListener tcpServer = null;
        private Int32 tcpPort;


        // the context of the UI thread
        private SynchronizationContext UIThread;

        // Thread variables
        // this thread handles client requests
        private ClientRequestParser clientRequestParser;
        private ThreadStart clientRequestParserThreadStart;
        private Thread clientRequestHandleThread;

        // this thread handles incoming tcp connections
        private TCPConnectionHandler tCPConnectionHandler;
        private ThreadStart ts;
        private Thread TCPConnectionHandlerThread;


        private Barrier sync;


        // Notification managers
        private NotifyIcon notify;
        private ToastNotification toast;

        // Other variables
        private BlockingCollection<string> blockingCollections;

        // setting up the main window
        public MainWindow()
        {
            Console.WriteLine("Initialising components");
            InitializeComponent();
            Console.WriteLine("Initialising components completed");


            // initialize sync context
            UIThread = SynchronizationContext.Current;

            // check if needs to start minimized
            if (GyroMouseServer.Properties.Settings.Default.startMin)
                this.WindowState = WindowState.Minimized;

            // setup system tray
            Console.WriteLine("setting up system tray");
            sysTraySetup();
            Console.WriteLine("setting up system complete");

            // autostart server if necessary
            if (GyroMouseServer.Properties.Settings.Default.autoServe)
                button_startServer_Click(null, null);


            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            //// Fill in the text elements
            //XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            //for (int i = 0; i < stringElements.Length; i++)
            //{
            //    stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            //}

            //// Specify the absolute path to an image
            ////String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
            ////XmlNodeList imageElements = toastXml.GetElementsByTagName("image");

            //toast = new ToastNotification(toastXml);

            //toast.Activated += ToastActivated;
            //toast.Dismissed += ToastDismissed;
            //toast.Failed += ToastFailed;

            ServerState.applicationRunning = true;
            Console.WriteLine("initialising system variables complete");
        }

        //private void ToastFailed(ToastNotification sender, ToastFailedEventArgs args)
        //{
        //    //throw new NotImplementedException();
        //}

        //private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs args)
        //{
        //    //throw new NotImplementedException();
        //}

        //private void ToastActivated(ToastNotification sender, object args)
        //{
        //    //throw new NotImplementedException();
        //}

        private void sysTraySetup()
        {
            this.notify = new NotifyIcon
            {
                Text = "Gyro Mouse Server",
                Icon = SystemIcons.Information,
                Visible = true,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    new MenuItem("Open Window", (s, e) => this.WindowState=WindowState.Normal),
                    new MenuItem("Close Window", (s, e) => this.WindowState=WindowState.Minimized),
                    new MenuItem("-"),
                    new MenuItem("Exit", (s, e) => this.Close())
                })
            };
        }

        private void button_startServer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("server starting");
            
            // initialise serverside endpoint - the address of this machine
            serverEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(GyroMouseServer.Properties.Settings.Default.preferredUDPPort));

            // start listening for incoming commands at this port
            listeningPort = new UdpClient(serverEndPoint);

            // update UI elements
            textBlock_ip.Text = LocalHost.getLocalHost();
            textBlock_port.Text = serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1);
            textBlock_notifications.Text = "Waiting for a client...";

            // setup client endpoint to accept any incoming address this is the address of the phone
            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

            // initialise a barrier
            sync = new Barrier(2);


            // start the thread which handles client requests. The request parser thread waits till a client is available
            Console.WriteLine("starting thread which listends on the udp port");
            clientRequestParser = new ClientRequestParser(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, UIThread, ref sync);
            clientRequestParser.SetUIElements(textBlock_notifications, textBlock_ip);
            clientRequestParserThreadStart = new ThreadStart(clientRequestParser.ParseRequests);
            clientRequestHandleThread = new Thread(clientRequestParserThreadStart);
            clientRequestHandleThread.Name = "clientRequestHandleThread";
            clientRequestHandleThread.Start();
            Console.WriteLine("starting thread which listends on the udp port complete");

            // start the tcpConnectionHandler Thread. This thread is responsible for connecting new clients
            Console.WriteLine("starting thread which listends for incoming connections");
            tcpPort = Int32.Parse(GyroMouseServer.Properties.Settings.Default.preferredTCPPort);
            tCPConnectionHandler = new TCPConnectionHandler(serverIPAddr, tcpServer, tcpPort);
            ts = new ThreadStart(tCPConnectionHandler.Run);
            TCPConnectionHandlerThread = new Thread(ts);
            TCPConnectionHandlerThread.Name = "TCPConnectionHandlerThread";
            TCPConnectionHandlerThread.Start();
            Console.WriteLine("starting thread which listends for incoming connections complete");

            // toggle UI elements
            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;

            // generate a toast
            Toast.generateToastInfo(5000, "Server Started", LocalHost.getLocalHost() + " : " + serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1));

            ServerState.serverRunning = true;
            Console.WriteLine("Server started");
        }

        private void button_stopServer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("stopping server");
            try
            {
                // reset the client object
                Client clientInstance = Client.getInstance();
                if (clientInstance != null)
                {
                    clientInstance.sendMessage("SHUTTINGDOWN");
                    clientInstance.closeConnection();
                }


                // close the udp port
                Console.WriteLine("closing UDP Port");
                if (listeningPort != null)
                {
                    listeningPort.Close();
                }
                Console.WriteLine("closing UDP Port complete");

                // kill the thread which receives the commands
                Console.WriteLine("aborting client request handler");
                if (clientRequestHandleThread != null && clientRequestHandleThread.IsAlive)
                {
                    clientRequestHandleThread.Abort("Application shutting down");
                }
                Console.WriteLine("aborting client request handler");

                Console.WriteLine("aborting tcp handler");
                // kill the thread which connects the clients
                if (tCPConnectionHandler!=null && TCPConnectionHandlerThread.IsAlive)
                {
                    tCPConnectionHandler.Kill();
                    TCPConnectionHandlerThread.Abort();
                }
                Console.WriteLine("aborting tcp handler complete");

                //update UI elements
                textBlock_notifications.Text = "Server Stopped";
                textBlock_ip.Text = "";
                textBlock_port.Text = "";

                // toggle buttons
                button_startServer.IsEnabled = true;
                button_stopServer.IsEnabled = false;

                

                // update server status
                ServerState.serverRunning = false;

                Console.WriteLine("Server stopped");
            }
            catch(Exception exception)
            {
                Console.WriteLine("exception occured");
                Console.WriteLine(exception.StackTrace);
            }
        }

        private void button_settings_Click(object sender, RoutedEventArgs e)
        {
            // open settings window
            Console.WriteLine("opening preferences dialog");
            PreferencesWindow prefWin = new PreferencesWindow();
            prefWin.Owner = this;
            prefWin.ShowDialog();
            Console.WriteLine("opening preferences dialog");
        }

        private void button_about_Click(object sender, RoutedEventArgs e)
        {
            //generic button

            //this.WindowState = System.Windows.WindowState.Minimized;
            //Toast.generateToastInfo(3000, "Hi", "This is a BallonTip from Windows Notification");

            //ToastNotificationManager.CreateToastNotifier("GyroMouseServer").Show(toast);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            // if minimise to tray is enabled, minimise to tray
            if (this.WindowState.Equals(System.Windows.WindowState.Minimized) && GyroMouseServer.Properties.Settings.Default.minTray)
            {
                this.ShowInTaskbar = false;
                Toast.generateToastInfo(3000, "Hi", "Gyro Mouse Server is running in the system tray");
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("closing application");
            Console.WriteLine("disposing tray icon");
            // dispose tray icon
            if (this.notify != null)
            {
                this.notify.Dispose();
            }
            Console.WriteLine("tray icon disposed");

            //shutdown server
            button_stopServer_Click(null, null);

            // Shutdown the application.
            Console.WriteLine("shut down application");
            System.Windows.Application.Current.Shutdown();
        }

        public void restartServer()
        {
            Console.WriteLine("restaring server");
            button_stopServer_Click(null, null);
            Thread.Sleep(200);
            button_startServer_Click(null, null);
            Console.WriteLine("server restarted");
        }
        
    }
}