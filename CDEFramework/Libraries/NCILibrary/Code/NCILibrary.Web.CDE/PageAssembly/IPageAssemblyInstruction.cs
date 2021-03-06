﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using NCI.Web.CDE.WebAnalytics;

namespace NCI.Web.CDE
{
    /// <summary>
    /// IPageAssemblyInstruction interface provides access to the components,layout information and fields of a page published by percussion.
    /// </summary>
    public interface IPageAssemblyInstruction 
    {

        /// <summary>
        /// Gets the name of the page template i.e the actual aspx page to be loaded.
        /// </summary>
        /// <value>The name of the page template.</value>
        string PageTemplateName { get; }

        /// <summary>
        /// Gets a bool indicating if this page requires HTTP5 Push State.  This means that this
        /// PIA will be loaded for any requests to its PrettyURL, even if additional URL segments
        /// have been added.
        /// 
        /// For example, if this PIA has the PrettyURL of /foo, then a request for /foo/bar/bazz
        /// would load this page as well if this is true.  Otherwise it would return a 404 as normal.
        /// One thing to note, we do not have a great way to enforce that no other content should
        /// live under the url of /foo.  So technically a page could live at /foo/bar and 
        /// a user could request /foo/bar/bazz.  In that case, /foo/bar would be loaded, and if it
        /// does not implement push state, then a 404 would be returned even if /foo/bar did implement
        /// it.  This is just a performance thing -- we don't want to load every PIA along the path
        /// the user has requested.  So basically, don't add a PushState page and add sub folders 
        /// under that.
        /// </summary>
        bool ImplementsPushState { get; }

        /// <summary>
        /// Gets the template theme this page should be using.  (Set on a SectionDetails withing the parent folders of this page)
        /// </summary>
        /// <value>The name of the Template Theme</value>
        string TemplateTheme { get; }

        /// <summary>
        /// The path of all parent folders of the page assembly instruction.        
        /// </summary>
        string SectionPath { get; }
        /// <summary>
        /// Gets a collection of SnippetInfo objects for the page assembly instruction which are needed to render a page.
        /// </summary>
        IEnumerable<SnippetInfo> Snippets { get; }
        /// <summary>
        /// Provides the Metadata information
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        string GetField(string fieldName);

        /// <summary>
        /// Provides components of system with a URL for a page.
        /// </summary>
        /// <param name="urlType"></param>
        /// <returns></returns>
        NciUrl GetUrl(string urlType);

        /// <summary>
        /// Provides components of system with translation URLs for a page.
        /// </summary>
        /// <param name="urlType"></param>
        /// <returns></returns>
        NciUrl GetTranslationUrl(string urlType);

        /// <summary>        
        ///  When a component needs to modify the metadata of a page a field filter is added for that field name using AddFieldFilter.
        /// </summary>
        /// <param name="fieldname"></param>
        /// <param name="filter"></param>
        void AddFieldFilter(string fieldname, FieldFilterDelegate filter);

        /// <summary>
        ///  When a component needs to modify the URL of a page a URL filter is added for that URL name using AddUrlFilter.
        /// </summary>
        /// <param name="urlType"></param>
        /// <param name="fieldFilter"></param>
        void AddUrlFilter(string urlType, UrlFilterDelegate fieldFilter);

        /// <summary>
        ///  When a component needs to modify the URL of a page a URL filter is added for that URL name using AddTranslationFilter.
        /// </summary>
        /// <param name="urlType"></param>
        /// <param name="fieldFilter"></param>
        void AddTranslationFilter(string urlType, UrlFilterDelegate fieldFilter);

        /// <summary>
        /// Gets or sets the language for the page displayed.
        /// </summary>
        /// <value>The language.</value>
        string Language { get; set; }

        /// <summary>
        /// BlockedSlots contain information about the blocked slot which should not be displayed on the page rendered. 
        /// </summary>
        /// <value>The blocked slot names.</value>
        string[] BlockedSlotNames { get; }

        /// <summary>
        /// This property returns the keys which represent the available content versions. 
        /// </summary>
        /// <value>A string array which are the keys to the alternate content versions.</value>
        string[] AlternateContentVersionsKeys { get; }

        /// <summary>
        /// This property returns the keys which represent the available translations. 
        /// </summary>
        /// <value>A string array which are the keys to the alternate content versions.</value>
        string[] TranslationKeys { get; }

        /// <summary>
        /// This method returns the web analytics settings.
        /// </summary>
        WebAnalyticsSettings GetWebAnalytics();

        /// <summary>
        /// When a data point related to web anlytics is to be modified it is done using this method. 
        /// </summary>
        /// 

        void SetWebAnalytics(WebAnalyticsOptions.Events webAnalyticType, WebAnalyticsDataPointDelegate filter);
        void SetWebAnalytics(WebAnalyticsOptions.eVars webAnalyticType, WebAnalyticsDataPointDelegate filter);
        void SetWebAnalytics(WebAnalyticsOptions.Props webAnalyticType, WebAnalyticsDataPointDelegate filter);
        void Initialize();

        /// <summary>
        /// Provides a list of all SocialMetaTag objects defined for the current assembly.
        /// </summary>
        /// <returns>A potentially-empty array of SocialMetaTag objects.</returns>
        SocialMetaTag[] GetSocialMetaTags();

        /// <summary>
        /// Provides the PageResources object encapsulating CSS and JS for the page.
        /// </summary>
        /// <returns>A PageResources object.</returns>
        PageResources GetPageResources();

    }
}
