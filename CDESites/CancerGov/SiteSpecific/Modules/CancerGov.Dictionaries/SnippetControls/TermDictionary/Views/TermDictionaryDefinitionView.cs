﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Linq;
using System.Globalization;

using CancerGov.Text;
using Common.Logging;

using NCI.Web;
using NCI.Web.CDE;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;
using NCI.Web.Dictionary;
using NCI.Web.Dictionary.BusinessObjects;
using CancerGov.Dictionaries.SnippetControls.Helpers;
using CancerGov.Dictionaries.Configuration;
using Microsoft.Security.Application;

namespace CancerGov.Dictionaries.SnippetControls.TermDictionary
{
    public class TermDictionaryDefinitionView : BaseDictionaryControl
    {
        protected TermDictionaryHome termDictionaryHome;

        protected Repeater termDictionaryDefinitionView;

        static ILog log = LogManager.GetLogger(typeof(TermDictionaryDefinitionView));

        public string CdrID { get; set; }

        public string DictionaryURL { get; set; }

        public String DictionaryLanguage { get; set; }

        public int RelatedTermCount { get; set; }

        private DictionaryTerm currentItem;

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupCanonicalUrls(DictionaryURL, DictionaryURL);
            GetDefinitionTerm();
            ValidateCDRID();

            DictionaryURL = this.DictionaryRouter.GetBaseURL();

            DictionaryLanguage = PageAssemblyContext.Current.PageAssemblyInstruction.Language;

            DictionaryAppManager _dictionaryAppManager = new DictionaryAppManager();

            DictionaryTerm dataItem = _dictionaryAppManager.GetTerm(Convert.ToInt32(CdrID), NCI.Web.Dictionary.DictionaryType.term, DictionaryLanguage, "v1");
            if (dataItem != null && dataItem.Term != null)
            {
                ActivateDefinitionView(dataItem);
                currentItem = dataItem;
                // Web Analytics *************************************************
                if (WebAnalyticsOptions.IsEnabled)
                {
                    // Set analytics for definition view page load
                    SetAnalytics();
                }
            }
            else
            {
                termDictionaryDefinitionView.Visible = false;
            }
        }

        // Activate definition view for given term
        private void ActivateDefinitionView(DictionaryTerm dataItem)
        {
            var myDataSource = new List<DictionaryTerm> { dataItem };

            termDictionaryDefinitionView.Visible = true;
            termDictionaryDefinitionView.DataSource = myDataSource;
            termDictionaryDefinitionView.DataBind();

            string termName = dataItem.Term;

            CdrID = dataItem.ID.ToString();

            if (DictionaryLanguage == "es")
            {
                PageInstruction.AddFieldFilter("browser_title", (name, data) =>
                {
                    data.Value = "Definici&oacute;n de " + termName + " - Diccionario de c&aacute;ncer";
                });

                this.Page.Title = PageInstruction.GetField("short_title");

                DictionaryDefinitionHelper.SetMetaTagDescription(dataItem, DictionaryLanguage, PageInstruction);
            }
            else
            {
                PageInstruction.AddFieldFilter("browser_title", (name, data) =>
                {
                    data.Value = "Definition of " + termName + " - NCI Dictionary of Cancer Terms";
                });

                //CHANGE MADE BY CHRISTIAN RIKONG ON 12/08/2017 AT 11:47 AM
                DictionaryDefinitionHelper.SetMetaTagDescription(dataItem, DictionaryLanguage, PageInstruction);
            }

            PageInstruction.AddFieldFilter("meta_keywords", (name, data) =>
            {
                data.Value = termName + ", definition";
            });
        }

        /// <summary>
        ///    Sets the meta tag description. The function checks if the Dictionary Term has a valid (not null and length greater than 0) definition.
        ///    If it is the case the function attempts to extract the first two sentences of the Definition and set them as the meta tag description.
        ///    If not, we revert to using the term itself has the meta tag description.
        ///    
        ///    AUTHOR: CHRISTIAN RIKONG
        ///    LAST PUBLISHED DATE: 12/08/2017 11:47 AM
        /// </summary>
        /// <param name="dataItem">Stores the Dictionary Term that is used to create the description meta tag</param>
        private void SetMetaTagDescription(DictionaryTerm dataItem, string DictionaryLanguage)
        {
            string termName = dataItem.Term;

            if (dataItem.Definition != null && dataItem.Definition.Text != null && dataItem.Definition.Text.Length > 0)
            {
                string sentences = "";
                string[] definitionsSentences = System.Text.RegularExpressions.Regex.Split(dataItem.Definition.Text, @"(?<=[\.!\?])\s+");


                if (definitionsSentences != null && definitionsSentences.Length > 0)
                {
                    int sentencesCount = 0;

                    foreach (string existingSentence in definitionsSentences)
                    {
                        sentencesCount = sentencesCount + 1;

                        if (sentencesCount <= 2)
                        {
                            sentences = sentences + existingSentence + ". ";
                        }
                        else
                        {
                            break;
                        }
                    }

                    PageInstruction.AddFieldFilter("meta_description", (name, data) =>
                    {
                        data.Value = sentences;
                    });
                }
                else
                {
                    SetMetaTagDescriptionToTerm(PageInstruction, termName, DictionaryLanguage);
                }

            }
            else
            {
                SetMetaTagDescriptionToTerm(PageInstruction, termName, DictionaryLanguage);
            }

        }

        /// <summary>
        ///    Sets the Meta Tag Description to a given term
        /// </summary>
        /// <param name="PageInstruction"></param>
        /// <param name="termName"></param>
        /// <param name="DictionaryLanguage"></param>
        private void SetMetaTagDescriptionToTerm(IPageAssemblyInstruction PageInstruction, string termName, string DictionaryLanguage)
        {
            switch (DictionaryLanguage.ToLower().Trim())
            {
                case "es":
                    PageInstruction.AddFieldFilter("meta_description", (name, data) =>
                    {
                        data.Value = "Definición de " + termName;
                    });
                    break;
                default:
                    PageInstruction.AddFieldFilter("meta_description", (name, data) =>
                    {
                        data.Value = "Definition of " + termName;
                    });
                    break;
            }
        }
        
        //Add a filter for the Canonical URL.
        private void SetupCanonicalUrls(string englishDurl, string spanishDurl)
        {
            PageAssemblyContext.Current.PageAssemblyInstruction.AddUrlFilter(PageAssemblyInstructionUrls.CanonicalUrl, SetupUrlFilter);

            foreach (var lang in PageAssemblyContext.Current.PageAssemblyInstruction.TranslationKeys)
            {
                SetupTranslationFilters(PageAssemblyContext.Current.PageAssemblyInstruction, lang);
                //PageAssemblyContext.Current.PageAssemblyInstruction.AddTranslationFilter(lang, SetupUrlTranslationFilter);
            }
        }

        private void SetupTranslationFilters(IPageAssemblyInstruction pai, string lang)
        {
            // check provided PageAssemblyInstruction
            if (pai == null)
            {
                log.Warn("SetupTranslationFilter(): null PageAssemblyInstruction provided.");

                return;
            }

            CultureInfo info = new CultureInfo(lang);
            string translationLang = info.TwoLetterISOLanguageName;

            PageAssemblyContext.Current.PageAssemblyInstruction.AddTranslationFilter(lang, (name, url) => {
                url.SetUrl(url.ToString() + "/def/" + GetCDRIDForLanguageToggle(translationLang));
            });
        }

        // Sets the URL Filter for the canonical URL
        private void SetupUrlFilter(string name, NciUrl url)
        {
            url.SetUrl(this.DictionaryRouter.GetDefinitionUrl() + GetFriendlyName(CdrID));
        }

        /*// Set the URL Translation Filter
        private void SetupUrlTranslationFilter(string name, NciUrl url)
        {
            url.SetUrl(url.ToString() + "/def/" + GetCDRIDForLanguageToggle());
        }*/

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

                        Literal pronunciationKey = (Literal)e.Item.FindControl("pronunciationKey");
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
                        if (termDetails.Related.Term.Length > 0 ||
                            termDetails.Related.Summary.Length > 0 ||
                            termDetails.Related.DrugSummary.Length > 0 ||
                            termDetails.Related.External.Length > 0 ||
                            termDetails.Images.Length > 0)
                        {
                            pnlRelatedInfo.Visible = true;
                            Literal litMoreInformation = e.Item.FindControl("litMoreInformation") as Literal;
                            if (litMoreInformation != null)
                            {
                                //don't display more information text when only images are being displayed
                                if (termDetails.Related.Term.Length > 0 ||
                                termDetails.Related.Summary.Length > 0 ||
                                termDetails.Related.DrugSummary.Length > 0 ||
                                termDetails.Related.External.Length > 0)
                                {
                                    litMoreInformation.Visible = true;
                                    if (DictionaryLanguage == "es")
                                        litMoreInformation.Text = "M&aacute;s informaci&oacute;n";
                                    else
                                        litMoreInformation.Text = "More Information";
                                }
                                else
                                {
                                    litMoreInformation.Visible = false;
                                }
                            }

                            if (termDetails.Related.External.Length > 0)
                            {
                                Repeater relatedExternalRefs = (Repeater)e.Item.FindControl("relatedExternalRefs");
                                if (relatedExternalRefs != null)
                                {
                                    relatedExternalRefs.Visible = true;
                                    relatedExternalRefs.DataSource = termDetails.Related.External;
                                    relatedExternalRefs.DataBind();
                                }
                            }

                            if (termDetails.Related.Summary.Length > 0)
                            {
                                Repeater relatedSummaryRefs = (Repeater)e.Item.FindControl("relatedSummaryRefs");
                                if (relatedSummaryRefs != null)
                                {
                                    relatedSummaryRefs.Visible = true;
                                    relatedSummaryRefs.DataSource = termDetails.Related.Summary;
                                    relatedSummaryRefs.DataBind();
                                }
                            }

                            if (termDetails.Related.DrugSummary.Length > 0)
                            {
                                Repeater relatedDrugInfoSummaries = (Repeater)e.Item.FindControl("relatedDrugInfoSummaries");
                                if (relatedDrugInfoSummaries != null)
                                {
                                    relatedDrugInfoSummaries.Visible = true;
                                    relatedDrugInfoSummaries.DataSource = termDetails.Related.DrugSummary;
                                    relatedDrugInfoSummaries.DataBind();
                                }
                            }

                            if (termDetails.Related.Term.Length > 0)
                            {
                                RelatedTermCount = termDetails.Related.Term.Length;
                                PlaceHolder phRelatedTerms = (PlaceHolder)e.Item.FindControl("phRelatedTerms");
                                if (phRelatedTerms != null)
                                {
                                    phRelatedTerms.Visible = true;
                                    Label labelDefintion = (Label)e.Item.FindControl("labelDefintion");
                                    if (labelDefintion != null)
                                    {
                                        if (DictionaryLanguage == "es")
                                            labelDefintion.Text = "Definici&oacute;n de:";
                                        else
                                            labelDefintion.Text = "Definition of:";
                                    }
                                    Repeater relatedTerms = (Repeater)e.Item.FindControl("relatedTerms");
                                    if (relatedTerms != null)
                                    {
                                        relatedTerms.DataSource = termDetails.Related.Term;
                                        relatedTerms.DataBind();
                                    }
                                }
                            }

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

                            Repeater relatedVideos = (Repeater)e.Item.FindControl("relatedVideos");
                            if (relatedVideos != null)
                            {
                                if (termDetails.HasVideos && termDetails.Videos.Length > 0)
                                {
                                    relatedVideos.Visible = true;
                                    relatedVideos.DataSource = termDetails.Videos;
                                    relatedVideos.DataBind();
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

        protected void relatedTerms_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //get the RelatedTerm object that is bound to the current row.
                RelatedTerm relatedTerm = (RelatedTerm)e.Item.DataItem;
                if (relatedTerm != null)
                {
                    HyperLink relatedTermLink = (HyperLink)e.Item.FindControl("relatedTermLink");
                    if (relatedTermLink != null)
                    {
                        relatedTermLink.NavigateUrl = this.DictionaryRouter.GetDefinitionUrl() + GetFriendlyName(relatedTerm.Termid.ToString());
                        relatedTermLink.Text = relatedTerm.Text;

                        //make sure the comma is only displayed when there is more than one related term
                        Literal relatedTermSeparator = (Literal)e.Item.FindControl("relatedTermSeparator");
                        if (relatedTermSeparator != null)
                        {
                            if (e.Item.ItemIndex >= 0 && e.Item.ItemIndex < RelatedTermCount - 1)
                                relatedTermSeparator.Visible = true;
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
                                    termEnlargeImage.InnerText = DictionaryLanguage == "es" ? "Ampliar" : "Enlarge";
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
                                        termEnlargeImage.InnerText = DictionaryLanguage == "es" ? "Ampliar" : "Enlarge";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void relatedVideos_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                //get the ImageReference object that is bound to the current row.
                VideoReference videoDetails = (VideoReference)e.Item.DataItem;

                if (videoDetails != null)
                {
                    // Set the CSS class for the video's containing figure element.
                    HtmlGenericControl container = (HtmlGenericControl)e.Item.FindControl("videoContainer");
                    if (container != null)
                    {
                        // These are the templates allowed by the CDR's DTD for GlossaryTerm Embedded videos.
                        // Others do exist in Percussion, but are deprecated per OCECDR-3558.
                        switch (videoDetails.Template.ToLowerInvariant())
                        {
                            case "video100notitle":
                            case "video100title":
                                container.Attributes.Add("class", "video center size100");
                                break;

                            case "video50notitle":
                                container.Attributes.Add("class", "video center size50");
                                break;

                            case "video50notitleright":
                            case "video50titleright":
                                container.Attributes.Add("class", "video right size50");
                                break;

                            case "video75notitle":
                            case "video75title":
                                container.Attributes.Add("class", "video center size75");
                                break;

                            default:
                                log.ErrorFormat("Unknown video template '{0}'.", videoDetails.Template);
                                container.Attributes.Add("class", "video center size100");
                                break;
                        }
                    }

                    // Set up the title display.
                    // Requires a title be present, and not using one of the "NoTitle" templates.
                    if (!String.IsNullOrWhiteSpace(videoDetails.Title) && !videoDetails.Template.ToLowerInvariant().Contains("notitle"))
                    {
                        HtmlGenericControl title = (HtmlGenericControl)e.Item.FindControl("videoTitle");
                        if (title != null)
                        {
                            title.Visible = true;
                            title.InnerText = videoDetails.Title;
                        }
                    }

                    // Display the caption
                    HtmlGenericControl caption = (HtmlGenericControl)e.Item.FindControl("captionContainer");
                    if (caption != null && !String.IsNullOrWhiteSpace(videoDetails.Caption))
                    {
                        caption.Visible = true;
                        caption.InnerHtml = videoDetails.Caption;
                    }
                }
            }
        }

        private void ValidateCDRID()
        {
            if (!string.IsNullOrEmpty(CdrID))
            {
                try
                {
                    Int32.Parse(CdrID);
                }
                catch (Exception)
                {
                    throw new Exception("Invalid CDRID" + CdrID);
                }
            }
        }

        /// <summary>
        /// Gets the CDRID (definition term) from the URL.
        /// If a friendly name is given, it determines the CDRID from it.
        /// </summary>
        private void GetDefinitionTerm()
        {
            if (!string.IsNullOrEmpty(this.GetDefinitionParam()))
            {
                string param = this.GetDefinitionParam();

                // Get friendly name to CDRID mappings
                string dictionaryMappingFilepath = null;

                dictionaryMappingFilepath = this.DictionaryConfiguration.Files.Single(a => a.Locale == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName).Filepath;

                if (!string.IsNullOrEmpty(dictionaryMappingFilepath))
                {
                    TerminologyMapping map = TerminologyMapping.GetMappingForFile(dictionaryMappingFilepath);

                    // If pretty name is in label mappings, set CDRID
                    if (map.MappingContainsFriendlyName(param))
                    {
                        CdrID = map.GetCDRIDFromFriendlyName(param);
                    }
                    else
                    {
                        CdrID = param;
                    }
                }
                else
                {
                    CdrID = param;
                }
            }
        }

        /// <summary>
        /// Gets the CDRID (definition term) for the language toggle using the current defintion term in the URL.
        /// If a friendly name is given, it determines the CDRID from it.
        /// </summary>
        private string GetCDRIDForLanguageToggle(string translationLang)
        {
            string cdridForLangToggle = CdrID;

            if (!string.IsNullOrEmpty(this.GetDefinitionParam()))
            {
                // Get friendly name to CDRID mappings
                string dictionaryMappingFilepath = null;

                dictionaryMappingFilepath = this.DictionaryConfiguration.Files.Single(a => a.Locale == translationLang).Filepath;

                if (!string.IsNullOrEmpty(dictionaryMappingFilepath))
                {
                    TerminologyMapping map = TerminologyMapping.GetMappingForFile(dictionaryMappingFilepath);

                    // If pretty name is in label mappings, set CDRID
                    if (map.MappingContainsCDRID(cdridForLangToggle))
                    {
                        cdridForLangToggle = map.GetFriendlyNameFromCDRID(cdridForLangToggle);
                    }
                }
            }

            return cdridForLangToggle;
        }

        /// <summary>
        /// Set default pageLoad analytics for this page
        /// </summary>
        protected void SetAnalytics()
        {
            // Format string for analytics params: Dictionary|Language|Term|ID
            string[] analyticsParams = new string[4];

            analyticsParams[0] = DictionaryAnalyticsType.Term.Name;

            if (PageAssemblyContext.Current.PageAssemblyInstruction.Language == "es")
                analyticsParams[1] = "Spanish";
            else
                analyticsParams[1] = "English";

            analyticsParams[2] = currentItem.Term;
            analyticsParams[3] = currentItem.ID.ToString();
            
            string dictionaryAnalytics = string.Join("|", analyticsParams);

            // Set event
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Events.event11, wbField =>
            {
                wbField.Value = WebAnalyticsOptions.Events.event11.ToString();
            });

            // Set props
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.Props.prop16, wbField =>
            {
                wbField.Value = dictionaryAnalytics;
            });

            // Set eVars
            this.PageInstruction.SetWebAnalytics(WebAnalyticsOptions.eVars.evar16, wbField =>
            {
                wbField.Value = dictionaryAnalytics;
            });
        }
    }
}