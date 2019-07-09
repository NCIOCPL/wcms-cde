using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using NCI.Web.CDE;
using NCI.Web.CDE.Configuration;
using NCI.Web.CDE.UI.Configuration;
using NCI.Web.CDE.Modules;

namespace CancerGov.Search.BestBets
{
    public static class BestBetsPresentationManager
    {
        /// <summary>
        /// Gets the API client this Best Bets Presentation Manager will use
        /// </summary>
        private static IBestBetsAPIClient Client = BestBetsAPIClientHelper.GetClientInstance();

        static ILog log = LogManager.GetLogger(typeof(BestBetsPresentationManager));

        /// <summary>
        /// This class is the business layer that interfaces between the user interface and the API layer. 
        /// </summary>
        public static BestBetUIResult[] GetBestBets(string searchTerm, DisplayLanguage lang)
        {
            List<BestBetUIResult> rtnResults = new List<BestBetUIResult>();
            BestBetAPIResult[] apiResults = null;
            
            // Set collection based on current environment
            string collection = Settings.IsLive ? "live" : "preview";

            // Set language string
            // Default to English, as only "en" and "es" are accepted by API
            string twoCharLang = "en";
            if (lang == DisplayLanguage.Spanish)
            {
                twoCharLang = "es";
            }

            try
            {
                // Call API to retrieve autosuggest results
                apiResults = Client.Search(collection, twoCharLang, searchTerm);
            }
            catch (Exception ex)
            {
                // Log error if unable to retrieve results
                log.Error("Error retrieving results from Best Bets API Client in BestBetsPresentationManager", ex);
            }

            rtnResults = apiResults.Select(r => new BestBetUIResult { CategoryName = r.Name, CategoryDisplay = r.HTML }).ToList();

            return rtnResults.ToArray();
        }

    }
}
