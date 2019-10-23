using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NCI.Search
{
    /// <summary>
    /// Represents a collection of SiteWide Search results as returned by the Search endpoint
    /// </summary>
    public class SiteWideSearchAPIResultCollection
    {
        /// <summary>
        /// List of all the Search results
        /// </summary>
        [JsonProperty("results")]
        public SiteWideSearchAPIResult[] SearchResults { get; set; }

        /// <summary>
        /// Gets the total number of results
        /// </summary>
        [JsonProperty("totalResults")]
        public int ResultCount { get; set; }
    }
}
