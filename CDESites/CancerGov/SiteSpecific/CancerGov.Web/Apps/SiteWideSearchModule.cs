using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CancerGov.Search.BestBets;
using Common.Logging;
using NCI.Search;
using NCI.Util;
using NCI.Web.CDE;
using NCI.Web.CDE.Configuration;
using NCI.Web.CDE.Modules;
using NCI.Web.CDE.WebAnalytics;
using NCI.Web.UI.WebControls;


namespace NCI.Web.CancerGov.Apps
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SiteWideSearchModule runat=server></{0}:SiteWideSearchModule>")]
    public class SiteWideSearchModule : AppsBaseUserControl
    {
        static ILog log = LogManager.GetLogger(typeof(SiteWideSearchModule));

        #region Control Members
        protected DropDownList ddlPageUnit;
        protected Label lblResultsForText;
        protected Label lblResultsForKeyword;
        protected Repeater rptBestBets;
        protected Label lblBBCatName;
        protected Repeater rptBBListItems;
       
        protected Literal litDidYouMeanText;
        protected PlaceHolder phError;
        protected Label lblTopResultsXofYKeyword;
        protected HyperLink lnkDym;
        protected Label lblTopResultsXofY;
        protected Literal litError;
        protected NCI.Web.UI.WebControls.MultiTemplatedRepeater rptResults;
        
        protected Label lblBottomResultsXofY;
        protected Label lblDDLPageUnitShowText;
        protected Label lblPageUnit;
        protected Label lblSearchWithinResultsFound;
        protected Label lblSearchWithinResultKeyword;
        protected Panel pnlSWR;
        protected RadioButtonList rblSWRSearchType;
        protected Label lblSWRKeywordLabel;
        protected TextBox txtSWRKeyword;
        protected Button btnSWRTxtSearch;
        protected Label lblDDLPageUnitResultsPPText;
        protected Button btnTextChangePageUnit;
        protected SimpleUlPager spPager;
        #endregion

        #region Private Memebers
        private bool _allowedToShowDYM = false;
        private string _topResultsXofYFormatter = "Results {0}-{1} of {2} for:";
        private string _bottomResultsXofYFormatter = "Results {0}-{1} of {2}.";
        private string _swrResultsFor = "{0} results found for: ";
        private bool isBestBet = false;
        private bool _hasPageUnitChanged = false;
        private bool _isIENoJSAndHitEnderInTheSearchBox = false;
        private int _resultOffset = 1;

        #endregion
        #region Properties

        /// <summary>
        /// This is the keyword.
        /// </summary>
        private string Keyword
        {
            get
            {
                return (string)ViewState["Keyword"] ?? string.Empty;
            }
            set
            {
                ViewState["Keyword"] = value;
            }
        }

        /// <summary>
        /// Gets and sets the ItemsPerPage. (The pageunit parameter)
        /// </summary>
        private int ItemsPerPage
        {
            get
            {
                return Strings.ToInt(ddlPageUnit.SelectedValue, 10);
            }
            set
            {
                foreach (System.Web.UI.WebControls.ListItem li in ddlPageUnit.Items)
                {
                    if (li.Value == value.ToString())
                    {
                        ddlPageUnit.Text = value.ToString();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets and sets the current page number
        /// </summary>
        private int CurrentPage
        {
            get
            {
                return (int)(ViewState["CurrentPage"] ?? 1);
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }

        private long TotalNumberOfResults
        {
            get
            {
                return (long)(ViewState["TotalNumberOfResults"] ?? 0);
            }
            set
            {
                ViewState["TotalNumberOfResults"] = value;
            }
        }

        private string SearchCollection { get; set; }
        
        /// <summary>
        /// Gets and sets the PreviousItemsPerPage.  
        /// </summary>
        /// <remarks>
        /// This is to support the moving to the
        /// correct page when the ItemsPerPage changes.  A DropDownList does not store the 
        /// previous selected value, even if it is set with an autopostback.  So we need to store
        /// it after we get the search results in order to figure out what the position of the first 
        /// item was before the user changed the number of items.  
        /// </remarks>
        private int PreviousItemsPerPage
        {
            get
            {
                return (int)(ViewState["PreviousItemsPerPage"] ?? ItemsPerPage);
            }
            set
            {
                ViewState["PreviousItemsPerPage"] = value;
            }
        }


        /// <summary>
        /// Gets the text for the search term.
        /// </summary>
        private string SearchTerm
        {
            get
            {
                string rtnText = "";
                if (OldKeywords.Count > 0)
                {
                    rtnText = string.Join(" ", OldKeywords.ToArray());
                    rtnText += " " + Keyword;
                }
                else
                {
                    rtnText = Keyword;
                }

                return rtnText;
            }
        }

        /// <summary>
        /// Gets the text for the different keyword labels.
        /// </summary>
        private string KeywordText
        {
            get
            {
                string rtnText = "";
                if (OldKeywords.Count > 0)
                {
                    rtnText = string.Join(" : ", OldKeywords.ToArray());
                    rtnText += " : " + Keyword;
                }
                else
                {
                    rtnText = Keyword;
                }

                return rtnText;
            }
        }


        /// <summary>
        /// Gets a list of OldKeywords.
        /// </summary>
        private List<string> OldKeywords
        {
            get
            {
                if (ViewState["OldKeywords"] == null)
                    ViewState["OldKeywords"] = new List<string>();

                return (List<string>)ViewState["OldKeywords"];
            }
        }

        /// <summary>
        /// Gets the old keywords suitable for query parameters.
        /// </summary>
        private string OldKeywordsForQuery
        {
            get
            {
                string rtnText = "";

                if (OldKeywords.Count > 0)
                    rtnText = string.Join("|", OldKeywords.ToArray());

                return rtnText;
            }
        }

        #endregion

        /// <summary>
        /// Event method sets content version and template and user control properties<br /><br />
        /// [1] Input form parameters:<br />
        /// keyword {string: search string},<br />
        /// first {integer: ordinal index of first result},<br />
        /// resultSearch {string: search within results search string},<br />
        /// chkCategories {comma-delimited set of strings: specific virtual paths to search},<br />
        /// selectedPage {integer: current result page number}<br />
        /// [2] Builds Verity and BestBet query language and Verity query xml document<br />
        /// [3] Parses Verity results and errors<br />
        /// [4] Builds Verity and BestBet result HTML<br />
        /// [5] Builds paging control for results browsing<br />
        /// </summary>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Setup the title, header, and left nav
            SetupCancerGovPageStuff();

            SetLabels();

            //Hide the error message
            phError.Visible = false;

            //set up the search collection (CancerGovEnglish or CancerGovSpanish)
            //determine what text needs to be removed from the title e.g. - National Cancer Institute
            SiteWideSearchConfig searchConfig = ModuleObjectFactory<SiteWideSearchConfig>.GetModuleObject(SnippetInfo.Data);
            if (searchConfig != null)
               SearchCollection = searchConfig.SearchCollection;
               
                                    
            if (Page.Request.RequestType == "POST")
            {
                if (!IsPostBack)
                {
                    /*********************************************/
                    /*        Search using header search box     */
                    /*********************************************/

                    Keyword = Request.Params["swKeyword"];

                    if (Keyword == string.Empty)
                    {
                        ShowErrorMessage();
                    }
                    else
                    {
                        LoadResults();

                        //Log User Input for initial search.
                        LogUserInput("Keyword:" + Keyword);
                    }
                }
                else
                {
                    //This is a postback from a user changing the items per page or
                    //searching within results.  There is one problem though.
                    //By default, 
                    if (Strings.Clean(Request.Params["__EVENTTARGET"]) == null
                        && Strings.Clean(Request.Params["__EVENTARGUMENT"]) == null
                        && Strings.Clean(Request.Params[btnTextChangePageUnit.UniqueID]) == null
                        && Strings.Clean(Request.Params[btnSWRTxtSearch.UniqueID]) == null)
                    {
                        //THIS IS FOR IE with no JS.  Oddly in most ways it is more broken than firefox, but
                        //is so broken that we can easily tell that the user hit enter on the only
                        //text box.
                        //Basically if all the buttons that can be clicked are null, and there
                        //is no event target or argument, then this is IE with Javascript turned
                        //off and the user clicked enter on the text box.  Also, image buttons do
                        //not come back ID, but ID.x and ID.y. (Hopefully screen readers send those
                        //things too.
                        //
                        //We need to handle this stuff after all the other postback events happen.
                        //so lets just set a variable for now.
                        _isIENoJSAndHitEnderInTheSearchBox = true;
                    }
                }
            }
            else
            {
                /*********************************************/
                /*    Either Paging or a DYM Click           */
                /*********************************************/

                //No Matter if this is a DYM or a Paging, swKeyword is the keyword. (There may be an old one, but that is for later)
                Keyword = Request.Params["swKeyword"];

                if (Keyword == string.Empty)
                {
                    ShowErrorMessage();
                }
                else
                {
                    //These things only apply to paging

                    //Get Items per page.
                    ItemsPerPage = Strings.ToInt(Request.Params["pageunit"], 10);

                    //Get the current Page
                    CurrentPage = Strings.ToInt(Request.Params["page"], 1);

                    //"De-serialize" old keywords.
                    if (!string.IsNullOrEmpty(Request.Params["old_keywords"]))
                    {
                        string[] oldKeys = Request.Params["old_keywords"].Split(new char[] { '|' });
                        OldKeywords.AddRange(oldKeys);
                    }
                    
                    LoadResults();
                }
            }

            // build the OnClineClick value for results search
            var siteResultSearchSubmitParameter = "";
            if (PageDisplayInformation.Language == DisplayLanguage.Spanish)
                siteResultSearchSubmitParameter = "true";
            // removed search validation for now
            var siteResultSearchSubmitCall = ""; //"return siteResultSearchSubmit(" + siteResultSearchSubmitParameter + ")";
            // Web Analytics *************************************************
            if (WebAnalyticsOptions.IsEnabled)
                siteResultSearchSubmitCall += "return NCIAnalytics.SiteWideSearchResultsSearch(this,'" + txtSWRKeyword.ClientID + "','" + rblSWRSearchType.UniqueID + "')";
            // End Web Analytics *********************************************
            if (!String.IsNullOrEmpty(siteResultSearchSubmitCall))
            {
                siteResultSearchSubmitCall += ";";
                btnSWRTxtSearch.OnClientClick = siteResultSearchSubmitCall;
            }

        }

        protected override void OnPreRender(EventArgs e)
        {
            //Handle the stupid IE No JS problem.  Really we just need to simulate the go
            //button click.
            if (_isIENoJSAndHitEnderInTheSearchBox)
                ChangePageUnitBtnClick(btnTextChangePageUnit, new EventArgs());


            base.OnPreRender(e);

            pnlSWR.DefaultButton = btnSWRTxtSearch.ID;

            var headers = HttpContext.Current.Response.Headers;
            if (headers.Get("Cache-Control") != null)
            {
                headers.Set("Cache-Control", "private");
            }
        }

        protected string BestBetResultsHyperlinkOnclick(RepeaterItem result)
        {
            string resultData = "";
            if (WebAnalyticsOptions.IsEnabled)
            {
                try
                {
                    BestBetUIResult bbResultItem = (BestBetUIResult)result.DataItem;
                    resultData = bbResultItem.CategoryDisplay;

                    // Did not use html parser here, seemed like a overkill just to inject a simple string. 
                    int hrefIndex = resultData.IndexOf("<a");

                    if (hrefIndex != -1)
                    {
                        // The analytics rank was previously set to '1' for all listed items in best bets;
                        // added some String methods to increment the value.
                        // First, split the results string into an array based on the number of <a> tags found.
                        int i = 1;
                        string[] separators = new string[] { "<a" };
                        string[] temp = resultData.Split(separators, StringSplitOptions.None);

                        foreach (string t in temp)
                        {
                            // Add the BestBets arguments with the rank incrementing with each pass through 
                            // the links in the results. Then smoosh the whole thing back together with the new opening
                            // <a onClick="..."
                            temp[i] = String.Concat("(this,true,'" + i.ToString() + "');\"", temp[i]);
                            resultData = String.Join("<a onClick=\"NCIAnalytics.SiteWideSearchResults", temp);
                            i++;

                            // Return the re-assembled string once the rank and total number of links match.
                            if (i == temp.Length)
                            {
                                return resultData;
                            }
                        } // End foreach
                    } // End href index 'if' statement
                }
                catch { }
            }

            return "";
        }

        protected string ResultsHyperlinkOnclick(IDataItemContainer result)
        {
            if (WebAnalyticsOptions.IsEnabled)
                return "NCIAnalytics.SiteWideSearchResults(this," + "false" + ",'" + (result.DisplayIndex + _resultOffset).ToString() + "');"; // Load results onclick script
            else
                return "";
        }


        #region Event Handlers

        /// <summary>
        /// This is the callback for the Go button next to the page unit dropdown.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ChangePageUnitBtnClick(object sender, EventArgs e)
        {
            //If we are here, then there is no Javascript.  This can happen because the user is in firefox and
            //hit enter in the textbox, or they are in either IE or Firefox and clicked the "Go" button.
            //Now if the textbox has text then we need to do a new search, if it is empty then the ChangePageUnit
            //handler has already done its work.  If the user changed the page size then the search function will
            //use the new page size.

            if (Strings.Clean(txtSWRKeyword.Text) != null)
            {
                //This will also take the new page unit size if the user changed it.
                DoSearchWithinResults();
            }
            else if (!_hasPageUnitChanged)
            {
                if (Keyword != string.Empty)
                {
                    //The button was clicked and nothing was changed.  So just rebind.
                    LoadResults();
                }
                else
                {
                    ShowErrorMessage();
                }
            }
        }
        
        protected void ChangeItemsPerPageAndBind(object sender, EventArgs e)
        {
            _hasPageUnitChanged = true; //This fired so it must have changed.

            // clear the keyword search field
            txtSWRKeyword.Text = "";

            //Get the last total number of results so if the user changes the ItemsPerPage(pageunit)
            //then we can move them to the closest page.  Say you are viewing 10 items per page and
            //you are on page 6. This shows 51-60.  If you change to 50 items per page you should put
            //the user on page 2 and not page 1.  That way they will see 51-100.

            //So we should know the current page number, it was part of the postback.
            //we do have to know what the old page unit was, that will give us the first record.
            int firstItem, lastItem;
            SimpleUlPager.GetFirstItemLastItem(CurrentPage, PreviousItemsPerPage, (int)TotalNumberOfResults, out firstItem, out lastItem);

            //Set the current page.                
            CurrentPage = firstItem / ItemsPerPage + (firstItem % ItemsPerPage > 0 ? 1 : 0);

            //If the CurrentPage is 0, then Endeca will throw errors, so we need to set a curretnpage ==0
            //to 1.  This brings up a very good point, why do we show the change number of items per page
            //if we have 0 results.  One might also ask, why do we go to Endeca to bind the results when we 
            //know they will not change? The answer is partially due to lazyness and making the code simple.
            //LoadResults not only loads the results, but also makes sure that things like total results and
            //previousitemsperpage are set.  I would rather just call this and not add yet another code 
            //branch to this already complicated code.
            if (CurrentPage == 0)
                CurrentPage = 1;

            //Note LoadResults gets the page unit on its own.
            LoadResults();
        }

        /// <summary>
        /// This is the callback for the search within results button's onclick event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SearchWithinResults(object sender, EventArgs e)
        {
            //If JS is turned off, someone can say they want to see 50 items, and then do a search instead of hitting the go button to select the items
            //per page.  This would show as a bug in QA, although it might not be a bad things since you can do 2 things at once...
            if (PreviousItemsPerPage != ItemsPerPage)
                ItemsPerPage = PreviousItemsPerPage;



            DoSearchWithinResults();
        }

        #endregion

        /// <summary>
        /// Logs the search term a users searched with.
        /// </summary>
        /// <param name="clickItem"></param>
        private void LogUserInput(string clickItem)
        {
            //CancerGov.Common.Functions.LogUserInput(
            //    Request.UrlReferrer == null ? "" : Request.UrlReferrer.ToString(),
            //    Request.Url.AbsoluteUri,
            //    Request.UserHostAddress,
            //    clickItem,
            //    Page.Session.SessionID);
        }

        /// <summary>
        /// This sets up things like the left nav and title...
        /// </summary>
        private void SetupCancerGovPageStuff()
        {
            this.Page.Header.Title = GetResource("Title");

            this.txtSWRKeyword.Attributes.Add("placeholder", "Enter keywords or phrases");
            this.txtSWRKeyword.Attributes.Add("aria-label", "Enter keywords or phrases");
        }

        /// <summary>
        /// Sets up the spanish labels
        /// </summary>
        private void SetLabels()
        {
            //Set formatters
            this._topResultsXofYFormatter = GetResource("topResultsXofYFormatter"); // "Resultados {0}-{1} de {2} para:";
            this._bottomResultsXofYFormatter = GetResource("bottomResultsXofYFormatter"); // "Resultados {0}-{1} de {2}.";
            this._swrResultsFor = GetResource("ResultsFor");// "{0} resultados de b&uacute;squeda para: ";

            //Set controls.
            lblResultsForText.Text = GetResource("ResultsForText"); //"Resultados para:";
            //litDidYouMeanText.Text = GetResource("DidYouMeanText"); //"Quiz&aacute;s quiso decir";
            btnTextChangePageUnit.Text = GetResource("TextChangePageUnit"); //"Sí";

            //Pager previous and next
            spPager.PagerStyleSettings.NextPageText = GetResource("NextPageText");// "Siguiente&nbsp;&gt;";
            spPager.PagerStyleSettings.PrevPageText = GetResource("PrevPageText");//"&lt;&nbsp;Anterior";

            //Search within results Radio button list.
            if (rblSWRSearchType.Items.Count == 2)
            {
                rblSWRSearchType.Items[0].Text = GetResource("SearchTypeText1"); //"Nueva b&uacute;squeda&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
                rblSWRSearchType.Items[1].Text = GetResource("SearchTypeText2"); // "B&uacute;squeda dentro de los resultados";
            }

            //Search within results button
            btnSWRTxtSearch.Text = GetResource("Search");  //"Buscar";

            //Error message
            litError.Text = GetResource("ErrorMessage"); //"Se necesita término de búsqueda.";

            //Page unit dropdown text and label
            lblDDLPageUnitShowText.Text = GetResource("PageUnit"); //"Mostrar&nbsp;";
            lblDDLPageUnitResultsPPText.Text = GetResource("PageUnitResults"); //"&nbsp;resultados por p&aacute;gina.";
        }

        private void DoSearchWithinResults()
        {
            if (Strings.Clean(txtSWRKeyword.Text) == null)
            {
                ShowErrorMessage();
            }
            else
            {
                if (rblSWRSearchType.SelectedValue == "2") //Search Within Results
                {
                    //Add the last keyword to the old keywords
                    if (!String.IsNullOrEmpty(Keyword))
                    {
                        OldKeywords.Add(Keyword);
                    }

                    //Set the current keyword
                    Keyword = Strings.Clean(txtSWRKeyword.Text);

                    LogUserInput("Refine-Keyword:" + (
                        OldKeywordsForQuery.Length > 0 ? OldKeywordsForQuery + "|" + Keyword : Keyword));
                }
                else //New Search
                {
                    //Log new search
                    OldKeywords.Clear();
                    Keyword = Strings.Clean(txtSWRKeyword.Text);

                    LogUserInput("New-Keyword:" + Keyword);
                }

                this.CurrentPage = 1;
                txtSWRKeyword.Text = ""; //Clear the text box.  .NET will leave in the text, which is what google does, but not what cancer.gov has done.

                LoadResults();
            }
        }
         
        /// <summary>
        /// This method is the one that sets up label text and binds the search results.
        /// </summary>
        private void LoadResults()
        {

            //Get Bestbets. Only show if we are on page one and we are not doing a search within
            //the results.
            if (CurrentPage == 1 && OldKeywords.Count == 0)
            {
                BestBetUIResult[] bbResults = GetBestBetsResults(SearchTerm);

                if (bbResults.Length > 0)
                {
                    isBestBet = true;
                    rptBestBets.DataSource = bbResults;
                    rptBestBets.DataBind();
                    rptBestBets.Visible = true;
                }
                else
                {
                    rptBestBets.Visible = false;
                }
            }
            else
            {
                rptBestBets.Visible = false;
            }

            //Get Results...  
            SiteWideSearchAPIResultCollection results = null;
            
            try
            {
                results = SiteWideSearchManager.Search(SearchCollection, SearchTerm, ItemsPerPage,
                        (CurrentPage - 1) * ItemsPerPage);
            }
            catch (Exception e)
            {
                //capture exactly which keyword caused the error
                log.ErrorFormat("Search with the following keyword returned an error: {0}", e, KeywordText);
            }

            //Set the last total number of results so if the user changes the ItemsPerPage(pageunit)
            //then we can move them to the closest page.  Say you are viewing 10 items per page and
            //you are on page 6. This shows 51-60.  If you change to 50 items per page you should put
            //the user on page 2 and not page 1.  That way they will see 51-100.  
            TotalNumberOfResults = results.ResultCount;
            PreviousItemsPerPage = ItemsPerPage;

            int firstIndex, lastIndex;

            //Get first index and last index
            SimpleUlPager.GetFirstItemLastItem(CurrentPage, ItemsPerPage, (int)results.ResultCount, out firstIndex, out lastIndex);
            _resultOffset = firstIndex;

            if (results != null && results.ResultCount > 0)
            {
                //the title text that needs to be removed from the search result Title
                string removeTitleText = ContentDeliveryEngineConfig.PageTitle.AppendPageTitle.Title;
                rptResults.DataSource = from res in results.SearchResults
                                        select new NCI.Web.UI.WebControls.TemplatedDataItem(
                                            GetSearchResultTemplate(res),
                                            new
                                            {
                                                URL = res.Url,
                                                Title = (!string.IsNullOrEmpty(removeTitleText) && res.Title.Contains(removeTitleText)) ? res.Title.Remove(res.Title.IndexOf(removeTitleText)) : res.Title,
                                                DisplayUrl = res.Url,
                                                Description = res.Description,
                                                Label = GetSearchResultLabel(res),
                                            });
                rptResults.DataBind();
            }

            //Set Keywords in labels
            lblResultsForKeyword.Text = KeywordText;
            lblTopResultsXofYKeyword.Text = KeywordText;
            lblSearchWithinResultKeyword.Text = KeywordText;

            //Show labels for results X of Y
            ShowResultsXoYLabels(firstIndex, lastIndex, results.ResultCount);

            //Setup pager
            SetupPager();

            //Set the Search Within Results Radio button to new search
            rblSWRSearchType.SelectedIndex = 0;

            //// Web Analytics *************************************************
            // Add number of search results to analytics
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar10, wbField =>
            {
                wbField.Value = TotalNumberOfResults.ToString();
            });

            this.PageInstruction.AddFieldFilter("channelName", (name, data) =>
            {
                data.Value = "NCI Home";
            });

            // Add Best Bets event to analytics
            if (rptBestBets.Visible)
            {
                this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Events.event10, wbField =>
                {
                    wbField.Value = WebAnalyticsOptions.Events.event10.ToString();
                });
            }
            //// End Web Analytics *************************************************
        }

        /// <summary>
        /// Returns an appropriate content-type label for the given EndecaResult.
        /// </summary>
        /// <param name="res">The source EndecaResult used to generate the label.</param>
        /// <returns>A language-specific label for the result.</returns>
        private String GetSearchResultLabel(SiteWideSearchAPIResult res)
        {
            string language = PageAssemblyContext.Current.PageAssemblyInstruction.Language;
            string label = res.ContentType;

            switch (res.ContentType)
            {
                case "cgvInfographic":
                    label = language == "es" ? "Infografía" : "Infographic";
                    break;
                case "gloVideo":
                    label = language == "es" ? "Video" : "Video";
                    break;
                case "gloVideoCarousel":
                    label = language == "es" ? "Lista de reproducción de videos" : "Video Playlist";
                    break;
            }

            return label;
        }

        /// <summary>
        /// Generates an appropriate template name for the given search result.
        /// </summary>
        /// <param name="res">The source EndecaResult.</param>
        /// <returns>A String template name.</returns>
        private string GetSearchResultTemplate(SiteWideSearchAPIResult res)
        {
            switch (res.ContentType)
            {
                case "cgvInfographic":
                case "gloVideo":
                case "gloVideoCarousel":
                    return "Media";
                default:
                    return "Default";
            }
        }

        private void ShowErrorMessage()
        {
            //Clear out the fact that the user did a search before this.
            Keyword = "";
            OldKeywords.Clear();

            lblResultsForKeyword.Text = "";
            lblTopResultsXofYKeyword.Text = "";
            lblSearchWithinResultKeyword.Text = "";

            phError.Visible = true;
            ShowResultsXoYLabels(0, 0, 0);
            //For paging change.
            PreviousItemsPerPage = ItemsPerPage;

            //Set the Search Within Results Radio button to new search
            rblSWRSearchType.SelectedIndex = 0;
            spPager.RecordCount = 0;

            //// Web Analytics *************************************************
            // Add number of search results to analytics
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar10, wbField =>
            {
                wbField.Value = "0";
            });
            //// End Web Analytics *********************************************
        }

        private void ShowResultsXoYLabels(int firstIndex, int lastIndex, long totalNumResults)
        {
            //This is the top Results X-Y of Z for: label
            lblTopResultsXofY.Text = String.Format(
                _topResultsXofYFormatter,
                firstIndex,
                lastIndex,
                totalNumResults);

            //This is the bottom Results X-Y of Z. label
            lblBottomResultsXofY.Text = String.Format(
                _bottomResultsXofYFormatter,
                firstIndex,
                lastIndex,
                totalNumResults);

            //This is the Z results found for: Label in the search within the results box.
            lblSearchWithinResultsFound.Text = String.Format(
                _swrResultsFor,
                totalNumResults);
        }

        /// <summary>
        /// Helper method to setup the pager
        /// </summary>
        private void SetupPager()
        {
            spPager.RecordCount = (int)this.TotalNumberOfResults;
            spPager.RecordsPerPage = ItemsPerPage;
            spPager.RecordsPerPageParamName = "pageunit";
            spPager.CurrentPage = CurrentPage;
            spPager.PageParamName = "page";
            spPager.ShowNumPages = 2;
            spPager.PagerStyleSettings.SelectedIndexCssClass = "pager-SelectedPage";

            //"Serialize" OldKeywords to string
            if (OldKeywords.Count > 0)
            {
                spPager.BaseUrl = string.Format(
                    "{0}?swKeyword={1}&old_keywords={2}",
                    PageInstruction.GetUrl("PrettyUrl").UriStem,
                    Server.UrlEncode(Keyword),
                    Server.UrlEncode(OldKeywordsForQuery));
            }
            else
            {
                spPager.BaseUrl = string.Format(
                    "{0}?swKeyword={1}",
                    PageInstruction.GetUrl("PrettyUrl").UriStem,
                    Server.UrlEncode(Keyword));
            }

            if (PageDisplayInformation.Language == DisplayLanguage.Spanish)
                spPager.BaseUrl += "&lang=spanish";
        }
                
        /// <summary>
        /// Method to get best bets
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        private BestBetUIResult[] GetBestBetsResults(string searchTerm)
        {
            BestBetUIResult[] results = new BestBetUIResult[0];

            try
            {
                results = BestBetsPresentationManager.GetBestBets(searchTerm, PageDisplayInformation.Language);
            }
            catch (Exception ex)
            {
                //Search results should be retrieved even if Best Bets fail
                //log the error  but don't throw an exception
                log.ErrorFormat("Error in GetBestBetsResults - {0}", ex, Request.Url.AbsoluteUri);
            }

            return results;
        }

        
         
    }
}
