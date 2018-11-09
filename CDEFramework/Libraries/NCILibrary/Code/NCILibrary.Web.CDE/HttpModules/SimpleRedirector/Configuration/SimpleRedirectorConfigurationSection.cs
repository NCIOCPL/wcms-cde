using System.Configuration;

namespace NCI.Web.CDE.SimpleRedirector.Configuration
{
    public class SimpleRedirectorConfigurationSection : ConfigurationSection
    {
        public static SimpleRedirectorConfigurationSection Get()
        {
            SimpleRedirectorConfigurationSection config = (SimpleRedirectorConfigurationSection)ConfigurationManager.GetSection("nci/web/simpleRedirection");
            return config;
        }

        [ConfigurationProperty("dataSource")]
        public DataSourceConfigurationElement DataSource
        {
            get { return (DataSourceConfigurationElement)base["dataSource"]; }
        }
    }
}
