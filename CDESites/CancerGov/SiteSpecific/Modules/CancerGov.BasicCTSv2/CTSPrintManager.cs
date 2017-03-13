﻿using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using NCI.Web.CDE.Application;
using CancerGov.DataManager;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public class CTSPrintManager
    {
        public string GetPrintContent(Guid printID)
        {
            // To do: add logic for determining isLive

            string printContent = CTSPrintDataManager.Execute(printID, true);

            return printContent;
        }

    }
}
