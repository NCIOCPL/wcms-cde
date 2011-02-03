﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NCI.Web.CDE;
using NCI.Web.CDE.UI;

namespace CancerGov.Web.SnippetTemplates
{
    public partial class DocTitleBlock : SnippetControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //Parse Data To Get Information
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(this.SnippetInfo.Data);

                XmlNode xnTitle = doc.SelectSingleNode("//Title");
                XmlNode titleDisplay = doc.SelectSingleNode("//TitleDisplay");
                XmlNode imageUrl = doc.SelectSingleNode("//ImageUrl");

                String title = PageAssemblyContext.Current.PageAssemblyInstruction.GetField("long_title");

                if (titleDisplay != null)
                {
                    switch (titleDisplay.Value)
                    {
                        case "DocTitleBlockTitle":
                            {
                                if (xnTitle != null)
                                {
                                    title = xnTitle.Value;
                                }
                            }
                            break;
                    }
                }

                if (imageUrl != null)
                {
                    imgImage.ImageUrl = imageUrl.Value;
                }

                if (PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.Print ||
                    PageAssemblyContext.CurrentDisplayVersion == DisplayVersions.PrintAll)
                {
                    phPrint.Visible = true;
                    litPrintTitle.Text = title;
                }
                else
                {
                    phWeb.Visible = true;
                    litTitle.Text = title;
                }

            }
            catch (Exception ex)
            {
                //Should have logging...
            }
        }
    }
}