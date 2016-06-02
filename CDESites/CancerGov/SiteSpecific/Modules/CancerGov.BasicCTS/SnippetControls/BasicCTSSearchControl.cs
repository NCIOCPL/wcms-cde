﻿using System;
using System.Web.UI;
using NCI.Web.CDE.Modules;

namespace CancerGov.ClinicalTrials.Basic.SnippetControls
{
    public partial class BasicCTSSearchControl : BasicCTSBaseControl
    {
        //private string _templatePath = "~/VelocityTemplates/BasicCTSSearch.vm";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LiteralControl ltl = new LiteralControl(VelocityTemplate.MergeTemplateWithResultsByFilepath(
                BasicCTSPageInfo.TemplatePath, new
                {
                    ResultsPagePrettyUrl = BasicCTSPageInfo.ResultsPagePrettyUrl
                }));
            Controls.Add(ltl);
        }
    }
}
