﻿using System;
using System.Configuration;
using NCI.Web.CDE;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;

namespace TCGA.Apps
{
    public class AppsBaseUserControl : SnippetControl
    {
        private WebAnalyticsPageLoad webAnalyticsPageLoad = new WebAnalyticsPageLoad();
        private DisplayInformation pageDisplayInformation;
        private string strHelpPageLink = "#";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        virtual protected string GetResource(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";
            object localizedObject = this.GetGlobalResourceObject("SiteWideSearch", key);
            if (localizedObject == null)
                return "key: " + key + " not localized";
            string val = localizedObject as string;
            if (string.IsNullOrEmpty(val))
                return key;
            return localizedObject as string;
        }

        public WebAnalyticsPageLoad WebAnalyticsPageLoad
        {
            get { return webAnalyticsPageLoad; }
            set { webAnalyticsPageLoad = value; }
        }

        public DisplayInformation PageDisplayInformation
        {
            get
            {
                pageDisplayInformation = new DisplayInformation();
                switch (PageInstruction.Language)
                {
                    case "en":
                        pageDisplayInformation.Language = DisplayLanguage.English;
                        break;
                    case "es":
                        pageDisplayInformation.Language = DisplayLanguage.Spanish;
                        break;
                    default:
                        pageDisplayInformation.Language = DisplayLanguage.English;
                        break;
                }
                pageDisplayInformation.Version = PageAssemblyContext.Current.DisplayVersion;

                return pageDisplayInformation;
            }

        }

        virtual public void RaiseErrorPage()
        {
            RaiseErrorPage("");
        }

        virtual public void RaiseErrorPage(string messageKey)
        {
            string systemMessagePageUrl = ConfigurationManager.AppSettings["SystemMessagePage"].Trim();

            if (systemMessagePageUrl.Substring(systemMessagePageUrl.Length - 1, 1) != "?")
                systemMessagePageUrl += "?";

            systemMessagePageUrl += "msg=" + messageKey.Trim();
            Response.Redirect(systemMessagePageUrl, true);
        }

        public string PrettyUrl
        {
            get
            {
                return this.PageInstruction.GetUrl("PrettyUrl").UriStem;
            }
        }
        public string PrettyUrlWithQS
        {
            get
            {
                return this.PageInstruction.GetUrl("PrettyUrl").ToString();
            }
        }

        public string CurrentPageUrl
        {
            get { return this.Request.RawUrl; }
        }

        public string HelpPageLink
        {
            get { return strHelpPageLink; }
            set { strHelpPageLink = value; }
        }
    }
}