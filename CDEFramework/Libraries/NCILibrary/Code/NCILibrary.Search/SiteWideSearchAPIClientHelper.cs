using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NCI.Search.Configuration;

namespace NCI.Search

{
    /// <summary>
    /// Helper class get getting an API client instance.
    /// </summary>
    public static class SiteWideSearchAPIClientHelper
    {
        /// <summary>
        /// Gets an instance of a SiteWideSearchAPI client
        /// </summary>
        /// <returns></returns>
        public static SiteWideSearchAPIClient GetClientInstance()
        {

            string baseApiPath = WebAPISection.GetAPIUrl();
            string appPath = ConfigurationManager.AppSettings["SiteWideSearchAPIAppPath"];
            string versionPath = ConfigurationManager.AppSettings["SiteWideSearchAPIVersionPath"];

            if (string.IsNullOrWhiteSpace(appPath))
                throw new ConfigurationErrorsException("error: SiteWideSearchAPIAppPath cannot be null or empty");

            if (string.IsNullOrWhiteSpace(versionPath))
                throw new ConfigurationErrorsException("error: SiteWideSearchAPIVersionPath cannot be null or empty");

            HttpClient client = new HttpClient();
            //NOTE: the base URL MUST have a trailing slash
            client.BaseAddress = new Uri(String.Format("{0}/{1}/{2}/", baseApiPath, appPath, versionPath));

            return new SiteWideSearchAPIClient(client);
        }   
    }
}
