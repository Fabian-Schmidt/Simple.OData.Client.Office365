using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.ExampleA
{
    public static class IODataClientExtension
    {
        public static async Task<IEnumerable<IDictionary<string, object>>> FindEntriesOnAllPagesAsync(this Simple.OData.Client.IODataClient odataClient, string commandText)
        {
            var ret = new List<IDictionary<string, object>>();

            var annotations = new Simple.OData.Client.ODataFeedAnnotations();
            var result = await odataClient.FindEntriesAsync(commandText, annotations);
            do
            {
                ret.AddRange(result);

                result = null;
                if (annotations.NextPageLink != null)
                {
                    result = await odataClient.FindEntriesAsync(annotations.NextPageLink.OriginalString, annotations);
                }
            } while (result != null);

            return ret;
        }
    }

}
