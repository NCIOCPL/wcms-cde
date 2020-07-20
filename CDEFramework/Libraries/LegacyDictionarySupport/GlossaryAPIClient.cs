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

using LegacyDictionarySupport.Configuration;

namespace LegacyDictionarySupport
{
    public class GlossaryAPIClient
    {
        static ILog log = LogManager.GetLogger(typeof(GlossaryAPIClient));

        private HttpClient _client = null;

        /// <summary>
        /// Creates a new instance of a Glossary API client.
        /// </summary>
        /// <param name="client">An HttpClient that has the BaseAddress set to the API address.</param>
        public GlossaryAPIClient()
        {
            string baseApiPath = WebAPISection.GetAPIUrl();
            string appPath = ConfigurationManager.AppSettings["GlossaryAPIAppPath"];
            string versionPath = ConfigurationManager.AppSettings["GlossaryAPIVersionPath"];

            if (string.IsNullOrWhiteSpace(appPath))
                throw new ConfigurationErrorsException("error: GlossaryAPIAppPath cannot be null or empty");

            if (string.IsNullOrWhiteSpace(versionPath))
                throw new ConfigurationErrorsException("error: GlossaryAPIVersionPath cannot be null or empty");

            HttpClient client = new HttpClient();
            //NOTE: the base URL MUST have a trailing slash
            client.BaseAddress = new Uri(String.Format("{0}/{1}/{2}/", baseApiPath, appPath, versionPath));

            this._client = client;
        }

        /// <summary>
        /// Calls the GetById endpoint of the Glossary API
        /// </summary>
        /// <param name="dictionary">Dictionary name (required)</param>
        /// <param name="audience">Audience type(required)</param>
        /// <param name="language">Language to use (required)</param>
        /// <param name="id">The ID of the term</param>
        /// <param name="useFallback">Whether or not to use the fallback logic</param>
        /// <returns>A Best Bet result</returns>
        public GlossaryTerm GetById(
            string dictionary,
            string audience,
            string language,
            string id,
            bool useFallback = false 
            )
        {
            //GlossaryTerm resultTerm = null;

            // Check fields
            if (String.IsNullOrWhiteSpace(dictionary))
            {
                throw new ArgumentNullException("The dictionary is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(audience))
            {
                throw new ArgumentNullException("The audience is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(language))
            {
                throw new ArgumentNullException("The language is null or an empty string");
            }
            if (String.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("The ID is null or an empty string");
            }

            // Set up search param string: {dictionary}/{audience}/{language}/{id:long}
            string[] searchParams = { dictionary, audience, language, id, useFallback.ToString() };
            string searchParam = string.Format("{0}/{1}/{2}/{3}?useFallback={4}", searchParams);

            //Get the HTTP response content from GET request
            HttpContent httpContent = ReturnGetRespContent("Terms", searchParam);
            if (httpContent != null)
            {
                Task<GlossaryTerm> term = ReadAsJsonAsync<GlossaryTerm>(httpContent);
                var resultTerm = term.Result;
                return resultTerm;
            }
            else
            {
                return null;
            }
            
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
            //NOTE: When using HttpClient.BaseAddress as we are, the path must not have a preceeding slash
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
                    // If trial is not found, log 404 message and return content as null
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

        public static async Task<GlossaryTerm> ReadAsJsonAsync<GlossaryTerm>(HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            GlossaryTerm result = JsonConvert.DeserializeObject<GlossaryTerm>(json);
            return result;
        }
    }
}
