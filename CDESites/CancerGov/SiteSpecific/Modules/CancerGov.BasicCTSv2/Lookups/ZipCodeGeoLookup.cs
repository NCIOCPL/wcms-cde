using System;
using System.Configuration;
using System.IO;
using System.Web;
using Common.Logging;
using Newtonsoft.Json;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Manager class for looking up a given zip code in the ZipCodeDictionary
    /// </summary>
    public class ZipCodeGeoLookup : IZipCodeGeoLookupService
    {
        static ILog log;

        /// <summary>
        /// The path to the zip file
        /// </summary>
        static string _zipFilePath;

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
        /// A static class constructor is gauranteed to be called first and only once.
        /// </summary>
        static ZipCodeGeoLookup()
        {
            _zipFilePath = ConfigurationManager.AppSettings["ZipCodesJsonMap"];

            log = LogManager.GetLogger(typeof(ZipCodeGeoLookup));
            zipCodeDictionary = LoadDictionary();
            WatchDictionaryFile();
        }

        /// <summary>
        /// Check against the dictionary of zipcodes and return a ZipCodeGeoEntry object 
        /// if a match is found.
        /// </summary>
        /// <param name="zipCodeEntry">5-digit zip code string</param>
        /// <returns>ZipCodeGeoEntry or null if no match</returns>
        public GeoLocation GetZipCodeGeoEntry(string zipCodeEntry)
        {
            GuaranteeData();  // Guarantee we have data before trying to load it.

            ZipCodeDictionary zipDict = zipCodeDictionary;

            if (zipDict != null && zipDict.ContainsKey(zipCodeEntry))
            {
                ZipCodeGeoEntry zip = zipDict[zipCodeEntry];

                return new GeoLocation(zip.Latitude, zip.Longitude);
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
                        zipCodeDictionary = LoadDictionary();
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize the json zip/coord mapping file into a dictionary object
        /// </summary>
        /// <returns>ZipCodeDictionary zipCodes</returns>
        private static ZipCodeDictionary LoadDictionary()
        {
            // - Get the context object for the current HTTP request and map the physical filepath to the 
            //   relative filepath (the relative filepath is specified in the Web.config).
            // - Read the json file using StreamReader.
            // - Deserialize the json data into a ZipCodeDictionary object.
            string locZipFilePath = HttpContext.Current.Server.MapPath(_zipFilePath);
            try
            {
                using (StreamReader r = new StreamReader(locZipFilePath))
                {
                    string json = r.ReadToEnd();
                    ZipCodeDictionary zipCodes = JsonConvert.DeserializeObject<ZipCodeDictionary>(json);
                    return zipCodes;
                }
            }
            catch (FileNotFoundException ex)
            {
                log.ErrorFormat("LoadDictionary(): Path {0} not found.", ex, locZipFilePath);
                return null;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("LoadDictionary(): Failed to read dictionary file on path {0}", ex, locZipFilePath);
                return null;
            }
        }

        /// <summary>
        /// Watch for and handle changes to the zip codes JSON file usied for the search params mapping.
        /// </summary>
        static void WatchDictionaryFile()
        {
            // Get the .json relative filepath from the Web.config map to the full filepath on the machine.
            
            if (String.IsNullOrWhiteSpace(_zipFilePath))
            {
                log.Error("WatchDictionaryFile(): 'ZipCodesJsonMap' value not set.");
                return;
            }
            string mappedZipFilePath = HttpContext.Current.Server.MapPath(_zipFilePath);

            // Set FileSystemWatcher for the file path and set properties/event methods.
            zipCodeFileWatcher = new FileSystemWatcher((Path.GetDirectoryName(mappedZipFilePath)));
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
            zipCodeDictionary = LoadDictionary();
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
