﻿using System;
using System.Web.UI;
using NCI.Util;
using NCI.Web.CDE.UI;

namespace CancerGov.Web.SnippetTemplates
{
    public partial class DrugDictionaryRouter : SnippetControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String searchString = Strings.Clean(Request.QueryString["search"]);
            String term = Strings.Clean(Request.QueryString["term"]);
            String cdrId = Strings.Clean(Request.QueryString["cdrid"]);
            String id = Strings.Clean(Request.QueryString["id"]);
            // default results to 'A' if no term chosen
            String expand = Strings.Clean(Request.QueryString["expand"], "A");
            String language = Strings.Clean(Request.QueryString["language"]);
            Control localControl = null;

            if (!String.IsNullOrEmpty(term))
            {
                searchString = term;
            }

            // Load appropriate control 
            if (!String.IsNullOrEmpty(searchString))
                localControl = Page.LoadControl("~/SnippetTemplates/DrugDictionary/Views/DrugDictionaryResultsList.ascx");
            else if (!String.IsNullOrEmpty(cdrId) || !String.IsNullOrEmpty(id))
                localControl = Page.LoadControl("~/SnippetTemplates/DrugDictionary/Views/DrugDictionaryDefinitionView.ascx");
            else if (!String.IsNullOrEmpty(expand))
                localControl = Page.LoadControl("~/SnippetTemplates/DrugDictionary/Views/DrugDictionaryResultsList.ascx");
            else
                localControl = Page.LoadControl("~/SnippetTemplates/DrugDictionary/Views/DrugDictionaryHome.ascx");
                       
            if (localControl != null)
                phTermDictionary.Controls.Add(localControl);
        }
    }
}