﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCI.Web;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Class is a factory that when given a URL will return a populated CTSSearchParam object
    /// </summary>
    public class CTSSearchParamFactory
    {
        ///Delegate definition so we can more cleanly list the parsers we will call.
        private delegate void ParameterParserDelegate(NciUrl url, CTSSearchParams searchParams);


        private ITerminologyLookupService _lookupSvc;
        private ParameterParserDelegate _parsers;

        /// <summary>
        /// Creates new instance of a search param factory
        /// </summary>
        /// <param name="lookupSvc">An instance of a ITerminologyLookupService </param>
        public CTSSearchParamFactory(ITerminologyLookupService lookupSvc)
        {
            this._lookupSvc = lookupSvc;

            //Add parser methods here
            this._parsers = 
                (ParameterParserDelegate) ParseKeyword + //First param needs the cast.
                ParseCancerType + ParseCity +
                ParseSubTypes;
        }

        /// <summary>
        /// Gets an instance of a CTSSearchParams object based on params in URL.
        /// </summary>
        /// <param name="url">The URL to parse</param>
        /// <returns></returns>
        public CTSSearchParams Create(string url)
        {
            CTSSearchParams rtnParams = new CTSSearchParams();

            NciUrl reqUrl = new NciUrl();
            reqUrl.SetUrl(url);

            _parsers(reqUrl, rtnParams); //This calls each of the parsers, one chained after another.
            

            return rtnParams; 
        }

        #region Parameter Parsers 

        //Parameter q
        private void ParseKeyword(NciUrl url, CTSSearchParams searchParams)
        {
            //TODO: Handle lowercase
            if (url.QueryParameters.ContainsKey("q"))
            {
                //TODO: Clean Param
                searchParams.Phrase = url.QueryParameters["q"];                
            }
        }
        
        //Parameter t
        private void ParseCancerType(NciUrl url, CTSSearchParams searchParms)
        {
            //TODO: Extra credit, refactor the term extraction logic so it does not get repeated for each type
            //TODO: Handle Lowercase
            if (url.QueryParameters.ContainsKey("t"))
            {
                TerminologyFieldSearchParam[] terms = GetTermFieldFromParam(url.QueryParameters["t"]);
                if (terms.Length == 1)
                {
                    searchParms.MainType = terms[0];
                }
                else if (terms.Length > 1)
                {
                    //Add error??
                }
            }
        }

        //Parameter st
        private void ParseSubTypes(NciUrl url, CTSSearchParams searchParms)
        {
            //TODO: Extra credit, refactor the term extraction logic so it does not get repeated for each type
            //TODO: Handle Lowercase
            if (url.QueryParameters.ContainsKey("st"))
            {
                searchParms.SubTypes = GetTermFieldFromParam(url.QueryParameters["st"]);
            }
        }

        //Parameter lcty
        private void ParseCity(NciUrl url, CTSSearchParams searchParams)
        {
            //TODO: Handle lowercase
            if (url.QueryParameters.ContainsKey("lcty"))
            {
                //TODO: Clean Param
                searchParams.City = url.QueryParameters["lcty"];
            }
        }

        private TerminologyFieldSearchParam[] GetTermFieldFromParam(string paramData)
        {
            List<TerminologyFieldSearchParam> rtnParams = new List<TerminologyFieldSearchParam>();

            TerminologyFieldSearchParam type = new TerminologyFieldSearchParam();

            //TODO: Handle validating codes, handling multiple codes, etc.
            type.Codes = new string[] { paramData };
            type.Label = this._lookupSvc.GetTitleCase(String.Join(",", type.Codes));

            rtnParams.Add(type);

            return rtnParams.ToArray();
        }

        #endregion

    }
}
