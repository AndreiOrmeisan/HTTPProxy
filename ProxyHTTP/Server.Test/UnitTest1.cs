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
        public void Test1()
        {

            var request = new MemoryStream();
            request.Write(Encoding.UTF8.GetBytes(
                "HTTP"+"\r\n"+
                "1"+ "\r\n" +
                "2" + "\r\n" +
                "n" +"\r\n\r\n"));

            request.Position = 0;
            var test = new StreamRead(request);

            Assert.Equal("HTTP", test.ReadLine());
            Assert.Equal("1", test.ReadLine());
            Assert.Equal("2", test.ReadLine());
            Assert.Equal("n", test.ReadLine());
            Assert.Equal("", test.ReadLine());
            Assert.Equal("", test.ReadLine());
        }

        [Fact]
        public void Test2()
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

    }
}
