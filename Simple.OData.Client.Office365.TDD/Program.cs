using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.TDD
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = "Foo";
            var password = "Bar";
            var url = "http://foobar.sharepoint.com/sites/pwa/";

            var odataEndpoint = url + "_api/ProjectData";

            var secureString = new System.Security.SecureString();
            foreach (char c in password.ToCharArray())
            {
                secureString.AppendChar(c);
            }
            var credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(username, secureString);
            var authCookieValue = credentials.GetAuthenticationCookie(new System.Uri(url));

            var settings = new Simple.OData.Client.ODataClientSettings();
            settings.UrlBase = odataEndpoint;
            //JSON has only 1/4 of the size of Atom (XML).
            settings.PayloadFormat = Simple.OData.Client.ODataPayloadFormat.Json;
            settings.IgnoreUnmappedProperties = true;
            settings.OnApplyClientHandler = (System.Net.Http.HttpClientHandler clientHandler) =>
            {
                //Deactivate cookie handling to be able to set my own one.
                clientHandler.UseCookies = false;
            };
            settings.BeforeRequest = (System.Net.Http.HttpRequestMessage request) =>
            {
                request.Headers.Add("Cookie", authCookieValue);
            };
            var client = new Simple.OData.Client.ODataClient(settings);

            var BusinessLogicA = new BuisnessLogic.BuisnessClassA();
            foreach (var project in BusinessLogicA.ReadAllProjects(client).Result)
            {
                Console.WriteLine(project.Item1.ToString() + ":" + project.Item2);
            }
        }
    }
}
