using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;

using NCI.Web.CDE;
using NCI.Search;

using Common.Logging;

namespace CancerGov.Search.AutoSuggest
{
    /// <summary>
    /// This class is the business layer that interfaces between the service and the API layer. 
    /// </summary>
    public static class AutoSuggestSearchManager
    {
        /// <summary>
        /// Gets the API client this AutoSuggest Manager will use
        /// </summary>
        private static ISiteWideSearchAPIClient Client = SiteWideSearchAPIClientHelper.GetClientInstance();

        static ILog log = LogManager.GetLogger(typeof(AutoSuggestSearchManager));

        /// <summary>
        /// This methods filters the information passed to it in order to refine what
        /// will be called by the API client.
        /// </summary>
        /// <param name="language">Enumeration indicating language</param>
        /// <param name="searchText">The partial text to search for</param>
        /// <param name="size">The maximum number of items that the API will return</param>
        /// <param name="contains">Indicates whether the text will be searched starting from the beginning or anywhere in the string</param>
        /// <returns>Returns the AutoSuggest API search results</returns>
        public static AutoSuggestAPIResultCollection Search(DisplayLanguage language, string searchText, int size, bool contains)
        {
            AutoSuggestAPIResultCollection rtnResults = new AutoSuggestAPIResultCollection();

            // Set collection based on web.config setting
            string collection = ConfigurationManager.AppSettings["SiteWideSearchAPICollection"];

            // Set language string
            // Default to English, as only "en" and "es" are accepted by API
            string twoCharLang = "en";
            if (language == DisplayLanguage.Spanish)
            {
                twoCharLang = "es";
            }

            try
            {
                // Call API to retrieve autosuggest results
                rtnResults = Client.Autosuggest(collection, twoCharLang, searchText, size);
            }
            catch (Exception ex)
            {
                // Log error if unable to retrieve results
                log.Error("Error retrieving results from SiteWideSearch API Client in AutoSuggestSearchManager", ex);
            }

            return rtnResults;
        }
    }
}
