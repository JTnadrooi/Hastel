using AsitLib.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Hastel.Server
{
    public static class Program
    {
        public const int Latency = 500; // ms.

        public static void Main(string[] args)
        {
            using HttpListener listener = new HttpListener();
            IRichLogger logger = new RichLogger();

            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();

            logger.Log("server running at http://localhost:5000/");
            logger.Log("press Ctrl+C to stop.");

            while (true)
            {
                Thread.Sleep(Latency);

                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                logger.Log($"received {request.HttpMethod} request for {request.Url}");

                string responseString = "Ahoy from Hastel Server!";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.AddHeader("Access-Control-Allow-Origin", "*");

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
    }
}
