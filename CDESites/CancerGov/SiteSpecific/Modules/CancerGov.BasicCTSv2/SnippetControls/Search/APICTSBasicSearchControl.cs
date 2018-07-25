using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2.SnippetControls
{
    public class APICTSBasicSearchControl : BaseMgrAPICTSControl
    {
        /// <summary>
        /// Gets the path to the template
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplatePath()
        {
            return this.Config.BasicSearchPageTemplatePath;
        }

        /// <summary>
        /// Binds to the template.
        /// </summary>
        /// <returns></returns>
        protected override object GetDataForTemplate()
        {
            //Return the object for binding.
            return new
            {
                Control = this,
                ResultsPagePrettyUrl = this.Config.ResultsPagePrettyUrl,
                TrialTools = new TrialVelocityTools()
            };
        }

        /// <summary>
        /// Get the search page type for analytics.
        /// </summary>
        /// <returns></returns>
        public override String GetPageType()
        {
            return "Basic";
        }
    }
}
