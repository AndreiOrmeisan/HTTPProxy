using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    public class Response
    {
        private const int ChunkSize = 512;

        private readonly string status;
        private Dictionary<string, string> headers;
        private readonly Stream clientStream;
        private  int countBody;
        private StreamRead mystream;
        private StreamWrite clientWriter;
        public Response(string status, Dictionary<string, string> headers, Stream clientStream)
        {
            this.status = status;
            this.headers = headers;
            this.clientStream = clientStream;
            this.countBody = 0;
            this.clientWriter = new StreamWrite(clientStream);
        }

        public Response(string status, Dictionary<string, string> headers, StreamRead mystream)
        {
            this.status = status;
            this.headers = headers;
            this.mystream = mystream;
        }

        public uint? ContentLength
            => headers.ContainsKey("Content-Length") ? uint.TryParse(headers["Content-Length"], out uint value) ? (uint?)value : null : null;

        internal void WriteHeaders(IReadOnlyDictionary<string, string> headers)
        {
            clientWriter.WriteHeaders(headers);
        }

        internal void WriteHead(string statusCode)
        {
            clientWriter.WriteHead(statusCode);  
        }

        internal void Write(ArraySegment<byte> bodyData)
        {
            clientWriter.WriteBody(bodyData.ToArray()); 
        }

        public string Status => status;

        public IReadOnlyDictionary<string, string> Headers => headers;

        public Action<ArraySegment<byte>> OnData { get; set; }
        public string StatusCode =>status;  

        public void ReadBody()
        {
            var countBody = 0;
            for (var buffer = mystream.ReadContent() ; countBody < ContentLength; buffer = mystream.ReadContent())
            {           
                OnData?.Invoke(buffer);
                countBody += buffer.Length;
            }

            
        }
    }
    
}