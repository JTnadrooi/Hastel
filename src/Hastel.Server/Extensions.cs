using AsitLib.CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Hastel.Server
{
    public static class Extensions // Hastel does not qualify for per type extension classes yet
    {
        public static string AsCommand(this HttpListenerRequest request)
        {
            return AsCommand(request.Url!, request.HttpMethod, request);
        }

        public static string AsCommand(this Uri url, string method, HttpListenerRequest? request = null)
        {
            string path = url.AbsolutePath.Trim('/').Replace("/", " ") + "_" + method.ToLower();

            if (path.Length == method.Length + 1)
                throw new ArgumentException("Uri has no target.", nameof(url));

            List<string> args = new();

            // -------------------------
            // Query parameters
            // -------------------------
            var query = HttpUtility.ParseQueryString(url.Query);

            foreach (string? key in query.AllKeys)
            {
                if (key is null) continue;

                var values = query.GetValues(key);
                if (values is null || values.Length == 0) continue;

                args.Add($"--{ParseHelpers.GetSignature(key)} {string.Join(" ", values)}");
            }

            // -------------------------
            // Body parameters
            // -------------------------
            if (request != null &&
                request.HasEntityBody &&
                (method == "POST" || method == "PUT" || method == "PATCH"))
            {
                using var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8);
                string body = reader.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(body))
                {
                    string? contentType = request.ContentType;

                    // JSON
                    if (contentType?.Contains("application/json") is true)
                    {
                        var obj = JObject.Parse(body);

                        foreach (var prop in obj.Properties())
                            args.Add($"--{ParseHelpers.GetSignature(prop.Name)} {prop.Value}");
                    }
                    // form-urlencoded
                    else if (contentType?.Contains("application/x-www-form-urlencoded") is true)
                    {
                        var form = HttpUtility.ParseQueryString(body);

                        foreach (string? key in form.AllKeys)
                        {
                            if (key is null) continue;
                            args.Add($"--{ParseHelpers.GetSignature(key)} {form[key]}");
                        }
                    }
                }
            }

            return $"{path} {string.Join(" ", args)}".Trim();
        }
    }
}
