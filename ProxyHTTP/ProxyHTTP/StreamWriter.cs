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
            try
            {
                clientStream.Write(Encoding.UTF8.GetBytes(text));
            }
            catch
            {

            }
        }

        internal void WriteContent(ArraySegment<byte> bodyData)
        {
            try
            {
                clientStream.Write(bodyData);
                Console.WriteLine(Encoding.UTF8.GetString(bodyData));
            }
            catch
            {

            }
        }
    }
}