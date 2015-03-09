using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Simple.OData.Client.Office365.TDD.BuisnessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Simple.OData.Client.Office365.TDD.BuisnessLogic.Tests
{
    [TestClass()]
    public class BuisnessClassATests
    {
        private IODataClient CreateTestDataODataClient()
        {
            var odataMock = new TDDTests.ODataMock.ProjectDataClient();

            return new Simple.OData.Client.Fakes.StubIODataClient()
            {
                FindEntryAsyncString = (commandText) => odataMock.FindEntryAsync(commandText),
                FindEntriesAsyncStringODataFeedAnnotations = (commandText, annotations) => odataMock.FindEntriesAsync(commandText, annotations),
                FindScalarAsyncString = (commandText) => odataMock.FindScalarAsync(commandText)
            };
        }

        [TestMethod()]
        public void ReadAllProjectsTest_CountResults()
        {
            var odataMock = CreateTestDataODataClient();

            var classToTest = new Simple.OData.Client.Office365.TDD.BuisnessLogic.BuisnessClassA();
            var resultTask = classToTest.ReadAllProjects(odataMock);
            resultTask.Wait();

            Assert.AreEqual(47, resultTask.Result.Count());
        }

        [TestMethod()]
        public void ReadAllAssignmentsTest_CountResults()
        {
            var odataMock = CreateTestDataODataClient();

            var classToTest = new Simple.OData.Client.Office365.TDD.BuisnessLogic.BuisnessClassA();
            var resultTask = classToTest.ReadAllAssignments(odataMock, new Guid("32988321-c6be-e111-9f1e-00155d022681"));
            resultTask.Wait();

            Assert.AreEqual(159, resultTask.Result.Count());
        }
    }
}
