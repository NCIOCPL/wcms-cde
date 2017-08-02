﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCI.Web;
using System.Web;
using System.Text.RegularExpressions;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Class is a factory that when given a URL will return a populated CTSSearchParam object
    /// </summary>
    public class CTSSearchParamFactory
    {
        ///Delegate definition so we can more cleanly list the parsers we will call.
        private delegate void ParameterParserDelegate(NciUrl url, CTSSearchParams searchParams);
        private delegate void ParameterSerializerDelegate(NciUrl url, CTSSearchParams searchParams);

        private ITerminologyLookupService _lookupSvc;
        private ParameterParserDelegate _parsers;

        private static ParameterSerializerDelegate _basicParamSerializers;
        private static ParameterSerializerDelegate _advParamSerializers;

        /// <summary>
        /// Static constructor to initialize 
        /// </summary>
        static CTSSearchParamFactory() {
            _basicParamSerializers = 
                (ParameterSerializerDelegate) SerializeCancerType + //First param needs the cast.
                SerializeSubTypes +
                SerializeStages +
                SerializeAge +
                SerializeKeyword +
                SerializeGender +
                SerializeLocation + //Theoretically this should only ever end up Zip or None
                SerializeResultsLinkFlag;

            _advParamSerializers =
                (ParameterSerializerDelegate)SerializeCancerType + //First param needs the cast.
                SerializeSubTypes +
                SerializeStages +
                SerializeFindings +
                SerializeAge +
                SerializeKeyword +
                SerializeGender +
                SerializeLocation +
                SerializeDrugs +
                SerializeOtherTreatments +
                SerializeTrialIDs +
                SerializeInvestigator +
                SerializeLeadOrg +
                SerializeResultsLinkFlag;
        }

        /// <summary>
        /// Creates new instance of a search param factory
        /// </summary>
        /// <param name="lookupSvc">An instance of a ITerminologyLookupService </param>
        public CTSSearchParamFactory(ITerminologyLookupService lookupSvc)
        {
            this._lookupSvc = lookupSvc;

            //Add parser methods here
            this._parsers =
                (ParameterParserDelegate)ParseResultsLinkFlag + //First param needs the cast.
                ParseCancerType +
                ParseSubTypes +
                ParseStages +
                ParseFindings +
                ParseAge +
                ParseKeyword +
                ParseGender +
                ParseLocation +
                ParseTrialTypes +
                ParseDrugs +
                ParseOtherTreatments +
                ParseTrialIDs +
                ParseInvestigator +
                ParseLeadOrg;
        }
         
        /// <summary>
        /// Gets an instance of a CTSSearchParams object based on params in URL.
        /// </summary>
        /// <param name="url">The URL to parse</param>
        /// <returns></returns>
        public CTSSearchParams Create(string url)
        {
            CTSSearchParams rtnParams = new CTSSearchParams();

            NciUrl reqUrl = new NciUrl(true);
            reqUrl.SetUrl(HttpUtility.UrlDecode(url));

            // Get lowercase query params for parsing
            reqUrl = reqUrl.CopyWithLowerCaseQueryParams();

            _parsers(reqUrl, rtnParams); //This calls each of the parsers, one chained after another.

            return rtnParams; 
        }

        /// <summary>
        /// Serialize the parameters to a URL
        /// </summary>
        /// <param name="searchParams">The search parameters to serialize</param>
        /// <returns>A URL with query params.</returns>
        public static NciUrl ConvertParamsToUrl(CTSSearchParams searchParams)
        {
            NciUrl url = new NciUrl();

            switch (searchParams.ResultsLinkFlag)
            {
                case ResultsLinkType.Basic: { _basicParamSerializers(url, searchParams); break; }
                case ResultsLinkType.Advanced: { _advParamSerializers(url, searchParams); break; }
            }

            return url;
        }

        #region Param Serializers

        /// <summary>
        /// Converts a TerminologyFieldSearchParam[] to a string for serialization
        /// </summary>
        /// <param name="fieldValues">An array of TerminologyFieldSearchParam[]</param>
        /// <returns></returns>
        private static string SerializeMultiTermFields(TerminologyFieldSearchParam[] fieldValues)
        {
            List<string> codes = new List<string>();

            foreach (TerminologyFieldSearchParam termField in fieldValues)
            {
                codes.Add(string.Join("|", termField.Codes));
            }

            return string.Join(",", codes.ToArray());
        }

        //Parameter t (Main Cancer Type)
        private static void SerializeCancerType(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.MainType))
            {
                url.QueryParameters.Add("t", string.Join("|", searchParams.MainType.Codes));
            }
        }

        //Parameter st (SubTypes)
        private static void SerializeSubTypes(NciUrl url, CTSSearchParams searchParms)
        {
            if (searchParms.IsFieldSet(FormFields.SubTypes))
            {
                url.QueryParameters.Add("st", SerializeMultiTermFields(searchParms.SubTypes));
            }
        }

        //Parameter stg (Stages)
        private static void SerializeStages(NciUrl url, CTSSearchParams searchParms)
        {
            if (searchParms.IsFieldSet(FormFields.Stages))
            {
                url.QueryParameters.Add("stg", SerializeMultiTermFields(searchParms.Stages));
            }
        }

        //Parameter fin (Findings)
        private static void SerializeFindings(NciUrl url, CTSSearchParams searchParms)
        {
            if (searchParms.IsFieldSet(FormFields.Findings))
            {
                url.QueryParameters.Add("fin", SerializeMultiTermFields(searchParms.Findings));
            }
        }

        // Parameter a (Age)
        private static void SerializeAge(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.Age))
            {
                url.QueryParameters.Add("a", searchParams.Age.ToString());
            }
        }

        // Parameter g (Gender)
        private static void SerializeGender(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.Gender))
            {
                url.QueryParameters.Add("g", HttpUtility.UrlEncode(searchParams.Gender));
            }
        }

        // Parameter loc (Location, and AtNIH if loc=nih)
        private static void SerializeLocation(NciUrl url, CTSSearchParams searchParams)
        {
            url.QueryParameters.Add("loc", searchParams.Location.ToString("d"));

            if (url.QueryParameters.ContainsKey("loc"))
            {
                switch (searchParams.Location)
                {
                    //None needs not to be set
                    //AtNIH is all that is needed for NIH
                    case LocationType.Zip:
                        {
                            SerializeZipCode(url, searchParams);
                            break;
                        }
                    case LocationType.CountryCityState:
                        {
                            SerializeCountryCityState(url, searchParams);
                            break;
                        }
                    case LocationType.Hospital:
                        {
                            SerializeHospital(url, searchParams);
                            break;
                        }
                }
            }
        }

        // Parameter z (Zip Code) && zp (Zip Proximity)
        private static void SerializeZipCode(NciUrl url, CTSSearchParams searchParams)
        {
            ZipCodeLocationSearchParams locParams = new ZipCodeLocationSearchParams();

            if (locParams.IsFieldSet(FormFields.ZipCode))
            {
                url.QueryParameters.Add("z", locParams.ZipCode);
                url.QueryParameters.Add("zp", locParams.ZipRadius.ToString()); //This has default, so might as well set it.
            }
        }

        //Parameter lst (State) && Parameter lcty (City) && Parameter lcnty (Country)
        private static void SerializeCountryCityState(NciUrl url, CTSSearchParams searchParams)
        {
            CountryCityStateLocationSearchParams locParams = new CountryCityStateLocationSearchParams();
            if (locParams.IsFieldSet(FormFields.State))
            {
                //TODO: do we need to encode the state?
                url.QueryParameters.Add("lst", locParams.State.Key);
            }

            if (locParams.IsFieldSet(FormFields.City))
            {
                url.QueryParameters.Add("lcty", HttpUtility.UrlEncode(locParams.City));
            }

            if (locParams.IsFieldSet(FormFields.Country))
            {
                url.QueryParameters.Add("lcnty", HttpUtility.UrlEncode(locParams.Country));
            }
        }

        //Parameter hos (Hospital)
        private static void SerializeHospital(NciUrl url, CTSSearchParams searchParams)
        {
            HospitalLocationSearchParams locParams = new HospitalLocationSearchParams();
            if (locParams.IsFieldSet(FormFields.Hospital))
            {
                url.QueryParameters.Add("hos", HttpUtility.UrlEncode(locParams.Hospital));
            }
        }

        //Parameter d (Drugs)
        private static void SerializeDrugs(NciUrl url, CTSSearchParams searchParms)
        {
            if (searchParms.IsFieldSet(FormFields.Drugs))
            {
                url.QueryParameters.Add("d", SerializeMultiTermFields(searchParms.Drugs));
            }
        }

        //Parameter i (Other treatments / interventions)
        private static void SerializeOtherTreatments(NciUrl url, CTSSearchParams searchParms)
        {
            if (searchParms.IsFieldSet(FormFields.OtherTreatments))
            {
                url.QueryParameters.Add("i", SerializeMultiTermFields(searchParms.OtherTreatments));
            }
        }

        // Parameter tp (Trial Phase)
        private static void SerializeTrialPhases(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.TrialPhases))
            {
                url.QueryParameters.Add(
                    "tp", 
                    string.Join(",", searchParams.TrialPhases.Select(tp => tp.Key))
                );
            }
        }

        // Parameter tid (Trial IDs)
        private static void SerializeTrialIDs(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.TrialIDs))
            {
                url.QueryParameters.Add(
                    "tid", 
                    string.Join(",", searchParams.TrialIDs.Select(tid => HttpUtility.UrlEncode(tid)))
                );
            }
        }

        //Parameter q (Keyword/Phrase)
        private static void SerializeKeyword(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.Phrase))
            {
                url.QueryParameters.Add("q", HttpUtility.UrlEncode(searchParams.Phrase));
            }
        }

        // Parameter in (Investigator)
        private static void SerializeInvestigator(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.Investigator))
            {
                url.QueryParameters.Add("in", HttpUtility.UrlEncode(searchParams.Investigator));
            }
        }

        // Parameter lo (Lead Org)
        private static void SerializeLeadOrg(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.LeadOrg))
            {
                url.QueryParameters.Add("lo", HttpUtility.UrlEncode(searchParams.LeadOrg));
            }
        }

        // Parameter rl (Results Link Flag)
        private static void SerializeResultsLinkFlag(NciUrl url, CTSSearchParams searchParams)
        {
            url.QueryParameters.Add("rl", searchParams.ResultsLinkFlag.ToString("d"));
        }

        #endregion

        #region Parameter Parsers 
        // Parameter rl (Results Link Flag)
        private void ParseResultsLinkFlag(NciUrl url, CTSSearchParams searchParams)
        {
            //TODO: Fix this to handle the enum parsing better.
            //Also if it is not in the URL we should set it to basic
            if (IsInUrl(url, "rl"))
            {
                int resLinkFlag = ParamAsInt(url.QueryParameters["rl"], 0);
                if (resLinkFlag == 0)
                {
                    LogParseError("ResultsLinkFlag", "Please enter a valid results link flag value (1 or 2).", searchParams);
                }
                else
                {
                    searchParams.ResultsLinkFlag = (ResultsLinkType)resLinkFlag;
                }
            }
        }
        
        //Parameter t (Main Cancer Type)
        private void ParseCancerType(NciUrl url, CTSSearchParams searchParams)
        {
            //TODO: Extra credit, refactor the term extraction logic so it does not get repeated for each type
            if (IsInUrl(url, "t"))
            {
                TerminologyFieldSearchParam[] terms = GetTermFieldFromParam(url.QueryParameters["t"], FormFields.MainType, searchParams);
                if (terms.Length == 1)
                {
                    searchParams.MainType = terms[0];
                }
                else if (terms.Length > 1)
                {
                    LogParseError(FormFields.MainType, "Please include only one main cancer type in your search.", searchParams);

                }
            }
        }

        //Parameter st (SubTypes)
        private void ParseSubTypes(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "st"))
            {
                searchParams.SubTypes = GetTermFieldFromParam(url.QueryParameters["st"], FormFields.SubTypes, searchParams);
            }

            //TODO: Error Handling
        }

        //Parameter stg (Stages)
        private void ParseStages(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "stg"))
            {
                searchParams.Stages = GetTermFieldFromParam(url.QueryParameters["stg"], FormFields.Stages, searchParams);
            }

            //TODO: Error Handling
        }

        //Parameter fin (Findings)
        private void ParseFindings(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "fin"))
            {
                searchParams.Findings = GetTermFieldFromParam(url.QueryParameters["fin"], FormFields.Findings, searchParams);
            }

            //TODO: Error Handling
        }

        // Parameter a (Age)
        private void ParseAge(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "a"))
            {
                int age = this.ParamAsInt(url.QueryParameters["a"], 0);
                if(age == 0)
                {
                    LogParseError(FormFields.Age, "Please enter a valid age parameter.", searchParams);
                }
                else if (age > 120)
                {
                    LogParseError(FormFields.Age, "Please enter a valid age parameter.", searchParams);
                }
                else
                {
                    searchParams.Age = age;
                }
            }
        }

        //Parameter q (Keyword/Phrase)
        private void ParseKeyword(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "q"))
            {
                string phrase = ParamAsStr(url.QueryParameters["q"]);
                if (!string.IsNullOrWhiteSpace(phrase))
                {
                    searchParams.Phrase = phrase;
                }
                else
                {
                    LogParseError(FormFields.Phrase, "Please enter a valid keyword parameter.", searchParams);
                }
            }
        }

        // Parameter g (Gender)
        private void ParseGender(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "g"))
            {
                string gender = ParamAsStr(url.QueryParameters["g"]);
                if (!string.IsNullOrWhiteSpace(gender))
                {
                    searchParams.Gender = gender;
                }
                else
                {
                    LogParseError(FormFields.Gender, "Please enter a valid gender.", searchParams);
                }
            }
        }

        // Parameter loc (Location, and AtNIH if loc=nih)
        private void ParseLocation(NciUrl url, CTSSearchParams searchParams)
        {
            searchParams.Location = LocationType.None;

            if (url.QueryParameters.ContainsKey("loc"))
            {
                try
                {
                    searchParams.Location = (LocationType)ParamAsInt(url.QueryParameters["loc"], 0);
                } catch(Exception) {
                    // 
                    LogParseError(FormFields.Location, "Please enter a valid location type.", searchParams);
                    return;
                }

                switch(searchParams.Location) {
                    case LocationType.AtNIH:
                        {
                            searchParams.LocationParams = new AtNIHLocationSearchParams();
                            break;
                        }
                    case LocationType.Zip:
                        {
                            ParseZipCode(url, searchParams);
                            break;
                        }
                    case LocationType.CountryCityState:
                        {
                            ParseCountryCityState(url, searchParams);
                            break;
                        }
                    case LocationType.Hospital:
                        {
                            ParseHospital(url, searchParams);
                            break;
                        }
                }
            }
            else if (searchParams.ResultsLinkFlag == ResultsLinkType.Basic && IsInUrl(url, "z"))
            {
                ParseZipCode(url, searchParams);
            }
        }

        // Parameter z (Zip Code) && zp (Zip Proximity)
        private void ParseZipCode(NciUrl url, CTSSearchParams searchParams)
        {
            ZipCodeLocationSearchParams locParams = new ZipCodeLocationSearchParams();

            if (IsInUrl(url, "z"))
            {
                string zipCode = ParamAsStr(url.QueryParameters["z"]);
 
                if (string.IsNullOrWhiteSpace(zipCode) || (zipCode.Length < 5))
                {
                    LogParseError(FormFields.ZipCode, "Please enter a valid zip code value.", searchParams);
                }
                
                string pattern = @"^[0-9]{5}$";
                if(Regex.IsMatch(zipCode, pattern))
                {
                    locParams.ZipCode = zipCode;
                }
                else
                {
                    LogParseError(FormFields.ZipCode, "Please enter a valid zip code value.", searchParams);
                }
            }
            else
            {
                //Handle when zipcode has not been specified, but location type is zip code
                LogParseError(FormFields.ZipCode, "Please enter a valid zip code value.", searchParams);
            }

            if (IsInUrl(url, "zp"))
            {
                int zipRadius = ParamAsInt(url.QueryParameters["zp"], 100);
                if (zipRadius < 1 || zipRadius > 12451)
                // TODO: check for type mismatch
                {
                    LogParseError(FormFields.ZipRadius, "Please enter a valid zip radius value.", searchParams);
                }
                else
                {
                    locParams.ZipRadius = zipRadius;
                }
            }

            //TODO: Figure out what to do if this is empty
            searchParams.LocationParams = locParams;
        }

        //Parameter lst (State) && Parameter lcty (City) && Parameter lcnty (Country)
        private void ParseCountryCityState(NciUrl url, CTSSearchParams searchParams)
        {
            CountryCityStateLocationSearchParams locParams = new CountryCityStateLocationSearchParams();

            //TODO: Handle label conversion
            if (IsInUrl(url, "lst"))
            {
                LabelledSearchParam[] states = GetLabelledFieldFromParam(url.QueryParameters["lst"], FormFields.State, searchParams);
                if (states.Length == 1)
                {
                    locParams.State = states[0];
                }
                else
                {
                    LogParseError(FormFields.State, "Please include only one state in your search.", searchParams);
                }
            }

            if (IsInUrl(url, "lcty"))
            {
                string city = ParamAsStr(url.QueryParameters["lcty"]);
                if (!string.IsNullOrWhiteSpace(city))
                {
                    locParams.City = city;
                }
                else
                {
                    LogParseError(FormFields.City, "Please enter a valid city parameter.", searchParams);
                }
            }

            if (IsInUrl(url, "lcnty"))
            {
                string country = ParamAsStr(url.QueryParameters["lcnty"]);
                if (!string.IsNullOrWhiteSpace(country))
                {
                    locParams.Country = country;
                }
                else
                {
                    LogParseError(FormFields.Country, "Please enter a valid country parameter.", searchParams);
                }
            }

            //TODO: If none of the items have been set, do we error??

            //TODO: Figure out what to do if this is empty
            searchParams.LocationParams = locParams;
        }

        //Parameter hos (Hospital)
        private void ParseHospital(NciUrl url, CTSSearchParams searchParams)
        {
            HospitalLocationSearchParams locParams = new HospitalLocationSearchParams();

            if (IsInUrl(url, "hos"))
            {
                string hospital = ParamAsStr(url.QueryParameters["hos"]);
                if (!string.IsNullOrWhiteSpace(hospital))
                {
                    locParams.Hospital = hospital;
                }
                else
                {
                    LogParseError(FormFields.Hospital, "Please enter a valid hospital/institution parameter.", searchParams);
                }
            }
            else
            {
                LogParseError(FormFields.Hospital, "Please enter a valid hospital/institution parameter.", searchParams);
            }

            //TODO: Figure out what to do if this is empty
            searchParams.LocationParams = locParams;
        }

        //Parameter tt (Trial Type)
        private void ParseTrialTypes(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "tt"))
            {
                searchParams.TrialTypes = GetLabelledFieldFromParam(url.QueryParameters["tt"], FormFields.TrialTypes, searchParams);
            }
        }

        //Parameter d (Drugs)
        private void ParseDrugs(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "d"))
            {
                searchParams.Drugs = GetTermFieldFromParam(url.QueryParameters["d"], FormFields.Drugs, searchParams);
            }

            //TODO: Error handling
        }

        //Parameter i (Other treatments / interventions)
        private void ParseOtherTreatments(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "i"))
            {
                searchParams.OtherTreatments = GetTermFieldFromParam(url.QueryParameters["i"], FormFields.OtherTreatments, searchParams);
            }

            //TODO: Error handling
        }

        // Parameter tp (Trial Phase)
        private void ParseTrialPhases(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "tp"))
            {
                LabelledSearchParam[] phases = GetLabelledFieldFromParam(url.QueryParameters["tp"], FormFields.TrialPhases, searchParams);
                searchParams.TrialPhases = phases;
            }

            // TODO: Error handling
        }

        // Parameter tid (Trial IDs)
        private void ParseTrialIDs(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "tid"))
            {
                string[] idArray = ParamAsStr(url.QueryParameters["tid"]).Split(new Char[] {',',';'} ).Select(id => id.Trim()).ToArray();
                if(idArray.Length == 1 && string.IsNullOrWhiteSpace(idArray[0]))
                {
                    LogParseError(FormFields.TrialIDs, "Please enter a valid trial ID parameter.", searchParams);
                }
                else
                {
                    searchParams.TrialIDs = idArray;
                }
            }

            //TODO: More error handling
        }

        // Parameter in (Investigator)
        private void ParseInvestigator(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "in"))
            {
                string investigator = ParamAsStr(url.QueryParameters["in"]);
                if (!string.IsNullOrWhiteSpace(investigator))
                {
                    searchParams.Investigator = investigator;
                }
                else
                {
                    LogParseError(FormFields.Investigator, "Please enter a valid trial investigator parameter.", searchParams);
                }
            }
        }

        // Parameter lo (Lead Org)
        private void ParseLeadOrg(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "lo"))
            {
                string leadOrg = ParamAsStr(url.QueryParameters["lo"]);
                if (!string.IsNullOrWhiteSpace(leadOrg))
                {
                    searchParams.LeadOrg = leadOrg;
                }
                else
                {
                    LogParseError(FormFields.LeadOrg, "Please enter a valid lead organization parameter.", searchParams);
                }
            }
        }
        #endregion


        #region Utility methods 

        /// <summary>
        /// Helper function to check if a param is used. (And not just set with an empty string.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="paramName"></param>
        /// <returns></returns>
        private bool IsInUrl(NciUrl url, string paramName)
        {
            return url.QueryParameters.ContainsKey(paramName) && !String.IsNullOrWhiteSpace(url.QueryParameters[paramName]);
        }

        /// <summary>
        /// Extract property values for 'Labelled' Field search params (e.g. state, phase, trial type)
        /// </summary>
        /// <param name="paramData"></param>
        /// <returns>Array of param objects</returns>
        private LabelledSearchParam[] GetLabelledFieldFromParam(string paramData, FormFields field, CTSSearchParams searchParams)
        {
            List<LabelledSearchParam> rtnParams = new List<LabelledSearchParam>();

            //TODO: Update mapping to handle different cases (like state abbreviation vs primary purpose label)
            try
            {
                string[] items = paramData.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    bool contains = this._lookupSvc.MappingContainsKey(items[i]);
                    if(this._lookupSvc.MappingContainsKey(items[i]))
                    {
                        LabelledSearchParam type = new LabelledSearchParam();
                        type.Key = items[i];
                        type.Label = this._lookupSvc.Get(type.Key);
                        rtnParams.Add(type);
                    }
                    else
                    {
                        LogParseError(field, "Invalid param(s) for lookup: " + items[i], searchParams);
                    }
                }
            }
            catch
            {
                LogParseError(field, "Please enter a valid parameter.", searchParams);
            }

            return rtnParams.ToArray();
        }

        /// <summary>
        /// Extract property values for Terminology Field search params (e.g. cancer type, subtype, drug)
        /// </summary>
        /// <param name="paramData"></param>
        /// <returns>Array of param objects</returns>
        private TerminologyFieldSearchParam[] GetTermFieldFromParam(string paramData, FormFields field, CTSSearchParams searchParams)
        {
            List<TerminologyFieldSearchParam> rtnParams = new List<TerminologyFieldSearchParam>();
            string codePattern = @"(?i)c\d{4}";

            //TODO: Handle validating codes, handling multiple codes, etc.
            try 
            {
                string[] items = paramData.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    if(items[i].Contains('|'))
                    {
                        string[] multiple = items[i].Split('|');
                        Array.Sort(multiple);

                        bool allMatchCodePattern = true;
                        for(int j = 0; j < multiple.Length; j++)
                        {
                            if(Regex.IsMatch(multiple[j], codePattern))
                            {
                                continue;
                            }
                            else
                            {
                                allMatchCodePattern = false;
                            }
                        }

                        if(allMatchCodePattern)
                        {
                            if(this._lookupSvc.MappingContainsKey(string.Join(",", multiple).ToLower()))
                            {
                                TerminologyFieldSearchParam type = new TerminologyFieldSearchParam();
                                type.Codes = multiple;
                                type.Label = this._lookupSvc.GetTitleCase(string.Join(",", multiple).ToLower());
                                rtnParams.Add(type);
                            }
                            else
                            {
                                LogParseError(field, "Invalid code(s) for lookup: " + string.Join(",", multiple).ToLower(), searchParams);
                            }
                        }
                    }
                    else
                    {
                        if(Regex.IsMatch(items[i], codePattern))
                        {
                            if (this._lookupSvc.MappingContainsKey(items[i].ToLower()))
                            {
                                TerminologyFieldSearchParam type = new TerminologyFieldSearchParam();
                                type.Codes = new string[] { items[i] };
                                type.Label = this._lookupSvc.GetTitleCase(items[i].ToLower());
                                rtnParams.Add(type);
                            }
                            else
                            {
                                LogParseError(field, "Invalid code(s) for lookup: " + items[i].ToLower(), searchParams);
                            }
                        }
                    }
                }
            }
            catch
            {
                LogParseError(field, "Please enter a valid parameter.", searchParams);
            }

            return rtnParams.ToArray();
        }

        /// <summary>
        /// Gets a query parameter as a string
        /// </summary>
        protected string ParamAsStr(string paramVal)
        {
            if (string.IsNullOrWhiteSpace(paramVal))
                return String.Empty;
            else
                return paramVal.Trim();
        }

        /// <summary>
        /// Converts a query param to an int; returns 0 if unable to parse
        /// </summary>
        private int ParamAsInt(string paramVal, int def)
        {
            if (string.IsNullOrWhiteSpace(paramVal))
            {
                return def;
            }
            else
            {
                int tmpInt = 0;
                if (int.TryParse(paramVal.Trim(), out tmpInt))
                {
                    return tmpInt;
                }
                else
                {
                    return def;
                }
            }
        }

        /// <summary>
        /// Logs an error in the ParseErrors array on the Search Params
        /// </summary>
        private void LogParseError(FormFields field, string errorMessage, CTSSearchParams searchParams)
        {
            CTSSearchFieldParamError error = new CTSSearchFieldParamError();
            error.Field = field;
            error.ErrorMessage = errorMessage;
            searchParams.ParseErrors.Add(error);
        }

        /// <summary>
        /// Logs an error in the ParseErrors array on the Search Params
        /// </summary>
        private void LogParseError(string param, string errorMessage, CTSSearchParams searchParams)
        {
            CTSSearchParamError error = new CTSSearchParamError();
            error.Param = param;
            error.ErrorMessage = errorMessage;
            searchParams.ParseErrors.Add(error);
        }

        #endregion

    }
}
