using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.TDD.BuisnessLogic
{
    public class BuisnessClassA
    {
        public async Task<IEnumerable<Tuple<Guid, String>>> ReadAllProjects(IODataClient client)
        {
            var ret = new List<Tuple<Guid, String>>();
            var odataResult = await client.FindEntriesOnAllPagesAsync("Projects?$select=ProjectId,ProjectName");
            foreach (var item in odataResult)
            {
                ret.Add(new Tuple<Guid, String>((Guid)item["ProjectId"], (String)item["ProjectName"]));
            }
            return ret;
        }

        public async Task<IEnumerable<Tuple<Guid, Guid>>> ReadAllAssignments(IODataClient client, Guid ProjectId)
        {
            var ret = new List<Tuple<Guid, Guid>>();

            var projectIdString = ProjectId.ToString("D");
            var odataResult = await client.FindEntriesOnAllPagesAsync(String.Format("Projects(guid'{0}')?Assignments", projectIdString));
            foreach (var item in odataResult)
            {
                ret.Add(new Tuple<Guid, Guid>((Guid)item["AssignmentId"], (Guid)item["ResourceId"]));
            }
            return ret;
        }
    }
}
