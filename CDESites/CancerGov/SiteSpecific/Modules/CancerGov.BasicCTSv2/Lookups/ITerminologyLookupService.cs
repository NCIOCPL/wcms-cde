using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// This interface represents a class that is able to lookup terminology names given one or more NCI Thesaurus 
    /// entity ids.
    /// </summary>
    public interface ITerminologyLookupService
    {
        /// <summary>
        /// Gets the title-cased term. (I.E. first letter of each word is upper case)
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The display label</returns>
        string GetTitleCase(string value);

        /// <summary>
        /// Gets the non-title-cased term.  This accounts for special initials, proper nouns and roman numerals though.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The display label</returns>
        string Get(string value);

        /// <summary>
        /// Checks to see if the lookup contains an entry for the ID(s)
        /// </summary>
        /// <param name="key">The ID(s) to lookup</param>
        /// <returns>True or false based on the existance of the ID(s) in the lookup</returns>
        bool MappingContainsKey(string key);
    }
}
