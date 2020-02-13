using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace Server
{
    internal class Headers
    {

        public Dictionary<string, string> headers;
        public Headers()
        {
           this.headers= new Dictionary<string, string>();
        }

        public Dictionary<string, string>  Content=>headers;

        internal bool ContainsKey(string key)
        {
            return headers.ContainsKey(key);
        }

        internal void Add(string key, string value)
        {
            headers.Add(key, value);
        }

        public string TransferEconding() 
        {
            string type = null;
            if (headers.ContainsKey("Content-Encoding"))
            {
                headers.TryGetValue("Content-Encoding", out type);
            }

            return type;
        }
        public int BodyLength()
        {
            string length = null;
            if (headers.ContainsKey("Content-Length"))
            {
               headers.TryGetValue("Content-Length", out length);
            }

            return Convert.ToInt32(length);
        }
    }
}