using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server
{
    public static class ThrowHelper
    {
        /// <summary>
        /// Throws if user is not logged in as the username passed to this methods. See <see cref="Program.CurrentUser"/>.
        /// </summary>
        public static void ThrowIfInvalidLogin(string username) // should only be thrown when someone is doing something malicious.
        {
            if (Program.CurrentUser is null || Program.CurrentUser.Username != username) throw new WebException(HttpStatusCode.Unauthorized);
        }
    }
}
