using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NCI.Search
{
    /// <summary>
    /// Represents a SiteWide Autosuggest Result as returned by the API
    /// </summary>
    public class AutoSuggestAPIResult
    {
        /// <summary>
        /// Gets or sets the term for this autosuggest result item
        /// </summary>s
        [JsonProperty("term")]
        public string Term { get; set; }
    }
}
