using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Configuration for a TrialTermLookup Instance
    /// </summary>
    public class TrialTermLookupConfig
    {
        private List<string> _mappingFiles = new List<string>();

        /// <summary>
        /// Gets a list of files to be used for lookups
        /// </summary>
        public List<string> MappingFiles { get { return _mappingFiles; } }
    }
}
