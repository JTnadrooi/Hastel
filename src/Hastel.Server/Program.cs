using AsitLib;
using AsitLib.CommandLine;
using AsitLib.Diagnostics;
using Hastel.Server.CommandProviders;
using MySqlConnector;
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
        public static User? CurrentUser { get; set; } // single threaded server, marvelous.

        private const string Origin = "http://127.0.0.1:5000/";

        public static void Main(string[] args)
        {
            IRichLogger logger = new RichLogger();
            logger.Log($"created logger.");

            logger.Log($">starting server.");
            using HttpListener listener = new HttpListener();
            listener.Prefixes.Add(Origin);
            listener.Start();
            logger.Log($"<server running at '{Origin}'.");

            logger.Log($">connecting to db.");
            using MySqlConnection connection = new MySqlConnection("Server=127.0.0.1;Port=3306;Database=hastel;User=root;Password=;");
            QueryHelper.Connection = connection;
            connection.Open();
            logger.Log($"<connected to db.");

            string query = "SELECT * FROM users";

            foreach (User user in QueryHelper.QueryList(query, User.Map))
                Console.WriteLine($"UserID: {user.UserId}, Username: {user.Username}, Password: {user.Password}");

            logger.Log($">creating and populating commandengine.");
            CommandEngine engine = new CommandEngine();
            engine.Populate();
            logger.Log($"<found commands: '{engine.Commands.Select(c => c.Key).ToJoinedString(", ")}'");

            logger.Log("press Ctrl+C to stop.");

            logger.Log(">listening...");
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                string frontendOrigin = "http://127.0.0.1:3001";

                CurrentUser = request.Cookies["authToken"] is null ? null : User.FromToken(request.Cookies["authToken"]!.Value);

                logger.Log($"received {request.HttpMethod} request for {request.Url}");
                if (CurrentUser is not null)
                    logger.Log($"user logged in as '{CurrentUser.Username}'.");

                if (request.HttpMethod == "OPTIONS")
                {
                    logger.Log($">preflight request.");

                    response.AddHeader("Access-Control-Allow-Origin", frontendOrigin);
                    response.AddHeader("Access-Control-Allow-Credentials", "true");
                    response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                    response.AddHeader("Access-Control-Allow-Headers", "Content-Type");
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.OutputStream.Close();

                    logger.Log($"<responded with preflight OK.");
                    continue;
                }

                CommandResult commandResult = engine.Execute(request.AsCommand());
                byte[] buffer;

                if (commandResult.IsVoid)
                    buffer = Array.Empty<byte>();
                else if (commandResult.Value is CommandWebResponse commandResponse)
                {
                    buffer = Encoding.UTF8.GetBytes(commandResponse.JSON);
                    response.StatusCode = (int)commandResponse.StatusCode;

                    foreach (Cookie cookie in commandResponse.Cookies)
                        response.Cookies.Add(cookie);
                }
                else
                    buffer = Encoding.UTF8.GetBytes(commandResult.ToOutputString());

                response.AddHeader("Access-Control-Allow-Origin", frontendOrigin);
                response.AddHeader("Access-Control-Allow-Credentials", "true");

                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();

                logger.Log($"<responded with {(HttpStatusCode)response.StatusCode}.");
                Console.WriteLine($"```\n{Encoding.UTF8.GetString(buffer)}\n```");
            }
        }
    }
}
