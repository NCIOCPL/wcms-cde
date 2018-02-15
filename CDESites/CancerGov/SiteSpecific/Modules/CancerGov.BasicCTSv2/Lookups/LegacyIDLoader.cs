using System;
using System.Configuration;
using System.IO;
using System.Web;
using Common.Logging;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Class to load and do heavy lifting on loading lookup data.
    /// </summary>
    class LegacyIDLoader
    {
        static ILog log = LogManager.GetLogger(typeof(LegacyIDLoader));

        /// <summary>
        /// Retrieves pipe 
        /// </summary>
        /// <returns>ZipCodeDictionary zipCodes</returns>
        public static LegacyCancerTypeDictionary LoadCancerTypeDictionary()
        {

            try
            {
                string[] mappingList = LoadMappingFile();
                
                // If no list is found, use an empty array to prevent an error.
                // Any  error loading the data file should be handled by LoadMappingFile().
                if(mappingList == null)
                    mappingList = new string[0];

                LegacyCancerTypeDictionary map = new LegacyCancerTypeDictionary();
                Array.ForEach(mappingList, (string entry) =>
                {
                    entry.Trim();

                        string[] parts = entry.Split('|');
                        string key, value;

                        if (parts.Length == 0)
                        {
                            return; // Blank line, go to next entry
                        }

                        // Check for insufficient pieces.
                        if (parts.Length == 1)
                        {
                            log.ErrorFormat("LoadCancerTypeDictionary(): Partial entry '{0}'", entry);
                            return; // Bad entry, go to next entry.
                        }

                        // Non-fatal, check for extra entries.
                        if (parts.Length > 2)
                        {
                            // Log, but keep going.
                            log.ErrorFormat("LoadCancerTypeDictionary(): Entry has more than one mapping '{0}'", entry);
                        }

                        // We know we have a key and value.
                        key = parts[0].Trim();
                        value = parts[1].Trim();

                        // Check for blanks.   
                        if(String.IsNullOrWhiteSpace(key) || String.IsNullOrWhiteSpace(value))
                        {
                           log.ErrorFormat("LoadCancerTypeDictionary(): Blank key or value '{0}'", entry);
                            return; // Bad entry, go to the next one.
                        }

                        // Check for duplicates
                        if (map.ContainsKey(key))
                        {
                            log.ErrorFormat("LoadCancerTypeDictionary(): duplicate key '{0}'", entry);
                            return; // Continue with next array entry.
                        }

                        // OK, we have something worth adding.
                        map.Add(key, value);
                });

                return map;
            }
            catch (Exception ex)
            {
                log.Error("LoadCancerTypeDictionary(): Error loading cancer ID mapping file.", ex);
                return null;
            }
        }

        private static string[] LoadMappingFile()
        {
            string cancerTypeFilePath = String.Empty;

            try
            {
                cancerTypeFilePath = ConfigurationManager.AppSettings["CancerIDMapping"].ToString();
                if (String.IsNullOrWhiteSpace(cancerTypeFilePath))
                {
                    log.Error("LoadCancerTypeDictionary(): CancerIDMapping not set.");
                    return null;
                }

                cancerTypeFilePath = HttpContext.Current.Server.MapPath(cancerTypeFilePath);
                string[] mappingList = File.ReadAllLines(cancerTypeFilePath);

                return mappingList;
            }
            catch (FileNotFoundException ex)
            {
                log.ErrorFormat("LoadCancerTypeDictionary(): Path {0} not found.", ex, cancerTypeFilePath);
                return null;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("LoadCancerTypeDictionary(): Failed to read dictionary file on path {0}", ex, cancerTypeFilePath);
                return null;
            }
        }
    }
}
