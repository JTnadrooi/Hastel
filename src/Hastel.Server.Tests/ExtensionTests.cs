using AwesomeAssertions;
using Moq;
using System;
using System.Net;
using Hastel.Server;

namespace Hastel.Server.Tests
{
    [TestClass]
    public sealed class ExtensionTests
    {
        [TestMethod]
        public void GetAsCommand_UriWithQueryParam()
        {
            Uri uri = new Uri("http://localhost/api/test?param1=value1");

            uri.GetAsCommand("GET").Should().Be("get_api-test --param1 value1");
        }

        [TestMethod]
        public void GetAsCommand_UriWithoutParams()
        {
            Uri uri = new Uri("http://localhost/api/test");

            uri.GetAsCommand("GET").Should().Be("get_api-test");
        }

        [TestMethod]
        public void GetAsCommand_UriWithArrayParam()
        {
            Uri uri = new Uri("http://localhost/api/test?param1=value1&param2=value2a&param2=value2b");

            uri.GetAsCommand("GET").Should().Be("get_api-test --param1 value1 --param2 value2a value2b");
        }
    }
}
