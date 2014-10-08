﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

using NCI.Logging;
using NCI.Text;


namespace NCI.Web.CDE.UI.SnippetControls
{
    public class BreadCrumbSnippet : SnippetControl
    {
        // Get root path from SectionDetails.xml
        public string _breadcrumbData = String.Empty;
        public string BreadcrumbData
        {
            get { return _breadcrumbData; }
            set { _breadcrumbData = value; }
        }

        public string GetRootPath()
        {
            if (SnippetInfo.SlotName == "cgvSlBreadcrumb")
            {
                BreadcrumbData = SnippetInfo.Data;
            }
            return BreadcrumbData;
        }

        protected string RootPath { get { return GetRootPath(); } }
        protected string CurrUrl { get { return PageAssemblyContext.Current.PageAssemblyInstruction.GetUrl("PrettyUrl").ToString(); } }

        public override void RenderControl(HtmlTextWriter writer)
        {

            string sectionPath = PageAssemblyContext.Current.PageAssemblyInstruction.SectionPath;
            SectionDetail details = SectionDetailFactory.GetSectionDetail(sectionPath);

            if (details == null)
            {
                NCI.Logging.Logger.LogError("BreadCrumbSnippet", "Section detail cannot be null.", NCIErrorLevel.Error);
                return;
            }

            //if the current page is the landing page of the root navon for the breadcrumbs, I.E. you are viewing the page that would be the root, then DO NOT DRAW ANYTHING.
            if (RootPath == details.ParentPath && details.LandingPageURL == CurrUrl)
            {
                return;
            }

            //Opening UL tag 
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "breadcrumbs");
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);

            //Draw parents
            RenderBreadcrumbSections(details, writer);

            //Draw this item if not the landing page of the folder
            if ((details.LandingPageURL != CurrUrl) && !String.IsNullOrEmpty(details.NavTitle))
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.Write(PageAssemblyContext.Current.PageAssemblyInstruction.GetField("short_title"));
                writer.RenderEndTag();
            }

            writer.RenderEndTag();
        }

        private void RenderBreadcrumbSections(SectionDetail section, HtmlTextWriter writer)
        {
            /*
             * Need to add FullPath to section details... this should combine ParentPath with SectionName (including a path separator between them)
             * Basically we need to check if we should draw this section in the bread crumbs, and the reason we would is because this
             * section is within the section that is the root of the navon.  Luckily we can do this with string comparisons...
            */
            // if (The current "section" is within the folder structure of the RootPath of the breadcrumb)
            if (RootPath != null &&
                System.Text.RegularExpressions.Regex.IsMatch(RootPath, section.FullPath, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            {
                //If the section has a parent, attempt to draw it first.        
                if (section.ParentPath != null)
                {
                    RenderBreadcrumbSections(section.Parent, writer);
                }

                //If the LandingPageURL is not set, DO NOT DRAW A BAD LINK!!!
                if (!String.IsNullOrEmpty(section.LandingPageURL) &&
                    !String.IsNullOrEmpty(section.NavTitle))
                {
                    //Draw this item
                    if (section.LandingPageURL == CurrUrl)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Li);
                        writer.Write(" > ");
                        writer.Write(section.NavTitle);
                        writer.RenderEndTag();
                    }
                    else
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Li);
                        writer.AddAttribute(HtmlTextWriterAttribute.Href, section.LandingPageURL);
                        writer.RenderBeginTag(HtmlTextWriterTag.A);
                        writer.Write(section.NavTitle);
                        writer.RenderEndTag();
                        writer.Write(" > ");
                        writer.RenderEndTag();
                    }
                }
            }
            else
            {
                return;
            }

        } // RenderBreadCrumbSections()
    } // BreadCrumbSnippet class
}

