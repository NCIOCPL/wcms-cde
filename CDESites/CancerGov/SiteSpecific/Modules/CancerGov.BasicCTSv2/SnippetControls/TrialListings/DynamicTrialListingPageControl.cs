using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using CancerGov.ClinicalTrials.Basic.v2.Configuration;
using Common.Logging;
using NCI.Web;
using NCI.Web.CDE;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.UI;
using CancerGov.ClinicalTrials.Basic.v2.SnippetControls.Configs;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CancerGov.ClinicalTrials.Basic.v2.SnippetControls
{
    /// <summary>
    /// Serves as the base class for the different types of dynamic trial listing pages.
    /// </summary>
    public abstract class DynamicTrialListingPageControl : BaseTrialListingControl
    {
        /// <summary>
        /// Set logging for this class.
        /// </summary>
        static ILog log = LogManager.GetLogger(typeof(DynamicTrialListingPageControl));

        protected bool needsRedirect = false;

        /// <summary>
        /// Creates a DynamicTrialListingConfig for use in implementation methods
        /// </summary>
        protected DynamicTrialListingConfig Config
        {
            get
            {
                return (DynamicTrialListingConfig)this.BaseConfig;
            }
        }

        /// <summary>
        /// Creates a DynamicTrialListingFriendlyNameMappingService for use in implementation methods
        /// </summary>
        protected DynamicTrialListingFriendlyNameMappingService FriendlyNameMapping
        {
            get
            {
               return DynamicTrialListingFriendlyNameMappingService.GetMapping(Config.FriendlyNameURLMapFilepath, Config.FriendlyNameURLOverrideMapFilepath, false);
            }
        }

        /// <summary>
        /// Creates a DynamicTrialListingFriendlyNameMappingService with overrides for use in implementation methods
        /// </summary>
        protected DynamicTrialListingFriendlyNameMappingService FriendlyNameWithOverridesMapping
        {
            get
            {
                return DynamicTrialListingFriendlyNameMappingService.GetMapping(Config.FriendlyNameURLMapFilepath, Config.FriendlyNameURLOverrideMapFilepath, true);
            }
        }

        /// <summary>
        /// Implementation of base trial listing page's GetConfigType()
        /// </summary>
        /// <returns>The type of the current configuration</returns>
        protected override Type GetConfigType()
        {
            return typeof(DynamicTrialListingConfig);
        }

        /// <summary>
        /// Implementation of base trial listing page's Trial Query
        /// </summary>
        /// <returns>A JObject with all of the query parameters for use </returns>
        protected sealed override JObject GetTrialQuery()
        {
            JObject query = new JObject(this.GetTypeSpecificQueryParameters());

            //Add any common parameters between all of the items.

            return query;
        }

        /// <summary>
        /// Implementation of base trial listing page's InternalGetNoTrialsHtml
        /// </summary>
        /// <returns>A string with the NoTrialsHtml from the configuration</returns>
        protected override String InternalGetNoTrialsHtml()
        {
            DynamicTrialListingConfigPattern pattern = this.Config.DynamicListingPatterns[this.GetCurrentPatternKey()];

            if (!string.IsNullOrWhiteSpace(pattern.NoTrialsHtml))
            {
                return this.ReplacePlaceholderText(pattern.NoTrialsHtml);
            }
            else
            {
                return string.Empty;
            }
        }

         /// <summary>
         /// Read property indicating if the Trial Listing Page URL contains the keyword "/notrials". If it is the case,
         /// the property will be true. Otherwise false will be sent back
         /// </summary>
        protected bool IsNoTrials
        {
            get
            {
                return (this.CurrAppPath.ToLower().Trim().Contains("/notrials"));
            }
        }

        /// <summary>
        /// Replaces the place holder text based on this Dynamic Trial Listing control type &
        /// user supplied URL parameters
        /// </summary>
        /// <param name="input">The string to replace</param>
        /// <returns>The string with placeholders replaced</returns>
        protected abstract string ReplacePlaceholderText(string input);

        /// <summary>
        /// Gets the Pattern Key for the Current URL parameters
        /// </summary>
        /// <returns>The keyname</returns>
        protected abstract string GetCurrentPatternKey();

        /// <summary>
        /// Gets the trial query parameters specific to the concrete implementations' type and
        /// user supplied URL parameters.
        /// </summary>
        /// <returns></returns>
        protected abstract JObject GetTypeSpecificQueryParameters();

        /// <summary>
        /// Gets or sets the PrettyUrl of the page this component lives on.
        /// </summary>
        protected string PrettyUrl { get; set; }

        /// <summary>
        /// Gets or sets the current Url as an NciUrl object.
        /// </summary>
        protected NciUrl CurrentUrl { get; set; }

        /// <summary>
        /// Gets or sets the Current AppPath, which is usually something like
        /// https://www.cancer.gov(PURL)(AppPath)
        /// (Where both PURL and AppPath start with /)
        /// </summary>
        protected string CurrAppPath
        {
            get
            {
                //Get the Current Application Path, e.g. if the URL is /foo/bar/results/chicken,
                //and the pretty URL is /foo/bar, then the CurrAppPath should be /results/chicken
                if (this.CurrentUrl.UriStem.Length == this.PrettyUrl.Length)
                    return "/";
                else
                    return this.CurrentUrl.UriStem.Substring(PrettyUrl.Length);
            }
        }

        /**
         * 
         * This section is the pipeline of events in order to render the trials.
         * 
         **/

        /// <summary>
        /// Method called during URL Parsing phase of rendering results.
        /// </summary>
        protected abstract void ParseURL();

        /// <summary>
        /// This sets up the Original Pretty URL, the current full URL and the app path
        /// </summary>
        private void SetupUrls()
        {
            //We want to use the PURL for this item.
            //NOTE: THIS 
            NciUrl purl = this.PageInstruction.GetUrl(PageAssemblyInstructionUrls.PrettyUrl);

            if (purl == null || string.IsNullOrWhiteSpace(purl.ToString()))
                throw new Exception("DynamicTrialListingPageDiseaseControl requires current PageAssemblyInstruction to provide its PrettyURL through GetURL.  PrettyURL is null or empty.");

            //It is expected that this is pure and only the pretty URL of this page.
            //This means that any elements on the same page as this app should NOT overwrite the
            //PrettyURL URL Filter.
            this.PrettyUrl = purl.ToString();

            //Now, that we have the PrettyURL, let's figure out what the app paths are...
            NciUrl currURL = new NciUrl();
            currURL.SetUrl(HttpContext.Current.Request.RawUrl);
            currURL.SetUrl(currURL.UriStem);

            //Make sure this URL starts with the pretty url
            if (currURL.UriStem.ToLower().IndexOf(this.PrettyUrl.ToLower()) != 0)
                throw new Exception(String.Format("JSApplicationProxy: Cannot Determine App Path for Page, {0}, with PrettyURL {1}.", currURL.UriStem, PrettyUrl));

            this.CurrentUrl = currURL;
        }

        /// <summary>
        /// Implement OnLoad Event to handle fetching of results.
        /// Prevents derrived classes from implementing.
        /// </summary>
        /// <param name="e"></param>
        sealed protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Step 1. Parse URL and pull out parameters
            this.SetupUrls();
            this.ParseURL();

            //Step 2. Get the Pattern for the placeholder text
            string pattern = this.GetCurrentPatternKey();
            
            //Step 3. Setup Page Metadata
            this.SetupPageMetadata(pattern);

        }

         /// <summary>
         /// This method is called when no results are returned by the query. In this case the function checks that the current URL does not
         /// have the word notrials in it. If it is the case, the function will redirect the user to a page with the following URL:
         /// PRETTYURL/NOTRIALS?p1=a&p2=b
         /// </summary>
        protected override void OnEmptyResults()
        {

            if (this.IsNoTrials)
            {
                return;
            }

            NciUrl noTrialsUrl = new NciUrl();
            noTrialsUrl.SetUrl(this.PrettyUrl + "/notrials");
            
            //Parameters is always assumed to be greater than one
            string[] parameters = this.GetParametersForNoTrials();
            
            for (int i = 0; i < parameters.Length; i++)
            {
                noTrialsUrl.QueryParameters.Add("p" + (i + 1), parameters[i]);
            }

            Response.Redirect(noTrialsUrl.ToString(), true);
        }

        /// <summary>
        /// Used to get the parameters for the /notrials URL based on the current request
        /// </summary>
        /// <returns></returns>
        protected abstract string[] GetParametersForNoTrials();

        /// <summary>
        ///   Retrieves parameters from the Query string and encapsulate them into a list
        /// </summary>
        /// <param name="ParsedReqUrlParams"></param>
        /// <returns></returns>
        protected List<string> GetRawParametersFromQueryString(NciUrl ParsedReqUrlParams)
        {
            List<string> rawParams = new List<string>() { "", "", "" };

            if (ParsedReqUrlParams.QueryParameters["p1"] != null)
                rawParams[0] = ParsedReqUrlParams.QueryParameters["p1"];

            if (ParsedReqUrlParams.QueryParameters.Count > 1 && ParsedReqUrlParams.QueryParameters["p2"] != null)
                rawParams[1] = ParsedReqUrlParams.QueryParameters["p2"];

            if (ParsedReqUrlParams.QueryParameters.Count > 2 && ParsedReqUrlParams.QueryParameters["p3"] != null)
                rawParams[2] = ParsedReqUrlParams.QueryParameters["p3"];

            return (rawParams);
        }

        /// <summary>
        /// Sets up the page metadata based on the given pattern key using overrides
        /// </summary>
        /// <param name="patternKey"></param>
        private void SetupPageMetadata(string patternKey)
        {
            DynamicTrialListingConfigPattern pattern = this.Config.DynamicListingPatterns[patternKey];

            string browserTitle = this.ReplacePlaceholderText(pattern.BrowserTitle);            
            this.PageInstruction.AddFieldFilter("browser_title", (name, data) =>
            {
                data.Value = browserTitle;
            });

            string pageTitle = this.ReplacePlaceholderText(pattern.PageTitle);            
            this.PageInstruction.AddFieldFilter("long_title", (name, data) =>
            {
                data.Value = pageTitle;
            });

            string metaDescription = this.ReplacePlaceholderText(pattern.MetaDescription);
            this.PageInstruction.AddFieldFilter("meta_description", (name, data) =>
            {
                data.Value = metaDescription;
            });

            //Setup the pretty url of this page, which feeds into the current URL and canonical url
            //TODO: When we build the magic Pretty URL mapping, we need to make sure the URL gets set here
            this.PageInstruction.AddUrlFilter(PageAssemblyInstructionUrls.PrettyUrl, (name, url) =>
            {
                url.SetUrl(this.CurrentUrl.ToString());
            });

            //Setup the addthis URL since it is forced to be the raw PrettyURL field on the page.
            //TODO: account for paging?
            this.PageInstruction.AddUrlFilter("add_this_url", (name, url) =>
            {
                url.SetUrl(this.CurrentUrl.ToString());
            });

        }

        /// <summary>
        /// Gets the IntroText for the dynamic trial listing page
        /// </summary>
        /// <returns>A string with the NoTrialsHtml from the configuration</returns>
        public string GetIntroText()
        {
            DynamicTrialListingConfigPattern pattern = this.Config.DynamicListingPatterns[this.GetCurrentPatternKey()];

            if (!string.IsNullOrWhiteSpace(pattern.IntroText))
            {
                return this.ReplacePlaceholderText(pattern.IntroText);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the Friendly Name to replace a c-code in the URL for the dynamic trial listing page. If there is no exact override for that c-code,
        /// attempt to find a match that contains the given c-code in the EVS mappings.
        /// Sets needsRedirect to true if there is a friendly name override found.
        /// </summary>
        /// <returns>A string with the friendly name for the URL (replaces c-code) if the override exists, otherwise the given c-codes</returns>
        protected string GetFriendlyNameForURL(string param)
        {
            if (FriendlyNameWithOverridesMapping.MappingContainsCode(param, true))
            {
                // If an exact match is found in the Friendly Name With Overrides mapping, return the friendly name and set redirection bool
                needsRedirect = true;
                return FriendlyNameWithOverridesMapping.GetFriendlyNameFromCode(param, true);
            }
            else if (FriendlyNameMapping.MappingContainsCode(param, true))
            {
                // If an exact match is found in the Friendly Name mapping, return the friendly name and set redirection bool
                needsRedirect = true;
                return FriendlyNameMapping.GetFriendlyNameFromCode(param, true);
            }
            else
            {
                if (FriendlyNameMapping.MappingContainsCode(param, false))
                {
                    // If an exact match is found in the Friendly Name mapping (without overrides), or if matches are found that contain the given codes and all have the same friendly name,
                    // get the friendly name and set redirection bool.
                    needsRedirect = true;

                    string evsFriendlyName = FriendlyNameMapping.GetFriendlyNameFromCode(param, false);
                    string codesToOverride = FriendlyNameMapping.GetCodeFromFriendlyName(evsFriendlyName);

                    // If the found friendly name has code(s) that there is an entry in the override friendly name mapping for, return the override friendly name.
                    if (FriendlyNameWithOverridesMapping.MappingContainsCode(codesToOverride, true))
                    {
                        return FriendlyNameWithOverridesMapping.GetFriendlyNameFromCode(codesToOverride, true);
                    }
                    // If the found friendly name do not have code(s) that there is an entry in the override friendly name mapping for, return the original friendly name.
                    else
                    {
                        return FriendlyNameMapping.GetFriendlyNameFromCode(param, false);
                    }
                }
                else
                {
                    // If a user comes in with a friendly name, get the codes associated with that friendly name.
                    if (FriendlyNameMapping.MappingContainsFriendlyName(param))
                    {
                        string codesToOverride = FriendlyNameMapping.GetCodeFromFriendlyName(param);

                        // If the code(s) have an entry in the override friendly name mapping, and that new friendly name is not the same as the original,
                        // return the override friendly name and set redirection bool.
                        if (FriendlyNameWithOverridesMapping.MappingContainsCode(codesToOverride, true))
                        {
                            if(!string.Equals(param, FriendlyNameWithOverridesMapping.GetFriendlyNameFromCode(codesToOverride, true)))
                            {
                                needsRedirect = true;
                                return FriendlyNameWithOverridesMapping.GetFriendlyNameFromCode(codesToOverride, true);
                            }
                        }
                    }
                }
            }

            return param;
        }
    }
}
 