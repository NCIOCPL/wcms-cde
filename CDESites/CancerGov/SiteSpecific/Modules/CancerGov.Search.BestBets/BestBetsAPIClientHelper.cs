using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NCI.Search.Configuration;

namespace CancerGov.Search.BestBets

{
    /// <summary>
    /// Helper class get getting an API client instance.
    /// </summary>
    public static class BestBetsAPIClientHelper
    {
        /// <summary>
        /// Gets an instance of a BestBetsAPI client
        /// </summary>
        /// <returns></returns>
        public static BestBetsAPIClient GetClientInstance()
        {
            
            string baseApiPath = WebAPISection.GetAPIUrl();
            string appPath = ConfigurationManager.AppSettings["BestBetsAPIAppPath"];
            string versionPath = ConfigurationManager.AppSettings["BestBetsAPIVersionPath"];

            if (string.IsNullOrWhiteSpace(appPath))
                throw new ConfigurationErrorsException("error: BestBetsAPIAppPath cannot be null or empty");

            if (string.IsNullOrWhiteSpace(versionPath))
                throw new ConfigurationErrorsException("error: BestBetsAPIVersionPath cannot be null or empty");

            HttpClient client = new HttpClient();
            //NOTE: the base URL MUST have a trailing slash
            client.BaseAddress = new Uri(String.Format("{0}/{1}/{2}/", baseApiPath, appPath, versionPath));

            return new BestBetsAPIClient(client);
        }   
    }
}
