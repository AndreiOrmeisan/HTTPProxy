using System;
using System.IO;
using System.Text;

namespace Server
{
    internal class StreamWriter
    {
        Stream clientStream;
        public StreamWriter(Stream clientStream)
        {
            this.clientStream = clientStream;
        }

        internal void Write(string text)
        {
            clientStream.Write(Encoding.UTF8.GetBytes(text));
        }

        internal void WriteContent(ArraySegment<byte> bodyData)
        {
            clientStream.Write(bodyData);      
        }
    }
}