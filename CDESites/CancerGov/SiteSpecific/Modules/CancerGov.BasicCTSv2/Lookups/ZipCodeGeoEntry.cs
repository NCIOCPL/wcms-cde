using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public class ZipCodeGeoEntry
    {
        /// <summary>
        /// Zip code atitude
        /// </summary>
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        /// <summary>
        /// Zip code longitude
        /// </summary>
        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }
}
