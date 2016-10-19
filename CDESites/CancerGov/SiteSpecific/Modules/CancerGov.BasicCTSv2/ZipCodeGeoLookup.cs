using System;
using System.Configuration;
using System.IO;
using System.Web;
using Common.Logging;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Manager class for looking up a given zip code in the ZipCodeDictionary
    /// </summary>
    public static class ZipCodeGeoLookup
    {
        static ILog log = LogManager.GetLogger(typeof(ZipCodeGeoLookup));

        /// <summary>
        /// ZipCodeDictionary field that will be used for Loader/Reloader
        /// </summary>
        private static ZipCodeDictionary zipCodeDictionary;

        /// <summary>
        /// Used by WatchTemplateDirectory() to watch for changes to zip_code.json
        /// </summary>
        private static FileSystemWatcher zipCodeFileWatcher;

        /// <summary>
        /// Static constructor - initializes ZipCodeDictionary object
        /// </summary>
        static ZipCodeGeoLookup()
        {
            zipCodeDictionary = ZipCodeGeoLoader.LoadDictionary();
            WatchDictionaryFile();
        }

        /// <summary>
        /// Check against the dictionary of zipcodes and return a ZipCodeGeoEntry object 
        /// if a match is found.
        /// </summary>
        /// <param name="zipCodeEntry">5-digit zip code string</param>
        /// <returns>ZipCodeGeoEntry or null if no match</returns>
        public static ZipCodeGeoEntry GetZipCodeGeoEntry(string zipCodeEntry)
        {
            GuaranteeData();  // Guarantee we have data before trying to load it.

            ZipCodeDictionary zipDict = zipCodeDictionary;

            if (zipDict != null && zipDict.ContainsKey(zipCodeEntry))
            {
                return zipDict[zipCodeEntry];
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
            if(zipCodeDictionary == null)
            {
                lock(lockObject)
                {
                    if(zipCodeDictionary == null)
                    {
                        zipCodeDictionary = ZipCodeGeoLoader.LoadDictionary();
                    }
                }
            }
        }

        /// <summary>
        /// Watch for and handle changes to the zip codes JSON file usied for the search params mapping.
        /// </summary>
        static void WatchDictionaryFile()
        {
            // Get the .json relative filepath from the Web.config map to the full filepath on the machine.
            String zipFilePath = ConfigurationManager.AppSettings["ZipCodesJsonMap"].ToString();
            if (String.IsNullOrWhiteSpace(zipFilePath))
            {
                log.Error("WatchDictionaryFile(): 'ZipCodesJsonMap' value not set.");
                return;
            }
            zipFilePath = HttpContext.Current.Server.MapPath(zipFilePath);

            // Set FileSystemWatcher for the file path and set properties/event methods.
            zipCodeFileWatcher = new FileSystemWatcher((Path.GetDirectoryName(zipFilePath)));
            zipCodeFileWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.Attributes;
            zipCodeFileWatcher.Filter = "*.json";
            zipCodeFileWatcher.EnableRaisingEvents = true;
            zipCodeFileWatcher.Created += new FileSystemEventHandler(OnChange);
            zipCodeFileWatcher.Changed += new FileSystemEventHandler(OnChange);
            zipCodeFileWatcher.Deleted += new FileSystemEventHandler(OnRemove);
            zipCodeFileWatcher.Renamed += new RenamedEventHandler(OnRename);
        }

        /// <summary>
        /// Event handler for .json file in the Configuration\files directory being modified or created.
        /// Loads the dictionary again upon file update and logs modify/create event.
        /// </summary>
        /// <param name="src">event source (not used)</param>
        /// <param name="e">event arguments (not used)</param>
        private static void OnChange(object src, FileSystemEventArgs e)
        {
            zipCodeDictionary = ZipCodeGeoLoader.LoadDictionary();
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
