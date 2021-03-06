﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Logging;
using NCI.Web.CDE.WebAnalytics.Configuration;

namespace NCI.Web.CDE.WebAnalytics
{
    public static class WebAnalyticsOptions
    {
        static ILog log = LogManager.GetLogger(typeof(WebAnalyticsOptions));

        private static readonly string webAnalyticsPath = "nci/web/analytics";
        private static WebAnalyticsSection _webAnalyticsConfig;
        private static Dictionary<string, UrlPathChannelElement> urlPathChannelLookUp = new Dictionary<string, UrlPathChannelElement>();
        private static Dictionary<string, UrlPathChannelElement> urlPathChannelWithUrlMatchLookUp = new Dictionary<string, UrlPathChannelElement>();

        public static void Initialize()
        {
            if (_webAnalyticsConfig == null)
            {
                _webAnalyticsConfig = (WebAnalyticsSection)ConfigurationManager.GetSection(webAnalyticsPath);

                // When the analytics sectionGroup isn't defined, _webAnalyticsConfig comes back null.
                if (_webAnalyticsConfig == null)
                {
                    throw new ConfigurationErrorsException(String.Format("Could not load web analytics configuration from {0}.  Please ensure the web analytics section group and configuration element have been defined in the configuration file.", webAnalyticsPath));
                }

                if (_webAnalyticsConfig.UrlPathChannelMappings != null)
                {
                    var urlPathChannelMappings = from urlPathChannel in _webAnalyticsConfig.UrlPathChannelMappings.Cast<UrlPathChannelElement>()
                                                 orderby urlPathChannel.UrlPath descending
                                                 select urlPathChannel;

                    foreach (UrlPathChannelElement urlPathChannelElement in urlPathChannelMappings)
                    {
                        string key = urlPathChannelElement.UrlPath.ToLower();
                        if (string.IsNullOrEmpty(urlPathChannelElement.UrlMatch))
                        {
                            if (!urlPathChannelLookUp.ContainsKey(key))
                                urlPathChannelLookUp.Add(key, urlPathChannelElement);
                        }
                        else
                        {
                            if (!urlPathChannelWithUrlMatchLookUp.ContainsKey(key))
                                urlPathChannelWithUrlMatchLookUp.Add(key, urlPathChannelElement);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This property determines if web analytics is enabled.
        /// Returns the value set for IsEnabled from the configuration. 
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                WebAnalyticsOptions.Initialize();

                if (_webAnalyticsConfig == null)
                    return false;
                else
                    return _webAnalyticsConfig.Enabled;
            }
        }

        /// <summary>
        /// This property determines if web analytics EnableNonJavaScriptTagging is enabled.
        /// </summary>
        public static bool EnableNonJavaScriptTagging
        {
            get
            {
                WebAnalyticsOptions.Initialize();

                if (_webAnalyticsConfig == null)
                    return false;
                else
                    return _webAnalyticsConfig.EnableNonJavaScriptTagging;
            }
        }

        /// <summary>
        /// This method returns a report suite set in the section detail. If the current section
        /// does not have a suite value set, recurse through parents until a suite 
        /// value is found. Suites set on a loweer folder overwrite parents.
        /// </summary>
        /// <param name="details">SectionDetail object.</param>
        /// <returns>The report suite name to be passed to analytics.</returns>
        public static string GetReportSuitesFromSectionDetail(SectionDetail detail)
        {
            try
            {
                string suites = detail.WebAnalyticsInfo.WAReportSuites;
                if (String.IsNullOrEmpty(suites))
                {
                    suites = GetReportSuitesFromSectionDetail(detail.Parent);
                }
                return suites;
            }
            catch (Exception ex)
            {
                log.Warn("GetReportSuitesFromSectionDetail(): Exception encountered while retrieving web analytics suites.", ex);
                return "";
            }
        }

        /// <summary>
        /// This method returns a list of all reporting suites for a given channel.
        /// </summary>
        /// <param name="channelName">The name of the channel</param>
        /// <param name="language">The language of the related suite.</param>
        /// <returns>A string[] containing reporting suite.</returns>
        public static string[] GetSuitesForChannel(string channelName, string language)
        {
            List<string> rtnSuites = new List<string>();

            WebAnalyticsOptions.Initialize();

            if (_webAnalyticsConfig != null)
            {
                var suites = from reportingSuite in _webAnalyticsConfig.ReportingSuites.Cast<ReportingSuiteElement>()
                             where
                                (((reportingSuite.Language == "") || (reportingSuite.Language == language)) &&
                                (reportingSuite.EnabledForAllChannels ||
                                 //When looking for channel make it case insensitive.
                                reportingSuite.Channels.Cast<ChannelElement>().Any(c => string.Compare(c.Name, channelName, true) == 0)))
                             select reportingSuite.Name;

                rtnSuites.AddRange(suites);
            }

            return rtnSuites.ToArray();
        }

        /// <summary>
        /// This method returns a list of all special page load function for a given channel.
        /// </summary>
        /// <param name="channelName">The name of the channel</param>
        /// <param name="language">The language of the related suite.</param>
        /// <returns>A string[] containing reporting suite.</returns>
        public static string[] GetSpecialPageLoadFunctionsForChannel(string channelName, string language)
        {
            List<string> rtnFunctions = new List<string>();

            WebAnalyticsOptions.Initialize();

            if (_webAnalyticsConfig != null)
            {
                var functions = from reportingSuite in _webAnalyticsConfig.ReportingSuites.Cast<ReportingSuiteElement>()
                                where
                                   (((reportingSuite.Language == "") || (reportingSuite.Language == language)) &&
                                   (reportingSuite.EnabledForAllChannels ||
                                    //When looking for channel make it case insensitive.
                                   reportingSuite.Channels.Cast<ChannelElement>().Any(c => string.Compare(c.Name, channelName, true) == 0)))
                                select reportingSuite.SpecialPageLoadFunctions;

                rtnFunctions.AddRange(functions);
            }

            return rtnFunctions.ToArray();
        }

        /// <summary>
        /// This method returns a channel name set in the section detail. If the current section
        /// does not have a channel value set, recurse through parents until a channel 
        /// value is found. Channels set on a loweer folder overwrite parents' channels.
        /// </summary>
        /// <param name="details">SectionDetail object.</param>
        /// <returns>The channel name to be passed to analytics.</returns>
        public static string GetChannelsFromSectionDetail(SectionDetail detail)
        {
            try
            {
                string channels = detail.WebAnalyticsInfo.WAChannels;
                if (String.IsNullOrEmpty(channels))
                {
                    channels = GetChannelsFromSectionDetail(detail.Parent);
                }
                return channels;
            }
            catch (Exception ex)
            {
                log.Debug("GetChannelsFromSectionDetail(): Error retrieving Web Analytics channel. Check analytics values in section details.", ex);
                return "";
            }
        }

        /// <summary>
        /// This method returns a channel name for a given folderpath. It also matches occurence
        /// of certain text in the url.
        /// </summary>
        /// <param name="urlFolderPath">This value contains the complete url</param>
        /// <returns>The channel name which matches the urlFolderPath.</returns>
        public static string GetChannelForUrlPath(string urlFolderPath)
        {
            try
            {
                WebAnalyticsOptions.Initialize();

                if (_webAnalyticsConfig.UrlPathChannelMappings != null)
                {
                    string url = urlFolderPath.Substring(urlFolderPath.LastIndexOf('/') + 1).ToLower();
                    int urlDelimiter = urlFolderPath.LastIndexOf('/');
                    string currUrlPath = urlFolderPath;
                    while (!string.IsNullOrEmpty(currUrlPath))
                    {

                        var urlPathWithUrlChannelMappings = from urlPathChannel in urlPathChannelWithUrlMatchLookUp
                                                            where urlPathChannel.Key == currUrlPath && urlFolderPath.Contains(((UrlPathChannelElement)urlPathChannel.Value).UrlMatch)
                                                            select urlPathChannel;
                        if (urlPathWithUrlChannelMappings.Count() != 0)
                            return ((UrlPathChannelElement)urlPathWithUrlChannelMappings.FirstOrDefault().Value).ChannelName;

                        var urlPathChannelMappings = from urlPathChannel in urlPathChannelLookUp
                                                     where urlPathChannel.Key == currUrlPath
                                                     select urlPathChannel;

                        if (urlPathChannelMappings.Count() != 0)
                            return ((UrlPathChannelElement)urlPathChannelMappings.FirstOrDefault().Value).ChannelName;

                        // did not find, backtrack the url path to find the matching elements
                        urlDelimiter = currUrlPath.LastIndexOf('/');

                        if (currUrlPath == "/" && urlDelimiter == -1)
                        {
                            // shoud never come here, there should be the minumum '/' mapping in the web.config
                            currUrlPath = String.Empty;
                            continue;
                        }

                        currUrlPath = currUrlPath.Substring(0, urlDelimiter).ToLower();

                        //Reached the beginning of the url path or the request is for the home page.
                        //when this happens set the path '/' so the lookup can succeed. This 
                        //is the final fall back.
                        if (string.IsNullOrEmpty(currUrlPath))
                            currUrlPath = "/";
                    }

                    // if it reaches here then, no mapping could be found.
                    log.InfoFormat("GetChannelForUrlPath(): No channel mapping exists for pretty url: {0}", urlFolderPath);
                }
                else
                    log.Info("GetChannelForUrlPath(): Url to channel mapping information not present in config file.");
            }
            catch (Exception ex)
            {
                log.Error("GetChannelForUrlPath(): Failed to process url to channel mapping", ex);
            }

            // Should never be executed.
            return "";
        }

        public static class Language
        {
            public static readonly string Spanish = "spanish";
            public static readonly string English = "english";
        }

        /// <summary>
        /// Named constants for Event data point types -
        /// "Success Events" in Adobe/Omniture
        /// </summary>
        public enum Events
        {
            event1 = 1,		// Page View Event
            event2 = 2,		// Used Search Tool
            event3 = 3,		// Clicked Dictionary Widget Link
            event4 = 4,		// Viewed Clinical Trial
            event5 = 5,		// ? About Cancer Footer
            event6 = 6,		// Downloaded PDF Version
            event7 = 7,		// Emailed Link
            event8 = 8,		// Clicked All Pages Option
            event9 = 9,		// Subscribed to Bulletin
            event10 = 10, 	// Best Bets Impression
            event11 = 11, 	// Viewed Dictionary of Cancer Term
            event12 = 12,	// Viewed Drug Dictionary Term
            event13 = 13,	// Viewed Clinical Trial (non-search)
            event14 = 14,	// Sent to Printer
            event15 = 15,	// Launched Clinical Trial Course
            event16 = 16,	// Clicked Header Footer Link
            event17 = 17,	// Clicked Bookmark & Share
            event18 = 18,	// Questions About Cancer Header
            event19 = 19,	// Find Cancer Type Box
            event20 = 20,	// Clicked a Tile
            event21 = 21,	// Clicked Page Link
            event22 = 22,	// Downloaded Kindle
            event23 = 23,	// Downloaded Other eReader
            event24 = 24,	// Shopping Cart Begin
            event25 = 25,	// Shopping Cart End
            event26 = 26,	// Mega Menu Click
            event27 = 27,	// Card Click
			event28 = 28,	// Events 28-92 are unassigned Custom Events
			event29 = 29,
			event30 = 30,
			event31 = 31,
			event32 = 32,
			event33 = 33,
			event34 = 34,
			event35 = 35,
			event36 = 36,
			event37 = 37,
			event38 = 38,
			event39 = 39,
			event40 = 40,
			event41 = 41,
			event42 = 42,
			event43 = 43,
			event44 = 44,
			event45 = 45,
			event46 = 46,
			event47 = 47,
			event48 = 48,
			event49 = 49,
			event50 = 50,
			event51 = 51,
			event52 = 52,
			event53 = 53,
			event54 = 54,
			event55 = 55,
			event56 = 56,
			event57 = 57,
			event58 = 58,
			event59 = 59,
			event60 = 60,
			event61 = 61,
			event62 = 62,
			event63 = 63,
			event64 = 64,
			event65 = 65,
			event66 = 66,
			event67 = 67,
			event68 = 68,
			event69 = 69,
			event70 = 70,
			event71 = 71,
			event72 = 72,
			event73 = 73,
			event74 = 74,
			event75 = 75,
			event76 = 76,
			event77 = 77,
			event78 = 78,
			event79 = 79,
			event80 = 80,
			event81 = 81,
			event82 = 82,
			event83 = 83,
			event84 = 84,
			event85 = 85,
			event86 = 86,
			event87 = 87,
			event88 = 88,
			event89 = 89,
			event90 = 90,
			event91 = 91,
			event92 = 92,
            event93 = 93, 	// Mentions
            event94 = 94,	// Potential Audience
            event95 = 95,	// Total Sentiment
            event96 = 96,	// Like Adds (Social Properties)
            event97 = 97,	// Page Views (Social Properties)
            event98 = 98,	// Post Views (Social Properties)
            event99 = 99,	// People Talking About This (Social Properties)
            event100 = 100	// Interactions (Social Properties)
        }

        /// <summary>
        /// Named constants for props data point types - 
        /// "Traffic Variables" in Adobe/Omniture
        /// </summary>
        public enum Props
        {
            prop1 = 1,	 // Full URL 1-100
            prop2 = 2,	 // Full URL 101-200
            prop3 = 3,	 // Root Pretty URL
            prop4 = 4,	 // Page Links
            prop5 = 5,	 // Links + Pg Name
            prop6 = 6,	 // Short Title
            prop7 = 7,	 // Health Professional vs. Patient
            prop8 = 8,	 // Language
            prop9 = 9,	 // Top Navigation Section
            prop10 = 10, // Title from Browser
            prop11 = 11, // Search Type
            prop12 = 12, // Search Module
            prop13 = 13, // Search Result Rank
            prop14 = 14, // General Site Search Keyword
            prop15 = 15, // Protocol Search ID
            prop16 = 16, // CDR ID
            prop17 = 17, // Cancer Type/Condition
            prop18 = 18, // Location
            prop19 = 19, // Trial/Treatment Type
            prop20 = 20, // Trial Status/Phase
            prop21 = 21, // Trial ID/Sponsor
            prop22 = 22, // Boutique Search Keywords
            prop23 = 23, // Specialty
            prop24 = 24, // Starts With/Contains
            prop25 = 25, // Release Date
            prop26 = 26, // Time Stamp
            prop27 = 27, // Right Navigation Section Clicked
            prop28 = 28, // Link Tracking Label
            prop29 = 29, // Logged In User
            prop30 = 30, // Sub-section
            prop31 = 31, // NCI Country
            prop32 = 32, // NCI Country:Region:City
            prop33 = 33, // NCI US DMA
            prop34 = 34, // NCI US State
            prop35 = 35, // VISTA US DMA (p35)
            prop36 = 36, // Header Footer Link
            prop37 = 37, // Timely Content Zone Tab Title
            prop38 = 38, // Timely Content Zone Link Title
            prop39 = 39, // Timely Content Zone Link URL
            prop40 = 40, // Timely Content Zone Panel Title
            prop41 = 41, // Tile Carousel Tile Title
            prop42 = 42, // Tile Carousel Tile URL
            prop43 = 43, // Print/Share/Email Action
            prop44 = 44, // Topic
            prop45 = 45, // Sub-topic
            prop46 = 46, // Publication Title
            prop47 = 47, // Widget Name
            prop48 = 48, // Domain + Widget Name
            prop49 = 49, // Widget Host Page Name
            prop50 = 50, // Download Link Text
            prop51 = 51, // Download File
            prop52 = 52, // Download Page
            prop53 = 53, // Top Nav Section Clicked
            prop54 = 54, // Top Nav Sub Section Clicked
            prop55 = 55, // Top Nav Link Clicked
            prop56 = 56, // Top Nav URL Clicked From
            prop57 = 57, // Card Title
            prop58 = 58, // Card Link Text
            prop59 = 59, // Card Type and Position
            prop60 = 60, // Card URL Clicked From
            prop61 = 61, // Previous Page Value
            prop62 = 62, // Content Type
            prop63 = 63, // Content Sub-type
			prop64 = 64, // Custom Insight 64
            prop65 = 65, // Custom Insight 65
            prop66 = 66, // Custom Insight 66
            prop67 = 67, // Custom Insight 67
            prop68 = 68, // Custom Insight 68
            prop69 = 69, // Custom Insight 69
            prop70 = 70, // Custom Insight 70
            prop71 = 71, // Custom Insight 71
            prop72 = 72, // Custom Insight 72
            prop73 = 73, // Custom Insight 73
            prop74 = 74, // Custom Insight 74
            prop75 = 75, // Custom Insight 75
			prop76 = 76, // props 76-100 are unassigned Custom props
			prop77 = 77,
			prop78 = 78,
			prop79 = 79,
			prop80 = 80,
			prop81 = 81,
			prop82 = 82,
			prop83 = 83,
			prop84 = 84,
			prop85 = 85,
			prop86 = 86,
			prop87 = 87,
			prop88 = 88,
			prop89 = 89,
			prop90 = 90,
			prop91 = 91,
			prop92 = 92,
			prop93 = 93,
			prop94 = 94,
			prop95 = 95,
			prop96 = 96,
			prop97 = 97,
			prop98 = 98,
			prop99 = 99,
			prop100 = 100
		}

        /// <summary>
        /// Named constants for eVars data point types - 
        /// "Conversion Variables" in Adobe/Omniture
        /// </summary>
        public enum eVars
        {
            evar1 = 1,	 // Page
            evar2 = 2,	 // Language
            evar3 = 3,	 // Recent Topic
            evar4 = 4,	 // Linear Topic
            evar5 = 5,	 // Page Links
            evar6 = 6,	 // Links + Pg Name
            evar7 = 7,	 // Health Professional vs. Patient
            evar8 = 8,	 // Logged In User
            evar9 = 9,	 // Commerce Variable 9
            evar10 = 10, // Number of Search Results
            evar11 = 11, // Search Type
            evar12 = 12, // Search Module
            evar13 = 13, // Search Count
            evar14 = 14, // General Site Search Keyword
            evar15 = 15, // Protocol Search ID
            evar16 = 16, // CDR ID
            evar17 = 17, // Cancer Type/Condition
            evar18 = 18, // Location
            evar19 = 19, // Trial/Treatment Type
            evar20 = 20, // Trial Status/Phase
            evar21 = 21, // Trial ID/Sponsor
            evar22 = 22, // Clinical Trial View Count
            evar23 = 23, // Start Date
            evar24 = 24, // End Date
            evar25 = 25, // Specialty
            evar26 = 26, // Starts With/Contains
            evar27 = 27, // NCI Keyword
            evar28 = 28, // NCI Unified Sources
            evar29 = 29, // NCI Unified Sources Roll-Up
            evar30 = 30, // Article Download Counter
            evar31 = 31, // NCI Country
            evar32 = 32, // NCI Country:Region:City
            evar33 = 33, // NCI US DMA
            evar34 = 34, // NCI US State
            evar35 = 35, // NCI Unified Sources Campaign
            evar36 = 36, // Header Footer Link
            evar37 = 37, // Timely Content Zone Tab Title
            evar38 = 38, // Timely Content Zone Link Title
            evar39 = 39, // Timely Content Zone Link URL
            evar40 = 40, // Timely Content Zone Panel Title
            evar41 = 41, // Tile Carousel Tile Title
            evar42 = 42, // Tile Carousel Tile URL
            evar43 = 43, // Link Tracking Label
            evar44 = 44, // Content Group
            evar45 = 45, // Commerce Variable 45
            evar46 = 46, // Publication Title
            evar47 = 47, // Commerce Variable 47
            evar48 = 48, // Commerce Variable 48
            evar49 = 49, // Commerce Variable 49
            evar50 = 50, // Download Link Text
            evar51 = 51, // Download File
            evar52 = 52, // Download Page
            evar53 = 53, // Mega Menu Section
            evar54 = 54, // evars 54-72 are unassigned Custom eVars
            evar55 = 55,
            evar56 = 56,
            evar57 = 57,
            evar58 = 58,
            evar59 = 59,
            evar60 = 60,
            evar61 = 61,
            evar62 = 62,
            evar63 = 63,
			evar64 = 64,
			evar65 = 65,
			evar66 = 66,
			evar67 = 67,
			evar68 = 68,
			evar69 = 69,
			evar70 = 70,
			evar71 = 71,
			evar72 = 72,
            evar73 = 73, // Terms
            evar74 = 74, // Social Platforms / Properties
            evar75 = 75, // Authors
			evar76 = 76, // evars 76-100 are unassigned Custom eVars
			evar77 = 77,
			evar78 = 78,
			evar79 = 79,
			evar80 = 80,
			evar81 = 81,
			evar82 = 82,
			evar83 = 83,
			evar84 = 84,
			evar85 = 85,
			evar86 = 86,
			evar87 = 87,
			evar88 = 88,
			evar89 = 89,
			evar90 = 90,
			evar91 = 91,
			evar92 = 92,
			evar93 = 93,
			evar94 = 94,
			evar95 = 95,
			evar96 = 96,
			evar97 = 97,
			evar98 = 98,
			evar99 = 99,
			evar100 = 100
        }
    }
}