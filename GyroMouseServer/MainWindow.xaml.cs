using System;
using System.Windows.Controls;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using GyroMouseServer_LocalHost;
using GyroMouseServer_ClientRequestHandler;
using System.Collections.Concurrent;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Xml;

namespace GyroMouseServer
{
    /// <summary>
    /// Interaction logic for Window1.xaml
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
    

        private void button_startServer_Click(object sender, RoutedEventArgs e)
        {
            MainThread = SynchronizationContext.Current;

            serverEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(GyroMouseServer.Properties.Settings.Default.preferredPort));
            listeningPort = new UdpClient(serverEndPoint);

            textBlock_ip.Text = LocalHost.getLocalHost();
            textBlock_port.Text = serverEndPoint.ToString().Substring(serverEndPoint.ToString().LastIndexOf(':') + 1);
            textBlock_notifications.Text = "Waiting for a client...";
            

            clientEndpoint = new IPEndPoint(IPAddress.Any, 0);

            blockingCollections = new BlockingCollection<string> { };

            ClientRequestParser clientRequestHandler = new ClientRequestParser(blockingCollections, serverEndPoint, clientEndpoint, listeningPort, MainThread);
            clientRequestHandler.setUIElements(textBlock_notifications, textBlock_ip);

            clientRequestHandleThreadStart = new ThreadStart(clientRequestHandler.parseRequests);
            clientRequestHandleThread = new Thread(clientRequestHandleThreadStart);
            clientRequestHandleThread.Start();

            button_startServer.IsEnabled = false;
            button_stopServer.IsEnabled = true;
        }
        

        private void button_stopServer_Click_1(object sender, RoutedEventArgs e)
        {
            clientRequestHandleThread.Abort();
            listeningPort.Close();
            
            MainThread.Send((object state) =>
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
            //Preferences pref = new Preferences();
            //pref.loadPreferences();
        }
    }
}
