﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Linq;
using NCI.Util;
using NCI.Web;
using NCI.Web.CDE.UI;

namespace CancerGov.Dictionaries.SnippetControls
{
    public class TermDictionaryRouter : BaseDictionaryRouter
    {
        protected Control localControl;

        protected override Control LoadHomeControl()
        {
            localControl = Page.LoadControl("~/SnippetTemplates/TermDictionary/Views/TermDictionaryHome.ascx");
            return localControl;
        }

        protected override Control LoadResultsListControl()
        {
            localControl = Page.LoadControl("~/SnippetTemplates/TermDictionary/Views/TermDictionaryResultsList.ascx");
            return localControl;
        }

        protected override Control LoadDefinitionViewControl()
        {
            localControl = Page.LoadControl("~/SnippetTemplates/TermDictionary/Views/TermDictionaryDefinitionView.ascx");
            return localControl;
        }
    }
}