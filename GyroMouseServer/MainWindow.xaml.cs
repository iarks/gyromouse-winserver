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
        private Int32 tcpPort=13000;


        // Thread variables
        private SynchronizationContext UIThread;
        private ThreadStart clientRequestHandleThreadStart;
        private Thread clientRequestHandleThread;
        private Thread TCPConnectionHandlerThread;
        private TCPConnectionHandler tCPConnectionHandler;

        private ThreadStart waitingOnClientThreadStart;
        private Thread waitingOnClientThread;
        private Barrier sync;
        private ClientRequestParser clientRequestHandler;

        // Notification managers
        private NotifyIcon notify;
        private ToastNotification toast;

        // Other variables
        private BlockingCollection<string> blockingCollections;

        public MainWindow()
        {
            InitializeComponent();

            // initialize sync context
            UIThread = SynchronizationContext.Current;

            // check if needs to start minimized
            if (GyroMouseServer.Properties.Settings.Default.startMin)
                this.WindowState = WindowState.Minimized;

            // setup system tray
            sysTraySetup();

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

        void sysTraySetup()
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
            // initialise serverside endpoint
            serverEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(GyroMouseServer.Properties.Settings.Default.preferredPort));

            // start listening
            listeningPort = new UdpClient(serverEndPoint);


            // update UI elements
            textBlock_ip.Text = LocalHost.getLocalHost();
            textBlock_port.Text = serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1);
            textBlock_notifications.Text = "Waiting for a client...";

            // setup client endpoint to accept any incoming address
            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

            // initialise a barrier
            sync = new Barrier(2);
            

            // start the thread which handles client requests. The request parser thread waits till a client is available
            clientRequestHandler = new ClientRequestParser(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, UIThread, ref sync);
            clientRequestHandler.setUIElements(textBlock_notifications, textBlock_ip);
            clientRequestHandleThreadStart = new ThreadStart(clientRequestHandler.parseRequests);
            clientRequestHandleThread = new Thread(clientRequestHandleThreadStart);
            clientRequestHandleThread.Name = "clientRequestHandleThread";
            clientRequestHandleThread.Start();

            // start the tcpConnectionHandler Thread
            tCPConnectionHandler = new TCPConnectionHandler(serverIPAddr, tcpServer, tcpPort);
            ThreadStart ts = new ThreadStart(tCPConnectionHandler.run);
            TCPConnectionHandlerThread = new Thread(ts);
            TCPConnectionHandlerThread.Start();

            // toggle UI elements
            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;

            // generate a toast
            Toast.generateToastInfo(5000, "Server Started", LocalHost.getLocalHost() + " : " + serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1));
        }


        private void button_stopServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                //close the ports
                if (listeningPort != null)
                {
                    listeningPort.Close();
                }
                else if (listeningPort == null)
                {

                }

                if (clientRequestHandleThread != null && clientRequestHandleThread.IsAlive)
                {
                    clientRequestHandleThread.Abort("Application shutting down");
                }

                if (tCPConnectionHandler!=null && TCPConnectionHandlerThread.IsAlive)
                {
                    tCPConnectionHandler.kill();
                    //TCPConnectionHandlerThread.Abort();
                    
                }

               




                //update UI elements
                textBlock_notifications.Text = "Server Stopped";
                textBlock_ip.Text = "";
                textBlock_port.Text = "";

                // toggle buttons
                button_startServer.IsEnabled = true;
                button_stopServer.IsEnabled = false;

                Client.reset();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void button_settings_Click(object sender, RoutedEventArgs e)
        {
            // open settings window
            PreferencesWindow prefWin = new PreferencesWindow();
            prefWin.Show();
        }

        private void button_about_Click(object sender, RoutedEventArgs e)
        {
            //generic button

            //this.WindowState = System.Windows.WindowState.Minimized;
            //Toast.generateToastInfo(3000, "Hi", "This is a BallonTip from Windows Notification");

            ToastNotificationManager.CreateToastNotifier("GyroMouseServer").Show(toast);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            // minimise to tray
            if (this.WindowState.Equals(System.Windows.WindowState.Minimized) && GyroMouseServer.Properties.Settings.Default.minTray)
            {
                this.ShowInTaskbar = false;
                Toast.generateToastInfo(3000, "Hi", "Gyro Mouse Server is running in the system tray");
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // dispose tray icon
            if (this.notify != null)
            {
                this.notify.Dispose();
            }

            //shutdown server
            button_stopServer_Click(null, null);

            // Shutdown the application.
            System.Windows.Application.Current.Shutdown();
        }
        
    }
}