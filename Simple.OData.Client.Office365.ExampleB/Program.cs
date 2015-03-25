using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.ExampleB
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = "Foo";
            var password = "Bar";
            var url = "http://foobar.sharepoint.com/sites/pwa/";

            var authCookieValue = CreateAuthenticationCookie(username, password, url);

            Console.WriteLine("Reading all data without optimization.");
            MeasureTask(async () => { await BasicRead(url, authCookieValue); });

            Console.WriteLine("Reading all data as JSON.");
            MeasureTask(async () => { await JSONRead(url, authCookieValue); });

            Console.WriteLine("Reading filered data as JSON.");
            MeasureTask(async () => { await JSONFilteredRead(url, authCookieValue); });

            Console.WriteLine("Reading all data as JSON at the same time.");
            MeasureTask(async () => { await JSONAllPagesRead(url, authCookieValue); });

            Console.ReadLine();
        }

        /// <summary>
        /// Basic time measuring function
        /// </summary>
        /// <param name="function"></param>
        private static void MeasureTask(Func<Task> function)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var t = Task.Factory.StartNew(function).Unwrap();
            t.Wait();
            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}.{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        private static async Task BasicRead(string url, string authCookieValue)
        {
            var odataEndpoint = url + "_api/ProjectData";
            var odataCommand = "Tasks";

            var settings = new Simple.OData.Client.ODataClientSettings();
            settings.UrlBase = odataEndpoint;
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

            try
            {
                var odataCommandResult = await client.FindEntriesOnAllPagesAsync(odataCommand);
                Console.WriteLine("Read {0:0000} entities.", odataCommandResult.Count());
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerExceptions[0].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task JSONRead(string url, string authCookieValue)
        {
            var odataEndpoint = url + "_api/ProjectData";
            var odataCommand = "Tasks";

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

            try
            {
                var odataCommandResult = await client.FindEntriesOnAllPagesAsync(odataCommand);
                Console.WriteLine("Read {0:0000} entities.", odataCommandResult.Count());
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerExceptions[0].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task JSONFilteredRead(string url, string authCookieValue)
        {
            var odataEndpoint = url + "_api/ProjectData";
            var odataCommand = "Tasks?$filter=TaskIsMilestone";

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

            try
            {
                var odataCommandResult = await client.FindEntriesOnAllPagesAsync(odataCommand);
                Console.WriteLine("Read {0:0000} entities.", odataCommandResult.Count());
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerExceptions[0].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task JSONAllPagesRead(string url, string authCookieValue)
        {
            var odataEndpoint = url + "_api/ProjectData";

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

            try
            {
                var odataCommandResult = await client
                    .FindScalarAsync("Tasks/$count")
                    .ContinueWith(async (countEntitesTask) =>
                    {
                        var ret = new List<IDictionary<string, object>>();

                        var count = Int32.Parse(countEntitesTask.Result.ToString());
                        var pageSize = 100d;
                        var numberofPages = (int)Math.Ceiling(count / pageSize);
                        if (numberofPages > 0)
                        {
                            var subTasks = new Task<IEnumerable<IDictionary<string, object>>>[numberofPages];
                            for (int i = 0; i < numberofPages; i++)
                            {
                                subTasks[i] = client.FindEntriesOnAllPagesAsync(String.Format("Tasks?$top=100&$skip={0}", i * pageSize));
                            }
                            await Task.WhenAll(subTasks);
                            for (int i = 0; i < numberofPages; i++)
                            {
                                ret.AddRange(subTasks[i].Result);
                            }
                        }

                        return ret;
                    })
                    .Unwrap();

                Console.WriteLine("Read {0:0000} entities.", odataCommandResult.Count());
            }
            catch (AggregateException ex)
            {
                Console.WriteLine(ex.InnerExceptions[0].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static string CreateAuthenticationCookie(string username, string password, string url)
        {
            var secureString = new System.Security.SecureString();
            foreach (char c in password.ToCharArray())
            {
                secureString.AppendChar(c);
            }
            var credentials = new Microsoft.SharePoint.Client.SharePointOnlineCredentials(username, secureString);
            var authCookieValue = credentials.GetAuthenticationCookie(new System.Uri(url));
            return authCookieValue;
        }
    }
}
