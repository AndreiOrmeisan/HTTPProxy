using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class StreamReader
    {
        private Stream networkStream;
        private int index = 0;
        private byte[] buffer;

        public StreamReader(Stream networkStream)
        {
            this.networkStream = networkStream;
            buffer = new byte[512];
            Read();
        }

        public void Read()
        {
            var copyBuffer = new byte[0]; 
            if (buffer.Length != 512)
            {
                copyBuffer = buffer;
                buffer = new byte[512];
                copyBuffer.CopyTo(buffer, 0);
            }

            var bytesRead = networkStream.Read(buffer, copyBuffer.Length, buffer.Length - copyBuffer.Length);
            buffer = buffer[0..bytesRead];
        }
        
        public byte[] ReadBytes()
        {
            buffer = buffer["\r\n".Length..];

            if (index != 0)
            {
                Read();
                return buffer;
            }

            index++;
            return buffer;
        }

        public string ReadLine()
        {
            var endLine = "\r\n".Select(c => (byte)c).ToArray();
            int end = Find(buffer[0..buffer.Length], endLine);

            if (end == -1)
            {
                Read();
                end = Find(buffer[0..buffer.Length], endLine);
                return Encoding.UTF8.GetString(buffer[0..end]);
            }

            if (end == 0)
            {
                return "";
            }

            var result = Encoding.UTF8.GetString(buffer[0..end]);
            buffer = buffer[(end + "\r\n".Length)..];

            return result;
        }
        public int Find(ArraySegment<byte> x, byte[] y)
        {
            for (int i = 0; i <= x.Count - y.Length; i++)
            {
                var all = true;
                for (int j = 0; j < y.Length && all; j++)
                {
                    if (x[i + j] != y[j]) all = false;
                }
                if (all)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}