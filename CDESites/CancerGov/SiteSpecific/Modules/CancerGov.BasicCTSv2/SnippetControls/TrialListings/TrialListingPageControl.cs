using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Common.Logging;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using CancerGov.ClinicalTrialsAPI;
using CancerGov.ClinicalTrials.Basic.v2.SnippetControls.Configs;
using NCI.Web;
using NCI.Web.CDE;
using NCI.Web.CDE.Application;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;

namespace CancerGov.ClinicalTrials.Basic.v2.SnippetControls
{
    /// <summary>
    /// Page control for Trial Listing pages.
    /// This is the code behind for TrialListingPage.ascx snippet template.
    /// </summary>
    public class TrialListingPageControl : BaseTrialListingControl
    {
        /// <summary>
        /// Set logging for this class.
        /// </summary>
        static ILog log = LogManager.GetLogger(typeof(TrialListingPageControl));

        /// <summary>
        /// Creates a TrialListingConfig for use in implementation methods
        /// </summary>
        protected TrialListingConfig Config
        {
            get
            {
                return (TrialListingConfig)this.BaseConfig;
            }
        }

        /// <summary>
        /// Implementation of base trial listing page's GetConfigType()
        /// </summary>
        /// <returns>The type of the current configuration</returns>
        protected override Type GetConfigType()
        {
            return typeof(TrialListingConfig);
        }

        /// <summary>
        /// Implementation of base trial listing page's Trial Query
        /// </summary>
        /// <returns>A JObject of request filters from the TrialListingConfig.</returns>
        protected sealed override JObject GetTrialQuery()
        {
            return this.Config.RequestFilters;
        }

        /// <summary>
        /// Implementation of base trial listing page's InternalGetNoTrialsHtml
        /// </summary>
        /// <returns>The NoTrialsHTML string from the TrialListingConfig.</returns>
        protected override String InternalGetNoTrialsHtml()
        {
            return this.Config.NoTrialsHTML;
        }

         /// <summary>
         /// This method is called when no results are returned by the query
         /// </summary>
        protected override void OnEmptyResults() { }

        /// <summary>
        /// Format string for analytics params: Manual Parameters|Total Results
        /// </summary>
        /// <returns></returns>
        protected override String GetDynamicParams()
        {
            string[] analyticsParams = new string[2];
            analyticsParams[0] = "Manual Parameters";
            analyticsParams[1] = this.TotalSearchResults.ToString();
            return string.Join("|", analyticsParams);
        }

        /// <summary>
        /// Set default pageLoad analytics for this page
        /// </summary>
        protected override void SetAnalytics()
        {
            string val = "clinicaltrials_custom";
            string desc = "Clinical Trials: Custom";
            string manualAnalytics = GetDynamicParams();

            string resultsPerPage;
            if (this.TotalSearchResults < this.GetItemsPerPage())
            {
                resultsPerPage = this.TotalSearchResults.ToString();
            }
            else
            {
                resultsPerPage = this.GetItemsPerPage().ToString();
            }

            // Set event
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Events.event2, wbField =>
            {
                wbField.Value = WebAnalyticsOptions.Events.event2.ToString();
            });

            // Set props
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop11, wbField =>
            {
                wbField.Value = val;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop20, wbField =>
            {
                wbField.Value = manualAnalytics;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop62, wbField =>
            {
                wbField.Value = desc;
            });

            // Set eVars
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar10, wbField =>
            {
                wbField.Value = resultsPerPage;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar11, wbField =>
            {
                wbField.Value = val;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar20, wbField =>
            {
                wbField.Value = manualAnalytics;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar47, wbField =>
            {
                wbField.Value = val;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar62, wbField =>
            {
                wbField.Value = desc;
            });
        }
    }
}
