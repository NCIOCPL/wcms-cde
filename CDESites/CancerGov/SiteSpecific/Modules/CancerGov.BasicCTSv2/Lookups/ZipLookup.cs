using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Represents a ZIP Code Lookup from ElasticSearch
    /// </summary>
    public class ZipLookup
    {
        /// <summary>
        /// Gets and Sets the Zip Code
        /// </summary>
        public string PostalCode_ZIP { get; set; }

        /// <summary>
        /// Gets and Sets the GeoCode for that Zip Code
        /// </summary>
        public GeoLocation GeoCode { get; set; }

        public override string ToString()
        {
            return "{ PostalCode_ZIP: " + PostalCode_ZIP + ", GeoCode: " + GeoCode.ToString() + " }";
        }
    }
}
