using ProxyHTTP;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Xunit;

namespace Server.Test
{
    public class ReadStreamFact
    {
        [Fact]
        public void ReadLine()
        {
            var text = "HTTP/1.1 200 OK\r\n" +
                "Date: Mon, 10 Feb 2020 08:26:32 GMT\r\n" +
                "Server: Apache\r\n";

            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(text));
            stream.Position = 0;
            var reader = new StreamRead(stream);

            Assert.Equal("HTTP/1.1 200 OK", reader.ReadLine());
            Assert.Equal("Date: Mon, 10 Feb 2020 08:26:32 GMT", reader.ReadLine());
            Assert.Equal("Server: Apache", reader.ReadLine());
        }

        [Fact]
        public void ReadLine2()
        {
            var text = "HTTP/1.1 200 OK\r\n" +
                "Date: Mon, 10 Feb 2020 08:26:32 GMT\r\n" +
                "Server: Apache\r\n\r\n";

            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(text));
            stream.Position = 0;
            var reader = new StreamRead(stream);

            Assert.Equal("HTTP/1.1 200 OK", reader.ReadLine());
            Assert.Equal("Date: Mon, 10 Feb 2020 08:26:32 GMT", reader.ReadLine());
            Assert.Equal("Server: Apache", reader.ReadLine());
            Assert.Equal("", reader.ReadLine());
        }

        [Fact]
        public void ReadLineAndFinishHeaders()
        {
            var text = "HTTP/1.1 200 OK\r\n" +
                "Date: Mon, 10 Feb 2020 08:26:32 GMT\r\n" +
                "\r\n" +
                "Andrei";

            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(text));
            stream.Position = 0;
            var reader = new StreamRead(stream);

            Assert.Equal("HTTP/1.1 200 OK", reader.ReadLine());
            Assert.Equal("Date: Mon, 10 Feb 2020 08:26:32 GMT", reader.ReadLine());
            reader.ReadLine();
            Assert.Equal(Encoding.UTF8.GetBytes("Andrei"), reader.ReadBody());
        }

        [Fact]
        public void HeadersLenghtIsBiggerThan512()
        {
            var headers = new string('a', 600) + "\r\n\r\n";

            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(headers));
            stream.Position = 0;
            var reader = new StreamRead(stream);

            Assert.Equal(new string('a',512), reader.ReadLine());
            Assert.Equal(new string('a', 88), reader.ReadLine());
        }

        [Fact]
        public void HeadersLenghtIsBiggerThan512AndReadContent()
        {
            var headers = new string('a', 600) + "\r\n\r\n";
            var content = new string('a', 700);

            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(headers));
            stream.Write(Encoding.UTF8.GetBytes(content));
            stream.Position = 0;
            var reader = new StreamRead(stream);

            Assert.Equal(new string('a', 512), reader.ReadLine());
            Assert.Equal(new string('a', 88), reader.ReadLine());
            reader.ReadLine();
            Assert.Equal(new string('a', 420), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 280), Encoding.UTF8.GetString(reader.ReadBody()));
        }

        [Fact]
        public void ComplexHeadersAndContent()
        {
            var headers = new string('a', 600) + "\r\n\r\n";
            var content = new string('a', 3000);

            var stream = new MemoryStream();
            stream.Write(Encoding.UTF8.GetBytes(headers));
            stream.Write(Encoding.UTF8.GetBytes(content));
            stream.Position = 0;
            var reader = new StreamRead(stream);

            Assert.Equal(new string('a', 512), reader.ReadLine());
            Assert.Equal(new string('a', 88), reader.ReadLine());
            reader.ReadLine();
            Assert.Equal(new string('a', 420), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 512), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 512), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 512), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 512), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 512), Encoding.UTF8.GetString(reader.ReadBody()));
            Assert.Equal(new string('a', 20), Encoding.UTF8.GetString(reader.ReadBody()));
        }
    }
}
