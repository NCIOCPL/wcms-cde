﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic
{
    /// <summary>
    /// Represents the Parameters of a Cancer Type Search
    /// </summary>
    public class CancerTypeSearchParam : BaseCTSSearchParam
    {
        /// <summary>
        /// Get and Set the Cancer Type ID for this search
        /// </summary>
        public string CancerTypeID { get; set; }

        /// <summary>
        /// Gets and Sets the cancer type display name to use for this search
        /// </summary>
        public string CancerTypeDisplayName { get; set; }

        protected override void AddTemplateParams(Nest.FluentDictionary<string, object> paramdict)
        {

            // Set the cancertypeid only if we have one.  Maybe clean it up too if needbe.
            if (!String.IsNullOrWhiteSpace(CancerTypeID))
                paramdict.Add("cancertypeid", this.CancerTypeID);

            // Set the cancertypedisplayname only if we have one.  Maybe clean it up too if needbe.
            if (!String.IsNullOrWhiteSpace(CancerTypeID))
                paramdict.Add("cancertypedisplayname", this.CancerTypeDisplayName);

        }

        protected override Nest.SearchTemplateDescriptor<T> ModifySearchParams<T>(Nest.SearchTemplateDescriptor<T> descriptor)
        {
            return descriptor; 
        }
    }
}