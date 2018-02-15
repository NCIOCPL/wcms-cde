using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Create LegacyCancerTypeDictionary class to map CDR Cancer Type IDs to NCI Thesaurus Concept IDs.
    /// Key is a CDR Cancer Type (No leading "CDR" or zeros)
    /// Value is an NCI Thesaurus Concept IDs.
    /// </summary>
    class LegacyCancerTypeDictionary : Dictionary<String, String>
    {
    }
}
