using AsitLib;
using AsitLib.CommandLine;
using AsitLib.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;

namespace Hastel.Server
{
    public static class Program
    {
        private const string Prefix = "http://localhost:5000/";

        public static void Main(string[] args)
        {
            using HttpListener listener = new HttpListener();
            IRichLogger logger = new RichLogger();
            CommandEngine engine = new CommandEngine();
            engine.Populate();

            listener.Prefixes.Add(Prefix);
            listener.Start();

            logger.Log($"server running at {Prefix}");
            logger.Log($"found commands: '{engine.Commands.Select(c => c.Key).ToJoinedString(", ")}'");
            logger.Log("press Ctrl+C to stop.");

            logger.Log(">listening...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                logger.Log($"received {request.HttpMethod} request for {request.Url}");

                string responseString = engine.Execute(request.AsCommand()).ToOutputString();
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);

                response.AddHeader("Access-Control-Allow-Origin", "*");

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
    }
}
