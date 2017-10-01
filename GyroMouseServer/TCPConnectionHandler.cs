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

        public void Run()
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
                    stream.ReadTimeout = 3000;
                    int i;

                    // Loop to receive all the data sent by the client.
                    try
                    {
                        while ((i = stream.Read(receivedBytes, 0, receivedBytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            receivedString = System.Text.Encoding.ASCII.GetString(receivedBytes, 0, i);
                            Console.WriteLine("Received from new client: {0}", receivedString);
                            break;
                        }
                    }
                    catch(IOException e)
                    {
                        Console.WriteLine("Client did not response within given timeout");
                        // reject this client and go back to listening for new connections
                        client.Close();
                        goto Loopback;
                    }

                    // otherwise
                    Console.WriteLine("Received from new client - printing outside while: {0}", receivedString);
                    Console.WriteLine("Expected from client - printing outside while: {0}", "CANCONNECT?" + GyroMouseServer.Properties.Settings.Default.preferredUDPPort);

                    if (receivedString.Trim() == "CANCONNECT?" + GyroMouseServer.Properties.Settings.Default.preferredUDPPort)
                    {
                        receivedString = "";
                        connectThisClientFlag = 0;
                        // check if a previous client is available
                        Console.WriteLine("CANCONNECT? WAS RECEIVED - NEW CLIENT WANTS TO ESTABLISH CONNECTION AND IS REQUESTING A SESSIONKEY");
                        if (Client.getInstance()!=null)
                        {
                            Console.WriteLine("BUT A PREVIOUS CLIENT WAS ALREADY AVAILABLE - SO LETS SEE IF THEY ARE STILL AVAILABLE");
                            // means there was a previous client
                            // check if the client is still connected
                            Client previousClientInstance = Client.getInstance();

                            try
                            {
                                Console.WriteLine("ASKING UDERE?");
                                previousClientInstance.sendMessage("UDERE?");

                                //get stream of previous client
                                NetworkStream str = previousClientInstance.getSessionTcpClient().GetStream();
                                str.ReadTimeout = 3000;

                                while ((i = str.Read(receivedBytes, 0, receivedBytes.Length)) != 0)
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
                                    previousClientInstance.closeConnection();
                                    ConnectThisClient(client, stream);
                                    connectThisClientFlag = 1;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.StackTrace);
                                // means old stream is redundant and no client is available
                                Console.WriteLine("EXCEPTION THROWN - THEY ARE PROBABLY NOT AVAILABLE - OR TIMED OUT - SO GIVE THIS SLOT TO THE NEW CLIENT");
                                connectThisClientFlag = 1;
                                previousClientInstance.closeConnection();
                                ConnectThisClient(client, stream);
                            }

                            if (connectThisClientFlag == 0)
                            {
                                Console.WriteLine("NO EXCEPTION THROWN - THEY ARE PROBABLY AVAILABLE - REJECT THIS CLIENT");
                                byte[] msgn = System.Text.Encoding.ASCII.GetBytes("BUSY");
                                stream.Write(msgn, 0, msgn.Length);
                                previousClientInstance.closeConnection();
                                client.Close();
                            }

                        }
                        else
                        {
                            Console.WriteLine("NO PREVIOUS CLIENT AVAILABLE - GIVE THIS SLOT TO NEW CLIENT");
                            //no previous client - so allow new guy to connect
                            ConnectThisClient(client, stream);
                        }
                    }
                    else
                    {
                        Console.WriteLine("WE DONT LIKE WHAT THIS GUY SAID. FUCK HIM!");
                        byte[] msgn = System.Text.Encoding.ASCII.GetBytes("BUSY");
                        stream.Write(msgn, 0, msgn.Length);
                        client.Close();
                    }
                    Loopback:
                    Console.WriteLine("Looping Back");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        void ConnectThisClient(TcpClient currentClient, NetworkStream currentClientStream)
        {
            string sessionKey = SessionKeyGenerator.generateRandom();
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(sessionKey);

            // Send back a response.
            Console.WriteLine("SENT A SESSION KEY");
            currentClientStream.Write(msg, 0, msg.Length);
            Console.WriteLine("Sent: {0}", sessionKey);

            //update client details
            Console.WriteLine("UPDATE CLIENT OBJECT");
            //delete old client instance
            Client.reset();
            Console.WriteLine("CLIENT OBJECT UPDATED");

            //get a new client instance
            Client newInstance = Client.getNewInstance();
            newInstance.setProperties(true, currentClient, currentClientStream, sessionKey);
        }

        public void Kill()
        {
            if (client != null)
                client.Close();
            tcpServer.Stop();
        }

        public SynchronizationContext GetSyncContext()
        {
            return SynchronizationContext.Current;
        }

    }
}
