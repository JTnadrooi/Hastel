using AsitLib.CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hastel.Server.CommandProviders
{
    public class TestCommandProvider : CommandGroup // for get requests
    {
        public TestCommandProvider() : base("test", null)
        {

        }

        [Command(".")]
        public string Capitalize_GET(string word)
        {
            return word.ToUpper();
        }
    }
}
