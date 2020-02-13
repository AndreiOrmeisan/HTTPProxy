
using System;
using System.Text;

namespace Server
{
    public class Program
    {
        static void Main()
        {
            var server = new ServerProxy(55212);

            server.Start((request, response) =>
            {
                var client = new WebClient(request.Host);
                client.OneResponse = (serverRespons) => 
                {
                    Console.WriteLine("Response received\r\n");
                    Console.WriteLine($"Content-Length: {serverRespons.ContentLength}\r\n");
                    serverRespons.OnData = (bodyData) =>
                    {
                        Console.WriteLine($"Send {bodyData.Count} from body");
                        response.Write(bodyData);
                    };

                    response.WriteHead(serverRespons.Status);
                    response.WriteHeaders(serverRespons.Headers);
                };
                try
                {
                    client.Connect();
                    client.WriteHead("GET", request.Url());
                    client.WriteHeaders(request.Headers);
                }
                catch
                {
                    response.Write(Encoding.UTF8.GetBytes("HTTP/1.1 400 Bad Request"));
                }
                //var headers = request.Headers;
                //headers.Remove("Accept-Encoding");
            });
        }
    }
}
