using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCI.Logging;
using NCI.Util;

using Newtonsoft.Json;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public static class ZipCodeGeoLookup
    {
        ///// public ZipCodeGeoEntry zipCode;
        /*
         * This will be our manager class
         */

        /*
        public static void GetJson(string zipcode)
        {
            using (StreamReader r = new StreamReader(@"C:\Development\WCMS\sites\CancerGov\PublishedContent\Files\Configuration\data\zip_codes.json"))
            {
                string json = r.ReadToEnd();
                ZipCodeDictionary zipCodes = JsonConvert.DeserializeObject<ZipCodeDictionary>(json);
            }
        }
         */

        // Constructor 
        static ZipCodeGeoLookup()
        {
           ///// ZipCodeGeoLoader.GetJson();
        }
    }
}
