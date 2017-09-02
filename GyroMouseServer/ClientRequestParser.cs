﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Controls;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using GyroMouseServer_MouseMove;
using GyroMouseServer;
using System.Runtime.InteropServices;
using System.Windows.Forms;



namespace GyroMouseServer_ClientRequestHandler
{
    class ClientRequestParser
    {
        private byte[] receivedByte = new Byte[1024];
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint;
        private UdpClient newSocket;
        private SynchronizationContext uiThread;
        private String receivedCommand;
        private System.Windows.Controls.Label label_messages,label_ipAddress;
        private bool firstVal = false;
        private float dxf, dyf;

        private KeyboardInput kbi = new KeyboardInput();

        private Mouse mouse = new Mouse();

        private BlockingCollection<ClientRequest> requestQueue;

        public ClientRequestParser(BlockingCollection<ClientRequest> requestQueue, IPEndPoint serverEndPoint, IPEndPoint clientEndPoint, UdpClient newSocket, SynchronizationContext uiThread)
        {
            this.serverEndPoint = serverEndPoint;
            this.clientEndPoint = clientEndPoint;
            this.newSocket = newSocket;
            this.uiThread = uiThread;
            this.requestQueue = requestQueue;
        }

        public void setUIElements(System.Windows.Controls.Label label_messages, System.Windows.Controls.Label label_ipAddress)
        {
            this.label_messages = label_messages;
            this.label_ipAddress = label_ipAddress;
        }

        public void parseRequests()
        {
            JToken X=null;
            JToken Y=null;
            string header=null, param=null;
            while (true)
            {
                receivedByte = newSocket.Receive(ref this.clientEndPoint);

                // received data
                receivedCommand = Encoding.ASCII.GetString(receivedByte, 0, receivedByte.Length);

                try
                {
                    // converting it to json
                    JObject json = JObject.Parse(receivedCommand);

                    //extracting that data
                    

                    json.TryGetValue("X", out X);
                    json.TryGetValue("Y", out Y);

                    header = X.ToString();
                    param = Y.ToString();
                    
                }
                catch (Exception e)
                {
                   
                }

                //uiThread.Send((object state) =>
                //{
                //    label_messages.Content = receivedCommand;
                //    label_ipAddress.Content = Y.ToString() + ":" + X.ToString();
                //}, null);

                switch (header)
                {
                    case "EOT":
                        firstVal = true;
                        break;
                    //case "LD":
                    //    LeftDown();
                    //    break;
                    //case "LU":
                    //    LeftUp();
                    //    break;
                    //case "S":
                    //    dyf = Float.parseFloat(dy);
                    //    scrollPage(dyf);
                    //    break;
                    //case "RD":
                    //    rightDown();
                    //    break;
                    //case "RU":
                    //    rightUp();
                    //    break;
                    case "BS":
                        WinkeyInput.KeyDown(Keys.Back);
                        WinkeyInput.KeyUp(Keys.Back);
                        break;
                    case "EN":
                        WinkeyInput.KeyDown(Keys.Enter);
                        WinkeyInput.KeyUp(Keys.Enter);
                        break;
                    //case "U":
                    //    dyf = Float.parseFloat(dy);
                    //    pressUnicode((int)dyf);
                    //    break;
                    case "ESC":
                        WinkeyInput.KeyDown(Keys.Escape);
                        WinkeyInput.KeyUp(Keys.Escape);
                        break;
                    case "WIN":
                        WinkeyInput.KeyDown(Keys.LWin);
                        WinkeyInput.KeyUp(Keys.LWin);
                        break;
                    case "AL":
                        WinkeyInput.KeyDown(Keys.Right);
                        WinkeyInput.KeyUp(Keys.Right);
                        break;
                    case "AR":
                        WinkeyInput.KeyDown(Keys.Left);
                        WinkeyInput.KeyUp(Keys.Left);
                        break;
                    case "AD":
                        WinkeyInput.KeyDown(Keys.Down);
                        WinkeyInput.KeyUp(Keys.Down);
                        break;
                    case "AU":
                        WinkeyInput.KeyDown(Keys.Up);
                        WinkeyInput.KeyUp(Keys.Up);
                        break;
                    default:
                        dxf = float.Parse(header);
                        dyf = float.Parse(param);
                        if (!firstVal)
                        {
                            mouse.movePointer(dxf * 25, dyf * 25);
                        }
                        else
                        {
                            firstVal = false;
                        }
                        break;

                }

                //sending a message back to client
                //string connectionInitiateMessage = "Connected To " + System.Environment.MachineName;
                //data = Encoding.ASCII.GetBytes(connectionInitiateMessage);
                //listeningPort.Send(data, data.Length, this.remoteIPEndpoint);
            }
        }
    }
}
