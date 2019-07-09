using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCI.Search.Configuration
{
    /// <summary>
    /// This class is a collection of all the SiteWide Search Collection Elements
    /// </summary>
    [ConfigurationCollection(typeof(SiteWideSearchCollectionElement),
        AddItemName = "add",
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class SiteWideSearchCollectionElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new ConfigurationElement
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SiteWideSearchCollectionElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element 
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SiteWideSearchCollectionElement)element).Name;
        }
    }
}
