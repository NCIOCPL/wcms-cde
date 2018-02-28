using NCI.Web;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Helper class to prepare Trial Listing Page Queries
    /// </summary>
    public static class TrialListingQueryHelper
    {

        /// <summary>
        /// Create the Regex pattern for finding filters in the URL.
        /// Match the following pattern: "filter" + open square bracket ([) + any string value + close square bracket (]).
        /// </summary>
        private static readonly Regex FilterPattern = new Regex(@"filter\[([^]]*)\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets a query for the CTS API adding in filters supplied by a NameValueCollection (e.g. user supplied through URL query)
        /// </summary>
        /// <param name="baseQuery">The base query</param>
        /// <param name="filterParams">The filters in a NVC.  This could contain other types, so you can pass in the URL Query.</param>
        /// <returns></returns>
        public static JObject MergeQueryAndURLFilters(JObject baseQuery, NameValueCollection filterParams)
        {
            JObject filters = ExtractFiltersFromUrl(filterParams);

            //Add dynamic filter criteria
            if (baseQuery != null && filters != null)
            {
                // Merge both sets of dynamic filter params.'query' overrides 'urlParams', meaning that any duplicate elements in urlParams 
                // will not be included, e.g., given
                // query == { "key1":value1, "key2":["value2,value3"] }
                // and urlParms == { "key2":["value4,value5"], "key3":"value6" }
                // merged query == { "key1":value1, "key2":["value2,value3"], "key3":"value6" }

                //Merge objects (primary overrides secondary)
                filters.Merge(baseQuery, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Concat });
            }

            return filters;            
        }

        /// <summary>
        /// Extracts a JSON object matching the CTS Query based on the URL Query parameters
        /// </summary>
        /// <param name="urlQuery"></param>
        /// <returns></returns>
        public static JObject ExtractFiltersFromUrl(NameValueCollection urlQuery)
        {
            //TODO: Add code to handle multiple filter params


            JObject filterQuery = new JObject();

            // For each query param that matches the "filter[]" pattern, add it to the list of filter values
            foreach (string key in urlQuery.AllKeys)
            {
                if (!string.IsNullOrWhiteSpace(key) && FilterPattern.IsMatch(key))
                {
                    Match match = FilterPattern.Match(key);
                    string queryValue = match.Groups[1].Value;
                    string queryParam = urlQuery[match.Value];
                    if (!string.IsNullOrWhiteSpace(queryParam)) // Don't filter empty params
                    {
                        if (queryParam.IndexOf(",") > -1) // Split comma-separated values
                        {
                            filterQuery.Add(new JProperty(queryValue, new JArray(queryParam.Split(','))));
                        }
                        else
                        {
                            filterQuery.Add(new JProperty(queryValue, queryParam));                            
                        }
                    }
                }
            }

            return filterQuery;
        }

        /// <summary>
        /// Adds any filter parameter to the URL.
        /// </summary>
        /// <param name="url">An NCI URL to add the filters to</param>
        /// <param name="queryParams">The query parameters to get filters from</param>
        public static void AddFilterParamsToUrl(NciUrl url, NameValueCollection queryParams)
        {
            //TODO: Does this handle multiple filters?

            // For each query param that matches the "filter[]" pattern, add it to our query parameters
            foreach (string key in queryParams.AllKeys)
            {
                if (!string.IsNullOrWhiteSpace(key) && FilterPattern.IsMatch(key))
                {
                    Match match = FilterPattern.Match(key);
                    string queryValue = key;
                    string queryParam = queryParams[match.Value];
                    // Don't carry over empty or preexisting contained queries
                    if (!string.IsNullOrWhiteSpace(queryParam) && !url.QueryParameters.ContainsKey(key))
                    {
                        url.QueryParameters.Add(queryValue, queryParam);
                    }
                }
            }

        }
    }
}
