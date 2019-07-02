using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace NCI.Search
{
    public interface ISiteWideSearchAPIClient
    {
        /// <summary>
        /// Calls the autosuggest endpoint (/Autosuggest) of the sitewide search API
        /// </summary>
        /// <param name="collection">Collection to use (required)</param>
        /// <param name="language">Language to use (required)</param>
        /// <param name="searchTerm">Search term (required)</param>
        /// <param name="size"># of results to return (optional)</param>
        /// <returns>Collection of autosuggest results</returns> 
        AutoSuggestAPIResultCollection Autosuggest(
            string collection,
            string language,
            string searchTerm,
            int size = 10
        );
        
        /// <summary>
        /// Calls the search endpoint (/Search) of the sitewide search API
        /// </summary>
        /// <param name="collection">Collection to use (required)</param>
        /// <param name="language">Language to use (required)</param>
        /// <param name="searchTerm">Search term (required)</param>
        /// <param name="from">Beginning index for results (optional)</param>
        /// <param name="size"># of results to return (optional)</param>
        /// <param name="site">Filter items returned based on site (optional)</param>
        /// <returns>Collection of Sitewide Search results</returns> 
        SiteWideSearchAPIResultCollection Search(
            string collection,
            string language,
            string searchTerm,
            int from = 0,
            int size = 10,
            string site = "all"
        );
    }
}