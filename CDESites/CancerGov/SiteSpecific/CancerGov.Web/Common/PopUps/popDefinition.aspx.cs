using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Common.Logging;
using NCI.Util;
using NCI.Web.CDE;
using NCI.Web.CDE.WebAnalytics;
using NCI.Web.Dictionary;
using NCI.Web.Dictionary.BusinessObjects;

namespace Www.Common.PopUps
{
    ///<summary>
    ///Defines frame sources for definition popup<br/>
    ///<br/>
    ///<b>Author</b>:  Greg Andres<br/>
    ///<b>Date</b>:  10-15-2001<br/>
    ///<br/>
    ///<b>Revision History</b>:<br/>
    ///<br/>
    ///</summary>
    public partial class PopDefinition : System.Web.UI.Page
    {
        static ILog log = LogManager.GetLogger(typeof(PopDefinition));

        private string urlArgs = "";
        public string CdrID { get; set; }

        //set the language to English by default
        string dictionaryLanguage = "en";

        protected void Page_Load(object sender, EventArgs e) 
        {
            string input_term;

            if (PageAssemblyContext.Current.DisplayVersion == DisplayVersions.Web)
            {
                urlArgs = Request.Url.Query.Substring(1);
                input_term = Strings.Clean(Request.Params["term"]);
                CdrID = Strings.IfNull(Strings.Clean(Request.Params["id"]), Strings.Clean(Request.Params["cdrid"]));
                AudienceType audience = GetAudienceType(Strings.Clean(Request.Params["version"]));
                DictionaryType dictionary = GetDictionaryType(Strings.Clean(Request.Params["dictionary"]));

                if (Request.QueryString["language"] == "Spanish")
                {
                    dictionaryLanguage = "es";
                    logoAnchor.HRef = "/espanol";
                    logoImage.Alt = "Instituto Nacional Del Cáncer";
                    logoImage.Src = "/publishedcontent/images/images/design-elements/logos/nci-logo-full-es.svg";
                    closeWindowText.InnerText = "Cerrar";
                    definitionLabel.Text = "Definición:";
                }
                else
                {
                    logoAnchor.HRef = "/";
                    logoImage.Alt = "National Cancer Institute";
                    logoImage.Src = "/publishedcontent/images/images/design-elements/logos/nci-logo-full.svg";
                    closeWindowText.InnerText = "Close Window";
                    definitionLabel.Text = "Definition:";
                }
                                
                //load the definition
                DictionaryAppManager _dictionaryAppManager = new DictionaryAppManager();
                
                DictionaryTerm dataItem = null;

                if (!string.IsNullOrEmpty(CdrID))
                {
                    CdrID = Regex.Replace(CdrID, "^CDR0+", "", RegexOptions.Compiled);

                    // call appropriate method if dictionary type is known
                    if (dictionary == DictionaryType.Unknown)
                    {
                        dataItem = _dictionaryAppManager.GetTermForAudience(Convert.ToInt32(CdrID), dictionaryLanguage, "v1", audience);
                    }
                    else
                    {
                        dataItem = _dictionaryAppManager.GetTerm(Convert.ToInt32(CdrID), dictionary, dictionaryLanguage, "v1", audience);
                    }

                }
                
                if (dataItem != null && dataItem.Term != null)
                {
                    ActivateDefinitionView(dataItem);
                }
                else
                {
                    phDefinition.Visible = false;
                    phNoResult.Visible = true;
                }

                // Set analytics 
                this.DrawAnalyticsTags();
            }
            
        }

        private void ActivateDefinitionView(DictionaryTerm dataItem)
        {
            phDefinition.Visible = true;
            phNoResult.Visible = false;

            var myDataSource = new List<DictionaryTerm> { dataItem };

            termDictionaryDefinitionView.Visible = true;
            termDictionaryDefinitionView.DataSource = myDataSource;
            termDictionaryDefinitionView.DataBind();

            
        }
        
        protected void termDictionaryDefinitionView_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //get the TermReturn object that is bound to the current row.
                DictionaryTerm termDetails = (DictionaryTerm)e.Item.DataItem;

                if (termDetails != null)
                {
                    PlaceHolder phPronunciation = (PlaceHolder)e.Item.FindControl("phPronunciation");
                    if (termDetails.HasPronunciation && phPronunciation != null)
                    {
                        phPronunciation.Visible = true;
                        System.Web.UI.HtmlControls.HtmlAnchor pronunciationLink = (System.Web.UI.HtmlControls.HtmlAnchor)e.Item.FindControl("pronunciationLink");
                        if (pronunciationLink != null && termDetails.Pronunciation.HasAudio)
                        {
                            pronunciationLink.Visible = true;
                            pronunciationLink.HRef = termDetails.Pronunciation.Audio;
                        }
                        else
                            pronunciationLink.Visible = false;

                        Label pronunciationKey = (Label)e.Item.FindControl("pronunciationKey");
                        if (pronunciationKey != null && termDetails.Pronunciation.HasKey)
                            pronunciationKey.Text = " " + termDetails.Pronunciation.Key;

                    }
                    else
                        phPronunciation.Visible = false;

                    //Get Related Information from the Manager layer
                    //Add check to see if it exists and then display data accordingly
                    Panel pnlRelatedInfo = e.Item.FindControl("pnlRelatedInfo") as Panel;
                    if (pnlRelatedInfo != null)
                    {
                        //display the related information panel
                        //when atleast one of the related item exists
                        if (termDetails.Images.Length > 0)
                        {
                            pnlRelatedInfo.Visible = true;
                            
                            Repeater relatedImages = (Repeater)e.Item.FindControl("relatedImages");
                            if (relatedImages != null)
                            {
                                if (termDetails.Images.Length > 0)
                                {
                                    relatedImages.Visible = true;
                                    relatedImages.DataSource = termDetails.Images;
                                    relatedImages.DataBind();
                                }

                            }
                        }
                        else
                        {
                            pnlRelatedInfo.Visible = false;
                        }

                    }

                }
            }
        }

        protected void relatedImages_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //get the ImageReference object that is bound to the current row.
                ImageReference imageDetails = (ImageReference)e.Item.DataItem;

                if (imageDetails != null)
                {
                    System.Web.UI.HtmlControls.HtmlImage termImage = (System.Web.UI.HtmlControls.HtmlImage)e.Item.FindControl("termImage");
                    if (termImage != null)
                    {
                        termImage.Alt = imageDetails.AltText;

                        if (!string.IsNullOrEmpty(imageDetails.Filename))
                        {
                            System.Web.UI.HtmlControls.HtmlAnchor termEnlargeImage = (System.Web.UI.HtmlControls.HtmlAnchor)e.Item.FindControl("termEnlargeImage");

                            //if either the regular image size or the enlarge image size is not in the config file
                            //default to the full image in the database
                            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["CDRImageRegular"]) || string.IsNullOrEmpty(ConfigurationManager.AppSettings["CDRImageEnlarge"]))
                            {
                                termImage.Src = imageDetails.Filename;

                                if (termEnlargeImage != null)
                                {
                                    termEnlargeImage.HRef = imageDetails.Filename;
                                    termEnlargeImage.InnerText = dictionaryLanguage == "es" ? "Ampliar" : "Enlarge";
                                }

                                //log a warning
                                log.WarnFormat("Web.Config file does not specify image sizes for term id: {0}. Display full image.", CdrID);
                            }
                            else
                            {
                                string[] regularTermImage = imageDetails.Filename.Split('.');
                                if (regularTermImage.Length == 2)
                                {
                                    //termImage image size is 571
                                    //example format CDR526538-571.jpg
                                    termImage.Src = regularTermImage[0] + "-" + ConfigurationManager.AppSettings["CDRImageRegular"] + "." + regularTermImage[1];

                                    //enlarge image size is 750
                                    //example format CDR526538-750.jpg
                                    if (termEnlargeImage != null)
                                    {
                                        termEnlargeImage.HRef = regularTermImage[0] + "-" + ConfigurationManager.AppSettings["CDRImageEnlarge"] + "." + regularTermImage[1];
                                        termEnlargeImage.InnerText = dictionaryLanguage == "es" ? "Ampliar" : "Enlarge";
                                    }
                                }
                            }

                        }
                    }

                }
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
        }

        /// <summary>
        /// Converts the given string (ideally from the 'version' query parameter) to an AudienceType enum.
        /// Preserves the legacy mappings from the GetPDQVersion call.
        /// </summary>
        /// <param name="version">The string parameter to convert.</param>
        /// <returns>The matching AudienceType.  Defaults to 'Patient' if version is not recognized.</returns>
        private AudienceType GetAudienceType(string version)
        {
            version = CancerGov.Text.Strings.IfNull(version, "");

            switch (version.Trim().ToLower())
            {
                case "1":
                case "healthprofessional":
                case "provider":
                    return AudienceType.HealthProfessional;
                case "0":
                case "patient":
                default:
                    return AudienceType.Patient;
            }
        }

        /// <summary>
        /// Converts the given string (ideally from the 'dictionary' query parameter) to a DictionaryType enum.
        /// </summary>
        /// <param name="dictionary">The string parameter to convert.</param>
        /// <returns>The matching DictionaryType.  Defaults to 'Unknown' if string is not recognized.</returns>
        private DictionaryType GetDictionaryType(string dictionary)
        {
            dictionary = Strings.Clean(dictionary, "Unknown");

            return ConvertEnum<DictionaryType>.Convert(dictionary, DictionaryType.Unknown);
        }

        /// <summary>
        /// Set web analytics values and draw the required meta and script tags.
        /// </summary>
        private void DrawAnalyticsTags()
        {
            string popupSuites = "nciglobal,ncienterprise";
            WebAnalyticsPageLoad webAnalyticsPageLoad = new WebAnalyticsPageLoad();
            webAnalyticsPageLoad.SetReportSuites(popupSuites);
            webAnalyticsPageLoad.AddEvent(WebAnalyticsOptions.Events.event11); // Dictionary Term view (event11)

            if (dictionaryLanguage == "es")
            {
                webAnalyticsPageLoad.SetChannel("Diccionario de cancer (Dictionary of Cancer Terms)");
            }
            else
            {
                webAnalyticsPageLoad.SetChannel("Dictionary of Cancer Terms");
            }

            litDtmTop.Text = "<script src=\"" + webAnalyticsPageLoad.DTMTop + "\"></script>";
            litWaMeta.Text = webAnalyticsPageLoad.GetHeadTags();  // Load page load script 
            litDtmBottom.Text = "<script>" + webAnalyticsPageLoad.DTMBottom + "</script>";
        }

        #region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
