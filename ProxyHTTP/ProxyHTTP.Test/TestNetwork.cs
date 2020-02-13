using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyHTTP
{
    public class TestNetwork : INetwork
    {
        private readonly byte[] content;
        private readonly int[] chunks;
        private int servedSize = 0;
        private int chunkIndex = 0;
        public TestNetwork(string content, params int[] chunks)
        {
            this.content = Encoding.UTF8.GetBytes(content);
            this.chunks = chunks;
        }
        public int Read(Span<byte> buffer)
        {
            int size = chunkIndex == chunks.Length ? content.Length - servedSize : chunks[chunkIndex++];
            var span = new Span<byte>(content, servedSize, size);
            span.CopyTo(buffer);
            servedSize += size;
            return size;
        }
        public void Write(ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }
    }
}

