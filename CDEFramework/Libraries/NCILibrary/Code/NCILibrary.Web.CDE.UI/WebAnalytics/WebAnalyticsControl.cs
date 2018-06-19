using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;
using NCI.Web.CDE.WebAnalytics;

namespace NCI.Web.CDE.UI.WebControls
{
    /// <summary>
    /// This web controls renders the Omniture java script required for web analytics. 
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:WebAnalyticsControl runat=server></{0}:WebAnalyticsControl>")]
    public class WebAnalyticsControl : WebControl
    {
        static ILog log = LogManager.GetLogger(typeof(WebAnalyticsControl));

        protected override void OnInit(EventArgs e)
        {
        }
        
        /// <summary>
        /// Override this method to avoid rendering the default span tag.
        /// </summary>
        /// <param name="writer"></param>
        protected override void Render(HtmlTextWriter writer)
        {
            RenderContents(writer);
        }
        /// <summary>
        /// Uses the WebAnalyticsPageLoad helper class to render the required omniture java script 
        /// for Web Analytics.
        /// </summary>
        /// <param name="output">HtmlTextWriter object</param>
        protected override void RenderContents(HtmlTextWriter output)
        {
            base.RenderContents(output);
            IPageAssemblyInstruction pgInstruction = PageAssemblyContext.Current.PageAssemblyInstruction;
            WebAnalyticsSettings webAnalyticsSettings = pgInstruction.GetWebAnalytics();
            string configChannelName = "";
            if (webAnalyticsSettings != null)
            {
                WebAnalyticsPageLoad webAnalyticsPageLoad = new WebAnalyticsPageLoad();

                webAnalyticsPageLoad.SetLanguage(PageAssemblyContext.Current.PageAssemblyInstruction.GetField("language"));
                
                // Use pretty url to get channel name from the mapping, mapping information is in web.config
                string prettyUrl = pgInstruction.GetUrl("PrettyUrl").UriStem;
                if (!string.IsNullOrEmpty(prettyUrl))
                {
                    configChannelName = WebAnalyticsOptions.GetChannelForUrlPath(prettyUrl);
                }

                // Get the channel name from the section details. As of the cancer.gov Feline release, analytics channels
                // are set in the navons, not Web.config. The old functionality is being used in the catch block for now.
                try
                {
                    string sectionPath = pgInstruction.SectionPath;
                    SectionDetail detail = SectionDetailFactory.GetSectionDetail(sectionPath);
                    string channelName = WebAnalyticsOptions.GetChannelsFromSectionDetail(detail);
                    webAnalyticsPageLoad.SetChannel(channelName);
                }
                catch (Exception ex)
                {
                    log.Warn("RenderContents(): Error retrieving analytics channel.", ex);
                    webAnalyticsPageLoad.SetChannel(configChannelName);
                }
                foreach (KeyValuePair<WebAnalyticsOptions.eVars, string> kvp in webAnalyticsSettings.Evars)
                    webAnalyticsPageLoad.AddEvar(kvp.Key, kvp.Value ); 

                foreach (KeyValuePair<WebAnalyticsOptions.Events, string> kvp in webAnalyticsSettings.Events)
                    webAnalyticsPageLoad.AddEvent(kvp.Key); 

                foreach (KeyValuePair<WebAnalyticsOptions.Props, string> kvp in webAnalyticsSettings.Props)
                    webAnalyticsPageLoad.AddProp( kvp.Key, kvp.Value );

                output.Write(webAnalyticsPageLoad.TagHead());
            }
        }
    }
}
