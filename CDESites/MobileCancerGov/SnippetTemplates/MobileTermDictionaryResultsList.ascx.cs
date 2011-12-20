﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using MobileCancerGov.Web.SnippetTemplates;
using NCI.Web.CDE.UI;
using NCI.Web.CDE.WebAnalytics;
using CancerGov.CDR.TermDictionary;

namespace MobileCancerGov.Web.SnippetTemplates
{
    public partial class MobileTermDictionaryResultsList : SnippetControl
    {
        private string _dictionaryURL = "dictionaryURL/";
        private string _queryStringLanguage = "English";
        private int _numResults = 0;

        public string DictionaryURL
        {
            get { return _dictionaryURL; }
            set { _dictionaryURL = value; }
        }
        public int NumResults
        {
            get { return _numResults; }
            set { _numResults = value; }
        }
        public string QueryStringLang
        {
            get { return _queryStringLanguage; }
            set { _queryStringLanguage = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            azLink.HRef = Page.Request.Url.LocalPath;
            String searchStr = Request.QueryString["searchString"];
            String expand = Request.QueryString["expand"];

            TermDictionaryCollection dataCollection = null;
            if (!String.IsNullOrEmpty(searchStr))
            {
                searchString.Value = searchStr;
                dataCollection = TermDictionaryManager.Search("English", searchStr, 0, true);
            }
            else if(!String.IsNullOrEmpty(expand))
            {
                searchString.Value = "";
                dataCollection = TermDictionaryManager.Search("English", expand, 0, false);
            }

            if (dataCollection != null)
            {
                //TODO: check for lack of searchString
                if (dataCollection.Count < 1)
                {
                }
                else if (dataCollection.Count == 1) //if there is only 1 record - go directly to definition view
                {
                }
                else
                {
                    resultListView.DataSource = dataCollection;
                    resultListView.DataBind();
                    NumResults = dataCollection.Count;
                }
            }
        }

        protected string ResultListViewHrefOnclick(ListViewDataItem dataItem)
        {
            if (WebAnalyticsOptions.IsEnabled)
                return "onclick=\"NCIAnalytics.TermsDictionaryResults(this,'" + (dataItem.DataItemIndex + 1).ToString() + "');\""; // Load results onclick script
            else
                return "";
        }
        protected string LimitText(ListViewDataItem item, int numberOfCharacters)
        {
            const int MAX_WALKBACK = 30;
            string definition = DataBinder.Eval(item.DataItem, "DefinitionHTML").ToString();

            if (definition.Length > numberOfCharacters)
            {
                if (definition.Substring(numberOfCharacters - 1, 1) == " ")
                    definition = definition.Substring(0, numberOfCharacters - 1) + "...";
                else //walk back to next space
                {
                    int i;
                    for (i = numberOfCharacters; i > 0; i--)
                    {
                        if(i == MAX_WALKBACK)
                        {
                            i=0;
                            break;
                        }
                        if (definition.Substring(i, 1) == " ")
                            break;
                    }
                    if (i <= 0)
                        definition = definition.Substring(0, numberOfCharacters - 1) + "...";
                    else
                        definition = definition.Substring(0, i) + "...";
                }
            }

            return definition;
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            Page.Response.Redirect(Page.Request.Url.LocalPath + "?SearchString=" + searchString.Value.Trim().ToString());
        }
    }
}