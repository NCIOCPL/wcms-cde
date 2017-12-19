﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

using NCI.Web.CDE;
using CancerGov.Dictionaries.Configuration;

namespace CancerGov.Dictionaries.SnippetControls
{
    public class BaseDictionaryControl : UserControl
    {
        public IPageAssemblyInstruction PageInstruction { get; set; }

        public DictionaryConfig DictionaryConfiguration { get; set; }

        public string GetFriendlyName(int cdrId)
        {
            string CDRID = cdrId.ToString();

            // Get CDRID to friendly name mappings
            string dictionaryMappingFilepath = null;

            dictionaryMappingFilepath = this.DictionaryConfiguration.Files.Single(a => a.Locale == PageAssemblyContext.Current.PageAssemblyInstruction.Language).Filepath;

            if (!string.IsNullOrEmpty(dictionaryMappingFilepath))
            {
                TerminologyMapping map = TerminologyMapping.GetMappingForFile(dictionaryMappingFilepath);

                // If pretty name is in label mappings, set CDRID
                if (map.MappingContainsCDRID(CDRID))
                {
                    return map.GetFriendlyNameFromCDRID(CDRID);
                }
            }

            return CDRID;
        }
    }
}
