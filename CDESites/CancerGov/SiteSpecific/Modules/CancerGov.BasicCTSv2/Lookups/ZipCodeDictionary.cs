using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Create ZipCodeDictionary class that inherits from System.Collections.Generic.Dictionary
    /// Key is string (5-digit zip code). 
    /// Value is ZipCodeGeoEntry object containing latitude and logitude.
    /// </summary>
    class ZipCodeDictionary : Dictionary<string, ZipCodeGeoEntry>
    {
    }
}
