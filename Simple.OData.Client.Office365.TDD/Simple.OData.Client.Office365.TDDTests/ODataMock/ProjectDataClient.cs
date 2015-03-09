using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.TDDTests.ODataMock
{
    class ProjectDataClient : TestBase
    {
        //example from: https://github.com/object/Simple.OData.Client/blob/master/Simple.OData.Client.Tests.Core/ResponseReaderTests.cs
        public ProjectDataClient()
            : base(ProjectData.Mapping.ODataCommandTextToResourceName("$metadata"))
        {
            base._session.Settings.IgnoreUnmappedProperties = true;
        }

        public async Task<IDictionary<string, object>> FindEntryAsync(string commandText)
        {
            var response = SetUpResourceMock(ProjectData.Mapping.ODataCommandTextToResourceName(commandText));
            var responseReader = new Simple.OData.Client.V3.Adapter.ResponseReader(base._session, await _client.GetMetadataAsync<Microsoft.Data.Edm.IEdmModel>());
            var result = (await responseReader.GetResponseAsync(response)).Entry;
            return result;
        }

        public async Task<IEnumerable<IDictionary<string, object>>> FindEntriesAsync(string commandText, ODataFeedAnnotations annotations)
        {
            var response = SetUpResourceMock(ProjectData.Mapping.ODataCommandTextToResourceName(commandText));
            var responseReader = new Simple.OData.Client.V3.Adapter.ResponseReader(base._session, await _client.GetMetadataAsync<Microsoft.Data.Edm.IEdmModel>());
            var result = await responseReader.GetResponseAsync(response);

            var annotations_CopyFrom = annotations.GetType().GetMethod("CopyFrom", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            annotations_CopyFrom.Invoke(annotations, new object[] { result.Annotations });
            //annotations.Count = result.Annotations.Count;
            //annotations.DeltaLink = result.Annotations.DeltaLink;
            //annotations.Id = result.Annotations.Id;
            //annotations.InstanceAnnotations = result.Annotations.InstanceAnnotations;
            //annotations.NextPageLink = result.Annotations.NextPageLink;
            return result.Entries;
        }

        public Task<object> FindScalarAsync(string commandText)
        {
            var response = GetResourceAsString(ProjectData.Mapping.ODataCommandTextToResourceName(commandText));
            return Task.FromResult<object>(response);
        }
    }
}
