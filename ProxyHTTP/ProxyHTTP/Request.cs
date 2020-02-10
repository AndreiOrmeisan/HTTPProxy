using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server
{
    public class Request
    {
        private string message;
        internal int numberBytes;
        internal byte[] content;
        private string status;

        public Request((byte[] bytes, int bytesRecive) dataRequested)
        {
            this.content = dataRequested.bytes;
            this.numberBytes = dataRequested.bytesRecive;
            this.message = Encoding.ASCII.GetString(content, 0, numberBytes);
        }

        public string Url()
        {
            var indexStartHost = message.IndexOf(" ") + 1;
            var indexStopHost = message.IndexOf("HTTP/1.1") - indexStartHost;
            var result = message.Substring(indexStartHost, indexStopHost);

            return result;
        }

        public Uri Uri => new Uri(Url());

        public int NumberOfBytes => numberBytes;

        public byte[] Content => content;

        public string Host=> Uri.Host;

        public Dictionary<string, string> Headers { get=>GetHeader(message);  }
        public string Status()
        {
            var stop = message.IndexOf("HTTP");
            var status = message[0..stop];
            return status;
        }

        private static Dictionary<string, string> GetHeader(string text)
        {
            var start = text.IndexOf("Host");
            var indexStopHeader = text.IndexOf("\r\n\r\n") + "\r\n\r\n".Length;
           
            var newtext = text.Substring(start, indexStopHeader - start).Split("\r\n");
            var headers = new Dictionary<string, string>();
            foreach (var header in newtext)
            {
                var index = header.IndexOf(":");
                if (index!=-1 && !headers.ContainsKey(header[0..index]))
                {
                    headers.Add(header[0..index], header[(index + 1)..]);
                }           
            }

            return headers;
        }
    }
}