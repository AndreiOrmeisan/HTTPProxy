using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class ServerProxy
    {
        private int port;

        public Socket ClientSocket { get; internal set; }

        public ServerProxy(int port)
        {
            this.port = port;
        }

        internal void Start(Action<Request, Response> proxy)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHost.AddressList[ipHost.AddressList.Length - 1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, port);
            Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);

            s.Bind(localEndPoint);
            s.Listen(10);

            while (true)
            {
                Socket clientSocket = s.Accept();
                this.ClientSocket = clientSocket;

                byte[] bytes = new byte[clientSocket.ReceiveBufferSize];
                int bytesCount = clientSocket.Receive(bytes);
                if (bytesCount == 0)
                {
                    clientSocket.Close();
                }

                var request = new Request(Encoding.ASCII.GetString(bytes, 0, bytesCount));

                proxy(request, new Response(new NetworkStream(clientSocket)));

                clientSocket.Close();
            }    
        }
    }
}