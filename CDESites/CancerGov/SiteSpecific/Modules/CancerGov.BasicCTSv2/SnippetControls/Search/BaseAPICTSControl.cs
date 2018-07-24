using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using CancerGov.ClinicalTrialsAPI;
using CancerGov.ClinicalTrials.Basic.v2.Configuration;
using Common.Logging;
using NCI.Web;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;

namespace CancerGov.ClinicalTrials.Basic.v2.SnippetControls
{
    /// <summary>
    /// This is the base class for all API-Based Clinical Trial Search controls.
    /// This will also serve as the base for the Search controls as they do not need the API.
    /// <remarks>This replaces BasicCTSBaseControl</remarks>
    /// </summary>
    public abstract class BaseAPICTSControl: SnippetControl
    {
        static ILog log = LogManager.GetLogger(typeof(BaseAPICTSControl));

        /// <summary>
        /// Gets the Snippet Controls Config.
        /// </summary>
        protected BasicCTSPageInfo Config { get; private set; }

        /// <summary>
        /// Gets the path to the template.
        /// </summary>
        /// <returns></returns>
        protected abstract string GetTemplatePath();

        /// <summary>
        /// Gets the data that should be bound to the Velocity Control
        /// </summary>
        protected abstract object GetDataForTemplate();

        /// <summary>
        /// Provides a method to be called on initialization.  If overridden you must call base!
        /// </summary>
        protected virtual void Init()
        {
        }

        /// <summary>
        /// Overrides the OnPreRender event and forces derrived classes to handle events here.
        /// </summary>
        /// <param name="e"></param>
        protected override sealed void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string templatePath = this.GetTemplatePath();
            object templateData = this.GetDataForTemplate();

            LiteralControl ltl = new LiteralControl(VelocityTemplate.MergeTemplateWithResultsByFilepath(
                templatePath, 
                templateData
                )
            );
            Controls.Add(ltl);

            SetAnalytics();
        }

        /// <summary>
        /// DO NOT IMPLEMENT ANYTHING HERE OR IN DERRIVED CLASSES.
        /// </summary>
        /// <param name="e"></param>
        protected override sealed void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //Force config to be loaded here and not rely on others to call base from init. :)

            //////////////////////////////
            // Load the configuration XML from the App Settings
            Config = BasicCTSPageInfo.GetConfig();

            Init();
        }

        /// <summary>
        /// DO NOT IMPLEMENT ANYTHING HERE OR IN DERRIVED CLASSES.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        #region Analytics functions

        /// <summary>
        /// Implement shared analytics values 
        /// </summary>
        private void SetAnalytics()
        {
            // Call the GetPageTypeForAnalytics abstract method; each control must have a concrete implementation to populate the 
            // page type (e.g. Basic, Advanced, Custom)
            String pageType = this.GetPageTypeForAnalytics();// abstract method

            // Set prop62
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop62, wbField =>
            {
                wbField.Value = pageType;
            });

            // Set evar62
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar62, wbField =>
            {
                wbField.Value = pageType;
            });

            // Set additional analytics values.
            // Only implemented in results control for now.
            this.AddAdditionalAnalytics();
        }

        /// <summary>
        /// Abstract method to get the search page type for analytics.
        /// </summary>
        protected abstract String GetPageTypeForAnalytics();

        /// <summary>
        /// Virtual method to set additional, page-specific analytics values.
        /// </summary>
        protected virtual void AddAdditionalAnalytics() { }

        /// <summary>
        /// Get collection of analytics data values.
        /// </summary>
        /// <returns>Dictionary (key/value string pairs)</returns>
        public Dictionary<String, String> GetAnalytics()
        {
            Dictionary<string, string> waDictionary = new Dictionary<string, string>();
            waDictionary.Add(WebAnalyticsOptions.Props.prop62.ToString(), this.GetPageTypeForAnalytics());
            waDictionary.Add(WebAnalyticsOptions.eVars.evar62.ToString(), this.GetPageTypeForAnalytics());
            waDictionary = this.GetAdditionalAnalytics(waDictionary);
            return waDictionary;
        }

        /// <summary>
        /// Virtual method to get additional, page-specific analytics values.
        /// </summary>
        /// <param name="dict">Dictionary object</param>
        /// <returns>Dictionary (key/value string pairs)</returns>
        protected virtual Dictionary<String, String> GetAdditionalAnalytics(Dictionary<String, String> dict)
        {
            return dict;
        }

        #endregion
    }
}
