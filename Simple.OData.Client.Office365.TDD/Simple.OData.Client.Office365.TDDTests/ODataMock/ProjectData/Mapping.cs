using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple.OData.Client.Office365.TDDTests.ODataMock.ProjectData
{
    class Mapping
    {
        public static string ODataCommandTextToResourceName(string commandText)
        {
            switch (commandText)
            {
                case "https://foobar.sharepoint.com/sites/pwa/_api/ProjectData/Projects(guid'32988321-c6be-e111-9f1e-00155d022681')/Assignments?$skiptoken=guid'32988321-c6be-e111-9f1e-00155d022681',guid'20c8da92-46ca-e211-8fb0-00155d8c8e11'":
                case "Projects(guid'32988321-c6be-e111-9f1e-00155d022681')/Assignments?$skiptoken=guid'32988321-c6be-e111-9f1e-00155d022681',guid'20c8da92-46ca-e211-8fb0-00155d8c8e11'":
                    return "ODataMock.ProjectData.Projects(guid'32988321-c6be-e111-9f1e-00155d022681')_Assignments_Page2.xml";
                default:
                    commandText = commandText.Replace('/', '_');
                    commandText = commandText.Replace('?', '_');
                    return "ODataMock.ProjectData." + commandText + ".xml";
            }
        }
    }
}
