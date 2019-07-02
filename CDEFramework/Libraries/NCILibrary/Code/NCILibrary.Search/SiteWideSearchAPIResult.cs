using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NCI.Search
{
    /// <summary>
    /// Represents a SiteWide Search Result as returned by the API
    /// </summary>
    public class SiteWideSearchAPIResult
    {
        /// <summary>
        /// Gets or sets the title for this search result item
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the URL for this search result item
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the content type of this search result item
        /// </summary>
        [JsonProperty("contentType")]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the description for this search result item
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
