using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.UI;
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
        private const string WEB_ANALYTICS_COMMENT_START = "<!-- ***** NCI Web Analytics - DO NOT ALTER ***** -->";
        private const string WEB_ANALYTICS_COMMENT_END = "<!-- ***** End NCI Web Analytics ***** -->";
        private const bool TEST_MODE = false;  // When true, Omniture image request is not sent 

        private StringBuilder pageLoadPreTag = new StringBuilder();
        private StringBuilder pageLoadPostTag = new StringBuilder();
        private bool pageWideLinkTracking = false;
        private Dictionary<int, string> props = new Dictionary<int, string>();
        private Dictionary<int, string> evars = new Dictionary<int, string>();
        private List<string> events = new List<string>();
        private String suites = "";
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

        /// <summary>Draw noscript tag</noscript></summary>
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

        /// <summary>Draw the analytics metadata to be used in the document head.</summary>
        /// <param name="writer">Text writer object used to output HTML tags</param>
        public void TagHead(HtmlTextWriter writer)
        {
            string concatEvents = string.Empty;
            string propValue = string.Empty;
            string eVarValue = string.Empty;

            // Draw meta tag ID, suites, channel attributes
            writer.AddAttribute(HtmlTextWriterAttribute.Id, waDataID);
            writer.AddAttribute("data-suites", suites);
            writer.AddAttribute("data-channel", channel);

            // if events have been defined, output then to the tag
            if (events.Count > 0)
            {
                concatEvents = string.Join(",", events.ToArray<string>());
                writer.AddAttribute("data-events", concatEvents);
            }

            // if props are set, output them to the tag
            if (props.Count > 0)
            {
                foreach (int k in props.Keys.OrderBy(k => k))
                {
                    propValue = cleanQuotes(props[k]);
                    writer.AddAttribute(("data-prop" + k.ToString()), propValue);
                }
            }

            // if eVars are set, output them to the tag
            if (evars.Count > 0)
            {
                foreach (int k in evars.Keys.OrderBy(k => k))
                {
                    eVarValue = cleanQuotes(evars[k]);
                    writer.AddAttribute("data-evar" + k.ToString(), eVarValue);
                }
            }

            // Draw the <meta> tag 
            writer.RenderBeginTag(HtmlTextWriterTag.Meta);
            writer.RenderEndTag();
        }

        /// <summary>When DoWebAnalytics is true, this method renders the Omniture page load JavaScript code.</summary>
        [Obsolete("This is the legacy method for drawing analytics JavaScript into the page HTML.")]
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

                // Output analytics Javascript to HTML source in this order:
                // 1. wa_wcms_pre.js source URL
                // 2. Snippet to set s_account value
                // 3. NCIAnalyticsFunctions.js source URL (see line 47)
                // 4. s_code source URL
                // 5. Channel, Prop, eVar, and Event info
                // Note: as of the Feline release, the web analytics javascript is hosted on static.cancer.gov
                // output.AppendLine("<script language=\"JavaScript\" type=\"text/javascript\" src=\"" + WaPre + "\"></script>");
                output.AppendLine("<script language=\"JavaScript\" type=\"text/javascript\">");
                output.AppendLine("<!--");
                output.AppendLine("var s_account = AnalyticsMapping.GetSuites(\"" + reportSuites + "\");");
                output.AppendLine("-->");
                output.AppendLine("</script>");

                output.Append(pageLoadPreTag.ToString());

                if (pageWideLinkTracking)
                {
                    // Page-wide link tracking is current not used - this may be implemented at a future date
                    // This however should just output a JS function call that lives in the NCIAnalytics.js
                    // file instead of the inline code.
                    //output.AppendLine(LinkTrackPageLoadCode().ToString());
                }

                if (channel != "") // if channel is set, output them to the tag
                    output.AppendLine("s.channel=" + DELIMITER + channel + DELIMITER + ";");

                if (pageName != null) // if pageName is not null (empty string ok), output them to the tag
                    output.AppendLine("s.pageName=" + DELIMITER + pageName + DELIMITER + ";");

                if (pageType != "") // if pageType is set, output them to the tag
                    output.AppendLine("s.pageType=" + DELIMITER + pageType + DELIMITER + ";");

                if (props.Count > 0) // if props are set, output them to the tag
                {

                    foreach (var k in props.Keys.OrderBy(k => k))
                    {
                        output.AppendLine("s.prop" + k.ToString() + "=" + props[k] + ";");
                    }
                }

                if (evars.Count > 0) // if eVars are set, output them to the tag
                {
                    var items = from k in evars.Keys
                                orderby k ascending
                                select k;
                    foreach (int k in items)
                    {
                        output.AppendLine("s.eVar" + k.ToString() + "=" + evars[k] + ";");
                    }
                }

                if (events.Count > 0)  // if events have been defined, output then to the tag
                {
                    output.AppendLine("s.events=" + DELIMITER + string.Join(",", events.ToArray<string>()) + DELIMITER + ";");
                }

                output.AppendLine("");

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
            language = languageValue;
        }

        /// <summary>Sets the value of the Omniture pageName variable in the Omniture page load JavaScript code.</summary>
        /// <param name="pageNameValue">Value assigned to Omniture pageName variable</param>
        public void SetPageName(string pageNameValue)
        {
            pageName = pageNameValue.Replace("'", "\\'");
        }

        /// <summary>Sets the value of the Omniture pageType  variable in the Omniture page load JavaScript code.</summary>
        /// <param name="pageTypeValue">Value assigned to Omniture pageType variable</param>
        public void SetPageType(string pageTypeValue)
        {
            pageType = pageTypeValue;
        }

        /// <summary>Set custom suites based on section details. Default suites are being set in wa_wcms_pre.js.</summary>
        /// <param name="detail">Nav details for the current page</param>
        public void SetReportSuites(SectionDetail detail)
        {
            try
            {
                string customSuites = detail.GetWASuites();
                if (!string.IsNullOrEmpty(customSuites))
                {
                    suites += customSuites;
                }
            }
            catch (Exception ex)
            {
                log.Debug("WebAnalyticsPageLoad:SetReportSuites(): Exception encountered while retrieving web analytics suites.", ex);
            }
        }

        /// <summary>Trim quotes and spaces from string and replace with double quotes</summary>
        /// <param name="content">Meta content attribute</param>
        /// <returns>Cleaned string</returns>
        public String cleanQuotes(string value)
        {
            char[] charsToTrim = { '\'', ' ', '"' };
            return value.Trim(charsToTrim);
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