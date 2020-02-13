
using System;
using System.IO;
using System.Text;
using System.Linq;
namespace Server
{
    public class StreamRead
    {
        private Stream stream;
        private byte[] buffer;
        private int startReading;
        private int lastElementIndex;
        private int bufferSize;
        private int headerStart;
        private int bodyStart;
        public StreamRead(Stream stream)
        {
            this.bufferSize = 512;
            this.stream = stream;
            this.buffer = new byte[0];
            this.startReading = 0;
            this.lastElementIndex = 0;
            this.headerStart = 0;
        }
        public string ReadLine()
        {
            int newLineAt = Find(buffer, Encoding.UTF8.GetBytes("\r\n"));
            while (newLineAt == -1)
            {
                Array.Resize(ref buffer, buffer.Length + bufferSize);
                Read();
                newLineAt = Find(buffer, Encoding.UTF8.GetBytes("\r\n"));
            }
            bodyStart = newLineAt + "rn".Length;
            return GetStringOut(buffer, newLineAt);
        }
        public byte[] ReadBody()
        {
            startReading = lastElementIndex;
            Read();
            var start = bodyStart;
            var end = lastElementIndex;
            lastElementIndex = 0;
            bodyStart = 0;
            return buffer[(start)..end];
        }
        private void Read()
        {
            lastElementIndex = stream.Read(buffer, startReading, buffer.Length - startReading);
            lastElementIndex += startReading;
            startReading = lastElementIndex;
        }
        private int Find(byte[] buffer, byte[] spliter)
        {
            for (int i = headerStart; i <= lastElementIndex - spliter.Length; i++)
            {
                bool find = true;
                for (int j = 0; j < spliter.Length; j++)
                {
                    if (buffer[i + j] != spliter[j])
                    {
                        find = false;
                    }
                }
                if (find)
                {
                    return i;
                }
            }
            return -1;
        }
        private string GetStringOut(byte[] buffer, int headerStop)
        {
            var start = headerStart;
            headerStart = headerStop + "rn".Length;
            return Encoding.UTF8.GetString(buffer[start..headerStop]);
        }
    }
}