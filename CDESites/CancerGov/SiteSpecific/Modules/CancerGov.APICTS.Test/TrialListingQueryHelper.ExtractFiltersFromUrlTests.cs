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

        public static IEnumerable<object[]> UrlFilterTestData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //Empty Params
                    new object[] {
                        "",
                        new JObject()
                    },
                    //Single Param
                    new object[] {
                        "?filter[sites.org_name]=mayo",
                        JObject.Parse(@"{
                            ""sites.org_name"" : ""mayo""
                        }")
                    },
                    //Multi Param
                    new object[] {
                        "?filter[sites.org_name]=mayo&filter[sites.org_country]=united%20states",
                        JObject.Parse(@"{
                            ""sites.org_name"" : ""mayo"",
                            ""sites.org_country"": ""united states""
                        }")
                    },
                    //Array Param
                    new object[] {
                        "?filter[sites.org_name]=mayo,hopkins&filter[sites.org_country]=united%20states",
                        JObject.Parse(@"{
                            ""sites.org_name"" : [ ""mayo"", ""hopkins"" ],
                            ""sites.org_country"": ""united states""
                        }")
                    },
                    //Array Param
                    new object[] {
                        "?filter[sites.org_name]=mayo&filter[sites.org_name]=hopkins&filter[sites.org_country]=united%20states",
                        JObject.Parse(@"{
                            ""sites.org_name"" : [ ""mayo"", ""hopkins"" ],
                            ""sites.org_country"": ""united states""
                        }")
                    },
                };
            }
        }

        [Theory, MemberData("UrlFilterTestData")]
        public void ExtractFiltersFromUrl(string filterParams, JObject expectedCriteria)
        {
            NameValueCollection coll = HttpUtility.ParseQueryString(filterParams);

            JObject filterObject = TrialListingQueryHelper.ExtractFiltersFromUrl(coll);

            Assert.Equal(expectedCriteria, filterObject, new JTokenEqualityComparer());
        }
    }
}
