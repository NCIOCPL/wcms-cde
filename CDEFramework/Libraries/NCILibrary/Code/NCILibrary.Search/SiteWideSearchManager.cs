using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Globalization;
using NCI.Web.CDE;
using NCI.Search;
using NCI.Search.Configuration;

using Common.Logging;

namespace NCI.Search
{
    /// <summary>
    /// This class is the business layer that interfaces between the service and the API layer. 
    /// </summary>
    public static class SiteWideSearchManager
    {
        /// <summary>
        /// Gets the API client this SiteWide Search Manager will use
        /// </summary>
        private static ISiteWideSearchAPIClient Client = SiteWideSearchAPIClientHelper.GetClientInstance();

        static ILog log = LogManager.GetLogger(typeof(SiteWideSearchManager));

        /// <summary>
        /// This methods filters the information passed to it in order to refine what
        /// will be called by the API client.
        /// </summary>
        /// <param name="searchCollection">The search collection</param>
        /// <param name="searchText">The partial text to search for</param>
        /// <param name="size">The number of items that the API will return</param>
        /// <param name="from">Beginning index for results</param>
        /// <returns>Returns the SiteWide Search API search results</returns>
        public static SiteWideSearchAPIResultCollection Search(string searchCollection, string searchText, int size, int from)
        {
            SiteWideSearchAPIResultCollection rtnResults = null;

            // Get search collection config to set up collection and site
            SiteWideSearchCollectionElement config = SiteWideSearchSection.GetSearchCollectionConfig(searchCollection);

            // Set up collection parameter
            // Set to "doc" for doc sites, and "cgov" for all others
            string collection = null;
            if(config.Template == "docSearch")
            {
                collection = "doc";
            }
            else
            {
                collection = "cgov";
            }

            // Set up language based on current culture
            string twoCharLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            // Set up site parameter
            string site = config.Site;

            try
            {
                // Call API to retrieve search results
                rtnResults = Client.Search(
                    collection,
                    twoCharLang,
                    searchText,
                    size: size,
                    from: from,
                    site: site
                );
            }
            catch (Exception ex)
            {
                // Log error if unable to retrieve results
                log.Error("Error retrieving results from SiteWideSearch API Client in SiteWideSearchManager", ex);
            }

            foreach(SiteWideSearchAPIResult res in rtnResults.SearchResults)
            {
                if(string.IsNullOrWhiteSpace(res.Title))
                {
                    res.Title = "Untitled";
                }
            }

            return rtnResults;
        }
    }
}
