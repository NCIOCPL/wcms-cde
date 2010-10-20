﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Globalization;
using NCI.Util;
using NCI.Web.CDE.WebAnalytics;
using NCI.Web.CDE;
using NCI.Web.CDE.UI;

namespace NCI.Web.CancerGov.Apps
{
    public class AppsBaseUserControl : SnippetControl
    {
        private WebAnalyticsPageLoad webAnalyticsPageLoad = new WebAnalyticsPageLoad();
        protected DisplayInformation pageDisplayInformation;

        virtual protected string GetResource(string key)
        {
            if( string.IsNullOrEmpty(key) )
                return "";
            object localizedObject = this.GetGlobalResourceObject("SiteWideSearch", key);
            if (localizedObject == null)
                return "key: " + key + " not localized";
            return localizedObject as string;
        }

        public WebAnalyticsPageLoad WebAnalyticsPageLoad
        {
            get { return webAnalyticsPageLoad; }
            set { webAnalyticsPageLoad = value; }
        }

        public DisplayInformation PageDisplayInformation
        {
            get { return pageDisplayInformation; }
            set { pageDisplayInformation = value; }
        }

        virtual public void RaiseErrorPage(string messageKey)
        {
            string systemMessagePageUrl = ConfigurationSettings.AppSettings["SystemMessagePage"].Trim();

            if (systemMessagePageUrl.Substring(systemMessagePageUrl.Length - 1, 1) != "?")
                systemMessagePageUrl += "?";

            systemMessagePageUrl += "msg=" + messageKey.Trim();
            Response.Redirect(systemMessagePageUrl, true);
        }
    }
}