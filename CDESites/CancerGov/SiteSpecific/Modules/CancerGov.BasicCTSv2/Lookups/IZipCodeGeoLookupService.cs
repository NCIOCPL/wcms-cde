using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public interface IZipCodeGeoLookupService
    {
        GeoLocation GetZipCodeGeoEntry(string zipCode);
    }
}
