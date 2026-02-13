using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static string GetAsCommand(this HttpListenerRequest request)
        {
            return GetAsCommand(request.Url!, request.HttpMethod);
        }

        public static string GetAsCommand(this Uri url, string method)
        {
            string path = url.AbsolutePath.Trim('/').Replace("/", " ") + "_" + method.ToLower();

            if (path.Length == method.Length + 1) throw new ArgumentException("Uri has no target.", nameof(url));

            List<string> args = new List<string>();

            NameValueCollection queryParams = HttpUtility.ParseQueryString(url.Query);

            foreach (string? key in queryParams.AllKeys)
            {
                if (key is null) continue;

                string[]? values = queryParams.GetValues(key);
                if (values is null || values.Length == 0) continue;

                string joinedValues = string.Join(" ", values);
                args.Add($"--{key} {joinedValues}");
            }

            string command = $"{path} {string.Join(" ", args)}";
            return command.Trim();
        }
    }
}
