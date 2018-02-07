using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

using Xunit;

using NCI.Test.IO;
using NCI.Test.Net;


namespace CancerGov.ClinicalTrialsAPI.Test
{
    public class ClinicalTrialsAPIClientTests
    {

        [Fact]
        public void TestGet()
        {
            string baseUrl = "https://example.org/v1/";
            string trialID = "NCT02194738";

            string trialFilePath = TestFileTools.GetPathToTestFile(typeof(ClinicalTrialsAPIClientTests), Path.Combine(new string[] { "TrialExamples", trialID + ".json" }));

            HttpClient mockedClient = HttpClientMockHelper.GetClientMockForURLWithFileResponse(String.Format("{0}clinical-trial/{1}", baseUrl, trialID), trialFilePath);
            mockedClient.BaseAddress = new Uri(baseUrl);

            ClinicalTrialsAPIClient client = new ClinicalTrialsAPIClient(mockedClient);
            
            
            ClinicalTrial trial = client.Get(trialID);

            Assert.Equal(trialID, trial.NCTID);
        }

        [Fact]
        public void TestListNoParams()
        {
            //This is more of an integration test to get things worked out.  It would be nice to have some sort of adapter so that we can mock the service.
            //but for now we need to get this moving.

            /*
            ClinicalTrialsAPIClient client = new ClinicalTrialsAPIClient(GetMockedClient());

            ClinicalTrialsCollection results = client.List();

            //This is a safe test range. 500-100,000 trials
            Assert.InRange<int>(3000, 500, 100000);
            */
        }

        [Fact]
        public void TestListIncludeFields()
        {
            //This is more of an integration test to get things worked out.  It would be nice to have some sort of adapter so that we can mock the service.
            //but for now we need to get this moving.
            /*
            ClinicalTrialsAPIClient client = new ClinicalTrialsAPIClient(GetMockedClient());

            ClinicalTrialsCollection results = client.List(includeFields: new string[]{ "brief_title" } );

            //Every record should have one of these, so there should never be a case where these will fail if our listing worked.
            Assert.NotNull(results.Trials[0].BriefTitle);
            Assert.Null(results.Trials[0].NCIID); 
            */
        }


    }
}
