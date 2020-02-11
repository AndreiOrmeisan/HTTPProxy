
using System;

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
                        Console.WriteLine($"Received {bodyData.Count} from body");
                        serverRespons.Write(bodyData);
                    };

                    response.WriteHead(serverRespons.Status);
                    response.WriteHeaders(serverRespons.Headers);
                    
                };
                client.Connect();
                client.WriteHead("GET", request.Url());
                var headers = request.Headers;
                headers.Remove("Accept-Encoding");
                client.WriteHeaders(headers);
            });
        }
    }
}
