using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;
using Common.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NCI.Search
{
    public class SiteWideSearchAPIClient : ISiteWideSearchAPIClient
    {
        static ILog log = LogManager.GetLogger(typeof(SiteWideSearchAPIClient));


        private HttpClient _client = null;

        /// <summary>
        /// Creates a new instance of a SiteWideSearch API client.
        /// </summary>
        /// <param name="client">An HttpClient that has the BaseAddress set to the API address.</param>
        public SiteWideSearchAPIClient(HttpClient client)
        {
            //We pass in an HttpClient instance so that this class can be mocked up for testing.
            //Since client can have a BaseAddress set, it may be set on an instance and passed in here.
            this._client = client;
        }

        /// <summary>
        /// Calls the autosuggest endpoint (/Autosuggest) of the sitewide search API
        /// </summary>
        /// <param name="collection">Collection to use (required)</param>
        /// <param name="language">Language to use (required)</param>
        /// <param name="searchTerm">Search term (required)</param>
        /// <param name="size"># of results to return (optional)</param>
        /// <returns>Collection of autosuggest results</returns> 
        public AutoSuggestAPIResultCollection Autosuggest(
            string collection,
            string language,
            string searchTerm,
            int size = 10
            )
        {
            AutoSuggestAPIResultCollection rtnResult = null;

            // Check fields
            if (String.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("The collection is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException("The language is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentNullException("The search term is null or an empty string");
            }

            // Set up search param string: {collection}/{language}/{searchTerm}?size={size}
            string[] searchParams = { collection, language, searchTerm };
            string searchParam = string.Join("/", searchParams);
            searchParam += "?size=" + size.ToString();

            //Get the HTTP response content from GET request
            HttpContent httpContent = ReturnGetRespContent("Autosuggest", searchParam);
            rtnResult = httpContent.ReadAsAsync<AutoSuggestAPIResultCollection>().Result;

            return rtnResult;
        }
        
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
        public SiteWideSearchAPIResultCollection Search(
            string collection,
            string language,
            string searchTerm,
            int from = 0,
            int size = 10,
            string site = "all"
            )
        {
            SiteWideSearchAPIResultCollection rtnResult = null;

            // Check fields
            if (String.IsNullOrWhiteSpace(collection))
            {
                throw new ArgumentNullException("The collection is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException("The language is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentNullException("The search term is null or an empty string");
            }

            string[] searchParams = { collection, language, searchTerm };
            string searchParam = string.Join("/", searchParams);
            searchParam += "?size=" + size.ToString() + "&from=" + from + "&site=" + site;

            //Get the HTTP response content from GET request
            HttpContent httpContent = ReturnGetRespContent("Search", searchParam);
            rtnResult = httpContent.ReadAsAsync<SiteWideSearchAPIResultCollection>().Result;

            return rtnResult;
        }

        /// <summary>
        /// Gets the response content of a GET request.
        /// </summary>
        /// <param name="path">Path for client address</param>
        /// <param name="param">Param in URL</param>
        /// <returns>HTTP response content</returns>
        public HttpContent ReturnGetRespContent(String path, String param)
        {
            HttpResponseMessage response = null;
            HttpContent content = null;
            String notFound = "NotFound";

            
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            // We want this to be synchronus, so call Result right away.
            // NOTE: When using HttpClient.BaseAddress as we are, the path must not have a preceeding slash
            response = _client.GetAsync(path + "/" + param).Result;
            if (response.IsSuccessStatusCode)
            {
                content = response.Content;
            }
            else
            {
                string errorMessage = "Response: " + response.Content.ReadAsStringAsync().Result + "\nAPI path: " + this._client.BaseAddress.ToString() + path + "/" + param;
                if (response.StatusCode.ToString() == notFound)
                {
                    // If results are not found, log 404 message and return content as null
                    log.Debug(errorMessage);
                }
                else
                {
                    // If response is other error message, log and throw exception
                    log.Error(errorMessage);
                    throw new Exception(errorMessage);
                }
            }

            return content;
        }
    }
}
