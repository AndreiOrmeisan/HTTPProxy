using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Response
    {
        private const int ChunkSize = 512;
        private string status;
        private Dictionary<string, string> headers;
        private StreamWriter writer;

        public Response(string status, Dictionary<string, string> headers, Stream clientStream)
        {
            this.status = status;
            this.headers = headers;
            this.writer = new StreamWriter(clientStream);
        }

        public Response(NetworkStream networkStream)
            : this("", new Dictionary<string, string>(), networkStream)
        { }

        public uint? ContentLength
                    => headers.ContainsKey("Content-Length") ? uint.TryParse(headers["Content-Length"], out uint value) ? (uint?)value : null : null;
        internal void WriteHead(string status)
        {
            writer.Write(status + "\r\n");
            Console.WriteLine(status);
        }

        internal void WriteHeaders(IReadOnlyDictionary<string, string> headers)
        {
            if (headers.ContainsKey("Content-Encoding"))
            {
                Console.WriteLine("Andrei");
            }
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
            writer.WriteContent(bodyData);
        }

        public string Status => status;
        public IReadOnlyDictionary<string, string> Headers => this.headers;
        public Action<ArraySegment<byte>> OnData { get; set; }
        public void ReadBody(StreamReader myRead)
        {
            if (ContentLength != null)
            {
                var total = 0;
                for (var buffer = myRead.ReadBytes(); total < ContentLength.Value; buffer = myRead.ReadBytes())
                {
                    total += buffer.Length;
                    OnData?.Invoke(buffer);
                    Console.WriteLine(total);
                }
            }
        }
    }
}