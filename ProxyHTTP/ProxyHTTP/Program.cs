using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Server
{
    class Program
    {
        static void Main()
        {
            Server();
        }

        static public void Server()
        {
            IPHostEntry ipHost = Dns.GetHostEntry("Andrei");
            IPAddress ipAddress = ipHost.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 55212);

            Socket server = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(localEndPoint);
            server.Listen(10);

            while (true)
            {
                Socket clientSocket = server.Accept();
                byte[] bytes = new byte[clientSocket.ReceiveBufferSize];

                int bytesCount = clientSocket.Receive(bytes);
                if (bytesCount == 0)
                {
                    clientSocket.Close();
                    continue;
                }

                string messenger = Encoding.ASCII.GetString(bytes, 0, bytesCount);

                string url = GetUrl(messenger);
                Console.WriteLine($"Url : { url}");
                Uri uri = new Uri(url);

                IPAddress[] ipAdres = Dns.GetHostAddresses(uri.Host);

                Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);


                int index = 0;
                while (index < ipAdres.Length)
                {
                    try
                    {
                        client.Connect(ipAdres[index], 80);
                    }
                    catch
                    {
                        index++;
                        Console.WriteLine("Invalid ipAddress");
                    }
                }


                client.Send(Encoding.UTF8.GetBytes(messenger));

                byte[] bufferClient = new byte[client.ReceiveBufferSize];
                int numByte = client.Receive(bufferClient);
                var a = Encoding.ASCII.GetString(bufferClient, 0, numByte);
                string header = GetHeader(Encoding.ASCII.GetString(bufferClient, 0, numByte));
                int length = GetLength(header);
                int headerLength = header.Length;
                int totalByte = numByte - header.Length;

                clientSocket.Send(bufferClient, numByte, SocketFlags.None);
                Console.WriteLine($"Bytes : {numByte}");
                Console.WriteLine($"ContentLength : {length}");



                try
                {
                    while (totalByte < length)
                    {
                        numByte = client.Receive(bufferClient);
                        var x = Encoding.ASCII.GetString(bufferClient, 0, numByte);
                        totalByte += numByte;
                        clientSocket.Send(bufferClient, numByte, SocketFlags.None);
                        Console.WriteLine($"Bytes : {numByte}");
                        Console.WriteLine($"TotalBytesSend : {totalByte}");
                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("Client socket closed by client");
                }
                finally
                {
                    clientSocket.Close();
                }
            }
        }

        private static int GetLength(string header)
        {
            if (header.Contains("Content-Length:"))
            {
                var indexOf = header.IndexOf("Content-Length:") + "Content - Length:".Length - 1;
                var substring = header.Substring(indexOf);

                if (substring.Length < 15)
                {
                    return Convert.ToInt32(substring);
                }

                var result = substring.Substring(0, substring.IndexOf("\r\n"));

                return Convert.ToInt32(result);

            }
            return 0;
        }

        private static string GetHeader(string messenger)
        {

            var indexStopHeader = messenger.IndexOf("\r\n\r\n") + "\r\n\r\n".Length;
            return messenger.Substring(0, indexStopHeader);
           
        }

        private static string GetUrl(string messenger)
        {
            var indexStartHost = messenger.IndexOf(" ") + 1;
            var indexStopHost = messenger.IndexOf("HTTP/1.1") - indexStartHost;
            var result = messenger.Substring(indexStartHost, indexStopHost);

            return result;

        }
    }
}
