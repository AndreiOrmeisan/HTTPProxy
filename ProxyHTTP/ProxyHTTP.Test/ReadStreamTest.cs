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
            var reader = new StreamReader(stream);

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
            var reader = new StreamReader(stream);

            Assert.Equal("HTTP/1.1 200 OK", reader.ReadLine());
            Assert.Equal("Date: Mon, 10 Feb 2020 08:26:32 GMT", reader.ReadLine());
            Assert.Equal("Server: Apache", reader.ReadLine());
            Assert.Equal("", reader.ReadLine());
            Assert.Equal("", reader.ReadLine());
            Assert.Equal("", reader.ReadLine());
        }

        /*    [Fact]
            public void ReadHeader()
            {
                var text = "HTTP/1.1 200 OK\r\n\r\n";

                var stream = new MemoryStream();
                stream.Write(Encoding.UTF8.GetBytes(text));
                stream.Position = 0;
                var reader = new StreamReader(stream);

                Assert.Equal("HTTP/", reader.ReadLine());
                Assert.Equal("1.1 2", reader.ReadLine());
                Assert.Equal("00 OK", reader.ReadLine());
            }
        */
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
            var reader = new StreamReader(stream);

            Assert.Equal("HTTP/1.1 200 OK", reader.ReadLine());
            Assert.Equal("Date: Mon, 10 Feb 2020 08:26:32 GMT", reader.ReadLine());
            reader.ReadLine();
            Assert.Equal(Encoding.UTF8.GetBytes("Andrei"), reader.ReadBytes());
        }

    }
}
