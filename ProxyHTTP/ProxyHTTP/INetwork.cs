using System;
using System.Collections.Generic;
using System.Text;

namespace ProxyHTTP
{
    public interface INetwork
    {
        void Write(ReadOnlySpan<byte> buffer);
        int Read(Span<byte> buffer);
    }
}
