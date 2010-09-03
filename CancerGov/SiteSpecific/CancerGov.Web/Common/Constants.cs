﻿using System;

namespace CancerGov.Web
{
    /// <summary>
    /// Used with PopEmail.aspx to specify where the "Email this Page"
    /// pop up was invoked from.
    /// </summary>
    public enum EmailPopupInvokedBy
    {
        Unspecified = 0,
        ClinicalTrialSearchResults = 1,
        ClinicalTrialPrintableSearchResults = 2
    }

    public enum DisplayVersion
    {
        Image = 1,
        [Obsolete("Text-only is dead.  Long live text-only!")]
        Text = 2,
        Print = 3
    }

    public enum DisplayLanguage
    {
        English = 1,
        Spanish = 2
    }

    public struct DisplayInformation
    {
        public DisplayVersion Version;
        public DisplayLanguage Language;
    }
}