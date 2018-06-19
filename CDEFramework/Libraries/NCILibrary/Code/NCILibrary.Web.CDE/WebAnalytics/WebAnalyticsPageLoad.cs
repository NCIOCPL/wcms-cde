using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Common.Logging;

namespace NCI.Web.CDE.WebAnalytics
{

    /// <summary>
    /// WebAnalyticsPageLoad is used to create the necessary JavaScript code to create Omniture page load metrics.  
    /// It contains methods for setting different types of page view metrics including: 
    /// channel, custom variables (props), custom conversion variables (eVars), and events.    
    /// </summary>
    public class WebAnalyticsPageLoad
    {
        static ILog log = LogManager.GetLogger(typeof(WebAnalyticsPageLoad));

        private const string DELIMITER = "'";
        private const string WEB_ANALYTICS_COMMENT_START = "<!-- ***** Begin NCI Web Analytics data element ***** -->";
        private const string WEB_ANALYTICS_COMMENT_END = "<!-- ***** End NCI Web Analytics data element ***** -->";
        private const bool TEST_MODE = false;  // When true, Omniture image request is not sent 

        private StringBuilder pageLoadPreTag = new StringBuilder();
        private StringBuilder pageLoadPostTag = new StringBuilder();
        private Dictionary<int, string> props = new Dictionary<int, string>();
        private Dictionary<int, string> evars = new Dictionary<int, string>();
        private List<string> events = new List<string>();
        private String concatProps = "";
        private String concatEvars = "";
        private String concatEvents = "";
        private string channel = "";
        private string pageName = null;
        private string pageType = "";
        private string language = "";
        private IPageAssemblyInstruction pgInstruction = PageAssemblyContext.Current.PageAssemblyInstruction;
        private String waDataID = ConfigurationManager.AppSettings["WADataElementID"].ToString();

        /// <summary>the constructor builds base Omniture page load code.   
        /// Also sets the default custom variables (props), custom conversion variables (eVars), and events. .</summary>
        public WebAnalyticsPageLoad()
        {
            // Default props, eVars, and/or events
            AddEvent(WebAnalyticsOptions.Events.event1); // page view event
            pageLoadPostTag.AppendLine(WEB_ANALYTICS_COMMENT_END);
        }

        /**
         * No script tag 
         * TODO: update or remove
         */
        private StringBuilder NoScriptTag()
        {
            StringBuilder noScriptTag = new StringBuilder();
            noScriptTag.AppendLine("<noscript>");
            noScriptTag.AppendLine("<a href='http://www.omniture.com' title='Web Analytics'>");
            noScriptTag.AppendLine("<img src='http://metrics.cancer.gov/b/ss/nciglobal/1/H.20.3–NS/0' height='1' width='1' border='0' alt='' />");
            noScriptTag.AppendLine("</a>");
            noScriptTag.AppendLine("</noscript>");
            return noScriptTag;
        }

        /// <summary>When DoWebAnalytics is true, this method renders the Omniture page load JavaScript code.</summary>
        public string Tag()
        {
            StringBuilder output = new StringBuilder();
            string reportSuites = "";

            if (WebAnalyticsOptions.IsEnabled)
            {
                output.AppendLine("");
                output.AppendLine(WEB_ANALYTICS_COMMENT_START);

                // Report Suites JavaScript variable (s_account) must be set before the s_code file is loaded
                // Get custom suites that are set on the navon. Default suites are being set in wa_wcms_pre.js
                try
                {
                    string sectionPath = pgInstruction.SectionPath;
                    SectionDetail detail = SectionDetailFactory.GetSectionDetail(sectionPath);
                    string customSuites = detail.GetWASuites();
                    if (!string.IsNullOrEmpty(customSuites))
                    {
                        reportSuites += customSuites;
                    }
                }
                catch (Exception ex)
                {
                    log.Debug("Tag(): Exception encountered while retrieving web analytics suites.", ex);
                    reportSuites += "";
                }

                // TODO: clean / refactor this 
                // if props are set, output them to the tag
                if (props.Count > 0) 
                {
                    foreach (var k in props.Keys.OrderBy(k => k))
                    {
                        concatProps +=("data-prop" + k.ToString() + "=\"" + props[k] + "\" ");
                    }
                }

                // if eVars are set, output them to the tag
                if (evars.Count > 0) 
                {
                    var items = from k in evars.Keys
                                orderby k ascending
                                select k;
                    foreach (int k in items)
                    {
                        concatEvars += ("data-evar" + k.ToString() + "=\"" + evars[k] + "\" ");
                    }
                }

                // if events have been defined, output then to the tag
                if (events.Count > 0)  
                {
                    concatEvents = string.Join(",", events.ToArray<string>());
                }

                // Output analytics values to an HTML data element. 
                // This element will be queried by the DTM analytics JavaScript
                // waDataID is set in the Web.config
                output.AppendLine("<meta id=\"" + waDataID + "\" "
                                   + "data-suites=\"" + reportSuites + "\" "
                                   + "data-channel=\"" + channel + "\" "
                                   + "data-pagename=\"" + pageName + "\" "
                                   + "data-pagetype=\"" + pageType + "\" "
                                   + "data-events=\"" + concatEvents + "\" "
                                   + concatProps + concatEvars + " />");
                output.Append(pageLoadPreTag.ToString());

                // Add calls to special page-load functions for a specific channel
                bool firstTime = true;
                bool containsFunctions = false;
                foreach (string function in WebAnalyticsOptions.GetSpecialPageLoadFunctionsForChannel(channel, language))
                {
                    if (function != "")
                    {
                        if (firstTime)
                        {
                            output.AppendLine("//Special Page-Load Functions (SPLF)");
                            firstTime = false;
                        }
                        string[] functions = function.Split(',');
                        foreach (string item in functions)
                        {
                            output.AppendLine("if(typeof(NCIAnalytics." + item.Trim() + ") == 'function')");
                            output.AppendLine("   NCIAnalytics." + item.Trim() + "();");

                        }
                        containsFunctions = true;
                    }
                }
                if (containsFunctions)
                    output.AppendLine("");

                output.AppendLine(pageLoadPostTag.ToString());
            }
            return output.ToString();
        }

        /// <summary>Adds an Omniture custom variable (prop) to the Omniture page load JavaScript code 
        /// with delimiters attached to value parameter.</summary>
        /// <param name="propNumber">Omniture custom variable (prop) number</param>
        /// <param name="value">Value assigned to Omniture custom variable (prop)</param>
        public void AddProp(int propNumber, string value)
        {
            AddProp(propNumber, value, false);
        }

        /// <summary>Adds an Omniture custom variable (prop) to the Omniture page load JavaScript code 
        /// with delimiters attached to value parameter.</summary>
        /// <param name="propNumber">Omniture custom variable (prop) number</param>
        /// <param name="value">Value assigned to Omniture custom variable (prop)</param>
        public void AddProp(WebAnalyticsOptions.Props propNumber, string value)
        {
            AddProp((int)propNumber, value, false);

        }

        /// <summary>Adds Omniture custom variable (prop) to the Omniture page load JavaScript code.</summary>
        /// <param name="propNumber">Omniture custom variable (prop) number</param>
        /// <param name="value">Value assigned to Omniture custom variable (prop)</param>
        /// <param name="NoDelimiters">If true, delimiters are added to the beginning and end of the value parameter.  If false,
        /// no delimiters are added (used when value parameter already contains delimiters)</param>
        public void AddProp(WebAnalyticsOptions.Props propNumber, string value, bool NoDelimiters)
        {
            AddProp((int)propNumber, value, NoDelimiters);
        }

        /// <summary>Adds Omniture custom variable (prop) to the Omniture page load JavaScript code.</summary>
        /// <param name="propNumber">Omniture custom variable (prop) number</param>
        /// <param name="value">Value assigned to Omniture custom variable (prop)</param>
        /// <param name="NoDelimiters">If true, delimiters are added to the beginning and end of the value parameter.  If false,
        /// no delimiters are added (used when value parameter already contains delimiters)</param>
        public void AddProp(int propNumber, string value, bool NoDelimiters)
        {
            // if value is null set to empty string 
            value = value ?? string.Empty;
            string newValue = NoDelimiters ? value : DELIMITER + value.Replace("'", "\\'") + DELIMITER;

            if (props.ContainsKey(propNumber))
                props[propNumber] = newValue;
            else
                props.Add(propNumber, newValue);
        }

        /// <summary>Adds an Omniture custom conversion variable (eVar) to the Omniture page load JavaScript code with delimiters.</summary>
        /// <param name="eVarNumber">Omniture custom conversion variable (eVar) number</param>
        /// <param name="value">Value assigned to Omniture custom conversion variable (eVar)</param>
        public void AddEvar(int eVarNumber, string value)
        {
            AddEvar(eVarNumber, value, false);
        }

        /// <summary>Adds an Omniture custom conversion variable (eVar) to the Omniture page load JavaScript code with delimiters.</summary>
        /// <param name="eVarNumber">Omniture custom conversion variable (eVar) number</param>
        /// <param name="value">Value assigned to Omniture custom conversion variable (eVar)</param>
        public void AddEvar(WebAnalyticsOptions.eVars eVarNumber, string value)
        {
            AddEvar((int)eVarNumber, value, false);
        }

        /// <summary>Adds Omniture custom conversion variable (eVar) to the Omniture page load JavaScript code.</summary>
        /// <param name="eVarNumber">Omniture custom conversion variable (eVar) number</param>
        /// <param name="value">Value assigned to Omniture custom conversion variable (eVar)</param>
        /// <param name="NoDelimiters">If true, delimiters are added to the beginning and end of value parameter.  If false,
        /// no delimiters are added (used when value param already contains delimiters)</param>
        public void AddEvar(WebAnalyticsOptions.eVars eVarNumber, string value, bool NoDelimiters)
        {
            AddEvar((int)eVarNumber, value, NoDelimiters);
        }

        /// <summary>Adds Omniture custom conversion variable (eVar) to the Omniture page load JavaScript code.</summary>
        /// <param name="eVarNumber">Omniture custom conversion variable (eVar) number</param>
        /// <param name="value">Value assigned to Omniture custom conversion variable (eVar)</param>
        /// <param name="NoDelimiters">If true, delimiters are added to the beginning and end of value parameter.  If false,
        /// no delimiters are added (used when value param already contains delimiters)</param>
        public void AddEvar(int eVarNumber, string value, bool NoDelimiters)
        {
            // if value is null set to empty string 
            value = value ?? string.Empty;
            string newValue = NoDelimiters ? value : DELIMITER + value.Replace("'", "\\'") + DELIMITER;

            if (evars.ContainsKey(eVarNumber))
                evars[eVarNumber] = newValue;
            else
                evars.Add(eVarNumber, newValue);
        }

        /// <summary>Adds Omniture event to the Omniture page load JavaScript code.</summary>
        /// <param name="eventNumber">Omniture event number</param>
        public void AddEvent(WebAnalyticsOptions.Events eventNumber)
        {
            AddEvent((int)eventNumber);
        }

        /// <summary>Adds Omniture event to the Omniture page load JavaScript code.</summary>
        /// <param name="eventNumber">Omniture event number</param>
        public void AddEvent(int eventNumber)
        {
            if (eventNumber > 0)
            {
                string eventString = "event" + eventNumber.ToString();
                if (!events.Contains(eventString))
                    events.Add(eventString);
            }
        }


        /// <summary>Sets the value of the Omniture channel variable in the Omniture page load JavaScript code.</summary>
        /// <param name="channelValue">Value assigned to Omniture channel variable</param>
        public void SetChannel(string channelValue)
        {
            channel = channelValue.Replace("'", "\\'");

        }

        /// <summary>Sets the language in the Omniture page load JavaScript code.</summary>
        /// <param name="languageValue">Value assigned to Omniture language variable: english, spanish</param>
        public void SetLanguage(string languageValue)
        {
            switch (languageValue.ToLower())
            {
                case "en":
                    languageValue = "english";
                    break;
                case "es":
                    languageValue = "spanish";
                    break;
                default:
                    languageValue = "english";
                    break;
            }
            this.AddProp(WebAnalyticsOptions.Props.prop8, languageValue); // Language
            this.AddEvar(WebAnalyticsOptions.eVars.evar2, languageValue); // Language
            language = languageValue;
        }

        /// <summary>Sets the value of the Omniture pageName variable in the Omniture page load JavaScript code.</summary>
        /// <param name="pageNameValue">Value assigned to Omniture pageName variable</param>
        public void SetPageName(string pageNameValue)
        {
            pageName = pageNameValue.Replace("'", "\\'");
            this.AddEvar(WebAnalyticsOptions.eVars.evar1, pageName); // Page name
        }

        /// <summary>Sets the value of the Omniture pageType  variable in the Omniture page load JavaScript code.</summary>
        /// <param name="pageTypeValue">Value assigned to Omniture pageType variable</param>
        public void SetPageType(string pageTypeValue)
        {
            pageType = pageTypeValue;
        }

        /// <summary>Clears all previously set props, eVars, events, channel, pageName, and pageType.</summary>
        public void ClearAll()
        {
            props.Clear();
            evars.Clear();
            events.Clear();
            channel = "";
            pageName = null;
            pageType = "";
        }

    }
}