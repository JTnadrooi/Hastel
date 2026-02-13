using AsitLib.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server.CommandProviders
{
    public class GetCommandProvider : CommandGroup // for get requests
    {
        public GetCommandProvider() : base("get", null)
        {

        }

        [Command(".")]
        public string Capitalize(string word)
        {
            return word.ToUpper();
        }
    }
}
