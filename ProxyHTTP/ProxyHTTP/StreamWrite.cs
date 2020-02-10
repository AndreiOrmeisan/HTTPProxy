using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    internal class StreamWrite
    {
        private Stream clientStream;
        private StreamWriter clientStreamWriter;

        public StreamWrite(Stream clientStream)
        {
            this.clientStream = clientStream;
            this.clientStreamWriter = new StreamWriter(clientStream);
        }

        internal void WriteHead(string statusCode)
        {
            clientStreamWriter.Write(statusCode + "\r\n");
            Console.Write(statusCode + "\r\n");
        }

        internal void WriteHeaders(IReadOnlyDictionary<string, string> headers)
        {
            foreach (var head in headers)
            {

                clientStreamWriter.Write($"{head.Key}:{head.Value}\r\n");
                Console.Write($"{head.Key}:{head.Value}\r\n");
            }

            clientStreamWriter.Write("\r\n");
            Console.Write("\r\n");
        }

        internal void WriteBody(byte[] body)
        {
            var diplayBody = Encoding.ASCII.GetString(body, 0, body.Length);
            clientStream.Write(body);

            Console.Write(diplayBody);
        }
    }
}