using AwesomeAssertions;
using Moq;
using System;
using System.Net;
using Hastel.Server;
using static AwesomeAssertions.FluentActions;

namespace Hastel.Server.Tests
{
    [TestClass]
    public sealed class ExtensionTests
    {
        [TestMethod]
        public void GetAsCommand_UriWithQueryParam()
        {
            Uri uri = new Uri("http://localhost/api/test?param1=value1");

            uri.AsCommand("GET").Should().Be("api test_GET --param1 value1");
        }

        [TestMethod]
        public void GetAsCommand_UriWithoutParams()
        {
            Uri uri = new Uri("http://localhost/api/test");

            uri.AsCommand("GET").Should().Be("api test_GET");
        }

        [TestMethod]
        public void GetAsCommand_UriWithArrayParam()
        {
            Uri uri = new Uri("http://localhost/api/test?param1=value1&param2=value2a&param2=value2b");

            uri.AsCommand("GET").Should().Be("api test_GET --param1 value1 --param2 value2a value2b");
        }

        [TestMethod]
        public void GetAsCommand_UriWithoutPath_ThrowsEx()
        {
            Uri uri = new Uri("http://localhost/");

            Invoking(() => uri.AsCommand("GET")).Should().Throw<ArgumentException>();
        }
    }
}
