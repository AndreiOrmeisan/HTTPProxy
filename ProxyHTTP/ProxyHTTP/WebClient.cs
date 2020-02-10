using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace Server
{
    internal class WebClient
    {

        private string host;
        private TcpClient client;
        private StreamWriter writer;
        private System.IO.StreamReader reader;

        public WebClient(string host)
        {
            this.client = new TcpClient();
            this.host = host;
        }
        public Action<Response> OnResponse { get; internal set; }

        internal void Connect()
        {

            client.Connect(host, 80);
            var stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new System.IO.StreamReader(stream);

        }

        internal void WriteHeaders(Dictionary<string, string> dictionary)
        {
            foreach (var pair in dictionary)
            {
                writer.Write($"{pair.Key}:{pair.Value}\r\n");
            }

            writer.Write("\r\n");
            writer.Flush();
            ReadResponse();
        }
        internal void WriteHead(string method, string path)
        {
            writer.Write($"{method} {path} HTTP/1.0\r\n");
            writer.Flush();
        }



        private void ReadResponse()
        {
            var stream = client.GetStream();
            var mystream = new StreamRead(stream);

            var status = mystream.ReadLine();
            var headers = new Dictionary<string, string>();      
            for (var read = mystream.ReadLine(); read!=""; read= mystream.ReadLine())
            {     
                var separator = read.IndexOf(":");
                if (!headers.ContainsKey((read[0..separator])))
                {
                    headers.Add(read[0..separator], read[(separator + 1)..]);
                }
            }

            var response = new Response(status, headers, mystream);
            OnResponse(response);
            reader.DiscardBufferedData();
        
            response.ReadBody();

        }



    }
}
