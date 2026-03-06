using System;
using System.Net;

namespace Hastel.Server
{
    public class CommandWebResponse
    {
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
        public string String { get; init; } = string.Empty;
        public Cookie[] Cookies { get; init; } = Array.Empty<Cookie>();

        public static CommandWebResponse FromStatusCode(HttpStatusCode statusCode)
            => new CommandWebResponse() { StatusCode = statusCode };

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
                    StatusCode = HttpStatusCode.InternalServerError,
                },
            };
        }
    }
}
