using System;
using System.Configuration;


namespace NCI.Web.CDE.SimpleRedirector.Configuration
{
    public class DataSourceConfigurationElement : ConfigurationElement
    {
        const String DEFAULT_FILE_LOCATION = "~/PublishedContent/Files/Configuration/RedirectionList.txt";
        const String DEFAULT_SEPARATOR_STRING = ",";
        char DEFAULT_SEPARATOR_CHAR { get { return DEFAULT_SEPARATOR_STRING[0]; } }

        [ConfigurationProperty("file", IsRequired = false, DefaultValue = DEFAULT_FILE_LOCATION)]
        public String DataFile
        {
            get { return (String)this["file"]; }
            set { this["file"] = value; }
        }

        [ConfigurationProperty("separator", IsRequired = false, DefaultValue = DEFAULT_SEPARATOR_STRING)]
        public String SeparatorRaw
        {
            get { return (String)this["separator"]; }
            set { this["separator"] = value; }
        }

        public char Separator
        {
            get
            {
                char rval;
                String temp = SeparatorRaw;
                if (String.IsNullOrEmpty(temp))
                    rval = DEFAULT_SEPARATOR_CHAR;
                else
                    rval = temp.Trim()[0];
                return rval;
            }
        }
    }
}
