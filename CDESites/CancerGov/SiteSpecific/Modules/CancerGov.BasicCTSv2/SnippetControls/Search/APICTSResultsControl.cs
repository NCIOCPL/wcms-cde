using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Common.Logging;
using NCI.Web;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;
using CancerGov.ClinicalTrials.Basic.v2.Configuration;
using CancerGov.ClinicalTrialsAPI;

namespace CancerGov.ClinicalTrials.Basic.v2.SnippetControls
{
    /// <summary>
    /// Represents the new CTAPI Driven Results page for Basic & Advanced.
    /// </summary>
    public class APICTSResultsControl : BaseMgrAPICTSControl
    {
        static ILog log = LogManager.GetLogger(typeof(APICTSResultsControl));

        private ClinicalTrialsCollection _results = null;        

        /// <summary>
        /// Gets the path to the template
        /// </summary>
        /// <returns></returns>
        protected override string GetTemplatePath()
        {
            return this.Config.ResultsPageTemplatePath;
        }

        /// <summary>
        /// Goes and fetches the data from the API & Returns the results to base class to be bound to the template.
        /// </summary>
        /// <returns></returns>
        protected override object GetDataForTemplate()
        {

            //TODO: Don't do a search if there are param errors.
            if (SearchParams.HasInvalidParams() == true)
            {
                _results = new ClinicalTrialsCollection();
                _results.TotalResults = 0;
            }

            else
            {
                _results = CTSManager.Search(SearchParams, this.PageNum, this.ItemsPerPage);

            }

            //Let's setup some helpful items for the template, so they do not need to be helper functions
            //The start is either 0 if there are no results, or a 1 based offset based on Page number and items per page.
            int startItemNumber = _results.TotalResults == 0 ? _results.TotalResults : ((this.PageNum - 1) * this.ItemsPerPage) + 1;

            //Determine the last item.
            long lastItemNumber = (this.PageNum * this.ItemsPerPage);
            if (lastItemNumber > _results.TotalResults)
                lastItemNumber = _results.TotalResults;

            //Determine the max page
            int maxPage = (int)Math.Ceiling((double)_results.TotalResults / (double)this.ItemsPerPage);

            // Add URL filters
            PageInstruction.AddUrlFilter("CurrentUrl", (name, url) =>
            {
                //Convert the current search parameters into a NciUrl
                NciUrl paramsUrl = CTSSearchParamFactory.ConvertParamsToUrl(this.SearchParams);

                //Add or replace the currentURL params based on the *validated* query params.
                foreach (KeyValuePair<string, string> qp in paramsUrl.QueryParameters)
                {
                    if (!url.QueryParameters.ContainsKey(qp.Key))
                    {
                        url.QueryParameters.Add(qp.Key, qp.Value);
                    }
                    else
                    {
                        url.QueryParameters[qp.Key] = qp.Value;
                    }
                }

                url.QueryParameters.Add("ni", this.ItemsPerPage.ToString());
            });
            

            //Return the object for binding.
            return new
            {
                Results = _results,
                Control = this,
                Parameters = SearchParams,
                PageInfo = new {
                    CurrentPage = this.PageNum,
                    MaxPage = maxPage,
                    ItemsPerPage = this.ItemsPerPage,
                    StartItemNumber = startItemNumber,
                    LastItemNumber = lastItemNumber
                },
                TrialTools = new TrialVelocityTools()
            };
        }

        #region Velocity Helpers 

        /// <summary>
        /// Gets a detailed view URL
        /// </summary>
        /// <param name="nciID"></param>
        /// <returns></returns>
        public string GetDetailedViewUrl(string nciID)
        {
            //Create a new url for the current details page.
            NciUrl url = new NciUrl();
            url.SetUrl(this.Config.DetailedViewPagePrettyUrl);

            //Convert the current search parameters into a NciUrl
            NciUrl paramsUrl = CTSSearchParamFactory.ConvertParamsToUrl(this.SearchParams);

            //Copy the params 
            url.QueryParameters = paramsUrl.QueryParameters;

            //Add the NCI URL param.
            url.QueryParameters.Add("id", nciID);
            url.QueryParameters.Add("pn", this.PageNum.ToString());
            url.QueryParameters.Add("ni", this.ItemsPerPage.ToString());

            return url.ToString();
        }

        /// <summary>
        /// Gets the search parameter Query items for print URL.
        /// </summary>
        /// <returns></returns>
        public string GetSearchParamsForPrint()
        {
            //Convert the current search parameters into a NciUrl
            NciUrl paramsUrl = CTSSearchParamFactory.ConvertParamsToUrl(this.SearchParams);

            //TODO: ensure double quotes are encoded.

            return paramsUrl.ToString();
        }

        /// <summary>
        /// Gets URL for a Single Page of Results
        /// </summary>
        /// <param name="pageNum"></param>
        /// <returns></returns>
        public string GetPageUrl(int pageNum)
        {
            //Create a new url for the current details page.
            NciUrl url = new NciUrl();
            url.SetUrl(this.Config.ResultsPagePrettyUrl);

            //Convert the current search parameters into a NciUrl
            NciUrl paramsUrl = CTSSearchParamFactory.ConvertParamsToUrl(this.SearchParams);

            //Copy the params 
            url.QueryParameters = paramsUrl.QueryParameters;

            url.QueryParameters.Add("pn", pageNum.ToString());
            url.QueryParameters.Add("ni", this.ItemsPerPage.ToString());

            return url.ToString();
        }

        /// <summary>
        /// Gets the Urls and Labels for all the pages of results from Curr Page - numLeft to Curr Page + numRight
        /// </summary>
        /// <param name="numLeft">The number of pages to display left of the selected page</param>
        /// <param name="numRight">The number of pages to display to the right of the selected page</param>
        /// <param name="maxPage">The maximum number of pages that could be in this result set</param>
        /// <param name="totalResults">The total number of results </param>
        /// <returns></returns>
        public IEnumerable<object> GetPagerItems(int numLeft, int numRight, int maxPage, long totalResults)
        {
            int startPage = (this.PageNum - numLeft) >= 1 ? this.PageNum - numLeft : 1; // Current page minus left limit (numLeft)
            int endPage = (this.PageNum + numRight) <= maxPage ? this.PageNum + numRight : maxPage; // Current page plus right limit (numRight)

            // The maximum number of elements that can be drawn in this pager. his would be:
            // ("< Previous" + 1 + 2/ellipsis + numLeft + current + numRight + next-to-last/ellipsis + "Next >") 
            int itemsTotalMax = 7 + numLeft + numRight;

            // If the pageNumber parameter is set above the highest available page number, set the start page to the endPage value.
            if (this.PageNum > endPage)
            {
                startPage = (endPage - numLeft) >= 1 ? endPage - numLeft : 1;
            }

            // If maxPage == 1, then only one page of results is found. Therefore, return null for the pager items.
            // Otherwise, set up the pager accordingly.
            if (maxPage > 1)
            {
                // Create a list of pager item objects:
                //  Text (string) = link text
                //  PageUrl (string) = href value for item 
                //  IsLink (bool) - whether or not this item will be used as a link
                List<object> items = new List<object>();

                // Draw text and links for first & previous pages
                if (this.PageNum != 1)
                {
                    // Draw link to previous page
                    string PrevUrl = GetPageUrl(this.PageNum - 1);
                    items.Add(
                    new
                    {
                        Text = "&lt; Previous",
                        PageUrl = PrevUrl
                    });

                    // add previous url to filter
                    this.PageInstruction.AddUrlFilter("RelPrev", (name, url) =>
                    {
                        url.SetUrl(PrevUrl);
                    });

                    // Draw first page links and text
                    if (this.PageNum > (numLeft + 1))
                    {
                        // Draw link to first page
                        items.Add(
                        new
                        {
                            Text = "1",
                            PageUrl = GetPageUrl(1)
                        });

                        // Draw elipses to delimit first page. 
                        // If the ellipses only represent a single digit, draw that instead (in this case it will always be 2).
                        if (this.PageNum > (numLeft + 2))
                        {
                            if (this.PageNum == (numLeft + 3))
                            {
                                items.Add(
                                new
                                {
                                    Text = "2",
                                    PageUrl = GetPageUrl(2)
                                });
                            }
                            else
                            {
                                items.Add(
                                new
                                {
                                    Text = "...",
                                    IsLink = false
                                });
                            }
                        }
                    }
                }

                // Draw links before and after current
                for (int i = startPage; i <= endPage; i++)
                {
                    items.Add(
                        new
                        {
                            Text = i.ToString(),
                            PageUrl = GetPageUrl(i)
                        }
                    );
                }

                // Draw last page links and text
                if (this.PageNum < endPage)
                {
                    // Draw iink to last page
                    if (this.PageNum < (maxPage - numRight))
                    {
                        // Draw elipses to delimit last page. 
                        // If the ellipses only represent a single digit, draw that instead (in this case it will always maxmimum page minus one).
                        if (this.PageNum < (maxPage - numRight - 1))
                        {
                            if (this.PageNum == (maxPage - numRight - 2))
                            {
                                items.Add(
                                new
                                {
                                    Text = (maxPage - 1).ToString(),
                                    PageUrl = GetPageUrl(maxPage - 1)
                                });
                            }
                            else
                            {
                                items.Add(
                                new
                                {
                                    Text = "...",
                                    IsLink = false
                                });
                            }
                        }

                        // Draw link to last page
                        items.Add(
                        new
                        {
                            Text = maxPage.ToString(),
                            PageUrl = GetPageUrl(maxPage)
                        });
                    }

                    // Draw link to next page
                    string NextUrl = GetPageUrl(this.PageNum + 1);
                    items.Add(
                    new
                    {
                        Text = "Next &gt;",
                        PageUrl = NextUrl
                    });

                    this.PageInstruction.AddUrlFilter("RelNext", (name, url) =>
                    {
                        url.SetUrl(NextUrl);
                    });

                }

                // Remove any duplicate links that may have slipped though. This only occurs in cases where the URL query param 
                // is greater than the last available page.
                // Doing this after the fact to prevent mucking up the code above. This is an edge case and should be handled outside of
                // the general logic. 
                if (this.PageNum > maxPage)
                {
                    items = items.Distinct().ToList();
                    // Remove ellipsis if it shows betweeen two consecutive numbers
                    if (items.Count - 2 == maxPage)
                    {
                        items.RemoveAt(2);
                    }
                }

                return items;
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region Analytics methods

        /// <summary>
        /// Get the search page type for analytics.
        /// </summary>
        /// <returns></returns>
        protected override String GetPageTypeForAnalytics()
        {
            string type = GetSearchType(this.SearchParams);
            return "Clinical Trials: " + type;
        }

        /// <summary>
        /// Set additional, page-specific analytics values.
        /// Set search string analytics values.
        /// </summary>
        protected override void AddAdditionalAnalytics() 
        {
            // Get total results count
            string count = this._results.TotalResults.ToString();

            // Dynamic value the search type (e.g. basic or advanced)
            string searchType =  GetSearchType(this.SearchParams).ToLower();

            // Retrieve concatenated param/field strings using the CTSWebAnalyticsHelpder. 
            // These values will be used to populate props and evars below.
            string paramBlob = CTSWebAnalyticsHelper.GetAnalyticsAllParams(this.SearchParams); // List of all used parameters
            string locBlob = CTSWebAnalyticsHelper.GetAnalyticsLocation(this.SearchParams); // Location
            string ciBlob = string.Empty;
            string ttDrugBlob = string.Empty;
            string idOrgBlob = string.Empty;

            // Set event2
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Events.event2, wbField =>
            {
                wbField.Value = WebAnalyticsOptions.Events.event2.ToString();
            });

            // Set prop10
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop10, wbField =>
            {
                wbField.Value = count;
            });

            // Set prop11 & eVar11
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop11, wbField =>
            {
                wbField.Value = "clinicaltrials_" + searchType;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar11, wbField =>
            {
                wbField.Value = "clinicaltrials_" + searchType;
            });

            // Set prop15 & eVar 15
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop15, wbField =>
            {
                wbField.Value = paramBlob;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar15, wbField =>
            {
                wbField.Value = paramBlob;
            });

            // Set prop18 & eVar 18
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop18, wbField =>
            {
                wbField.Value = locBlob;
            });
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar18, wbField =>
            {
                wbField.Value = locBlob;
            });

            // Set these fields only if we're on a basic search.
            if (searchType == "basic")
            {
                // Retrieve concatenated param/field strings using the CTSWebAnalyticsHelpder. 
                ciBlob = CTSWebAnalyticsHelper.GetAnalyticsBasicCancerInfo(this.SearchParams); // Type/Subtype/Stage/Findings/Age/Keyword

                // Set prop17 & eVar 17
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop17, wbField =>
                {
                    wbField.Value = ciBlob;
                });
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar17, wbField =>
                {
                    wbField.Value = ciBlob;
                });
            }

            // Set these fields only if we're on an advanced search.
            if (searchType == "advanced")
            {
                // Retrieve concatenated param/field strings using the CTSWebAnalyticsHelpder. 
                ciBlob = CTSWebAnalyticsHelper.GetAnalyticsAdvCancerInfo(this.SearchParams); // Type/Subtype/Stage/Findings/Age/Keyword
                ttDrugBlob = CTSWebAnalyticsHelper.GetAnalyticsTmntDrugOther(this.SearchParams); // TrialType/Drug/Other Intervention
                idOrgBlob = CTSWebAnalyticsHelper.GetAnalyticsPhaseIdInvOrg(this.SearchParams); // Phase/Trial ID/Investigator/Org

                // Set prop17 & eVar 17
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop17, wbField =>
                {
                    wbField.Value = ciBlob;
                });
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar17, wbField =>
                {
                    wbField.Value = ciBlob;
                });

                // Set prop19 & eVar 19
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop19, wbField =>
                {
                    wbField.Value = ttDrugBlob;
                });
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar19, wbField =>
                {
                    wbField.Value = ttDrugBlob;
                });

                // Set prop20 & eVar20
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop20, wbField =>
                {
                    wbField.Value = idOrgBlob;
                });
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar20, wbField =>
                {
                    wbField.Value = idOrgBlob;
                });
            }
        }

        #endregion
    }
}
