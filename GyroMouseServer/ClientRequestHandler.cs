using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace GyroMouseServer_ClientRequestHandler
{
    class ClientRequestHandler
    {
        private byte[] receivedByte = new Byte[1024];
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint;
        private UdpClient newSocket;
        private SynchronizationContext uiThread;
        private String receivedCommand;
        private Label label_messages,label_ipAddress; 

        private BlockingCollection<string> blockingCollections;

        public ClientRequestHandler(BlockingCollection<string> blockingCollections, IPEndPoint serverEndPoint, IPEndPoint clientEndPoint, UdpClient newSocket, SynchronizationContext uiThread)
        {
            this.serverEndPoint = serverEndPoint;
            this.clientEndPoint = clientEndPoint;
            this.newSocket = newSocket;
            this.uiThread = uiThread;
            this.blockingCollections = blockingCollections;
        }

        public void setUIElements(Label label_messages, Label label_ipAddress)
        {
            this.label_messages = label_messages;
            this.label_ipAddress = label_ipAddress;
        }

        public void handleRequests()
        {
            while (true)
            {
                receivedByte = newSocket.Receive(ref this.clientEndPoint);

                // received data
                receivedCommand = Encoding.ASCII.GetString(receivedByte, 0, receivedByte.Length);
                
                // converting it to json
                JObject json = JObject.Parse(receivedCommand);

                //extracting that data
                JToken X;
                json.TryGetValue("X", out X);

                // converting the data back to string
                X.ToString();

                // printing received data to label
                uiThread.Send((object state) =>
                {
                    label_messages.Content = receivedCommand;
                    label_ipAddress.Content = X.ToString(); ;
                }, null);

                //sending a message back to client
                //string connectionInitiateMessage = "Connected To " + System.Environment.MachineName;
                //data = Encoding.ASCII.GetBytes(connectionInitiateMessage);
                //listeningPort.Send(data, data.Length, this.remoteIPEndpoint);
            }
        }
    }
}
