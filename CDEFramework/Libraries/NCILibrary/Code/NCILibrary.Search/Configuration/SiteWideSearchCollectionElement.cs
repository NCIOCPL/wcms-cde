using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCI.Search.Configuration
{
    /// <summary>
    /// This class has the details of a search collection element
    /// </summary>
    public class SiteWideSearchCollectionElement : ConfigurationElement
    {
        /// <summary>
        /// Name of the search collection
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        /// <summary>
        /// The template used for search
        /// </summary>
        [ConfigurationProperty("template", IsRequired = true)]
        public string Template
        {
            get { return (string)base["template"]; }
        }

        /// <summary>
        /// The index used for search
        /// </summary>
       [ConfigurationProperty("index", IsRequired = true)]
        public string Index
        {
            get { return (string)base["index"]; }
        }

        /// <summary>
        /// The search website - Cancer.Gov, DCEG, etc.
        /// </summary>
        [ConfigurationProperty("site", IsRequired = true)]
        public string Site
        {
            get { return (string)base["site"]; }
        }

    }
}
