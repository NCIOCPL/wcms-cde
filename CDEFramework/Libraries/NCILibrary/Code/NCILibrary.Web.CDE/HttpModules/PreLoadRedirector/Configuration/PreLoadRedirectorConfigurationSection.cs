using System;
using System.Configuration;
using NCI.Web.CDE.SimpleRedirector.Configuration;

namespace NCI.Web.CDE.PreLoadRedirector.Configuration
{
    public class PreLoadRedirectorConfigurationSection : ConfigurationSection
    {
        public static PreLoadRedirectorConfigurationSection Get()
        {
            PreLoadRedirectorConfigurationSection config = (PreLoadRedirectorConfigurationSection)ConfigurationManager.GetSection("nci/web/preLoadRedirection");
            return config;
        }

        [ConfigurationProperty("dataSource")]
        public DataSourceConfigurationElement DataSource
        {
            get { return (DataSourceConfigurationElement)base["dataSource"]; }
        }
    }

}
