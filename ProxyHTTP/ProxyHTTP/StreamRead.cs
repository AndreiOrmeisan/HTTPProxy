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
        private int start;
        private int previous;
        private int sumBytes;
        private int bufferEnd;
        private bool stremEnd;
        private int bufferSize;

        public StreamRead(Stream stream)
        {
            this.bufferSize = 512;
            this.stream = stream;
            this.buffer = new byte[bufferSize];
            this.start = 0;
            this.previous = 0;
            this.stremEnd = false;
            Read();
        }
     
        private void Read()
        {
            buffer = buffer[start..];
            var buffer2 = new byte[bufferSize];
            buffer.CopyTo(buffer2, 0);
            previous = 0;
            start = 0;
            var from = 0;
            if (buffer.Length != bufferSize)
            {
                from = buffer.Length;
            }

            buffer = buffer2;
            
            bufferEnd = stream.Read(buffer, from, buffer.Length - from);
            if (bufferEnd == 0)
            {
                stremEnd = true;
            }
            buffer = buffer[0..bufferEnd];
        }

        public string ReadLine()
        {
           
            var spliter = Encoding.UTF8.GetBytes("\r\n");
            var endSpliter = Encoding.UTF8.GetBytes("\r\n\r\n");
            var headerEnd = Find(buffer, spliter, previous);
            var allHeders = Find(buffer, endSpliter, 0);
            previous = headerEnd + spliter.Length;

            if (headerEnd == -1 && allHeders == -1)
            {   
                Read();
                if (stremEnd)
                {
                    return "";
                }

                headerEnd = Find(buffer, spliter, previous);
                previous = headerEnd + spliter.Length;

            }

            if (allHeders != -1 && headerEnd == -1)
            {
                return "";
            }

            var header = Encoding.UTF8.GetString(buffer[start..headerEnd]);
            start = previous;
            return header;
        }

        private int Find(byte[] buffer, byte[] spliter, int previous)
        {
            for (int i = previous; i <= buffer.Length - spliter.Length; i++)
            {
                bool find = true;
                for (int j = 0; j < spliter.Length; j++)
                {
                    if (buffer[i+j] != spliter[j])
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


        internal byte[] Read(int chunkSize)
        {
            var returnBytes = new byte[] { };

            if (start != 0)
            {
                returnBytes = new byte[(buffer.Length - start)]; // tre vazut daca depaseste 
                buffer[start..].CopyTo(returnBytes, 0);
                start = 0;
                
                return returnBytes;
            }


            returnBytes = new byte[chunkSize];
            int count = stream.Read(returnBytes, 0, chunkSize);
            if (count != 0)
            {   
                var result2 = returnBytes[0..count];
                return result2;
            }
         
            return null; 
        }
    }
}