using System;
using System.Net;
using System.Text;

namespace Hastel.Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();

            Console.WriteLine("Server running at http://localhost:5000/");
            Console.WriteLine("Press Ctrl+C to stop.");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                Console.WriteLine($"Received {request.HttpMethod} request for {request.Url}");

                string responseString = "Hello from Hastel Server!";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.AddHeader("Access-Control-Allow-Origin", "*");

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
    }
}
