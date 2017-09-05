using GyroMouseServer_LocalHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GyroMouseServer
{
    class TCPClientListener
    {
        Int32 tcpPort = 13000;
        TcpListener server = null;
        IPAddress localAddr;
        Byte[] bytes = new Byte[256];
        String data = null;

        TCPClientListener(Int32 tcpPort)
        {
            this.tcpPort = tcpPort;
            localAddr = IPAddress.Parse(LocalHost.getLocalHost());
            server = new TcpListener(localAddr, tcpPort);
        }

        void startListening()
        {

            try
            {
                server.Start();
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also user server.AcceptSocket() here
                    // server waits here till it gets a connection request
                    TcpClient client = server.AcceptTcpClient();

                    //server comes here after it gets a connection
                    Console.WriteLine("Connected!");

                    data = null;

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();

                        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                        // Send back a response.
                        stream.Write(msg, 0, msg.Length);
                        Console.WriteLine("Sent: {0}", data);
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}

