using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server
{
    internal class ServerProxi
    {
        private Socket server;
        private Socket clientSocket;

        public ServerProxi(int port)
        {
            Port = port;
        }

        public int Port { get; }

        public  void Start(Action<Request, Response> onRequest)
        {

            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHost.AddressList[ipHost.AddressList.Length - 1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);

            this.server = new Socket(SocketType.Stream, ProtocolType.Tcp);

            server.Bind(localEndPoint);
            server.Listen(10);
            while (true)
            {
                this.clientSocket = server.Accept();
                byte[] bytes = new byte[1222333];

                int bytesRecive = clientSocket.Receive(bytes);
                if (bytesRecive == 0)
                {
                    server.Close();
                    continue;
                }

                var request = new Request((bytes, bytesRecive));
                var response = new Response("", new Dictionary<string, string>(), new NetworkStream(clientSocket));
                onRequest(request, response);
                clientSocket.Close();
            }
           
        }

    }
}