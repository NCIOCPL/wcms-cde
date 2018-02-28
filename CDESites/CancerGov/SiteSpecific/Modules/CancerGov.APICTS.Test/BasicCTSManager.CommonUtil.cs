using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Moq;
using CancerGov.ClinicalTrialsAPI;

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{

    //Partial for the common functions across all the different tests for BasicCTSManager
    public partial class BasicCTSManager_Test
    {

        /// <summary>
        /// CTSSearchParams -> filterCriterea mapping tests
        /// </summary>
        /// <param name="searchParams">An instance of a CTSSearchParams object</param>
        /// <param name="expectedCriteria">The expected criteria for the search</param>
        private void MappingTest(CTSSearchParams searchParams, Dictionary<string, object> expectedCriteria)
        {
            Dictionary<string, object> actualCriteria = null;

            //When search gets called trap the criteria and set the actualCriteria
            var mockClient = GetClientMock(
                (filterCriteria, size, from, include, exclude) => actualCriteria = filterCriteria,
                new ClinicalTrialsCollection() { TotalResults = 0, Trials = new ClinicalTrial[] { } }
            );

            //Create a new instance of the factory, passing in the Mock's version of an implementation
            //of our IClinicalTrialsAPIClient interface.
            BasicCTSManager manager = new BasicCTSManager(mockClient.Object);

            //Get the results of parsing the URL
            ClinicalTrialsCollection returnedCol = manager.Search(searchParams);

            //Test the actual result to the expected.  NOTE: If you add fields to the CTSSearchParams, you need
            //to also modify the comparer
            Assert.Equal(expectedCriteria, actualCriteria);
        }


        /// <summary>
        /// Gets a mock that can be used for a IClinicalTrialsAPIClient
        /// See https://github.com/moq/moq4 for more details on the mock library.
        /// (You can do cool thinks like make sure a method was called a certain number of times too...)
        /// (You can pretend to throw an exception if this are not right...)
        /// </summary>
        /// 
        /// <returns>A mock to be used as the service.</returns>
        private Mock<IClinicalTrialsAPIClient> GetClientMock(
            Action<Dictionary<string, object>, int, int, string[], string[]> criteriaIntercept, ClinicalTrialsCollection rtnCollection)
        {
            Mock<IClinicalTrialsAPIClient> rtnMock = new Mock<IClinicalTrialsAPIClient>();

            //Handle the case when a string of C4872 is passed in to GetTitleCase and return the label "Breast Cancer"
            //This makes it so that we do not have to create a fake class that returns fake data.
            rtnMock.Setup(client => client.List(
                It.IsAny<Dictionary<string, object>>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string[]>(),
                It.IsAny<string[]>()))
                .Callback(criteriaIntercept) //This should be fleshed out to accept more params
                .Returns(rtnCollection);

            return rtnMock;
        }
    }
}
