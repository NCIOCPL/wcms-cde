using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CancerGov.Search.BestBets
{
    /// <summary>
    /// Represents a best bet as returned by the API
    /// </summary>
    public class BestBetAPIResult
    {
        /// <summary>
        /// Gets or sets the display HTML for this best bet
        /// </summary>
        [JsonProperty("html")]
        public string HTML { get; set; }

        /// <summary>
        /// Gets or the ID for this best bet
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name for this best bet
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the weight for this best bet
        /// </summary>
        [JsonProperty("weight")]
        public int Weight { get; set; }
    }
}
