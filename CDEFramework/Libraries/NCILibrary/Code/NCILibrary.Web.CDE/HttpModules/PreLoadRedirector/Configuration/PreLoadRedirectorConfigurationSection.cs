using System;
using System.Configuration;

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
        public PreLoadRedirectorDataSourceElement DataSource
        {
            get { return (PreLoadRedirectorDataSourceElement)base["dataSource"]; }
        }
    }

}
