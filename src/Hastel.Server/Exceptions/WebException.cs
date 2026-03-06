using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server
{
    public class WebException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public WebException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            StatusCode = statusCode;
        }

        public WebException(string? message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message)
        {
            StatusCode = statusCode;
        }

        public WebException(string? message, Exception? innerException, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
