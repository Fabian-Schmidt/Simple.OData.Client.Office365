using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.TDDTests.ODataMock
{
    //based on: https://github.com/object/Simple.OData.Client/blob/master/Simple.OData.Client.Tests.Core/TestBase.cs
    public class TestBase : IDisposable
    {
        protected readonly IODataClient _client;
        internal ISession _session;

        public TestBase(string metadataFile)
        {
            if (!string.IsNullOrEmpty(metadataFile))
            {
                _client = CreateClient(metadataFile);
            }
        }
        public IODataClient CreateClient(string metadataFile)
        {
            var urlBase = "http://localhost/" + metadataFile;
            var metadataString = GetResourceAsString(metadataFile);

            {
                //_session = Session.FromMetadata(urlBase, metadataString);

                var sessionType = typeof(ISession).Assembly.GetType("Simple.OData.Client.Session", true, false);
                var sessionType_FromMetdata = sessionType.GetMethod("FromMetadata", BindingFlags.Static | BindingFlags.NonPublic);
                _session = (ISession)sessionType_FromMetdata.Invoke(null, new object[] { urlBase, metadataString });
            }

            return new ODataClient(urlBase);
        }

        public void Dispose()
        {
        }

        public static string GetResourceAsString(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            string completeResourceName = resourceNames.FirstOrDefault(o => o.EndsWith("." + resourceName, StringComparison.CurrentCultureIgnoreCase));
            if (String.IsNullOrEmpty(completeResourceName))
                throw new ApplicationException("resource '" + resourceName + "' not found.");
            using (var resourceStream = assembly.GetManifestResourceStream(completeResourceName))
            {
                var reader = new StreamReader(resourceStream);
                return reader.ReadToEnd();
            }
        }

        public Microsoft.Data.OData.IODataResponseMessageAsync SetUpResourceMock(string resourceName)
        {
            var document = GetResourceAsString(resourceName);

            var mock = new Microsoft.Data.OData.Fakes.StubIODataResponseMessageAsync()
            {
                GetStreamAsync = () => Task.FromResult<Stream>(new MemoryStream(Encoding.UTF8.GetBytes(document))),
                GetStream = () => new MemoryStream(Encoding.UTF8.GetBytes(document)),
                GetHeaderString = (headerName) =>
                {
                    if (headerName == "Content-Type")
                        return "application/atom+xml; type=feed; charset=utf-8";
                    return null;
                }

            };
            return mock;
        }
    }
}
