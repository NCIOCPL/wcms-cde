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

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{
    public partial class TrialListingQueryHelper_Test
    {
        public static IEnumerable<object[]> MergeFilterTestData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //Empty Params
                    new object[] {
                        new JObject(),
                        "",
                        new JObject()
                    },
                    //Single Param
                    new object[] {
                        JObject.Parse(@"{
                            ""sites.org_name"" : ""mayo""
                        }"),
                        "",
                        JObject.Parse(@"{
                            ""sites.org_name"" : ""mayo""
                        }")
                    },
                    //Multi Param
                    new object[] {
                        JObject.Parse(@"{
                            ""sites.org_city"" : ""Baltimore""
                        }"),
                        "?filter[sites.org_name]=mayo&filter[sites.org_country]=united%20states",
                        JObject.Parse(@"{ 
                            ""sites.org_name"" : ""mayo"",
                            ""sites.org_country"": ""united states"",
                            ""sites.org_city"" : ""Baltimore""
                        }")
                    },
                    //Array Param
                    new object[] {
                        new JObject {
                            { "sites.org_city", "Baltimore" }
                        },
                        "?filter[sites.org_city]=minneapolis&filter[sites.org_name]=mayo&filter[sites.org_country]=united%20states",
                        new JObject {
                            { "sites.org_city", "Baltimore" },
                            { "sites.org_name", "mayo" },
                            { "sites.org_country", "united states" }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Test Meging of URL filters and original query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="filterParams"></param>
        /// <param name="expectedCriteria"></param>
        [Theory, MemberData("MergeFilterTestData")]
        public void MergeFilterQuery(JObject query, string filterParams, JObject expectedCriteria)
        {
            NameValueCollection coll = HttpUtility.ParseQueryString(filterParams);

            JObject filterObject = TrialListingQueryHelper.MergeQueryAndURLFilters(query, coll);

            //NOTE: Original Params (query) will come second.  So ORDER MATTERS!

            Assert.Equal(expectedCriteria, filterObject, new JTokenEqualityComparer());
        }
    }
}
