using System;
using System.Configuration;
using System.IO;
using System.Web;
using Common.Logging;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public static class LegacyIDLookup
    {
        static ILog log = LogManager.GetLogger(typeof(LegacyIDLookup));

        /// <summary>
        /// LegacyCancerTypeDictionary lookup dictionary.
        /// </summary>
        private static LegacyCancerTypeDictionary cancerTypeDictionary;

    
        /// <summary>
        /// Used by WatchTemplateDirectory() to watch for changes to zip_code.json
        /// </summary>
        private static FileSystemWatcher cancerMappingFileWatcher;

        /// <summary>
        /// Static constructor - initializes lookup objects
        /// </summary>
        static LegacyIDLookup()
        {
            cancerTypeDictionary = LegacyIDLoader.LoadCancerTypeDictionary();
            WatchDictionaryFile();
        }

        /// <summary>
        /// Check against the dictionary of zipcodes and return a ZipCodeGeoEntry object 
        /// if a match is found.
        /// </summary>
        /// <param name="oldCancerID">5-digit zip code string</param>
        /// <returns>ZipCodeGeoEntry or null if no match</returns>
        public static String MapLegacyCancerTypeID(string oldCancerID)
        {
            GuaranteeData(); // Guarantee we have data before trying to load it.

            LegacyCancerTypeDictionary idMap = cancerTypeDictionary;

            if (idMap != null && idMap.ContainsKey(oldCancerID))
            {
                return idMap[oldCancerID];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Locking method to safely handle file removal after the initial load
        /// </summary>
        static void GuaranteeData()
        {
            Object lockObject = new Object();
            if (cancerTypeDictionary == null)
            {
                lock (lockObject)
                {
                    if (cancerTypeDictionary == null)
                    {
                        cancerTypeDictionary = LegacyIDLoader.LoadCancerTypeDictionary();
                    }
                }
            }
        }

        /// <summary>
        /// Watch for and handle changes to the zip codes JSON file usied for the search params mapping.
        /// </summary>
        static void WatchDictionaryFile()
        {
            String cancerTypeFilePath = ConfigurationManager.AppSettings["CancerIDMapping"].ToString();
            if (String.IsNullOrWhiteSpace(cancerTypeFilePath))
            {
                log.Error("WatchDictionaryFile(): CancerIDMapping not set.");
                return;
            }
            cancerTypeFilePath = HttpContext.Current.Server.MapPath(cancerTypeFilePath);


            cancerMappingFileWatcher = new FileSystemWatcher((Path.GetDirectoryName(cancerTypeFilePath)));
            cancerMappingFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.Attributes;
            cancerMappingFileWatcher.Filter = Path.GetFileName(cancerTypeFilePath);
            cancerMappingFileWatcher.EnableRaisingEvents = true;
            cancerMappingFileWatcher.IncludeSubdirectories = false;
            cancerMappingFileWatcher.Created += new FileSystemEventHandler(OnChange);
            cancerMappingFileWatcher.Changed += new FileSystemEventHandler(OnChange);
            cancerMappingFileWatcher.Deleted += new FileSystemEventHandler(OnRemove);
            cancerMappingFileWatcher.Renamed += new RenamedEventHandler(OnRename);
        }

        /// <summary>
        /// Event handler for .json file in the Configuration\files directory being modified or created.
        /// Loads the dictionary again upon file update and logs modify/create event.
        /// </summary>
        /// <param name="src">event source (not used)</param>
        /// <param name="e">event arguments (not used)</param>
        private static void OnChange(object src, FileSystemEventArgs e)
        {
            // It's counterintuitive, but when we get a dictionary change event, we can only drop the reference,
            // we can't reload. Loading the dictionary requires an HttpContext in order to resolve the file mapping,
            // and that's not avaialble during a change event.
            // The next request attempting to use the dictionary will be responsible for loading it.
            cancerTypeDictionary = null;
            log.Warn("OnChange(): Dictionary file was updated.");
        }

        /// <summary>
        /// Event handler for .json file in the Configuration\files directory being deleted.
        /// Logs deletion event.
        /// </summary>
        /// <param name="src">event source (not used)</param>
        /// <param name="e">event arguments (not used)</param>
        private static void OnRemove(object src, FileSystemEventArgs e)
        {
            log.Warn("OnRemove(): Dictionary file was deleted.");
        }

        /// <summary>
        /// Event handler for .json file in the Configuration\files directory being renamed.
        /// Logs rename event.
        /// </summary>
        /// <param name="src">event source (not used)</param>
        /// <param name="e">event arguments (not used)</param>
        private static void OnRename(object source, RenamedEventArgs e)
        {
            log.Warn("OnRename(): Dictionary file was updated");
        }
    
    }
}
