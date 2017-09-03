using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using GyroMouseServer_LocalHost;
using GyroMouseServer_ClientRequestHandler;
using System.Collections.Concurrent;
using Microsoft.Win32;
using System.Windows.Forms;
using GyroMouseServer;


using System.Windows.Forms;
using System.Drawing;
using GyroMouseServer_LocalHost;


namespace GyroMouseServer
{
    public partial class MainWindow : Window
    {
        private IPEndPoint serverEndPoint;
        private UdpClient listeningPort;
        private IPEndPoint clientEndpoint;
        private SynchronizationContext UIThread;

        private ThreadStart clientRequestHandleThreadStart;
        private Thread clientRequestHandleThread;

        NotifyIcon notify = new NotifyIcon();

        




        private BlockingCollection<string> blockingCollections;

        public MainWindow()
        {
            InitializeComponent();
            if(GyroMouseServer.Properties.Settings.Default.startMin)
                this.WindowState = WindowState.Minimized;
            UIThread = SynchronizationContext.Current;


            this.notify = new System.Windows.Forms.NotifyIcon();
            this.notify.Text = "Taskbar Compass";
            this.notify.Icon = new System.Drawing.Icon(@"D:\Users\Arkadeep\Documents\OneDrive\GyroMouseServer\GyroMouseServer\resources\02_Acrobat.ico");
            this.notify.Visible = true;
            this.notify.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[]
            {
            new System.Windows.Forms.MenuItem("Show compass", (s, e) => this.Show()),
            new System.Windows.Forms.MenuItem("Hide compass", (s, e) => this.Hide()),
            new System.Windows.Forms.MenuItem("-"),
            new System.Windows.Forms.MenuItem("Close", (s, e) => this.Close())
            });

            //if (this.notify != null)
            //{
            //    this.notify.Dispose();
            //}

        }
    

        private void button_startServer_Click(object sender, RoutedEventArgs e)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(GyroMouseServer.Properties.Settings.Default.preferredPort));
            listeningPort = new UdpClient(serverEndPoint);

            textBlock_ip.Text = LocalHost.getLocalHost();
            textBlock_port.Text = serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1);
            textBlock_notifications.Text = "Waiting for a client...";
            

            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

            ClientRequestParser clientRequestHandler = new ClientRequestParser(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, UIThread);
            clientRequestHandler.setUIElements(textBlock_notifications, textBlock_ip);

            clientRequestHandleThreadStart = new ThreadStart(clientRequestHandler.parseRequests);
            clientRequestHandleThread = new Thread(clientRequestHandleThreadStart);
            clientRequestHandleThread.Start();

            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;

            Toast.generateToastInfo(5000, "Server Started", LocalHost.getLocalHost() + " : " + serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1));

            

        }
        

        private void button_stopServer_Click_1(object sender, RoutedEventArgs e)
        {
            clientRequestHandleThread.Abort();
            listeningPort.Close();
            
            UIThread.Send((object state) =>
            {
                textBlock_notifications.Text = "Server Stopped";
                textBlock_ip.Text = "";
                textBlock_port.Text = "";
            }, null);

            button_startServer.IsEnabled = true;
            button_stopServer.IsEnabled = false;
        }

        private void button_settings_Click(object sender, RoutedEventArgs e)
        {
            PreferencesWindow prefWin = new PreferencesWindow();
            prefWin.Show();
        }

        private void button_about_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
            Toast.generateToastInfo(3000, "Hi", "This is a BallonTip from Windows Notification");


        }

        

        

    }
}
