using System.Windows.Forms.VisualStyles;
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
using Windows.Data.Xml.Dom;
using System.Text;

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

        private NotifyIcon notify;

        private BlockingCollection<string> blockingCollections;

        ToastNotification toast;

        ThreadStart waitingOnClientThreadStart;
        Thread waitingOnClientThread;

        public MainWindow()
        {
            InitializeComponent();
            UIThread = SynchronizationContext.Current;

            if (GyroMouseServer.Properties.Settings.Default.startMin)
                this.WindowState = WindowState.Minimized;

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

            if (GyroMouseServer.Properties.Settings.Default.autoServe)
                button_startServer_Click(null, null);

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);

            // Fill in the text elements
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            for (int i = 0; i < stringElements.Length; i++)
            {
                stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            }

            // Specify the absolute path to an image
            //String imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
            //XmlNodeList imageElements = toastXml.GetElementsByTagName("image");

            toast = new ToastNotification(toastXml);

            toast.Activated += ToastActivated;
            toast.Dismissed += ToastDismissed;
            toast.Failed += ToastFailed;
        }

        private void ToastFailed(ToastNotification sender, ToastFailedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private void ToastDismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
            //throw new NotImplementedException();
        }

        private void ToastActivated(ToastNotification sender, object args)
        {
            //throw new NotImplementedException();
        }

        private void button_startServer_Click(object sender, RoutedEventArgs e)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(GyroMouseServer.Properties.Settings.Default.preferredPort));
            listeningPort = new UdpClient(serverEndPoint);

            textBlock_ip.Text = LocalHost.getLocalHost();
            textBlock_port.Text = serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1);
            textBlock_notifications.Text = "Waiting for a client...";
            

            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);


            waitingOnClientThread = new Thread(() => waitingOnClient(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, UIThread));
            waitingOnClientThread.Start();

            ClientRequestParser clientRequestHandler = new ClientRequestParser(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, UIThread);
            clientRequestHandler.setUIElements(textBlock_notifications, textBlock_ip);

//            clientRequestHandleThreadStart = new ThreadStart(clientRequestHandler.parseRequests);
//            clientRequestHandleThread = new Thread(clientRequestHandleThreadStart);
//            clientRequestHandleThread.Start();

            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;

            Toast.generateToastInfo(5000, "Server Started", LocalHost.getLocalHost() + " : " + serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1));

            

        }
        

        private void button_stopServer_Click_1(object sender, RoutedEventArgs e)
        {
            if(clientRequestHandleThread!=null && clientRequestHandleThread.IsAlive)
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
            //this.WindowState = System.Windows.WindowState.Minimized;
            //Toast.generateToastInfo(3000, "Hi", "This is a BallonTip from Windows Notification");

            ToastNotificationManager.CreateToastNotifier("GyroMouseServer").Show(toast);
           


        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if(this.WindowState.Equals(System.Windows.WindowState.Minimized) && GyroMouseServer.Properties.Settings.Default.minTray)
            {
                this.ShowInTaskbar = false;
                Toast.generateToastInfo(3000, "Hi", "Gyro Mouse Server is running in the system tray");
            }   
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (this.notify != null)
            {
                this.notify.Dispose();
            }

            button_stopServer_Click_1(null,null);
            // Shutdown the application.
            System.Windows.Application.Current.Shutdown();
            // OR You can Also go for below logic
            // Environment.Exit(0);
        }

        private void waitingOnClient(BlockingCollection<string> blockingCollection,IPEndPoint server, IPEndPoint client,UdpClient port,SynchronizationContext UIThread)
        {
            Label:
            Byte[] data = listeningPort.Receive(ref client);
            Console.WriteLine("Message received from {0}:", client.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            Console.WriteLine("Message received from {0}:", client.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, data.Length));

            string welcome = LocalHost.getLocalHost();

            if (Encoding.ASCII.GetString(data, 0, data.Length) == "DICK MOVE")
            {
                data = Encoding.ASCII.GetBytes(welcome);
                listeningPort.Send(data, data.Length, client);
            }
            else
                goto Label;
                
        }
    }
}
