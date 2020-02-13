using System;
using System.IO;
using System.Text;
using System.Linq;
using Xunit;
using System.Collections.Generic;

namespace Server.Test
{
    public class UnitTest1
    {
        [Fact]
        public void ReadHeadersAndBody()
        {

            var request = new MemoryStream();
            request.Write(Encoding.UTF8.GetBytes(
                "HTTP"+"\r\n"+
                "1"+ "\r\n" +
                "2" + "\r\n" +
                "n" +"\r\n\r\n"+
                "Error"));

            request.Position = 0;
            var test = new StreamRead(request);

            Assert.Equal("HTTP", test.ReadLine());
            Assert.Equal("1", test.ReadLine());
            Assert.Equal("2", test.ReadLine());
            Assert.Equal("n", test.ReadLine());
            Assert.Equal("", test.ReadLine());
            

            Assert.Equal("Error", Encoding.UTF8.GetString(test.ReadContent()));
        }

        [Fact]
        public void TestInOneBufferOnlyHeaders()
        {

            var request = new MemoryStream();
            request.Write(Encoding.UTF8.GetBytes(
                "1"+"\r\n"+
                "2" + "\r\n"));
            request.Position = 0;
            var test = new StreamRead(request);

            Assert.Equal("1", test.ReadLine());
            Assert.Equal("2", test.ReadLine());
        }

        [Fact]
        public void ReadLongHeaders()
        {

            var request = new MemoryStream();
            request.Write(Encoding.UTF8.GetBytes(
                new string('H', 300) +
                "\r\n" +
                "1" + "\r\n" +
                "2" + "\r\n" +
                "n" + "\r\n\r\n" +
                "Error"));

            request.Position = 0;
            var test = new StreamRead(request);

            Assert.Equal(new string('H',300), test.ReadLine());
            Assert.Equal("1", test.ReadLine());
            Assert.Equal("2", test.ReadLine());
            Assert.Equal("n", test.ReadLine());
            Assert.Equal("", test.ReadLine());


            Assert.Equal("Error", Encoding.UTF8.GetString(test.ReadContent()));
        }

        [Fact]
        public void ReadLongBody()
        {
            var request = new MemoryStream();
            request.Write(Encoding.UTF8.GetBytes(
                "Header" +
                "\r\n" +
                "1" + "\r\n" +
                "2" + "\r\n" +
                "n" + "\r\n\r\n" +
                new string('E', 700)));

            request.Position = 0;
            var test = new StreamRead(request);

            Assert.Equal("Header", test.ReadLine());
            Assert.Equal("1", test.ReadLine());
            Assert.Equal("2", test.ReadLine());
            Assert.Equal("n", test.ReadLine());
            Assert.Equal("", test.ReadLine());


            Assert.Equal(new string('E', 493),
                Encoding.UTF8.GetString(test.ReadContent()));
            Assert.Equal(new string('E', 207),
                Encoding.UTF8.GetString(test.ReadContent()));
        }

        [Fact]
        public void ReadBodyInTwo()
        {

            var request = new MemoryStream();
            request.Write(Encoding.UTF8.GetBytes(
                "Header" +
                "\r\n" +
                "1" + "\r\n" +
                "2" + "\r\n" +
                "n" + "\r\n\r\n" +
                new string('E', 1005) + new string('O',295)));

            request.Position = 0;
            var test = new StreamRead(request);

            Assert.Equal("Header", test.ReadLine());
            Assert.Equal("1", test.ReadLine());
            Assert.Equal("2", test.ReadLine());
            Assert.Equal("n", test.ReadLine());
            Assert.Equal("", test.ReadLine());


            Assert.Equal(new string('E', 493),
                Encoding.UTF8.GetString(test.ReadContent()));
            Assert.Equal(new string('E', 512),
              Encoding.UTF8.GetString(test.ReadContent()));
            Assert.Equal(new string('O', 295),
             Encoding.UTF8.GetString(test.ReadContent()));
        }

        
    }
}
