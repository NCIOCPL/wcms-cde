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

namespace CancerGov.Search.BestBets
{
    public class BestBetsAPIClient : IBestBetsAPIClient
    {
        static ILog log = LogManager.GetLogger(typeof(BestBetsAPIClient));


        private HttpClient _client = null;

        /// <summary>
        /// Creates a new instance of a BestBets API client.
        /// </summary>
        /// <param name="client">An HttpClient that has the BaseAddress set to the API address.</param>
        public BestBetsAPIClient(HttpClient client)
        {
            //We pass in an HttpClient instance so that this class can be mocked up for testing.
            //Since client can have a BaseAddress set, it may be set on an instance and passed in here.
            this._client = client;
        }

        /// <summary>
        /// Calls the BestBets endpoint (/BestBets) of the best bets API
        /// </summary>
        /// <param name="collection">Collection name (required)</param>
        /// <param name="language">Language to use (required)</param>
        /// <param name="searchParams">Search term (required)</param>
        /// <returns>A Best Bet result</returns>
        public BestBetAPIResult[] Search(
            string collection,
            string language,
            string searchTerm
            )
        {
            BestBetAPIResult[] rtnResult = null;

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

            // Set up search param string: {collection}/{language}/{searchTerm}
            string[] searchParams = { collection, language, searchTerm };
            string searchParam = string.Join("/", searchParams);

            //Get the HTTP response content from GET request
            HttpContent httpContent = ReturnGetRespContent("BestBets", searchParam);
            rtnResult = httpContent.ReadAsAsync<BestBetAPIResult[]>().Result;

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
                    // If best bet results are not found, log 404 message and return content as null
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
