using System;
using System.Net;

namespace Hastel.Server
{
    public class CommandWebResponse
    {
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
        /// <summary>
        /// The request body. Does not automatically get converted to JSON.
        /// </summary>
        public string String { get; init; } = string.Empty;
        public Cookie[] Cookies { get; init; } = Array.Empty<Cookie>();

        /// <summary>
        /// <code>
        /// FromStatusCode(HttpStatusCode.OK);
        /// </code>
        /// </summary>
        public static CommandWebResponse Succes => FromStatusCode(HttpStatusCode.OK);

        public static CommandWebResponse FromStatusCode(HttpStatusCode statusCode, string msg = "")
            => new CommandWebResponse() { StatusCode = statusCode, String = msg };

        public static CommandWebResponse FromString(string str)
            => new CommandWebResponse() { String = str };

        public static CommandWebResponse FromException(Exception exception)
        {
            return exception switch
            {
                WebException webException => new CommandWebResponse()
                {
                    StatusCode = webException.StatusCode,
                    String = webException.Message,
                },
                _ => new CommandWebResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError, // normal exceptions might hold sensitive data.
                },
            };
        }
    }
}
