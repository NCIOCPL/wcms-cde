using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCI.Search.Configuration
{
    public class SiteWideSearchSection : ConfigurationSection
    {
        /// <summary>
        /// Sitewide Search configuration path
        /// </summary>
        private static readonly string CONFIG_SECTION_NAME = "nci/search/siteWideSearch";

        /// <summary>
        /// Collection of sitewide search collections
        /// </summary>
        [ConfigurationProperty("siteWideSearchCollections")]
        [ConfigurationCollection(typeof(SiteWideSearchCollectionElement))]
        public SiteWideSearchCollectionElementCollection SiteWideSearchCollections
        {
            get { return (SiteWideSearchCollectionElementCollection)base["siteWideSearchCollections"]; }
        }

        /// <summary>
        /// Gets the SiteWideSearchCollection elements based on the name of the collection
        /// </summary>
        /// <param name="searchCollectionName">The name of the search collection</param>
        public static SiteWideSearchCollectionElement GetSearchCollectionConfig(string searchCollectionName)
        {
            SiteWideSearchSection config = (SiteWideSearchSection)ConfigurationManager.GetSection(CONFIG_SECTION_NAME);

            if (config == null)
                throw new ConfigurationErrorsException("The configuration section, " + CONFIG_SECTION_NAME + ", cannot be found");

            if (config.SiteWideSearchCollections == null)
                throw new ConfigurationErrorsException(CONFIG_SECTION_NAME + "error: siteWideSearchCollections cannot be null or empty");

            //Find the cluster
            foreach (SiteWideSearchCollectionElement searchCollection in config.SiteWideSearchCollections)
            {
                if (searchCollection.Name == searchCollectionName)
                    return searchCollection;
            }

            throw new ConfigurationErrorsException("SiteWideSearch collection, " + searchCollectionName + ", cannot be found in configuration.");
        }
    }
}
