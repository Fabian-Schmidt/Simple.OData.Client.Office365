using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.ExampleA
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = "Foo";
            var password = "Bar";
            var url = "http://foobar.sharepoint.com/sites/pwa/";

            //Example 1: Read from Project Online: all projects with their id and name.
            var odataEndpoint = url + "_api/ProjectData/";
            var odataCommand = "Projects?$select=ProjectId,ProjectName";

            ////Example 2: Read form Project Online: one project all Milestones names.
            //var odataEndpoint = url + "_api/ProjectData/";
            //var odataCommand = "Projects(guid'32988321-c6be-e111-9f1e-00155d022681')/Tasks?$select=TaskName&$filter=TaskIsMilestone";

            ////Example 3: Read from SharePoint: all webs with their id, title and url.
            //var odataEndpoint = url + "_api/";
            //var odataCommand = "Web/Webs?$select=Id,Url,Title";

            ////Example 4: Read from SharePoint: all list items from the list 'All Risks'.
            //var odataEndpoint = url + "_api/";
            //var odataCommand = "Web/Lists/getbytitle('All Risks')/items";

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

            ////Read the first page.
            //var odataCommandResult = client.FindEntriesAsync(odataCommand).Result;

            // Use the extension method to read result form all pages.
            var odataCommandResult = client.FindEntriesOnAllPagesAsync(odataCommand).Result;

            foreach (var row in odataCommandResult)
            {
                foreach (var field in row)
                {
                    Console.WriteLine(field.Key + ":" + field.Value);
                }
                Console.WriteLine("----------------");
            }

        }
    }
}
