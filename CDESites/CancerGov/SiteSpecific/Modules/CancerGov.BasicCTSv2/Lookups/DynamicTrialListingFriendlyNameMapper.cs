using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Common.Logging;

namespace CancerGov.ClinicalTrials.Basic.v2.Lookups
{
    public class DynamicTrialListingFriendlyNameMapper
    {
        private static string _evsMappingFile = null;
        private static string _overrideMappingFile = null;
        private Dictionary<string, MappingItem> _mappings = new Dictionary<string, MappingItem>();
        private static Regex CCODE_REGEX = new Regex("C[0-9]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DynamicTrialListingFriendlyNameMapper(string evsMappingFilepath, string overrideMappingFilepath, bool withOverrides)
        {
            _evsMappingFile = evsMappingFilepath;
            _overrideMappingFile = overrideMappingFilepath;
            LoadMapping(withOverrides);
        }

        private void LoadMapping(bool withOverrides)
        {
            // Load and store mappings
            _mappings = LoadDictionaryMappingFromFiles(_evsMappingFile, _overrideMappingFile, withOverrides);
        }

        private static Dictionary<string, MappingItem> LoadDictionaryMappingFromFiles(string evsFilePath, string overrideFilePath, bool withOverrides)
        {
            Dictionary<string, MappingItem> dict = new Dictionary<string, MappingItem>();

            if (!withOverrides)
            {
                try
                {
                    // If file exists, use streamreader to load EVS mappings into dictionary
                    using (StreamReader sr = new StreamReader(evsFilePath))
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

                            // Create MappingItem object for mapping
                            MappingItem item = new MappingItem();
                            item.Codes = parts[0].Split(',').ToList();
                            item.Text = parts[1];
                            item.IsOverride = false;

                            // Add mapping to dictionary if it isn't already present
                            if (!dict.ContainsKey(parts[0]))
                            {
                                dict.Add(parts[0], item);
                            }
                        }
                    }
                }
                catch
                {
                    // Log an error if the file exists but cannot be read.
                    // Do not make the page error out - we want the dictionaries to still work,
                    // even if there is something wrong with the friendly name mapping file.
                    LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMappingService)).ErrorFormat("Mapping file '{0}' could not be read.", evsFilePath);
                }
            }
            else
            {
                // Log an error if the file does not exist.
                // Do not make the page error out - we want the dictionaries to still work,
                // even if there is something wrong with the friendly name mapping file.
                LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMappingService)).ErrorFormat("Error while getting the mapping file located at '{0}'.", evsFilePath);
            }

            if (withOverrides)
            {
                try
                {
                    // If file exists and we need overrides, use streamreader to load override mappings into dictionary
                    using (StreamReader sr = new StreamReader(overrideFilePath))
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

                            // Create MappingItem object for mapping
                            MappingItem item = new MappingItem();
                            item.Codes = parts[0].Split(',').ToList();
                            item.Text = parts[1];
                            item.IsOverride = true;

                            if (!dict.ContainsKey(parts[0]))
                            {
                                // Add override mapping to dictionary if it isn't already present
                                dict.Add(parts[0], item);
                            }
                        }
                    }
                }
                catch
                {
                    // Log an error if the file exists but cannot be read.
                    // Do not make the page error out - we want the dictionaries to still work,
                    // even if there is something wrong with the friendly name mapping file.
                    LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMappingService)).ErrorFormat("Mapping file '{0}' could not be read.", overrideFilePath);
                }
            }
            else
            {
                // Log an error if the file does not exist.
                // Do not make the page error out - we want the dictionaries to still work,
                // even if there is something wrong with the friendly name mapping file.
                LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMappingService)).ErrorFormat("Error while getting the mapping file located at '{0}'.", overrideFilePath);
            }


            return dict;
        }

        /// <summary>
        /// Gets the friendly name that maps to the given C-code(s).
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The friendly name</returns>
        public string GetFriendlyNameFromCode(string value, bool hasExactMatch)
        {
            value = value.ToLower();

            if (hasExactMatch)
            {
                // If an exact match for the given code(s) is found, return the exact match.
                return _mappings[value].Text;
            }
            else
            {
                string[] splitIDs = value.Split(new char[] { ',' });

                // Return friendly name associated with any key that contains the first code, as all codes have the same friendly name
                return _mappings.FirstOrDefault(x => x.Key.Contains(splitIDs[0])).Value.Text;
            }
        }

        /// <summary>
        /// Gets the C-code (or combination of) that maps to the given pretty name.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The C-code(s)</returns>
        public string GetCodeFromFriendlyName(string value)
        {
            value = value.ToLower();

            return _mappings.FirstOrDefault(x => x.Value.Text == value).Key;
        }

        /// <summary>
        /// Checks to see if the lookup contains an entry for the C-code(s), whether an exact match or contained in keys that all have the same friendly name.
        /// </summary>
        /// <param name="key">The C-code(s) to lookup</param>
        /// <returns>True or false based on the existance of the C-code(s) in the lookup</returns>
        public bool MappingContainsCode(string code, bool needsExactMatch)
        {
            code = code.ToLower();

            if (needsExactMatch)
            {
                // If an exact match is needed, check if the mapping contains an entry with the exact given code as the key.
                return _mappings.ContainsKey(code);
            }
            else
            {
                // If no exact match is needed, check if the mapping contains entries whose keys contain the given code.

                // Split the given codes apart, if there are multiple.
                string[] splitIDs = code.Split(new char[] { ',' });
                List<string> splitIDFriendlyNames = new List<string>();

                // Loop through all the codes.
                foreach (string ID in splitIDs)
                {
                    if (!_mappings.Keys.Any(k => k.Contains(ID)))
                    {
                        // If any code given is not contained in a key in the mapping, return false.
                        return false;
                    }
                    else
                    {
                        if (!CCODE_REGEX.IsMatch(ID))
                        {
                            LogManager.GetLogger(typeof(DynamicTrialListingFriendlyNameMappingService)).ErrorFormat("Invalid parameter in dynamic listing page: {0} is not a valid c-code", ID);
                            NCI.Web.CDE.Application.ErrorPageDisplayer.RaisePageByCode("DynamicTrialListingFriendlyNameMappingService", 404, "Invalid parameter in dynamic listing page: value given is not a valid c-code");
                        }

                        // Add all of the friendly names for any entries whose key contains the current code to a list for later comparison.
                        splitIDFriendlyNames.AddRange(_mappings.Where(m => m.Key.Equals(ID) || m.Key.Split(new char[] { ',' }).Any(k => k.Equals(ID)))
                                                                .GroupBy(g => g.Key)
                                                                .Select(kvp => kvp.First().Value.Text)
                                                                .ToList());
                    }
                }

                if (splitIDFriendlyNames.Count() == 0 || splitIDFriendlyNames.Any(f => f != splitIDFriendlyNames[0]))
                {
                    // If no friendly names are found, there was no match and return false.
                    // Also, if the friendly names associated with keys that contain the given codes are not the same, return false.
                    return false;
                }
                else
                {
                    // If all of the friendly names associated with keys that contain the given codes match, return true.
                    return true;
                }
            }
        }

        /// <summary>
        /// Checks to see if the lookup contains an entry for the pretty name
        /// </summary>
        /// <param name="value">The pretty name to lookup</param>
        /// <returns>True or false based on the existance of the pretty name in the lookup</returns>
        public bool MappingContainsFriendlyName(string value)
        {
            value = value.ToLower();
            string myKey = _mappings.FirstOrDefault(x => x.Value.Text == value).Key;
            if (!string.IsNullOrEmpty(myKey))
            {
                return true;
            }

            return false;
        }
    }
}
