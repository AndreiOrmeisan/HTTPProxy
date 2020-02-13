
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
        private int startFind;
        private byte[] buffer;
        private int countRead;

        public StreamReader(Stream networkStream)
        {
            this.networkStream = networkStream;
            buffer = new byte[512];
            countRead = 0;
            startFind = 0;
        }

        public void Read()
        {
            var bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            countRead = bytesRead;
        }
        
        public byte[] ReadBytes()
        {
            var position = startFind;
            var previousCountRead = countRead;

            if (startFind < buffer.Length)
            {
                startFind = buffer.Length;
                return buffer[position..countRead];
            }

            Read();
            startFind = buffer.Length;
            return buffer[0..countRead];
        }

        public string ReadLine()
        {
            var endLine = Encoding.UTF8.GetBytes("\r\n");
            int end = Find(buffer[0..buffer.Length], endLine);
            var head = "";

            if (end == -1)
            {
                if (startFind != 0)
                {
                    head = Encoding.UTF8.GetString(buffer[startFind..]);
                    startFind = 0;
                }
                
                Read();
                end = Find(buffer[0..buffer.Length], endLine);
            }

            if (end == -1)
            {
                return Encoding.UTF8.GetString(buffer);
            }
            
            var result = Encoding.UTF8.GetString(buffer[startFind..end]);
            startFind = end + "\r\n".Length;

            return head + result;
        }
        public int Find(ArraySegment<byte> x, byte[] y)
        {
            for (int i = startFind; i <= x.Count - y.Length; i++)
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