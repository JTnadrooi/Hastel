using AsitLib.CommandLine;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server.CommandProviders
{
    public class CommandWebResponse
    {
        public HttpStatusCode StatusCode { get; init; }
        public string JSON { get; init; } = string.Empty;
        public Cookie[] Cookies { get; init; } = Array.Empty<Cookie>();

        public static CommandWebResponse FromStatusCode(HttpStatusCode statusCode)
            => new CommandWebResponse() { StatusCode = statusCode };
    }

    public class AuthCommandProvider : CommandGroup
    {
        public AuthCommandProvider() : base("auth")
        {

        }

        [Command(".")]
        public CommandWebResponse Login_POST(string username, string password)
        {
            if (QueryHelper.TryQuerySingle("SELECT userid, username, password FROM users WHERE username = @username", User.Map, out User? user, new MySqlParameter("@username", username))
                    && user.Password == password)
            {
                string token = user.GetToken();

                return new CommandWebResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    JSON = JsonConvert.SerializeObject(new { token = token }),
                    Cookies = [new Cookie("authToken", token)
                    {
                        HttpOnly = true,
                        Path = "/"
                    }]
                };
            }
            else
            {
                return CommandWebResponse.FromStatusCode(HttpStatusCode.Unauthorized);
            }
        }
    }
}
