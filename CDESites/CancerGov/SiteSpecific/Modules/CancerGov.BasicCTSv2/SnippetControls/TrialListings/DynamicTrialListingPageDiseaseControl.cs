using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NCI.Web;
using NCI.Web.CDE;
using Newtonsoft.Json.Linq;
using NCI.Web.CDE.WebAnalytics;
using Common.Logging;

namespace CancerGov.ClinicalTrials.Basic.v2.SnippetControls
{
    public class DynamicTrialListingPageDiseaseControl : DynamicTrialListingPageControl
    {
        /// <summary>
        /// Replaces the Placeholder Text
        /// </summary>
        /// <param name="input"></param>
        /// <returns>A string with the placeholder text</returns>
        protected override string ReplacePlaceholderText(string input)
        {
            // Replace all disease IDs with overrides
            if (!string.IsNullOrWhiteSpace(this.DiseaseIDs))
            {
                input = input.Replace("${disease_name}", GetOverride(this.DiseaseIDs, true));
                input = input.Replace("${disease_name_lower}", GetOverride(this.DiseaseIDs, false));
            }

            // Replace all trial types with overrides
            if (!string.IsNullOrWhiteSpace(this.TrialType))
            {
                input = input.Replace("${type_of_trial}", GetOverride(this.TrialType, true));
                input = input.Replace("${type_of_trial_lower}", GetOverride(this.TrialType, false));
            }

            // Replace all intervention IDs with overrides
            if (!string.IsNullOrWhiteSpace(this.InterventionIDs))
            {
                input = input.Replace("${intervention}", GetOverride(this.InterventionIDs, true));
                input = input.Replace("${intervention_lower}", GetOverride(this.InterventionIDs, false));
            }

            return input;
        }

        /// <summary>
        /// Gets the current PatternKey based on URL parameters
        /// </summary>
        /// <returns>A string with the current pattern key</returns>
        protected override string GetCurrentPatternKey()
        {
            // If intervention is present, pattern includes all three params
            if (!string.IsNullOrWhiteSpace(this.InterventionIDs))
            {
                return "DiseaseTypeIntervention";
            }
            // If trial type is present and intervention is not, pattern includes first two params
            else if (!string.IsNullOrWhiteSpace(this.TrialType) && string.IsNullOrWhiteSpace(this.InterventionIDs))
            {
                return "DiseaseType";
            }
            // If only disease is present, pattern includes just that param
            else {
                return "DiseaseOnly";
            }
        }

        /// <summary>
        /// Gets the type specific query parameters to be sent to the API from the given URL params
        /// </summary>
        /// <returns>A JObject with all of the parameters converted to those needed by API</returns>
        protected override JObject GetTypeSpecificQueryParameters()
        {
            JObject queryParams = new JObject();

            // Get friendly name to c-code mapping
            string disIDs = this.DiseaseIDs;
            if (!string.IsNullOrEmpty(this.BaseConfig.FriendlyNameURLMapFilepath))
            {
                DynamicTrialListingFriendlyNameMapping friendlyNameMap = DynamicTrialListingFriendlyNameMapping.GetMappingForFile(this.BaseConfig.FriendlyNameURLMapFilepath);
                if (friendlyNameMap.MappingContainsFriendlyName(this.DiseaseIDs.ToLower()))
                {
                    disIDs = friendlyNameMap.GetCodeFromFriendlyName(this.DiseaseIDs.ToLower());
                }
            }

            string[] diseaseIDsarr = disIDs.Split(new char[] { ',' });

            queryParams.Add("diseases.nci_thesaurus_concept_id", new JArray(diseaseIDsarr));

            if (!string.IsNullOrWhiteSpace(this.TrialType))
            {
                queryParams.Add("primary_purpose.primary_purpose_code", this.TrialType);
            }

            if (!string.IsNullOrWhiteSpace(this.InterventionIDs))
            {
                // Get friendly name to c-code mapping
                string ivIDs = this.InterventionIDs;
                if (!string.IsNullOrEmpty(this.BaseConfig.FriendlyNameURLMapFilepath))
                {
                    DynamicTrialListingFriendlyNameMapping friendlyNameMap = DynamicTrialListingFriendlyNameMapping.GetMappingForFile(this.BaseConfig.FriendlyNameURLMapFilepath);
                    if (friendlyNameMap.MappingContainsFriendlyName(this.InterventionIDs.ToLower()))
                    {
                        ivIDs = friendlyNameMap.GetCodeFromFriendlyName(this.InterventionIDs.ToLower());
                    }
                }

                string[] interventionIDsarr = ivIDs.Split(new char[] { ',' });
                queryParams.Add("arms.interventions.intervention_code", new JArray(interventionIDsarr));
            }

            return queryParams;
        }

        /// <summary>
        /// Replaces the Placeholder Codes (or text) with Override Labels
        /// </summary>
        /// <param name="codes"></param>
        /// <returns>A string with the override text</returns>
        private string GetOverride(string valToOverride, bool needsTitleCase)
        {
            // Get friendly name to c-code mapping
            if (!string.IsNullOrEmpty(this.BaseConfig.FriendlyNameURLMapFilepath))
            {
                DynamicTrialListingFriendlyNameMapping friendlyNameMap = DynamicTrialListingFriendlyNameMapping.GetMappingForFile(this.BaseConfig.FriendlyNameURLMapFilepath);
                if (friendlyNameMap.MappingContainsFriendlyName(valToOverride.ToLower()))
                {
                    valToOverride = friendlyNameMap.GetCodeFromFriendlyName(valToOverride);
                }
            }

            // Get label mappings
            var labelMapping = DynamicTrialListingMapping.Instance;
            string overrideText = "";
            
            // If combination of codes is in label mappings, set override
            if (labelMapping.MappingContainsKey(valToOverride))
            {
                if(needsTitleCase)
                {
                    overrideText = labelMapping.GetTitleCase(valToOverride);
                }
                else
                {
                    overrideText = labelMapping.Get(valToOverride);
                }
            }
            // Raise 404 error if overrides aren't found
            else
            {
                Response.Headers.Add("X-CTSMap", "ID not found");
                LogManager.GetLogger(typeof(DynamicTrialListingPageDiseaseControl)).ErrorFormat("Invalid parameter in dynamic listing page: {0} does not have override", valToOverride);
                NCI.Web.CDE.Application.ErrorPageDisplayer.RaisePageByCode("DynamicTrialListingPageDiseaseControl", 404, "Invalid parameter in dynamic listing page: value given does not have override");
            }

            return overrideText;
        }

        /// <summary>
        /// Gets and sets the disease IDs for this listing
        /// </summary>
        private string DiseaseIDs { get; set; }

        /// <summary>
        /// Gets and sets the trial type for this listing
        /// </summary>
        private string TrialType { get; set; }

        /// <summary>
        /// Gets and sets the intervention ids for this listing
        /// </summary>
        private string InterventionIDs { get; set; }

         /// <summary>
         /// Used to get the parameters for the /notrials URL based on the current request
         /// </summary>
         /// <returns></returns>
        protected override string[] GetParametersForNoTrials()
        {
            List<string> parameters = new List<string>();

            if(this.DiseaseIDs != null && this.DiseaseIDs.Length > 0)
                parameters.Add(this.DiseaseIDs);

            if (this.TrialType != null)
                parameters.Add(this.TrialType);

            if(this.InterventionIDs != null && this.InterventionIDs.Length > 0)
                parameters.Add(this.InterventionIDs);

            return (parameters.ToArray());
        }

        /// <summary>
        /// Parses the URL for all of the parameters for a disease dynamic listing page
        /// </summary>
        protected override void ParseURL()
        {

            if (this.CurrAppPath == "/")
            {
                throw new HttpException(400, "Invalid Parameters");
            }

            List<string> rawParams = new List<string>();
            
            if (this.IsNoTrials)
            {
                NciUrl ParsedReqUrlParams = new NciUrl(true, true, true);  //We need this to be lowercase and collapse duplicate params. (Or not use an NCI URL)
                ParsedReqUrlParams.SetUrl(this.Request.Url.Query);

                if (ParsedReqUrlParams.QueryParameters.Count == 0)
                {
                    throw new HttpException(400, "Invalid Parameters");
                }

                rawParams = GetRawParametersFromQueryString(ParsedReqUrlParams);
                SetDoNotIndex();
                Response.StatusCode = 404;
                Response.TrySkipIisCustomErrors = true;
            }
            else
            {
              //Setup rawParams for /a/b/c structure
                rawParams = this.CurrAppPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
            }

            SetUpRawParametersForListingPage(rawParams);
            SetUpCanonicalUrl();
        }

        /// <summary>
        /// Sets the Canonical Url of the Disease Page
        /// </summary>
        private void SetUpCanonicalUrl()
        {
            // We set the Canonical Url. We make sure that the Canonical URL has the following format disease/trial type instead of 
            // disease/trial type/intervention
            string[] pathTokens = this.CurrAppPath.Split(new char[] { '/' });

            if (pathTokens != null && pathTokens.Length > 0)
            {
                string canonicalUrl = this.CurrentUrl.ToString().ToLower();

                // If there are disease IDs, we check if they have a friendly name for the canonical URL
                if(this.DiseaseIDs != null && this.DiseaseIDs.Length > 0)
                {
                    // Get c-code to friendly name mapping
                    if (!string.IsNullOrEmpty(this.BaseConfig.FriendlyNameURLMapFilepath))
                    {
                        DynamicTrialListingFriendlyNameMapping friendlyNameMap = DynamicTrialListingFriendlyNameMapping.GetMappingForFile(this.BaseConfig.FriendlyNameURLMapFilepath);
                        if (friendlyNameMap.MappingContainsCode(this.DiseaseIDs.ToLower()))
                        {
                            canonicalUrl = canonicalUrl.Replace(this.DiseaseIDs, friendlyNameMap.GetFriendlyNameFromCode(this.DiseaseIDs));
                        }
                    }
                }

                // If there are intervention IDS we strip them from the canonical url
                if (this.InterventionIDs != null && this.InterventionIDs.Length > 0)
                    canonicalUrl = canonicalUrl.Replace("/" + this.InterventionIDs + "/", "").Replace("/" + this.InterventionIDs, "");
             

                this.PageInstruction.AddUrlFilter(PageAssemblyInstructionUrls.CanonicalUrl, (name, url) =>
                {
                    url.SetUrl(canonicalUrl);
                });
            }
        }

        /// <summary>
        /// This method extracts the different pieces of the URLS and assign them as properties (i.e. DiseaseIDs, TrialType, etc) values of this control
        /// </summary>
        private void SetUpRawParametersForListingPage(List<string> urlParams)
        {
            if (urlParams.Count >= 4)
            {
                throw new HttpException(400, "Invalid Parameters");
            }

            //Has Disease
            if (urlParams.Count >= 1)
            {
                if (urlParams[0].Contains(","))
                {
                    string[] split = urlParams[0].Split(',');
                    // Sort c-codes in alphanumerical order for comparison to items in mapping file
                    Array.Sort(split);
                    // Lowercase all c-codes for comparison to items in mapping file
                    split = split.Select(s => s.ToLower()).ToArray();
                    this.DiseaseIDs = string.Join(",", split);
                }
                else
                {
                    // Lowercase all c-codes for comparison to items in mapping file
                    this.DiseaseIDs = urlParams[0].ToLower();
                }
            }

            //Has Type of Trial
            if (urlParams.Count >= 2)
            {
                // Lowercase all c-codes for comparison to items in mapping file
                this.TrialType = urlParams[1].ToLower();
            }

            //Has Intervention
            if (urlParams.Count >= 3)
            {
                if (urlParams[2].Contains(","))
                {
                    string[] split = urlParams[2].Split(',');
                    // Sort c-codes in alphanumerical order for comparison to items in mapping file
                    Array.Sort(split);
                    // Lowercase all c-codes for comparison to items in mapping file
                    split = split.Select(s => s.ToLower()).ToArray();
                    this.InterventionIDs = string.Join(",", split);
                }
                else
                {
                    // Lowercase all c-codes for comparison to items in mapping file
                    this.InterventionIDs = urlParams[2].ToLower();
                }
            }

            // Check for friendly names that override c-codes in URL: if exists, redirect to that URL
            if (!string.IsNullOrEmpty(this.BaseConfig.FriendlyNameURLMapFilepath))
            {
                string redirectUrl = this.PrettyUrl.ToString();

                List<string> urlParts = new List<string>();

                if (!string.IsNullOrEmpty(this.DiseaseIDs))
                {
                    // Add Disease friendly name override to redirect URL path
                    urlParts.Add(GetFriendlyNameForURL(this.DiseaseIDs));

                    if(!string.IsNullOrEmpty(this.TrialType))
                    {
                        // Add trial type to redirect URL path
                        urlParts.Add(this.TrialType);

                        if (!string.IsNullOrEmpty(this.InterventionIDs))
                        {
                            // Add Intervention friendly name override to redirect URL path
                            urlParts.Add(GetFriendlyNameForURL(this.InterventionIDs));
                        }
                    }

                }
                
                // If there are friendly name overrides, set up the redirect URL using those values
                if(needsRedirect)
                {
                    redirectUrl = redirectUrl.TrimEnd('/');

                    foreach (string urlPart in urlParts)
                    {
                        redirectUrl += "/" + urlPart;
                    }

                    Response.RedirectPermanent(redirectUrl, true);
                }
            }
        }

        /// <summary>
        /// Set do not index on no trials page
        /// </summary>
        private void SetDoNotIndex()
        {
            PageInstruction.AddFieldFilter("meta_robots", (name, data) =>
            {
                data.Value = "noindex, nofollow";
            });
        }


        /// <summary>
        /// Format string for analytics params: Intervention|Disease IDs|Trial Type|Intervention IDs|Total Results
        /// </summary>
        /// <returns></returns>
        public override String GetDynamicParams()
        {
            string[] analyticsParams = new string[5];
            analyticsParams[0] = "Disease";
            analyticsParams[1] = (!string.IsNullOrWhiteSpace(this.DiseaseIDs)) ? this.DiseaseIDs : "none";
            analyticsParams[2] = (!string.IsNullOrWhiteSpace(this.TrialType)) ? this.TrialType : "none";
            analyticsParams[3] = (!string.IsNullOrWhiteSpace(this.InterventionIDs)) ? this.InterventionIDs : "none";
            analyticsParams[4] = this.TotalSearchResults.ToString();
            return string.Join("|", analyticsParams);
        }

        /// <summary>
        /// Set default pageLoad analytics for this page
        /// </summary>
        protected override void SetAnalytics()
        {
            string val = "clinicaltrials_custom";
            string desc = "Clinical Trials: Custom";
            string dynamicAnalytics = GetDynamicParams();

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
                wbField.Value = dynamicAnalytics;
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
                wbField.Value = dynamicAnalytics;
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
