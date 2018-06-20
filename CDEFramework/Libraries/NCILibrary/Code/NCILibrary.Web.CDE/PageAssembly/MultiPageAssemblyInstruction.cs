using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Text.RegularExpressions;
using System.Web;
using System.IO;
using NCI.Web.CDE.Configuration;
using NCI.Web.CDE.WebAnalytics;
using NCI.Util;
using NCI.Core;
using NCI.Web.ProductionHost;

namespace NCI.Web.CDE
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.example.org/CDESchema")]
    [System.Xml.Serialization.XmlRootAttribute("MultiPageAssemblyInstruction", Namespace = "http://www.example.org/CDESchema", IsNullable = false)]
    public class MultiPageAssemblyInstruction : BasePageAssemblyInstruction, IMultiPageAssemblyInstruction
    {

        #region Member Variables

        /// <summary>
        /// Dictionary holds the registered multiple Url filters.
        /// </summary>
        private Dictionary<string, UrlFilterDelegate> _UrlFilterDelegates = new Dictionary<string, UrlFilterDelegate>();
        /// <summary>
        /// Dictionary holds the registered multiple Field Filters
        /// </summary>
        private Dictionary<string, FieldFilterDelegate> _FieldFilterDelegates = new Dictionary<string, FieldFilterDelegate>();
        /// <summary>
        /// A collection of the snippets to be displayed on the page.
        /// </summary>
        private SnippetInfoCollection _snippets;

        private WebAnalyticsSettings webAnalyticsSettings = null;
        /// <summary>
        /// Contains collection of Pages inside the multipage container
        /// </summary>
        private MultiPageCollection _pages;

        /// <summary>
        /// Defines the current url that is being requested.
        /// </summary>
        private int _currentPageIndex = -1;

        /// <summary>
        ///temporary fix to add cgvSiteBannerPrint to blocked slots if cgvSiteBanner is blocked.

        /// </summary>
        private string[] blockCgvBannerPrint = new[] { "cgvSiteBannerPrint" };
        private LocalFieldCollection _localFields;

        #endregion

        public MultiPageAssemblyInstruction()
        {
            // Initialize sub objects.
            _snippets = new SnippetInfoCollection();
            _pages = new MultiPageCollection();
            PageMetadata = new PageMetadata();
            SocialMetadata = new SocialMetadata();
            PageResources = new PageResources();
            _localFields = new LocalFieldCollection();

            //base.Initialize();
        }

        #region Properties

        /// <summary>
        /// Gets the URL filter delegates.
        /// </summary>
        /// <value>The URL filter delegates.</value>
        private Dictionary<string, UrlFilterDelegate> UrlFilterDelegates
        {
            get
            {
                return _UrlFilterDelegates;
            }
        }

        /// <summary>
        /// Gets the field filter delegates.
        /// </summary>
        /// <value>The field filter delegates.</value>
        private Dictionary<string, FieldFilterDelegate> FieldFilterDelegates
        {
            get
            {
                return _FieldFilterDelegates;
            }
        }


        #endregion


        #region IPageAssemblyInstruction Members
        [System.Xml.Serialization.XmlElement(ElementName = "LocalFields", Form = XmlSchemaForm.Unqualified)]
        public LocalFieldCollection LocalFields
        {
            get
            {
                return _localFields;
            }
            set
            {
                _localFields = value;
            }
        }
        /// <summary>
        /// BlockedSlots contain information about the blocked slot which should not be displayed on the page rendered.
        /// </summary>
        /// <value>The blocked slot names.</value>
        public string[] BlockedSlotNames
        {
            get
            {
                var names = from slot in BlockedSlots
                            select slot.Name;

                if (PageAssemblyContext.CurrentDisplayVersion != DisplayVersions.PrintAll && PageAssemblyContext.CurrentDisplayVersion != DisplayVersions.ViewAll)
                {
                    if (_currentPageIndex == -1)
                    {
                        names = names.Union(_pages._Pages[0].BlockedSlotNames);

                    }
                    else
                    {
                        names = names.Union(_pages._Pages[_currentPageIndex].BlockedSlotNames);

                    }
                }

                //temporary fix to add cgvSiteBannerPrint to blocked slots if cgvSiteBanner is blocked.
                if (names.Contains("cgvSiteBanner"))
                    names = names.Union(blockCgvBannerPrint);


                return names.ToArray();
            }
        }

        /// <summary>
        /// Gets the name of the page template i.e the actual aspx page to be loaded.
        /// </summary>
        /// <value>The name of the page template.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        [XmlIgnore()]
        public string PageTemplateName
        {
            get
            {
                if (PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.PrintAll ||
                    PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.ViewAll)
                {
                    return InternalPageTemplateName;
                }
                else
                {
                    //It must be one of the pages.
                    if (_currentPageIndex == -1)
                    {
                        //First page when using container url
                        return _pages._Pages[0].PageTemplateName;
                    }
                    else
                    {
                        return _pages._Pages[_currentPageIndex].PageTemplateName;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a bool indicating if this page requires HTTP5 Push State.  We are not going to
        /// support MPAI and PushState. 
        /// </summary>
        public bool ImplementsPushState { get { return false; } }

        /// <summary>
        /// Gets the template theme this page should be using.  (Set on a SectionDetails withing the parent folders of this page)
        /// </summary>
        /// <value>The name of the Template Theme</value>
        public string TemplateTheme
        {
            get
            {
                SectionDetail sectionDetail = SectionDetailFactory.GetSectionDetail(SectionPath);
                if (sectionDetail != null)
                {
                    return sectionDetail.GetEffectiveTemplateTheme();
                }

                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the name of the page template i.e the actual aspx page to be loaded.
        /// </summary>
        /// <value>The name of the page template.</value>
        [XmlElement(ElementName = "PageTemplateName", Form = XmlSchemaForm.Unqualified)]
        public string InternalPageTemplateName { get; set; }


        /// <summary>
        /// Gets or sets the language for the page displayed.
        /// </summary>
        /// <value>The language.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string Language { get; set; }

        /// <summary>
        /// The path of all parent folders of the page assembly instruction.
        /// </summary>
        /// <value></value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SectionPath { get; set; }

        /// <summary>
        /// Gets or sets the pretty URL.
        /// </summary>
        /// <value>The pretty URL.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string PrettyUrl { get; set; }

        /// <summary>
        /// Gets or sets the page metadata.
        /// </summary>
        /// <value>The page metadata.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public PageMetadata PageMetadata { get; set; }

        /// <summary>
        /// Gets or sets the social metadata.
        /// </summary>
        /// <value>The social metadata.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public SocialMetadata SocialMetadata { get; set; }

        /// <summary>
        /// Gets the page resources (JS and CSS)
        /// </summary>
        /// <value>The social metadata.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public PageResources PageResources { get; set; }

        /// <summary>
        /// Gets or sets the content dates for the page.
        /// </summary>
        /// <value>The page metadata.</value>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public ContentDates ContentDates { get; set; }

        /// <summary>
        /// Gets the collections of the snippets
        /// </summary>
        /// <value>The snippet infos.</value>
        [System.Xml.Serialization.XmlArray(ElementName = "Snippets", Form = XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItem("SnippetInfo", Form = XmlSchemaForm.Unqualified)]
        public SnippetInfoCollection SnippetInfos
        {
            get { return _snippets; }
        }

        [System.Xml.Serialization.XmlArray(ElementName = "Pages", Form = XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItem("Page", Form = XmlSchemaForm.Unqualified)]
        public MultiPageCollection Page
        {
            get { return _pages; }
        }
        /// <summary>
        /// A collection of SnippetInfo objects for the page assembly Instruction which are needed
        /// to render a page.
        /// </summary>
        /// <value></value>
        [System.Xml.Serialization.XmlIgnore()]
        public IEnumerable<SnippetInfo> Snippets
        {
            get
            {
                List<SnippetInfo> snippets = new List<SnippetInfo>();
                List<SnippetInfo> pageSnippets = new List<SnippetInfo>();

                if (PageAssemblyContext.Current.DisplayVersion == DisplayVersions.ViewAll || PageAssemblyContext.Current.DisplayVersion == DisplayVersions.PrintAll)
                {

                    pageSnippets = GetAllPageSnippets();
                    if (pageSnippets.Count > 0)
                    {

                        foreach (SnippetInfo pagesnipt in pageSnippets)
                        {
                            if (pagesnipt.OnlyDisplayFor.Count() == 0 || pagesnipt.OnlyDisplayFor.Contains(PageAssemblyContext.Current.DisplayVersion))
                            {
                                snippets.Add(pagesnipt);
                            }

                        }
                    }
                }
                else
                {
                    //Load current Page snippets
                    pageSnippets = GetPageSnippets();
                    if (pageSnippets.Count > 0)
                    {
                        foreach (SnippetInfo pagesnipt in pageSnippets)
                        {
                            if (pagesnipt.OnlyDisplayFor.Count() == 0 || pagesnipt.OnlyDisplayFor.Contains(PageAssemblyContext.Current.DisplayVersion))
                            {
                                snippets.Add(pagesnipt);
                            }

                        }
                    }
                }
                // Add all local snippets to the list to return.
                foreach (SnippetInfo snipt in _snippets)
                {
                    if (snipt.OnlyDisplayFor.Count() == 0 || snipt.OnlyDisplayFor.Contains(PageAssemblyContext.Current.DisplayVersion))
                    {
                        snippets.Add(snipt);
                    }
                }

                ////Find all of the Slots on the page which are not blocked and where those Slots do not have associated SnippetInfos in the SinglePageAssemblyInstruction XML file.
                IEnumerable<string> filledTemplateSlots = (from snippet in snippets select snippet.SlotName).Distinct<string>().Except(BlockedSlotNames);

                SectionDetail sectionDetail = SectionDetailFactory.GetSectionDetail(SectionPath);
                if (sectionDetail != null)
                {
                    List<SnippetInfo> snippetsFromParent = sectionDetail.GetSnippetsNotAssociatedWithSlots(filledTemplateSlots);

                    foreach (SnippetInfo sniptParents in snippetsFromParent)
                    {
                        if (sniptParents.OnlyDisplayFor.Count() == 0 || sniptParents.OnlyDisplayFor.Contains(PageAssemblyContext.Current.DisplayVersion))
                        {
                            snippets.Add(sniptParents);
                        }

                    }

                }
                return snippets;


            }
        }

        [System.Xml.Serialization.XmlIgnore()]
        public IEnumerable<MultiPage> Pages
        {
            get
            {
                List<MultiPage> page = new List<MultiPage>();

                // Add all pages to the list to return.
                page.AddRange(_pages);
                return page;
            }
        }

        /// <summary>
        /// Determines whether the specified requested URL contains URL.
        /// </summary>
        /// <param name="requestedURL">The requested URL.</param>
        /// <returns>
        /// 	<c>true</c> if the specified requested URL contains URL; otherwise, <c>false</c>.
        /// </returns>
        public Boolean ContainsURL(string requestedURL)
        {
            return GetPageIndexOfUrl(requestedURL) >= 0;
        }

        /// <summary>
        /// Gets the page index of URL.
        /// </summary>
        /// <param name="url">The page URL to get the index for.</param>
        /// <returns></returns>
        public int GetPageIndexOfUrl(string url)
        {

            int pageCount = _pages.Count();
            for (int i = 0; i <= pageCount - 1; i++)
            {
                if (string.Compare(_pages._Pages[i].PrettyUrl, url, true) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetCurrentPageIndex(int index)
        {
            _currentPageIndex = index;
        }

        /// <summary>
        /// Gets or sets the blocked slots which should not be displayed on the page rendered.
        /// </summary>
        /// <value>The blocked slots.</value>
        [System.Xml.Serialization.XmlArray(ElementName = "BlockedSlots", Form = XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlArrayItem("Slot", Form = XmlSchemaForm.Unqualified)]
        public BlockedSlot[] BlockedSlots { get; set; }



        /// <summary>
        /// Gets the value of the field referenced by "String" with all field filter applied.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <exception cref="ArgumentException">Thrown when the fieldName field is null or empty.</exception>
        /// <returns></returns>
        public string GetField(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("The field name may not be null or empty.");

            string rtnValue = string.Empty;

            FieldFilterDelegate del = FieldFilterDelegates[fieldName.ToLower()];
            if (del != null)
            {
                //Initialize the field data to empty field data
                FieldFilterData data = new FieldFilterData();

                //Call delegate, all delegates will modify the FieldData string of the
                //FieldFilterData object we are passing in.
                del(fieldName, data);

                //set the return value to the processed value of the FieldFilterData
                rtnValue = data.Value;
            }

            return rtnValue;
        }
        /// <summary>
        /// Adds a field filter which modifies the value of the field referenced by "string"
        /// when GetField is called.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="filter"></param>
        public void AddFieldFilter(string fieldName, FieldFilterDelegate filter)
        {
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("The fieldName parameter may not be null or empty.");

            string fieldNameKey = fieldName.ToLower();

            if (FieldFilterDelegates.ContainsKey(fieldNameKey) == false)
            {
                FieldFilterDelegates.Add(fieldNameKey, filter);
            }
            else
            {
                //Note, this must be called this way and cannot use an intermediate value, I.E.
                //  FieldFilterDelegate foo = FieldFilterDelegates[fieldName.ToLower()];
                //  foo += filter;
                //will not work.  The reason is, Delegates are Immutable!!
                FieldFilterDelegates[fieldNameKey] += filter;
            }
        }

        /// <summary>
        /// Gets the URL referenced by "String" with all URL filters.
        /// </summary>
        /// <param name="urlType"></param>
        /// <returns></returns>
        public NciUrl GetUrl(string urlType)
        {
            NciUrl nciUrl = new NciUrl();
            string linkTypeKey = urlType.ToLower();

            if (UrlFilterDelegates.ContainsKey(linkTypeKey) == true)
            {
                UrlFilterDelegate UrlfilterLinkDelegate = UrlFilterDelegates[linkTypeKey];
                UrlfilterLinkDelegate(linkTypeKey, nciUrl);
            }
            else
            {
                throw new PageAssemblyException(String.Format("Unknown link type \"{0}\"", urlType));
            }

            return nciUrl;
        }


        /// <summary>
        /// Adds a URL filter which modifies the URL referenced by "string" when GetUrl is called.
        /// </summary>
        /// <param name="urlType">Pretty URL or Cannonical URL</param>
        /// <param name="fieldFilter"></param>
        public void AddUrlFilter(string urlType, UrlFilterDelegate fieldFilter)
        {
            if (string.IsNullOrEmpty(urlType))
                throw new ArgumentException("The urlType parameter may not be null or empty.");

            string linkTypeKey = urlType.ToLower();

            if (UrlFilterDelegates.ContainsKey(linkTypeKey) == false)
            {
                UrlFilterDelegates.Add(linkTypeKey, fieldFilter);
            }
            else
            {
                UrlFilterDelegates[linkTypeKey] += fieldFilter;
            }
        }

        /// <summary>
        /// This property returns the keys which represent the available content versions. 
        /// </summary>
        /// <value>A string array which are the keys to the alternate content versions.</value>
        public string[] AlternateContentVersionsKeys
        {
            get
            {
                ArrayList keysList = new ArrayList();
                if (AlternateContentVersions.IsPrintAvailable)
                {
                    if (PageAssemblyContext.Current.DisplayVersion != DisplayVersions.ViewAll)
                        keysList.Add("print");
                    if (PageAssemblyContext.Current.DisplayVersion != DisplayVersions.PrintAll)
                        keysList.Add("print");
                }

                if (PageAssemblyContext.Current.DisplayVersion != DisplayVersions.ViewAll)
                    keysList.Add("viewall");
                if (AlternateContentVersions.IsShareBookmarkAvailable)
                    keysList.Add("bookmarkshare");
                if (AlternateContentVersions.IsEmailAvailable)
                    keysList.Add("email");
                if (AlternateContentVersions.IsMobileShareAvailable)
                    keysList.Add("mobileShare");
                if (AlternateContentVersions.IsFontResizeAvailable)
                    keysList.Add("fontResize");
                if (AlternateContentVersions.IsPublicArchive)
                    keysList.Add("publicArchive");
                if (AlternateContentVersions.IsPublicUse)
                    keysList.Add("publicUse");
                if (!string.IsNullOrEmpty(AlternateContentVersions.SyndicationURL))
                    keysList.Add("syndicated");
                if (!string.IsNullOrEmpty(AlternateContentVersions.OrderCopyURL))
                    keysList.Add("free");

                //Set AltLanguage Key
                if (_currentPageIndex == -1)
                {
                    if (PageMetadata.AltLanguageURL != null)
                    {
                        if (!string.IsNullOrEmpty(PageMetadata.AltLanguageURL.Trim()))
                        {
                            keysList.Add("altlanguage");
                        }
                    }
                }
                else
                {
                    if (_pages._Pages[_currentPageIndex].PageMetadata.AltLanguageURL != null)
                    {
                        if (!string.IsNullOrEmpty(_pages._Pages[_currentPageIndex].PageMetadata.AltLanguageURL.Trim()))
                        {
                            keysList.Add("altlanguage");
                        }
                    }
                }

                if (AlternateContentVersions.AltLanguage)
                    keysList.Add("altlanguage");

                //Set Mobile URL
                if (PageMetadata.MobileURL != null)
                {
                    if (!string.IsNullOrEmpty(PageMetadata.MobileURL.Trim()))
                    {
                        keysList.Add("mobileurl");
                    }
                }

                //Set Desktop URL
                if (PageMetadata.DesktopURL != null)
                {
                    if (!string.IsNullOrEmpty(PageMetadata.DesktopURL.Trim()))
                    {
                        keysList.Add("desktopurl");
                    }
                }

                // Set comments enabled
                if (SocialMetadata.IsCommentingAvailable != null)
                {
                    keysList.Add("commentsavailable");
                }

                // Enumerate the Files and set an URL filter.
                foreach (AlternateContentFile acFile in AlternateContentVersions.Files)
                {
                    keysList.Add(acFile.MimeType.ToLower());
                    AddUrlFilter(acFile.MimeType.ToLower(), (name, url) =>
                    {
                        foreach (AlternateContentFile file in AlternateContentVersions.Files)
                        {
                            if (file.MimeType.ToLower() == name.ToLower())
                                url.SetUrl(file.Url);
                        }
                    });
                }

                return (string[])keysList.ToArray(typeof(string));
            }
        }

        /// <summary>
        /// This method returns the web analytics settings.
        /// </summary>
        public override WebAnalyticsSettings GetWebAnalytics()
        {
            return base.GetWebAnalytics();
        }

        /// <summary>
        /// Provides a list of all SocialMetaTag objects defined for the current assembly.
        /// </summary>
        /// <returns>A potentially-empty array of SocialMetaTag objects.</returns>
        public SocialMetaTag[] GetSocialMetaTags()
        {
            return GenerateSocialMetaTags(SocialMetadata.Tags);
        }

        /// <summary>
        /// Provides the PageResources object encapsulating CSS and JS for the page.
        /// </summary>
        /// <returns>A PageResources object.</returns>
        public PageResources GetPageResources()
        {
            return PageResources;
        }

        /// <summary>
        /// When a data point related to web anlytics is to be modified it is done using this method. 
        /// </summary>
        /// <param name="type">The type of the </param>
        /// <param name="propNumber"></param>
        /// <param name="filter"></param>
        public void SetWebAnalytics(WebAnalyticsOptions.Events webAnalyticType, WebAnalyticsDataPointDelegate filter)
        { base.SetWebAnalytics(webAnalyticType.ToString(), filter); }
        public void SetWebAnalytics(WebAnalyticsOptions.eVars webAnalyticType, WebAnalyticsDataPointDelegate filter)
        { base.SetWebAnalytics(webAnalyticType.ToString(), filter); }
        public void SetWebAnalytics(WebAnalyticsOptions.Props webAnalyticType, WebAnalyticsDataPointDelegate filter)
        { base.SetWebAnalytics(webAnalyticType.ToString(), filter); }

        #endregion

        /// <summary>
        /// Returns Alternate content versions object which contains information necessary to display 
        /// the page options.
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public AlternateContentVersions AlternateContentVersions { get; set; }

        /// <summary>
        /// Gets the meta description.
        /// </summary>
        /// <returns>meta description string which can be used for the meta tag on the page</returns>
        private string GetMetaDescription()
        {
            string meta = GetField("meta_description");

            if (string.IsNullOrEmpty(meta))
            {
                meta = GetField("short_description");

                if (!string.IsNullOrEmpty(meta))
                {
                    //string meta tags from HTML.
                    meta = Regex.Replace(meta, @"<(.|\n)*?>", String.Empty);
                }

                else
                {
                    meta = GetField("long_description");
                    if (!string.IsNullOrEmpty(meta))
                    {
                        //string meta tags from HTML.
                        meta = Regex.Replace(meta, @"<(.|\n)*?>", String.Empty);
                    }

                }
            }
            return meta;
        }

        /// <summary>
        /// Gets an instance of the HttpServerUtility for the current request.
        /// </summary>
        private HttpServerUtility Server
        {
            get { return HttpContext.Current.Server; }
        }

        /// <summary>
        /// Gets the page snippets.
        /// </summary>
        /// <returns>Collection of page snippets</returns>
        public List<SnippetInfo> GetPageSnippets()
        {
            List<SnippetInfo> pageSnippets = new List<SnippetInfo>();

            if (_pages._Pages.Count > 0)
            {
                int tmpPageIndex = _currentPageIndex;

                if (tmpPageIndex == -1)
                    tmpPageIndex = 0;

                pageSnippets.AddRange(_pages._Pages[tmpPageIndex].SnippetInfos);

            }

            return pageSnippets;
        }

        /// <summary>
        /// Gets all page snippets.
        /// </summary>
        /// <returns>Collection of all the page snippets</returns>
        public List<SnippetInfo> GetAllPageSnippets()
        {
            List<SnippetInfo> pageSnippets = new List<SnippetInfo>();
            string URL = PageAssemblyContext.Current.requestedUrl;
            int pageCount = _pages._Pages.Count;
            string requestedPage = URL.Substring(URL.LastIndexOf('/'));
            for (int i = 0; i <= pageCount - 1; i++)
            {
                pageSnippets.AddRange(_pages._Pages[i].SnippetInfos);
            }

            return pageSnippets;
        }

        public override void Initialize()
        {
            base.Initialize();
            RegisterFieldFilters();
            RegisterUrlFilters();
            RegisterWebAnalyticsFieldFilters();
            base.RegisterTranslationFilters();
        }

        #region InitializeFunctions

        /// <summary>
        /// Registers the field filters        
        /// </summary> 
        private void RegisterFieldFilters()
        {
            AddFieldFilter("long_title", (name, data) =>
            {
                data.Value = this.PageMetadata.LongTitle;
            });

            AddFieldFilter("short_title", (name, data) =>
            {
                data.Value = this.PageMetadata.ShortTitle;
            });

            AddFieldFilter(PageAssemblyInstructionFields.HTML_Title, (name, data) =>
            {
                data.Value = GetField("short_title") + ContentDeliveryEngineConfig.PageTitle.AppendPageTitle.Title;
            });

            AddFieldFilter("short_description", (name, data) =>
            {
                data.Value = this.PageMetadata.ShortDescription;
            });

            AddFieldFilter("long_description", (name, data) =>
            {
                data.Value = this.PageMetadata.LongDescription;
            });

            AddFieldFilter("meta_description", (name, data) =>
            {
                data.Value = this.PageMetadata.MetaDescription;
            });

            AddFieldFilter(PageAssemblyInstructionFields.HTML_MetaDescription, (name, data) =>
            {
                string metaDescription = GetMetaDescription();
                data.Value = metaDescription;
            });

            AddFieldFilter("meta_keywords", (name, data) =>
            {
                data.Value = this.PageMetadata.MetaKeywords;
            });

            AddFieldFilter(PageAssemblyInstructionFields.HTML_MetaKeywords, (name, data) =>
            {
                data.Value = GetField("meta_keywords");
            });

            AddFieldFilter("meta_robots", (name, data) =>
            {
                if (PageAssemblyContext.Current.DisplayVersion == DisplayVersions.Print)
                    data.Value = "noindex, nofollow";
                else
                    data.Value = "";
            });

            AddFieldFilter(PageAssemblyInstructionFields.HTML_MetaRobots, (name, data) =>
            {
                data.Value = GetField("meta_robots");
            });

            AddFieldFilter("channelName", (name, data) =>
            {
                data.Value = this.SectionPath;
            });

            AddFieldFilter("page_short_title", (name, data) =>
            {
                int pageIndex = (_currentPageIndex == -1) ? 0 : _currentPageIndex;
                data.Value = _pages._Pages[pageIndex].PageMetadata.ShortTitle;
            });

            //Register MarkupExtension Field Filters
            foreach (LocalField localField in _localFields)
            {
                AddFieldFilter(localField.Name, (name, data) =>
                {
                    data.Value = _localFields[name].Value;
                });
            }

            AddFieldFilter("add_this_title", (name, data) =>
            {
                data.Value = this.PageMetadata.ShortTitle;
            });

            AddFieldFilter("add_this_description", (name, data) =>
            {
                data.Value = this.PageMetadata.ShortDescription;
            });

            AddFieldFilter("site_name", (name, data) =>
            {
                data.Value = ProductionHostConfig.Sitename;
            });

            // also initialize social metadata field filters
            RegisterSocialMetadataFieldFilters(this, SocialMetadata);
        }

        /// <summary>
        /// Registers the URL filters        
        /// </summary> 
        private void RegisterUrlFilters()
        {

            AddUrlFilter("CurrentURL", (name, url) =>
            {
                string currentURL = GetUrl(PageAssemblyInstructionUrls.PrettyUrl).ToString();

                if (PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.ViewAll)
                    currentURL += "/AllPages";
                else if (PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.Print)
                    currentURL += "#";
                else if (PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.PrintAll)
                    currentURL += "/AllPages#";

                url.SetUrl(currentURL);

            });

            AddUrlFilter(PageAssemblyInstructionUrls.CanonicalUrl, (name, url) =>
            {
                url.SetUrl(GetUrl("CurrentURL").ToString());
            });

            AddUrlFilter(PageAssemblyInstructionUrls.PrettyUrl, (name, url) =>
            {
                int pageIndex = (_currentPageIndex == -1) ? 0 : _currentPageIndex;
                //Set Property 
                PrettyUrl = _pages._Pages[pageIndex].PrettyUrl;
                url.SetUrl(PrettyUrl);
            });

            AddUrlFilter("add_this_url", (name, url) =>
            {
                int pageIndex = (_currentPageIndex == -1) ? 0 : _currentPageIndex;
                //Set Property 
                PrettyUrl = _pages._Pages[pageIndex].PrettyUrl;
                url.SetUrl(PrettyUrl);
            });
			
			AddUrlFilter("fontResize", (name, url) =>
            {
                url.SetUrl(GetUrl("CurrentURL").ToString());
                url.UriStem += "#";
            });

            AddUrlFilter("syndicated", (name, url) =>
            {
                string syndicationUrl = string.Empty;
                if (!string.IsNullOrEmpty(AlternateContentVersions.SyndicationURL))
                {
                    syndicationUrl = AlternateContentVersions.SyndicationURL.Trim();
                }
                url.SetUrl(syndicationUrl, true);
            });

            #region MobileURL
            // Mobule URL filter 
            if (_currentPageIndex == -1)
            {
                if (PageMetadata.MobileURL != null)
                {
                    if (!string.IsNullOrEmpty(PageMetadata.MobileURL.ToString().Trim()))
                    {
                        AddUrlFilter("mobileurl", (name, url) =>
                        {
                            url.SetUrl(PageMetadata.MobileURL);
                        });
                    }
                }
            }
            else
            {
                if (_pages._Pages[_currentPageIndex].PageMetadata.MobileURL == null)
                {
                    AddUrlFilter("mobileurl", (name, url) =>
                    {
                        url.SetUrl(PageMetadata.MobileURL);
                    });
                }
                else
                {
                    AddUrlFilter("mobileurl", (name, url) =>
                    {
                        url.SetUrl(PageMetadata.MobileURL + _pages._Pages[_currentPageIndex].PageMetadata.MobileURL);
                    });
                }
            }

            #endregion

            #region DesktopURL
            if (_currentPageIndex == -1)
            {
                if (PageMetadata.DesktopURL != null)
                {
                    if (!string.IsNullOrEmpty(PageMetadata.DesktopURL.ToString().Trim()))
                    {
                        AddUrlFilter("desktopurl", (name, url) =>
                        {
                            url.SetUrl(PageMetadata.DesktopURL);
                        });
                    }
                }
            }
            else
            {
                if (_pages._Pages[_currentPageIndex].PageMetadata.DesktopURL == null)
                {
                    AddUrlFilter("desktopurl", (name, url) =>
                    {
                        url.SetUrl(PageMetadata.DesktopURL);
                    });
                }
                else
                {
                    AddUrlFilter("desktopurl", (name, url) =>
                    {
                        url.SetUrl(PageMetadata.DesktopURL + _pages._Pages[_currentPageIndex].PageMetadata.DesktopURL);
                    });
                }
            }
            #endregion

            #region AltLanguageURL
            // Alt Language URL filter 
            if (_currentPageIndex == -1)
            {
                if (PageMetadata.AltLanguageURL != null)
                {
                    if (!string.IsNullOrEmpty(PageMetadata.AltLanguageURL.ToString().Trim()))
                    {
                        AddUrlFilter("AltLanguage", (name, url) =>
                        {
                            url.SetUrl(PageMetadata.AltLanguageURL);
                        });
                    }
                }
            }

            else
            {
                if (_pages._Pages[_currentPageIndex].PageMetadata.AltLanguageURL != null)
                {
                    if (!string.IsNullOrEmpty(_pages._Pages[_currentPageIndex].PageMetadata.AltLanguageURL.ToString().Trim()))
                    {
                        AddUrlFilter("AltLanguage", (name, url) =>
                        {
                            url.SetUrl(PageMetadata.AltLanguageURL + _pages._Pages[_currentPageIndex].PageMetadata.AltLanguageURL);
                        });
                    }
                }
            }
            #endregion

            #region AddFilter For PageOptions
            // URL Filter specifically for PageOptions

            AddUrlFilter("Print", (name, url) =>
            {
                string printURL = GetUrl("CurrentURL").ToString();
                if (PageAssemblyContext.CurrentDisplayVersion != DisplayVersions.Print)
                    printURL += "#";
                url.SetUrl(printURL);
            });

            AddUrlFilter("Email", (name, url) =>
            {
                url.SetUrl(GetEmailUrl());
            });

            AddUrlFilter("free", (name, url) =>
            {
                string freeCopyUrl = string.Empty;
                if (!string.IsNullOrEmpty(AlternateContentVersions.OrderCopyURL))
                    freeCopyUrl = AlternateContentVersions.OrderCopyURL.Trim();
                url.SetUrl(freeCopyUrl, true);
            });

            AddUrlFilter("ViewAll", (name, url) =>
            {
                string viewAllUrl = GetUrl("CurrentURL").ToString();
                if (PageAssemblyContext.CurrentDisplayVersion != DisplayVersions.ViewAll)
                    //viewAllUrl = viewAllUrl.Substring(0, viewAllUrl.LastIndexOf("/")) + "/AllPages" ;
                    viewAllUrl = viewAllUrl + "/AllPages";

                url.SetUrl(viewAllUrl);
            });

            AddUrlFilter("PrintAll", (name, url) =>
            {
                url.SetUrl(GetUrl("ViewAll").ToString() + "#");
            });

            #endregion

            #region Previous and Next rel link URLs (default to '#', set by pager app modules)

            AddUrlFilter("RelNext", (name, url) =>
            {
                url.Clear();
            });

            AddUrlFilter("RelPrev", (name, url) =>
            {
                url.Clear();
            });

            #endregion

            //AddUrlFilter("PostBackURL", (name, url) =>
            //{
            //    url.SetUrl(GetUrl("CurrentURL").ToString() + "?" + HttpContext.Current.Request.QueryString);
            //});
        }

        /// <summary>
        /// Override this method to add any page specifc web analytics data points.
        /// </summary>
        protected override void RegisterWebAnalyticsFieldFilters()
        {
            base.RegisterWebAnalyticsFieldFilters();

            if (PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.ViewAll)
            {
                SetWebAnalytics(WebAnalyticsOptions.Events.event8, wbField =>
                {
                    wbField.Value = "";
                });
            }
        }

        #endregion
    }
}


