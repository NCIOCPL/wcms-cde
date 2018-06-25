using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NCI.Util;
using System.Collections;
using NCI.Web.CDE;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.Globalization;
using CancerGov.UI.HTML;
using CancerGov.CDR.TermDictionary;
using CancerGov.UI;
using NCI.Web.CDE.WebAnalytics;
namespace CancerGov.Web.Common.PopUps
{
    public partial class Definition :PopUpPage

    {
        private IRenderer content;
        protected string strSendPrinter = "Send to Printer";
        //protected string strHeading = "<h3 class='popup-definition'>Definition from NCI's Dictionary of Cancer Terms</h3>";
        protected string strHeading = "<div class=\"heading\">Definition:</div>";
        public DisplayLanguage dl = new DisplayLanguage();

        #region Page properties

        public IRenderer Content
        {
            get { return content; }
            set { content = value; }
        }

        #endregion

        /// <summary>
        /// Default web form class constructor
        /// </summary>
        public Definition()
        {
        }

        /// <summary>
        /// Event method sets frame content version and parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string input_term;
            string term;
            string id;
            string mediaHtml = "";
            string audioMediaHTML = String.Empty;
            PDQVersion version;
            string pronunciation;
            string termDefinition;


            if (Request.QueryString["language"] == "English")
                dl = DisplayLanguage.English;
            else if (Request.QueryString["language"] == "Spanish")
                dl = DisplayLanguage.Spanish;
            else
                dl = DisplayLanguage.English;

            ValidateParams();
            
            //include page title
            this.pageHtmlHead.Title = "Definition - National Cancer Institute";
            input_term = Strings.Clean(Request.Params["term"]);
            id = Strings.IfNull(Strings.Clean(Request.Params["id"]), Strings.Clean(Request.Params["cdrid"]));
            version = PDQVersionResolver.GetPDQVersion(Strings.Clean(Request.Params["version"]));
            //version = PDQVersion.version;

            ArrayList result = null;

            if (input_term == null && id == null)
            {
                term = "Error";
                pronunciation = "invalid input";
                termDefinition = "You have not specified either a CDRID nor a term name.";
            }
            else
            {
                if (input_term != null && input_term.Length > 0)
                {
                    result = get_definition("term", input_term, version, dl);
                    if (result == null && input_term.EndsWith("s"))
                    {
                        result = get_definition("term", input_term.Substring(0, input_term.Length - 1), version, dl);
                        if (result == null && input_term.EndsWith("es"))
                        {
                            result = get_definition("term", input_term.Substring(0, input_term.Length - 2), version, dl);
                            if (result == null && input_term.EndsWith("ies"))
                            {
                                result = get_definition("term", input_term.Substring(0, input_term.Length - 3) + "y", version, dl);
                                if (result == null && input_term.EndsWith("um"))
                                {
                                    result = get_definition("term", input_term.Substring(0, input_term.Length - 2) + "a", version, dl);
                                    if (result == null && input_term.EndsWith("ly"))
                                    {
                                        result = get_definition("term", input_term.Substring(0, input_term.Length - 2), version, dl);
                                        if (result == null && input_term.EndsWith("ii"))
                                        {
                                            result = get_definition("term", input_term.Substring(0, input_term.Length - 2) + "us", version, dl);
                                        }

                                    }

                                }
                            }
                        }
                    }
                }
                else
                {
                    id = Regex.Replace(id, "^CDR0+", "", RegexOptions.Compiled);
                    result = get_definition("id", id, version, dl);
                }

                if (result == null)
                {
                    term = "term not found";
                    pronunciation = "";
                    termDefinition = "The term you are looking for does not exist in the glossary.";
                }
                else
                {
                    term = result[0].ToString();
                    pronunciation = result[1].ToString();
                    termDefinition = result[2].ToString();
                    mediaHtml = result[3].ToString();
                    mediaHtml = mediaHtml.Replace("[__imagelocation]", ConfigurationManager.AppSettings["CDRImageLocation"]);

                    if (result[4] != null)
                    {
                        audioMediaHTML = result[4].ToString();
                        audioMediaHTML = audioMediaHTML.Replace("[_audioMediaLocation]", ConfigurationManager.AppSettings["CDRAudioMediaLocation"]);
                    }
                }
            }
            
            if (dl == DisplayLanguage.Spanish)
            {
                strSendPrinter = "Imprima esta página";
                pronunciation = String.Empty;
                //strHeading = "<h3 class='popup-definition'>Definición del Diccionario de cáncer del NCI</h3>";
                strHeading = "<div class=\"heading\">Definición:</div>";
            }

            content = new HtmlSegment("<div class=\"audioPronounceLink\">" 
                + String.Format("<span class=\"term\">{0}</span>", term) 
                + String.Format("<span class=\"pronunciation\">{0}</span>", ((Strings.Clean(pronunciation) != null) ? " " + pronunciation : "")) 
                + audioMediaHTML + "</div>" 
                + String.Format("<div class=\"definition\">{0}</div>", termDefinition) 
                + String.Format("<div class=\"definitionImage\">{0}</div>", mediaHtml)
                );

            // Set analytics 
            this.DrawAnalyticsTags();
        }

        private void ValidateParams()
        {
            //input_term = Strings.Clean(Request.Params["term"]);
            string id;
            string version;
            string language;
            id = Strings.IfNull(Strings.Clean(Request.Params["id"]), Strings.Clean(Request.Params["cdrid"]));
            if(!string.IsNullOrEmpty(id))
                id = Regex.Replace(id, "^CDR0+", "", RegexOptions.Compiled);

            version = Strings.Clean(Request.Params["version"]);
            language = Request.QueryString["language"];
            try
            {
                if (!string.IsNullOrEmpty(id))
                    Int32.Parse(id.Trim());
                if (!string.IsNullOrEmpty(version))
                {
                    if (version.ToString().ToLower().Trim() != "patient" && version.ToString().ToLower().Trim() != "healthprofessional")
                    {
                        throw new Exception("Invalid Version " + version);
                    }
                }

                if (!string.IsNullOrEmpty(language))
                {
                    if (language.ToLower().Trim() != "english" && language.ToLower().Trim() != "spanish")
                    {
                        throw new Exception("Invalid Language " + language);

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// takes a term or id to the glossary and returns the glossary term and related info.
        /// </summary>
        /// <param name="type">either 'term' or 'id'</param>
        /// <param name="param">either a glossary term or CDRID</param>
        /// <returns>an array consisting of the glossary name, pronounciation, and definition</returns>
        private ArrayList get_definition(string type, string param, PDQVersion pdqVersion, DisplayLanguage language)
        {
            string lng = string.Empty;
            if (language == DisplayLanguage.Spanish)
            {
                lng = "Spanish";
            }
            else
            {
                lng = "English";
            }

            ArrayList returnvalue = new ArrayList(3);
            returnvalue = CancerGov.CDR.TermDictionary.TermDictionaryManager.GetDefinition(type, param, pdqVersion, lng);
            return returnvalue;
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

            if (dl == DisplayLanguage.Spanish)
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

        protected void Page_Init(object sender, EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
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
