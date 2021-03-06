﻿using System;
using System.Configuration;
using CancerGov.CDR.DataManager;
using NCI.Util;
using NCI.Web.CDE;
using NCI.Web.CDE.WebAnalytics;

namespace CancerGov.Web.SnippetTemplates
{
    public partial class ViewGenericsProfessionals : SearchBaseUserControl
    {
        private string content;

        #region Page properties

        /// <summary>
        /// Gets cancer genetics professional summary
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }

        #endregion


        /// <summary>
        /// Event method sets content version and template and user control properties<br/>
        /// <br/>
        /// [1] Uses input parameter, personid (comma-delimited intIds or recnum;intId pairs), to <br/>
        ///     identify instance of template<br/>
        /// [2] Uses usp_GetGeneticProfessional to pull professional summary data<br/> 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string args = Strings.Clean(Request.Params["personid"]);

            if (args != null)
            {
                string[] personids = args.Split(',');
                GeneticProfessional geneticPro;
                String geneticProHtml;

                foreach (string id in personids)
                {
                    if (Strings.Clean(id) != null)
                    {
                        geneticPro = new GeneticProfessional(id);
                        geneticProHtml = geneticPro.GetHtml(Server.MapPath("/Stylesheets"));
                        geneticProHtml = geneticProHtml.Replace("/search/search_geneticsservices.aspx", SearchPageInfo.SearchPagePrettyUrl);
                        geneticProHtml = geneticProHtml.Replace("<GeneticsProfessional>", "");
                        geneticProHtml = geneticProHtml.Replace("</GeneticsProfessional>", "");

                        content += "<li><div class='result'>" + geneticProHtml + "</div></li>";
                    }
                }

                if (Strings.Clean(content) == null)
                {
                    content = "The cancer genetic professional(s) you selected was not found.";
                }
                else
                {
                    // wrap in ul element
                    content = "<div class='slot-item'><div class='results'><ul class='no-bullets'>" + content + "</ul></div></div>";
                }
            }
            else
            {
                content = "No genetic professional(s) were selected.";
            }

            string pagePrintUrl = PageAssemblyContext.Current.requestedUrl + "?personid=" + args + "&print=1";
            PageAssemblyContext.Current.PageAssemblyInstruction.AddUrlFilter("Print", (name, url) =>
            {
                url.SetUrl(pagePrintUrl);
            });
            
            //Web Analytics
            if (WebAnalyticsOptions.IsEnabled)
            {
                // Add page name to analytics
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar1, wbField =>
                {
                    wbField.Value = ConfigurationManager.AppSettings["HostName"] + SearchPageInfo.SearchResultsPrettyUrl;
                });
                //End of Web Anlytics
            }

            this.PageInstruction.AddUrlFilter(PageAssemblyInstructionUrls.CanonicalUrl, (name, url) =>
            {
                string localUrl = url.ToString();

                if (args != "")
                    localUrl += "?personid=" + args;

                url.SetUrl(localUrl);
            });

        }
		
    }
}