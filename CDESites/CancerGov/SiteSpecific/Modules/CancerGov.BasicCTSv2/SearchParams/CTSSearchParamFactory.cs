using System;
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
        private IZipCodeGeoLookupService _zipLookupSvc;
        private ParameterParserDelegate _parsers;

        private static ParameterSerializerDelegate _paramSerializers;

        /// <summary>
        /// Static constructor to initialize 
        /// </summary>
        static CTSSearchParamFactory() {
            _paramSerializers = 
                (ParameterSerializerDelegate) SerializeCancerType + //First param needs the cast.
                SerializeSubTypes +
                SerializeStages +
                SerializeFindings +
                SerializeAge +
                SerializeKeyword +
                SerializeHealthyVolunteers +
                SerializeGender +
                SerializeIsVAOnly +
                SerializeLocation +
                SerializeTrialTypes +
                SerializeDrugs +
                SerializeOtherTreatments +
                SerializeTrialPhases +
                SerializeTrialIDs +
                SerializeInvestigator +
                SerializeLeadOrg +
                SerializeResultsLinkFlag +
                SerializeRedirectFlag;
        }

        /// <summary>
        /// Creates new instance of a search param factory
        /// </summary>
        /// <param name="lookupSvc">An instance of a ITerminologyLookupService </param>
        public CTSSearchParamFactory(ITerminologyLookupService lookupSvc, IZipCodeGeoLookupService zipLookupSvc)
        {
            this._lookupSvc = lookupSvc;
            this._zipLookupSvc = zipLookupSvc;

            //Add parser methods here
            this._parsers =
                (ParameterParserDelegate)ParseResultsLinkFlag + //First param needs the cast.
                ParseCancerType +
                ParseSubTypes +
                ParseStages +
                ParseFindings +
                ParseAge +
                ParseKeyword +
                ParseHealthyVolunteers +
                ParseGender +
                ParseIsVAOnly +
                ParseLocation +
                ParseTrialTypes +
                ParseDrugs +
                ParseOtherTreatments +
                ParseTrialPhases +
                ParseTrialIDs +
                ParseInvestigator +
                ParseLeadOrg +
                ParseRedirectFlag;
        }
         
        /// <summary>
        /// Gets an instance of a CTSSearchParams object based on params in URL.
        /// </summary>
        /// <param name="url">The URL to parse</param>
        /// <returns></returns>
        public CTSSearchParams Create(string url)
        {
            NciUrl reqUrl = new NciUrl(true, true, true);
            reqUrl.SetUrl(url);

            return Create(reqUrl); 
        }

        /// <summary>
        /// Gets an instance of a CTSSearchParams object based on params in URL.
        /// </summary>
        /// <param name="reqUrl">The URL to parse</param>
        /// <returns></returns>
        public CTSSearchParams Create(NciUrl reqUrl)
        {
            CTSSearchParams rtnParams = new CTSSearchParams();

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

            _paramSerializers(url, searchParams);

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

        //Parameter q (Keyword/Phrase)
        private static void SerializeKeyword(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.Phrase))
            {
                url.QueryParameters.Add("q", HttpUtility.UrlEncode(searchParams.Phrase));
            }
        }

        // Parameter hv (Healthy Volunteers)
        private static void SerializeHealthyVolunteers(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.HealthyVolunteers))
            {                
                url.QueryParameters.Add("hv", searchParams.HealthyVolunteer.ToString("d")); //Output the decimal value of the enum as a string.
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

        // Parameter va (Search VA Only)
        private static void SerializeIsVAOnly(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.IsVAOnly))
            {
                url.QueryParameters.Add("va", searchParams.IsVAOnly ? "1" : "0" );
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
            ZipCodeLocationSearchParams locParams = (ZipCodeLocationSearchParams)searchParams.LocationParams;

            if (locParams.IsFieldSet(FormFields.ZipCode))
            {
                url.QueryParameters.Add("z", locParams.ZipCode);
                url.QueryParameters.Add("zp", locParams.ZipRadius.ToString()); //This has default, so might as well set it.
            }
        }

        //Parameter lst (State) && Parameter lcty (City) && Parameter lcnty (Country)
        private static void SerializeCountryCityState(NciUrl url, CTSSearchParams searchParams)
        {
            CountryCityStateLocationSearchParams locParams = (CountryCityStateLocationSearchParams)searchParams.LocationParams;
            if (locParams.IsFieldSet(FormFields.State))
            {
                //TODO: do we need to encode the state?
                url.QueryParameters.Add(
                    "lst",
                    string.Join(",", locParams.State.Select(lst => lst.Key))
                );
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
            HospitalLocationSearchParams locParams = (HospitalLocationSearchParams) searchParams.LocationParams;
            if (locParams.IsFieldSet(FormFields.Hospital))
            {
                url.QueryParameters.Add("hos", HttpUtility.UrlEncode(locParams.Hospital));
            }
        }

        // Parameter tt (Trial Type)
        private static void SerializeTrialTypes(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.IsFieldSet(FormFields.TrialTypes))
            {
                url.QueryParameters.Add(
                    "tt",
                    string.Join(",", searchParams.TrialTypes.Select(tp => tp.Key))
                );
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

        // Parameter r (Redirect Flag)
        private static void SerializeRedirectFlag(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.RedirectFlag == true)
            {
                url.QueryParameters.Add("r", "1");
            }
        }

        #endregion

        #region Parameter Parsers 
        // Parameter rl (Results Link Flag)
        private void ParseResultsLinkFlag(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "rl"))
            {
                int resLinkFlag = url.CTSParamAsInt("rl", 0);
                

                if(resLinkFlag == 1 || resLinkFlag == 2)
                {
                    searchParams.ResultsLinkFlag = (ResultsLinkType)resLinkFlag;
                }
                else
                {
                    LogParseError("ResultsLinkFlag", "Results Link Flag can only equal 1 or 2.", searchParams);
                }
            }
            else
            {
                searchParams.ResultsLinkFlag = ResultsLinkType.Basic;
            }
        }

        // Parameter r (Redirect Flag)
        private void ParseRedirectFlag(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "r"))
            {
                searchParams.RedirectFlag = true;
            }
        }
        
        //Parameter t (Main Cancer Type)
        private void ParseCancerType(NciUrl url, CTSSearchParams searchParams)
        {
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
        }

        //Parameter stg (Stages)
        private void ParseStages(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "stg"))
            {
                searchParams.Stages = GetTermFieldFromParam(url.QueryParameters["stg"], FormFields.Stages, searchParams);
            }
        }

        //Parameter fin (Findings)
        private void ParseFindings(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "fin"))
            {
                searchParams.Findings = GetTermFieldFromParam(url.QueryParameters["fin"], FormFields.Findings, searchParams);
            }
        }

        // Parameter a (Age)
        private void ParseAge(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "a"))
            {
                int age = url.CTSParamAsInt("a", 0);
                if(age <= 0)
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
                string phrase = url.CTSParamAsStr("q");
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

        // Parameter hv (HealthyVolunteers)
        private void ParseHealthyVolunteers(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "hv"))
            {
                int hvType = url.CTSParamAsInt("hv", -1);
                if (hvType == 0 || hvType == 1 || hvType == 2)
                {
                    searchParams.HealthyVolunteer = (HealthyVolunteerType)hvType;
                }
                else
                {
                    LogParseError(FormFields.HealthyVolunteers, "Please enter a valid healthy volunteer indicator.", searchParams);
                }
            }
        }
    

        // Parameter g (Gender)
        private void ParseGender(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "g"))
            {
                string gender = url.CTSParamAsStr("g");
                if (!string.IsNullOrWhiteSpace(gender))
                {
                    if(gender == "male" || gender == "female")
                    {
                        searchParams.Gender = gender;
                    }
                    else
                    {
                        LogParseError(FormFields.Gender, "Please enter a valid gender.", searchParams);
                    }
                }
                else
                {
                    LogParseError(FormFields.Gender, "Please enter a valid gender.", searchParams);
                }
            }
        }

        // Parameter va (VA Only Search)
        private void ParseIsVAOnly(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "va"))
            {
                int va = url.CTSParamAsInt("va", -1);
                if (va == -1)
                {
                    LogParseError(FormFields.IsVAOnly, "Please enter a valid Veterans Health Administration facility search parameter.", searchParams);
                }
                else if (va > 1)
                {
                    LogParseError(FormFields.IsVAOnly, "Please enter a valid Veterans Health Administration facility search parameter.", searchParams);
                }
                else
                {
                    if (va == 0)
                        searchParams.IsVAOnly = false;
                    else
                        searchParams.IsVAOnly = true;
                }
            }
        }

        // Parameter loc (Location, and AtNIH if loc=nih)
        private void ParseLocation(NciUrl url, CTSSearchParams searchParams)
        {
            if (searchParams.ResultsLinkFlag == ResultsLinkType.Advanced)
            {
                if (url.QueryParameters.ContainsKey("loc"))
                {
                    int locType = url.CTSParamAsInt("loc", -1);
                    if (locType == 0 || locType == 1 || locType == 2 || locType == 3 || locType == 4)
                    {
                        searchParams.Location = (LocationType)locType;

                        switch (searchParams.Location)
                        {
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
                    else
                    {
                        LogParseError(FormFields.Location, "Please enter a valid location type.", searchParams);
                    }
                }
            }
            else if (searchParams.ResultsLinkFlag == ResultsLinkType.Basic && IsInUrl(url, "z"))
            {
                searchParams.Location = LocationType.Zip;
                ParseZipCode(url, searchParams);
            }
        }

        // Parameter z (Zip Code) && zp (Zip Proximity)
        private void ParseZipCode(NciUrl url, CTSSearchParams searchParams)
        {
            ZipCodeLocationSearchParams locParams = new ZipCodeLocationSearchParams();
            searchParams.LocationParams = locParams;

            if (IsInUrl(url, "z"))
            {
                string zipCode = url.CTSParamAsStr("z");
 
                if (string.IsNullOrWhiteSpace(zipCode) || (zipCode.Length < 5))
                {
                    LogParseError(FormFields.ZipCode, "Please enter a valid zip code value.", searchParams);
                }
                
                string pattern = @"^[0-9]{5}$";
                if(Regex.IsMatch(zipCode, pattern))
                {
                    GeoLocation geolocation = this._zipLookupSvc.GetZipCodeGeoEntry(zipCode);
                    if (geolocation != null)
                    {
                        locParams.ZipCode = zipCode;
                        locParams.GeoLocation = geolocation;

                        if (IsInUrl(url, "zp"))
                        {
                            int zipRadius = url.CTSParamAsInt("zp", -1);
                            if (zipRadius < 1 || zipRadius > 12451)
                            {
                                LogParseError(FormFields.ZipRadius, "Please enter a valid zip radius value.", searchParams);
                                searchParams.LocationParams = new ZipCodeLocationSearchParams();
                            }
                            else
                            {
                                locParams.ZipRadius = zipRadius;
                                searchParams.LocationParams = locParams;
                            }
                        }
                        else
                        {
                            searchParams.LocationParams = locParams;
                        }
                    }
                    else
                    {
                        LogParseError(FormFields.ZipCode, "Please enter a valid zip code value.", searchParams);
                    }
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
        }

        //Parameter lst (State) && Parameter lcty (City) && Parameter lcnty (Country)
        private void ParseCountryCityState(NciUrl url, CTSSearchParams searchParams)
        {
            CountryCityStateLocationSearchParams locParams = new CountryCityStateLocationSearchParams();
            bool hasInvalidParam = false;

            if (IsInUrl(url, "lst"))
            {
                locParams.State = GetLabelledFieldFromParam(url.QueryParameters["lst"], FormFields.State, searchParams);
                if(locParams.State.Length == 0)
                {
                    hasInvalidParam = true;
                }
            }

            if (IsInUrl(url, "lcty"))
            {
                if (!IsInUrl(url, "lcnty"))
                {
                    hasInvalidParam = true;
                    LogParseError(FormFields.Country, "Please enter a country parameter if entering a city parameter.", searchParams);
                }
                else
                {
                    string city = url.CTSParamAsStr("lcty");
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        locParams.City = city;
                    }
                    else
                    {
                        hasInvalidParam = true;
                        LogParseError(FormFields.City, "Please enter a valid city parameter.", searchParams);
                    }
                }
            }

            if (IsInUrl(url, "lcnty"))
            {
                string country = url.CTSParamAsStr("lcnty");
                if (!string.IsNullOrWhiteSpace(country))
                {
                    locParams.Country = country;
                }
                else
                {
                    hasInvalidParam = true;
                    LogParseError(FormFields.Country, "Please enter a valid country parameter.", searchParams);
                }
            }

            if (!IsInUrl(url, "lcty") && !IsInUrl(url, "lst") && !IsInUrl(url, "lcnty"))
            {
                hasInvalidParam = true;
                LogParseError(FormFields.Location, "You must enter either a valid country, city, or state parameter.", searchParams);
            }

            if(!hasInvalidParam)
            {
                searchParams.LocationParams = locParams;
            }
            else
            {
                searchParams.LocationParams = new CountryCityStateLocationSearchParams();
            }
        }

        //Parameter hos (Hospital)
        private void ParseHospital(NciUrl url, CTSSearchParams searchParams)
        {
            HospitalLocationSearchParams locParams = new HospitalLocationSearchParams();
            bool hasInvalidParam = false;

            if (IsInUrl(url, "hos"))
            {
                string hospital = url.CTSParamAsStr("hos");
                if (!string.IsNullOrWhiteSpace(hospital))
                {
                    locParams.Hospital = hospital;
                }
                else
                {
                    hasInvalidParam = true;
                    LogParseError(FormFields.Hospital, "Please enter a valid hospital/institution parameter.", searchParams);
                }
            }
            else
            {
                hasInvalidParam = true;
                LogParseError(FormFields.Hospital, "Please enter a valid hospital/institution parameter.", searchParams);
            }

            if(!hasInvalidParam)
            {
                searchParams.LocationParams = locParams;
            }
            else
            {
                searchParams.LocationParams = new HospitalLocationSearchParams();
            }
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
        }

        //Parameter i (Other treatments / interventions)
        private void ParseOtherTreatments(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "i"))
            {
                searchParams.OtherTreatments = GetTermFieldFromParam(url.QueryParameters["i"], FormFields.OtherTreatments, searchParams);
            }
        }

        // Parameter tp (Trial Phase)
        private void ParseTrialPhases(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "tp"))
            {
                searchParams.TrialPhases = GetLabelledFieldFromParam(url.QueryParameters["tp"], FormFields.TrialPhases, searchParams);
            }

            // TODO: Error handling
        }

        // Parameter tid (Trial IDs)
        private void ParseTrialIDs(NciUrl url, CTSSearchParams searchParams)
        {
            if (IsInUrl(url, "tid"))
            {
                string[] idArray = CTSSearchParamExtensions.ParamAsStr(url.QueryParameters["tid"]).Split(new Char[] {',',';'} ).Select(id => id.Trim()).ToArray();
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
                string investigator = url.CTSParamAsStr("in");
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
                string leadOrg = url.CTSParamAsStr("lo");
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
            bool hasInvalidParam = false;

            //TODO: Update mapping to handle different cases (like state abbreviation vs primary purpose label)
            try
            {
                string[] items = paramData.Split(',');
                for (int i = 0; i < items.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(items[i]))
                    {
                        bool contains = this._lookupSvc.MappingContainsKey(items[i]);
                        if (this._lookupSvc.MappingContainsKey(items[i]))
                        {
                            LabelledSearchParam type = new LabelledSearchParam();
                            type.Key = items[i];
                            type.Label = this._lookupSvc.Get(type.Key);
                            rtnParams.Add(type);
                        }
                        else
                        {
                            hasInvalidParam = true;
                            LogParseError(field, "Invalid param(s) for lookup: " + items[i], searchParams);
                        }
                    }
                }
            }
            catch
            {
                LogParseError(field, "Please enter a valid parameter.", searchParams);
            }

            if(hasInvalidParam == false)
            {
                return rtnParams.ToArray();
            }
            else
            {
                return new LabelledSearchParam[]{};
            }
        }

        /// <summary>
        /// Extract property values for Terminology Field search params (e.g. cancer type, subtype, drug)
        /// </summary>
        /// <param name="paramData"></param>
        /// <returns>Array of param objects</returns>
        private TerminologyFieldSearchParam[] GetTermFieldFromParam(string paramData, FormFields field, CTSSearchParams searchParams)
        {
            List<TerminologyFieldSearchParam> rtnParams = new List<TerminologyFieldSearchParam>();
            string codePattern = @"(?i)c\d+";
            bool allMatchCodePattern = true;

            try 
            {
                // Split params on comma for separate params
                string[] items = paramData.Split(',');

                for (int i = 0; i < items.Length; i++)
                {
                    // Capitalize all items
                    items[i] = items[i].ToUpper();

                    // Check if there are multiple codes for one param
                    if(items[i].Contains('|'))
                    {
                        // Split items on | as there are multiple codes in one param
                        string[] multiple = items[i].Split('|');

                        // Sort in alphanumerical order for label lookup
                        Array.Sort(multiple);

                        // Match all codes to C-code pattern
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

                        // If all match the C-code pattern, attempt lookup for label
                        if(allMatchCodePattern)
                        {
                            if(this._lookupSvc.MappingContainsKey(string.Join(",", multiple).ToLower()))
                            {
                                // Only add to the stored parameters if a label is found for the combination of codes
                                TerminologyFieldSearchParam type = new TerminologyFieldSearchParam();
                                type.Codes = multiple;
                                type.Label = this._lookupSvc.GetTitleCase(string.Join(",", multiple).ToLower());

                                bool containsCode = rtnParams.Any(p => p.Codes == type.Codes);
                                if (!containsCode)
                                {
                                    rtnParams.Add(type);
                                }
                            }
                            else
                            {
                                LogParseError(field, "Invalid code(s) for lookup: " + string.Join(",", multiple).ToLower(), searchParams);
                            }
                        }
                    }
                    else
                    {
                        // There are no params with multiple codes
                        if(Regex.IsMatch(items[i], codePattern))
                        {
                            // If this code matches the C-code pattern, attempt the lookup for the label
                            if (this._lookupSvc.MappingContainsKey(items[i].ToLower()))
                            {
                                // Only add to the stored parameters if a label is found for the code
                                TerminologyFieldSearchParam type = new TerminologyFieldSearchParam();
                                type.Codes = new string[] { items[i].ToUpper() };
                                type.Label = this._lookupSvc.GetTitleCase(items[i].ToLower());

                                bool containsCode = rtnParams.Any(p => p.Codes == type.Codes);
                                if (!containsCode)
                                {
                                    rtnParams.Add(type);
                                }
                            }
                            else
                            {
                                LogParseError(field, "Invalid code(s) for lookup: " + items[i].ToLower(), searchParams);
                            }
                        }
                        else
                        {
                            allMatchCodePattern = false;
                        }
                    }
                }
            }
            catch
            {
                LogParseError(field, "Please enter a valid parameter.", searchParams);
            }

            if(allMatchCodePattern == false)
            {
                // If any of the params have an invalid C-code, log an error and return an empty array.
                LogParseError(field, "Please enter a valid parameter.", searchParams);
                return new TerminologyFieldSearchParam[]{};
            }
            else
            {
                // If multiple codes map to the same label, combine those codes and add them to the
                // array of TerminologyFieldSearchParams as one entry.
                // i.e. "?stg=C88375,C85385,C85386 
                // C88375, C85385, and C85386 all map to "Stage I Breast Cancer" label
                // TerminologyFieldSearchParam: Key = "C88375,C85385,C85386" and Label = "Stage I Breast Cancer"
                List<TerminologyFieldSearchParam> filteredParams = rtnParams.Select(p => p.Label).Distinct().Select(l => new TerminologyFieldSearchParam() { Label = l }).ToList();
                foreach(TerminologyFieldSearchParam param in filteredParams)
                {
                    param.Codes = rtnParams.Where(p => p.Label == param.Label).SelectMany(p => p.Codes).Distinct().ToArray();
                    Array.Sort(param.Codes);
                }
                return filteredParams.ToArray();
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
