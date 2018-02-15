using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;

using NCI.Text;
using NCI.Web.CDE;
using NCI.Web.CDE.Application;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.UI.Configuration;
using CancerGov.ClinicalTrials.Basic.v2.Configuration;
using CancerGov.ClinicalTrials.Basic.v2.DataManagers;
using CancerGov.ClinicalTrials.Basic.v2.SnippetControls;
using CancerGov.ClinicalTrialsAPI;
using System.Net.Http;
using NCI.Web;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public class CTSPrintManager
    {
        private static BasicCTSPageInfo _config = null;

        /// <summary>
        /// Creates a new instance of a CTSPrintManager
        /// </summary>
        /// <param name="config">CTS Config</param>
        public CTSPrintManager(BasicCTSPageInfo config)
        {
            _config = config;
        }

        public Guid StorePrintContent(List<String> trialIDs, DateTime date, CTSSearchParams searchTerms)
        {
            // Retrieve the collections given the ID's
            //TODO: THese dependencies should be passed in!
            BasicCTSManager manager = new BasicCTSManager(APIClientHelper.GetV1ClientInstance());

            List<ClinicalTrial> results = manager.GetMultipleTrials(trialIDs).ToList();

            // Send results to Velocity template
            var formattedPrintContent = FormatPrintResults(results, date, searchTerms);

            // Save result to cache table
            Guid guid = CTSPrintResultsDataManager.SavePrintResult(formattedPrintContent, trialIDs, searchTerms, Settings.IsLive);

            if (guid == Guid.Empty)
            {
                // Something went wrong with the save/return from the DB
                ErrorPageDisplayer.RaisePageByCode(this.GetType().ToString(), 500);
                throw new DbConnectionException("Unable to connect to the database. ");
            }

            return guid;
        }

        private string FormatPrintResults(IEnumerable<ClinicalTrial> results, DateTime searchDate, CTSSearchParams searchTerms)
        {
            string searchUrl = _config.BasicSearchPagePrettyUrl;
            if (searchTerms.ResultsLinkFlag == ResultsLinkType.Advanced)
            {
                searchUrl = _config.AdvSearchPagePrettyUrl;
            }

            PrintVelocityHelpers helpers = new PrintVelocityHelpers(_config);

            // Bind results to velocity template
            LiteralControl ltl = new LiteralControl(VelocityTemplate.MergeTemplateWithResultsByFilepath(
                _config.PrintPageTemplatePath,
                 new
                 {
                     Control = new { SearchFormUrl = searchUrl },
                     Results = results,
                     SearchDate = searchDate.ToString("M/d/yyyy"),
                     Parameters = searchTerms,
                     PrintHelper = helpers,
                     TrialTools = new TrialVelocityTools()
                 }
            ));

            return (ltl.Text);
        }

        public string GetPrintContent(Guid printID)
        {
            // Call the data manager to retrieve the cached print results based on guid printID
            string printContent = CTSPrintResultsDataManager.RetrieveResult(printID, Settings.IsLive);

            return printContent;
        }
    }

    /// <summary>
    /// Helper class for Velocity Template specifically for print.
    /// </summary>
    public class PrintVelocityHelpers
    {
        private static BasicCTSPageInfo _config = null;

        /// <summary>
        /// Creates a new instance of a PrintVelocityHelpers
        /// </summary>
        /// <param name="config">CTS Config</param>
        public PrintVelocityHelpers(BasicCTSPageInfo config)
        {
            _config = config;
        }

        public string GetCheckForUpdatesURL(string trialID, CTSSearchParams searchParams)
        {
            //Create a new url for the current details page.
            NciUrl url = new NciUrl();
            url.SetUrl(_config.DetailedViewPagePrettyUrl);

            //Convert the current search parameters into a NciUrl
            NciUrl paramsUrl = CTSSearchParamFactory.ConvertParamsToUrl(searchParams);

            //Copy the params 
            url.QueryParameters = paramsUrl.QueryParameters;

            url.QueryParameters.Add("id", trialID);

            return url.ToString();
        }
    }
}
