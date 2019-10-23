using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NCI.Search
{
    public class AutoSuggestAPIResultCollection
    {
        /// <summary>
        /// Gets the total number of results.
        /// </summary>
        [JsonProperty("total")]
        public int TotalResults { get; set; }
        
        /// <summary>
        /// Gets or sets the collection of Autosuggest Result Items
        /// <remarks>NOTE: this will not be *all* of the terms, but a subset based on the size and code parameters passed to the API</remarks>
        /// </summary>
        [JsonProperty("results")]
        public AutoSuggestAPIResult[] Results { get; set; }
    }
}
