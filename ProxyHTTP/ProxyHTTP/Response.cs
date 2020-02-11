using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Response
    {
        private const int ChunkSize = 8192;
        private string status;
        private Dictionary<string, string> headers;
        private readonly Stream clientStream;
        private System.IO.StreamWriter writer;
        string a = "";
        public Response(string status, Dictionary<string, string> headers, Stream clientStream)
        {
            this.status = status;
            this.headers = headers;
            this.clientStream = clientStream;
            this.writer = new System.IO.StreamWriter(clientStream);
        }

        public Response(NetworkStream networkStream)
            : this("", new Dictionary<string, string>(), networkStream)
        { }

        public uint? ContentLength
                    => headers.ContainsKey("Content-Length") ? uint.TryParse(headers["Content-Length"], out uint value) ? (uint?)value : null : null;

        internal void WriteHead(string status) 
        {
            var writer = new StreamWriter(clientStream);
            writer.Write(status);
            writer.Write("\r\n");

            Console.WriteLine(status);
        }

        internal void WriteHeaders(IReadOnlyDictionary<string, string> headers)
        {
            var writer = new StreamWriter(clientStream);
            foreach (var element in headers)
            {
                writer.Write($"{element.Key}: {element.Value}");
                writer.Write("\r\n");
                Console.WriteLine($"{element.Key}: {element.Value}");
            }

            writer.Write("\r\n");
            Console.WriteLine("\r\n");
        }

        internal void Write(ArraySegment<byte> bodyData)
        {
            var writer = new StreamWriter(clientStream);
            a += Encoding.UTF8.GetString(bodyData);
            Console.WriteLine(Encoding.UTF8.GetString(bodyData));
            writer.WriteContent(bodyData);
        }

        public string Status => status;
        public IReadOnlyDictionary<string, string> Headers => this.headers;
        public Action<ArraySegment<byte>> OnData { get; set; }
        public void ReadBody(StreamReader stream)
        {
            var firstContent = stream.ReadBytes();
            OnData?.Invoke(firstContent);
            var total = firstContent.Length;

            while(total != 0)
            {
                var content = stream.ReadBytes();
                if (content != null)
                {
                    OnData?.Invoke(content);
                    total = content.Length;
                }
                else
                {
                    total = 0;
                }
                
            }

            Console.WriteLine($"Received : {total}\r\n");

        }
    }
}