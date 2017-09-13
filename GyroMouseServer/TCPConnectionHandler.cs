using System.Drawing.Drawing2D;
using System.IO;
using System.Data.OleDb;
using System.Reflection.Emit;
using System.Net.Sockets;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GyroMouseServer
{
    class TCPConnectionHandler
    {
        IPAddress tcpserverIP;
        TcpListener tcpServer;
        Int32 tcpPort;
        TcpClient client;

        public TCPConnectionHandler(IPAddress tcpserverIP, TcpListener tcpServer, Int32 tcpPort)
        {
            this.tcpserverIP = tcpserverIP;
            this.tcpServer = tcpServer;
            this.tcpPort = tcpPort;
        }

        public void run()
        {
            Byte[] receivedBytes = new Byte[256];
            String receivedString = null;
            int connectThisClientFlag = 0;

            tcpServer = new TcpListener(tcpserverIP, tcpPort);
            tcpServer.Start();

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for a connection... ");

                    client = tcpServer.AcceptTcpClient();

                    Console.WriteLine("Connected!");

                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(receivedBytes, 0, receivedBytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        receivedString = System.Text.Encoding.ASCII.GetString(receivedBytes, 0, i);
                        Console.WriteLine("Received from new client: {0}", receivedString);
                        break;
                    }

                    Console.WriteLine("Received from new client - printing outside while: {0}", receivedString);

                    if (receivedString == "CANCONNECT?")
                    {
                        receivedString = "";
                        connectThisClientFlag = 0;
                        // check if a previous client is available
                        Console.WriteLine("CANCONNECT? WAS RECEIVED - NEW CLIENT WANTS TO ESTABLISH CONNECTION AND IS REQUESTING A SESSIONKEY");
                        if (Client.isConnected)
                        {
                            Console.WriteLine("BUT A PREVIOUS CLIENT WAS ALREADY AVAILABLE - SO LETS SEE IF THEY CAN STILL CONNECT");
                            // means there was a previous client
                            // check if the client is still connected

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes("UDERE?");

                            try
                            {
                                Console.WriteLine("ASKING UDERE?");
                                NetworkStream str = Client.tcpClient.GetStream();
                                str.Write(msg, 0, msg.Length);
                                str.Flush();

                                while ((i = Client.tcpStream.Read(receivedBytes, 0, receivedBytes.Length)) != 0)
                                {
                                    // Translate data bytes to a ASCII string.
                                    receivedString = System.Text.Encoding.ASCII.GetString(receivedBytes, 0, i);
                                    Console.WriteLine("Received from new client: {0}", receivedString);
                                    break;
                                }
                                Console.WriteLine("break from while");
                                Console.WriteLine("WHAT WAS RECEIVED?" + receivedString);
                                if (receivedString != "YES")
                                {
                                    Console.WriteLine("PREVIOUS CLIENT IS UNAVAILABLE");
                                    connectThisClient(client, stream);
                                    connectThisClientFlag = 1;
                                }
                            }
                            catch (IOException e)
                            {
                                // means old stream is redundant and no client is available
                                Console.WriteLine("EXCEPTION THROWN - THEY ARE PROBABLY NOT AVAILABLE - SO GIVE THIS SLOT TO THE NEW CLIENT");
                                connectThisClientFlag = 1;
                                connectThisClient(client, stream);
                            }

                            if (connectThisClientFlag == 0)
                            {
                                Console.WriteLine("NO EXCEPTION THROWN - THEY ARE PROBABLY AVAILABLE - REJECT THIS CLIENT");
                                byte[] msgn = System.Text.Encoding.ASCII.GetBytes("BUSY");
                                stream.Write(msgn, 0, msgn.Length);
                                client.Close();
                            }

                        }
                        else
                        {
                            Console.WriteLine("NO PREVIOUS CLIENT AVAILABLE - GIVE THIS SLOT TO NEW CLIENT");
                            //no previous client - so allow new guy to connect
                            connectThisClient(client, stream);
                        }
                    }
                    else
                    {
                        Console.WriteLine("WE DONT LIKE WHAT THIS GUY SAID. FUCK HIM!");
                        client.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        void connectThisClient(TcpClient currentClient, NetworkStream currentClientStream)
        {
            string sessionKey = SessionKeyGenerator.generateRandom();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(sessionKey);

            // Send back a response.
            Console.WriteLine("SENT A SESSION KEY");
            currentClientStream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", sessionKey);

            //update client details
            Console.WriteLine("UPDATED CLIENT OBJECT");
            Client.isConnected = true;
            Client.tcpClient = currentClient;
            Client.tcpStream = currentClientStream;
            Client.ssKey = sessionKey;
        }

        public void kill()
        {
            if (Client.tcpClient != null)
            {
                Client.tcpClient.Close();
                Client.isConnected = false;
                Client.ssKey = null;
                Client.tcpStream = null;
            }
            if (client != null)
                client.Close();
            tcpServer.Stop();
        }

        public SynchronizationContext getSyncContext()
        {
            return SynchronizationContext.Current;
        }

    }
}
