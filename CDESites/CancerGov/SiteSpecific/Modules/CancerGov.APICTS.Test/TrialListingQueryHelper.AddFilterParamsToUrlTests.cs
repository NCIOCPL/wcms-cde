using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Moq;
using CancerGov.ClinicalTrialsAPI;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Web;
using NCI.Web;

using NCI.Web.Test;

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{
    public partial class TrialListingQueryHelper_Test
    {
        public static IEnumerable<object[]> AddToURLTestData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //Empty Params
                    new object[] {
                        new NciUrl(),
                        "",
                        new NciUrl()
                    },
                    //Test normal
                    new object[] {
                        new NciUrl() {
                            UriStem = "/test"
                        },
                        "?filter[site.org_name]=mayo",
                        new NciUrl() {
                            UriStem = "/test",
                            QueryParameters = new Dictionary<string, string>
                            {
                                {  "filter[site.org_name]", "mayo" }
                            }
                        }
                    },
                    //Test multiple filters
                    new object[] {
                        new NciUrl() {
                            UriStem = "/test"
                        },
                        "?filter[site.org_name]=mayo",
                        new NciUrl() {
                            UriStem = "/test",
                            QueryParameters = new Dictionary<string, string>
                            {
                                {  "filter[site.org_name]", "mayo" }
                            }
                        }
                    },
                    //Test multiple filters
                    new object[] {
                        new NciUrl() {
                            UriStem = "/test"
                        },
                        "?filter[site.org_name]=mayo&filter[site.org_country]=united%20states",
                        new NciUrl() {
                            UriStem = "/test",
                            QueryParameters = new Dictionary<string, string>
                            {
                                {  "filter[site.org_name]", "mayo" },
                                {  "filter[site.org_country]", "united states" }
                            }
                        }
                    },
                    //Test multiple filters
                    new object[] {
                        new NciUrl() {
                            UriStem = "/test"
                        },
                        "?filter[site.org_name]=mayo&filter[site.org_name]=hopkins",
                        new NciUrl() {
                            UriStem = "/test",
                            QueryParameters = new Dictionary<string, string>
                            {
                                {  "filter[site.org_name]", "mayo,hopkins" }
                            }
                        }
                    },
                };
            }
        }
        

        /// <summary>
        /// Test Meging of URL filters and original query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterParams"></param>
        /// <param name="expectedCriteria"></param>
        [Theory, MemberData("AddToURLTestData")]
        public void AddFilterParamsToUrl(NciUrl url, string filterParams, NciUrl expectedUrl)
        {
            NameValueCollection coll = HttpUtility.ParseQueryString(filterParams);

            TrialListingQueryHelper.AddFilterParamsToUrl(url, coll);

            Assert.Equal(expectedUrl, url, new NciUrlComparer());
        }
    }
}
