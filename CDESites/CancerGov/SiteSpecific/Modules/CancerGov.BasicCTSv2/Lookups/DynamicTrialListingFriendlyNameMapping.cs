using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using Common.Logging;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Class represents a singleton that can be used to lookup a CDRID for
    /// one user-friendly URL name.  This is used by the Dictionary pages.
    /// </summary>
    public class DynamicTrialListingFriendlyNameMapping

    {
        // Lock synchronization object
        private static object syncLock = new Object();

        private Dictionary<string, string> _mapping = new Dictionary<string, string>();

        private DynamicTrialListingFriendlyNameMapping(Dictionary<string, string> mapping)
        {
            _mapping = mapping;
        }

        public static DynamicTrialListingFriendlyNameMapping GetMappingForFile(string dictMappingFilepath)
        {
            if (string.IsNullOrEmpty(dictMappingFilepath))
            {
                throw new ArgumentException("The dictionary mapping filepath must not be empty or null.");
            }

            if (HttpContext.Current.Cache[dictMappingFilepath] == null)
            {
                lock (syncLock)
                {
                    if (HttpContext.Current.Cache[dictMappingFilepath] == null)
                    {
                        DynamicTrialListingFriendlyNameMapping instance = LoadDictionaryMappingFromFile(dictMappingFilepath);

                        // Store instance in cache for five minutes
                        HttpContext.Current.Cache.Add(dictMappingFilepath, instance, null, DateTime.Now.AddMinutes(5), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                    }
                }
            }

            return (DynamicTrialListingFriendlyNameMapping)HttpContext.Current.Cache[dictMappingFilepath];
        }

        private static DynamicTrialListingFriendlyNameMapping LoadDictionaryMappingFromFile(string filePath)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            bool fileExists = false;
            
            fileExists  = File.Exists(HttpContext.Current.Server.MapPath(filePath));

            if(fileExists)
            {
                try
                {
                    // If file exists, use streamreader to load mappings into dictionary
                    using (StreamReader sr = new StreamReader(HttpContext.Current.Server.MapPath(filePath)))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            line = line.ToLower();

                            string[] parts = line.Split('|');
                            // Lowercase c-codes (for comparison to codes from URL parameters later)
                            parts[0] = parts[0].ToLower();

                            // Sort c-codes alphabetically/numerically if there are multiple 
                            if (parts[0].Contains(","))
                            {
                                string[] split = parts[0].Split(',');
                                Array.Sort(split);
                                string newKey = string.Join(",", split);
                                parts[0] = newKey;
                            }

                            // Add mapping to dictionary if it isn't already present
                            if (!dict.ContainsKey(parts[0]))
                            {
                                dict.Add(parts[0], parts[1]);
                            }
                        }
                    }
                }
                catch
                {
                    // Log an error if the file exists but cannot be read.
                    // Do not make the page error out - we want the dictionaries to still work,
                    // even if there is something wrong with the friendly name mapping file.
                    LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMapping)).ErrorFormat("Mapping file '{0}' could not be read.", filePath);
                }
            }
            else
            {
                // Log an error if the file does not exist.
                // Do not make the page error out - we want the dictionaries to still work,
                // even if there is something wrong with the friendly name mapping file.
                LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMapping)).ErrorFormat("Error while getting the mapping file located at '{0}'.", filePath);
            }

            return new DynamicTrialListingFriendlyNameMapping(dict);
        }

        /// <summary>
        /// Gets the CDRID that maps to the given pretty name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The CDRID</returns>
        public string GetFriendlyNameFromCode(string value)
        {
            value = value.ToLower();
            return _mapping[value];
        }

        /// <summary>
        /// Gets the CDRID that maps to the given pretty name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The CDRID</returns>
        public string GetCodeFromFriendlyName(string value)
        {
            value = value.ToLower();
            return _mapping.FirstOrDefault(x => x.Value == value).Key;
        }

        /// <summary>
        /// Checks to see if the lookup contains an entry for the CDRID
        /// </summary>
        /// <param name="key">The CDRID to lookup</param>
        /// <returns>True or false based on the existance of the CDRID in the lookup</returns>
        public bool MappingContainsCode(string key)
        {
            key = key.ToLower();
            return _mapping.ContainsKey(key);
        }

        /// <summary>
        /// Checks to see if the lookup contains an entry for the pretty name
        /// </summary>
        /// <param name="value">The pretty name to lookup</param>
        /// <returns>True or false based on the existance of the pretty name in the lookup</returns>
        public bool MappingContainsFriendlyName(string value)
        {
            value = value.ToLower();
            string myKey = _mapping.FirstOrDefault(x => x.Value == value).Key;
            if (!string.IsNullOrEmpty(myKey))
            {
                return true;
            }

            return false;
        }
    }
}

