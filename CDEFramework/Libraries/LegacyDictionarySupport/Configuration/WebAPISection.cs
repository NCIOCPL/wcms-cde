using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyDictionarySupport.Configuration
{
    /// <summary>
    /// Configuration information for a Web API
    /// </summary>
    public class WebAPISection : ConfigurationSection
    {
        private static readonly string CONFIG_SECTION_NAME = "legacyServices/webAPI";

        /// <summary>
        /// Gets the host name of the Web API server.
        /// </summary>
        [ConfigurationProperty("apiHost", IsRequired=true)]
        public string APIHost
        {
            get { return (string)base["apiHost"]; }
        }

        /// <summary>
        /// Gets the port number for the API
        /// </summary>
        [ConfigurationProperty("apiPort", IsRequired=false)]
        public string APIPort
        {
            get { return (string)base["apiPort"]; }
        }

        /// <summary>
        /// Gets the protocol for the API
        /// </summary>
        [ConfigurationProperty("apiProtocol", IsRequired = true)]
        public string APIProtocol
        {
            get { return (string)base["apiProtocol"]; }
        }

        /// <summary>
        /// Gets the URL for the Web API from the configuration
        /// </summary>
        public static string GetAPIUrl()
        {
            string url = "";
            WebAPISection config = (WebAPISection)ConfigurationManager.GetSection(CONFIG_SECTION_NAME);

            if (config == null)
                throw new ConfigurationErrorsException("The configuration section, " + CONFIG_SECTION_NAME + ", cannot be found");

            if (string.IsNullOrWhiteSpace(config.APIProtocol))
                throw new ConfigurationErrorsException(CONFIG_SECTION_NAME + "error: apiProtocol cannot be null or empty");

            if (string.IsNullOrWhiteSpace(config.APIHost))
                throw new ConfigurationErrorsException(CONFIG_SECTION_NAME + "error: apiHost cannot be null or empty");

            url = string.Format("{0}://{1}", config.APIProtocol, config.APIHost);

            if (!string.IsNullOrWhiteSpace(config.APIPort))
            {
                url += ":" + config.APIPort;
            }

            return url;
        }
    }
}
