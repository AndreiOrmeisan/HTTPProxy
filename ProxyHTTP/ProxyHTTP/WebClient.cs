using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class WebClient
    {
        private string host;
        TcpClient client;
        System.IO.StreamWriter writer;
        System.IO.StreamReader reader;

        public WebClient(string host)
        {
            client = new TcpClient();
            this.host = host;
        }

        public Action<Response> OneResponse { get; internal set; }

        public void Connect()
        {
            client.Connect(host, 80);
            var stream = client.GetStream();
            writer = new System.IO.StreamWriter(stream);
            reader = new System.IO.StreamReader(stream);
        }
        
        internal void WriteHead(string method, string url)
        {
            writer.Write($"{method} {url} HTTP/1.1\r\n");
            writer.Flush();
        }

        internal void WriteHeaders(Dictionary<string, string> dictionary)
        {
            foreach (var pair in dictionary)
            {
                writer.Write($"{pair.Key}: {pair.Value}\r\n");
            }

            writer.Write("\r\n");
            writer.Flush();
            ReadResponse();
        }

        private void ReadResponse()
        {
            var myRead = new StreamRead(client.GetStream());
            var headers = new Dictionary<string, string>();
            var status = myRead.ReadLine();

            for (var header = myRead.ReadLine(); !string.IsNullOrEmpty(header); header = myRead.ReadLine())
            {
                if (header[0] == 0)
                {
                    break;
                }
                var separator = header.IndexOf(":");
                if (!headers.ContainsKey((header[0..separator])))
                {
                    headers.Add(header[0..separator], header[(separator + 1)..]);
                }
            }

            var response = new Response(status, headers, client.GetStream());

            OneResponse(response);
            response.ReadBody(myRead);
        }
    }
}
