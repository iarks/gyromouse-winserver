using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Controls;
using System.Collections.Concurrent;
using GyroMouseServer_MouseMove;
using GyroMouseServer;
using System.Windows.Forms;

namespace GyroMouseServer_ClientRequestHandler
{
    internal class ClientRequestParser
    {
        private byte[] receivedByte = new Byte[1024];
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint;
        private UdpClient newSocket;
        private SynchronizationContext uiThread;
        private String receivedCommand;
        private TextBlock label_messages,label_ipAddress;
        private bool firstVal = false;
        private float dxf, dyf;
        private float dxPast=0.0f, dyPast=0.0f;
        private Barrier sync;

        string header = null, param = null;

        private KeyboardInput kbi = new KeyboardInput();

        private Mouse mouse = new Mouse();

        private BlockingCollection<string> requestQueue;

        public ClientRequestParser(BlockingCollection<string> requestQueue, IPEndPoint serverEndPoint, IPEndPoint clientEndPoint, UdpClient newSocket, SynchronizationContext uiThread, ref Barrier sync)
        {
            Console.WriteLine("initialising client request parser");
            this.serverEndPoint = serverEndPoint;
            this.clientEndPoint = clientEndPoint;
            this.newSocket = newSocket;
            this.uiThread = uiThread;
            this.requestQueue = requestQueue;
            this.sync = sync;
            Console.WriteLine("initialising client request parser complete");
        }


        public void SetUIElements(TextBlock label_messages, TextBlock label_ipAddress)
        {
            Console.WriteLine("setting up ui elements");
            this.label_messages = label_messages;
            this.label_ipAddress = label_ipAddress;
            Console.WriteLine("setting up ui elements complete");
        }

        string sessionKey;

        public void ParseRequests()
        {
            Console.WriteLine("staring to parse requests");

            if (Client.getInstance() != null)
                sessionKey = Client.getInstance().getKey();
            else
                sessionKey = "";

            while (true)
            {
                Console.WriteLine("inside infinite while loop that listens on udp port");

                try
                {
                    // received byte on socket
                    receivedByte = newSocket.Receive(ref this.clientEndPoint);
                    // convert byte to string
                    receivedCommand = Encoding.UTF8.GetString(receivedByte, 0, receivedByte.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine("exception occurred");
                    Console.WriteLine(e.StackTrace);
                }
                Console.WriteLine(receivedCommand);

                string[] extractedCommand = receivedCommand.Split(';');

                //uiThread.Send((object state) =>
                //{
                //    label_messages.Text = receivedCommand;
                //    label_ipAddress.Text = Y.ToString() + ":" + X.ToString();
                //}, null);

                if ((extractedCommand.Length == 3 && extractedCommand[2] == Client.ssKeyGlobal) || (extractedCommand[0] == "CANHAVEIP?" && extractedCommand[2] == "GMO") || (extractedCommand.Length == 4 && extractedCommand[3]== Client.ssKeyGlobal))
                {
                    try
                    {
                        Console.WriteLine(extractedCommand[2]);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                    try
                    {
                        Console.WriteLine(extractedCommand.Length);
                        if (extractedCommand.Length == 4 && extractedCommand[0] == "U")
                        {
                            Console.WriteLine("here");
                            kbi.typeIn(";");
                        }
                        switch (extractedCommand[0])
                        {
                            case "CANHAVEIP?":
                                Byte[] responseData = Encoding.ASCII.GetBytes(System.Environment.MachineName);
                                string[] requstIP = clientEndPoint.ToString().Split(':');
                                Console.WriteLine("ClientIP>> " + requstIP[0]);
                                newSocket.Send(responseData, responseData.Length, clientEndPoint);
                                break;
                            case "EOT":
                                firstVal = true;
                                dxPast = 0.0f;
                                dyPast = 0.0f;
                                break;
                            case "LD":
                                mouse.leftDown();
                                break;
                            case "LU":
                                mouse.leftUp();
                                break;
                            case "S":
                                dyf = float.Parse(extractedCommand[1]);
                                mouse.scroll(dyf);
                                break;
                            case "RD":
                                mouse.rightDown();
                                break;
                            case "RU":
                                mouse.rightUp();
                                break;
                            case "BS":
                                WinkeyInput.KeyDown(Keys.Back);
                                WinkeyInput.KeyUp(Keys.Back);
                                break;
                            case "EN":
                                WinkeyInput.KeyDown(Keys.Enter);
                                WinkeyInput.KeyUp(Keys.Enter);
                                break;
                            case "U":
                                kbi.typeIn(extractedCommand[1]);
                                break;
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
                                dxf = float.Parse(extractedCommand[0]);
                                dyf = float.Parse(extractedCommand[1]);
                                if (!firstVal)
                                {
                                    //mouse.movePointer((dxf/10) * GyroMouseServer.Properties.Settings.Default.sensitivity, (dyf/10) * GyroMouseServer.Properties.Settings.Default.sensitivity);
                                    //mouse.movePointer((dxf  ) * 25, (dyf ) * 25);
                                    mouse.smoothMovePointer(dxf, dyf, dxPast, dyPast);
                                    dxPast = dxf;
                                    dyPast = dyf;
                                    //mouse.movePointer(dxf*25, dyf*25);
                                }
                                else
                                    firstVal = false;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(header + "," + param);
                        Console.WriteLine(e.StackTrace);
                    }
                }
                
                //string connectionInitiateMessage = "Connected To " + System.Environment.MachineName;
                //data = Encoding.ASCII.GetBytes(connectionInitiateMessage);
                //listeningPort.Send(data, data.Length, this.remoteIPEndpoint);
            }
        }

        public void Kill()
        {
            newSocket.Close();
        }
    }
}
