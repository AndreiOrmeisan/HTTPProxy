using System;
using System.Collections.Generic;

namespace Server
{
    internal class Request
    {
        private string message;

        public Request(string message)
        {
            this.message = message;
        }

        public string Host { get => new Uri(Url()).Host;}
        public Dictionary<string,string> Headers { get => Header();}
        public string Head { get => message.Substring(0, message.IndexOf("\r\n") + "\r\n".Length); }

        internal string Url()
        {
            var indexStartHost = message.IndexOf(" ") + 1;
            var indexStopHost = message.IndexOf("HTTP/1.1") - indexStartHost;
            var result = message.Substring(indexStartHost, indexStopHost);
            return result;
        }

        internal Dictionary<string,string> Header()
        {
            var dictionary = new Dictionary<string, string>();
            var a = message;
            a = a.Substring(a.IndexOf("\r\n") + "\r\n".Length);

            while (a != string.Empty)
            {
                var substring = a.Substring(0, a.IndexOf("\r\n") + "\r\n".Length);
                var separator = a.IndexOf(":");

                if (separator > -1) 
                {
                    dictionary.Add(substring[0..separator], substring[(separator + 1)..].Trim());

                }
                a = a.Substring(a.IndexOf("\r\n") + "\r\n".Length);
            }

            return dictionary;
        }
    }
}