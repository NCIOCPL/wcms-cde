using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Represents the Search Parameters of a new API-based Clinical Trials Search.
    /// This includes the parameters used for a search, the labels to be displayed,
    /// as well as helpers to determine if a field was used.
    /// </summary>
    public class CTSSearchParams
    {
        /// <summary>
        /// Identifies which fields of the form were used.
        /// </summary>
        FormFields _usedFields = FormFields.None;
        List<CTSSearchParamError> _parseErrors = new List<CTSSearchParamError>();

        TerminologyFieldSearchParam _mainType           = null;
        TerminologyFieldSearchParam[] _subTypes         = { };
        TerminologyFieldSearchParam[] _stages           = { };
        TerminologyFieldSearchParam[] _findings         = { };
        TerminologyFieldSearchParam[] _drugs            = { };
        TerminologyFieldSearchParam[] _otherTreatments  = { };

        LabelledSearchParam[] _trialTypes   = { };
        LabelledSearchParam[] _trialPhases  = { };

        //int _pageNum                = 1;
        //int _itemsPerPage           = 10;
        ResultsLinkType _resultsLinkFlag = ResultsLinkType.Unknown;
        bool _redirectFlag          = false;
        int _age                    = 0;
        HealthyVolunteerType _healthyVols = HealthyVolunteerType.Any;
        string _gender              = string.Empty;
        string _phrase              = string.Empty;
        string _investigator        = string.Empty;
        string _leadOrg             = string.Empty;
        string[] _trialIDs          = { };
        LocationType _locationType  = LocationType.None;
        bool _isVAOnly                = false;

        LocationSearchParams _locationParams = null;

        /// <summary>
        /// Identified is a field within this location parameter is set.  This is a helper for Velocity that
        /// does not understand enums.
        /// </summary>
        /// <param name="field">The field to check using the String representation of its name</param>
        /// <returns>true if set, false if not.</returns>
        public bool IsFieldSet(string fieldName)
        {
            FormFields field = (FormFields)Enum.Parse(typeof(FormFields), fieldName, true);
            return IsFieldSet(field);
        }

        /// <summary>
        /// Identified is a field within this location parameter is set.
        /// </summary>
        /// <param name="field">The field to check</param>
        /// <returns>true if set, false if not.</returns>
        public bool IsFieldSet(FormFields field)
        {
            return (_usedFields & field) == field;
        }

        /// <summary>
        /// Determines if this search parameters has not set form fields
        /// </summary>
        /// <returns>true if no fields were set, false if a field has been set</returns>
        public bool IsEmpty()
        {
            return _usedFields == FormFields.None;
        }

        /// <summary>
        /// Determines if the parameters had parse errors
        /// </summary>
        /// <returns></returns>
        public bool HasInvalidParams()
        {
            return this.ParseErrors.Count > 0;
        }

        /// <summary>
        /// Identified is a field within this location parameter is set.  This is a helper for Velocity that
        /// does not understand enums.
        /// </summary>
        /// <param name="field">The field to check using the String representation of its name</param>
        /// <returns>true if set, false if not.</returns>
        public bool IsFieldInvalid(string fieldName)
        {
            FormFields field = (FormFields)Enum.Parse(typeof(FormFields), fieldName, true);
            return IsFieldInvalid(field);
        }

        /// <summary>
        /// Identified is a field within this location parameter is set.
        /// Parse errors can contain either a FormField or a string.
        /// </summary>
        /// <param name="field">The field to check</param>
        /// <returns>true if set, false if not.</returns>
        public bool IsFieldInvalid(FormFields field)
        {
            bool invalid = _parseErrors.Exists(pe => pe.Param == field.ToString());
            return invalid;
        }

        /// <summary>
        /// Gets a fields value as a string suitable for things like, oh, a velocity template
        /// </summary>
        /// <param name="fieldName">The string representation of a FormFields enum value</param>
        /// <returns>The value of the field, OR, and error message</returns>
        public string GetFieldAsString(string fieldName)
        {
            try
            {
                FormFields field = (FormFields)Enum.Parse(typeof(FormFields), fieldName, true);
                return GetFieldAsString(field);
            }
            catch (Exception)
            {
                return "Error Retrieving Field";
            }

        }

        /// <summary>
        /// Gets a fields value as a string suitable for things like, oh, a velocity template
        /// </summary>
        /// <param name="field">A FormFields enum value</param>
        /// <returns>The value of the field, OR, and error message</returns>
        public string GetFieldAsString(FormFields field)
        {
            try
            {
                switch (field)
                {
                    case FormFields.MainType:
                        {
                            return MainType.Label;
                        }
                    case FormFields.SubTypes:
                        {
                            return String.Join(", ", SubTypes.Select(st => st.Label));
                        }
                    case FormFields.Stages:
                        {
                            return String.Join(", ", Stages.Select(stg => stg.Label));;
                        }
                    case FormFields.Findings:
                        {
                            return String.Join(", ", Findings.Select(fin => fin.Label));
                        }
                    case FormFields.Age:
                        {
                            return Age.ToString();
                        }
                    case FormFields.Phrase:
                        {
                            return Phrase;
                        }
                    case FormFields.HealthyVolunteers:
                        {
                            switch(_healthyVols)
                            {
                                case HealthyVolunteerType.Healthy   : { return "Only Accepting Healthy Volunteers"; }
                                case HealthyVolunteerType.Infirmed  : { return "Not Accepting Healthy Volunteers"; }
                                default                             : { return "Accepting All Volunteers"; }
                            }
                        }
                    case FormFields.Gender:
                        {
                            return Gender;
                        }
                    case FormFields.TrialTypes:
                        {
                            return String.Join(", ", TrialTypes.Select(tt => tt.Label));
                        }
                    case FormFields.Drugs:
                        {
                            return String.Join(", ", Drugs.Select(d => d.Label));
                        }
                    case FormFields.OtherTreatments:
                        {
                            return String.Join(", ", OtherTreatments.Select(ot => ot.Label)); ;
                        }
                    case FormFields.TrialPhases:
                        {
                            return String.Join(", ", TrialPhases.Select(tp => tp.Label));
                        }
                    case FormFields.TrialIDs:
                        {
                            return String.Join(", ", TrialIDs);
                        }
                    case FormFields.Investigator:
                        {
                            return Investigator;
                        }
                    case FormFields.LeadOrg:
                        {
                            return LeadOrg;
                        }
                    case FormFields.IsVAOnly:
                        {
                            if (IsVAOnly)
                                return "True";
                            else
                                return "False";
                        }
                    case FormFields.AtNIH:
                    case FormFields.City:
                    case FormFields.State:
                    case FormFields.Country:
                    case FormFields.Hospital:
                    case FormFields.ZipCode:
                    case FormFields.ZipRadius:
                        {
                            return GetLocFieldAsString(field);
                        }
                    default:
                        {
                            return "Error Retrieving Field";
                        }
                }
            }
            catch (Exception)
            {
                return "Error Retrieving Field";
            }
        }

        /// <summary>
        /// Gets a location field value as a string suitable for things like, oh, a velocity template
        /// </summary>
        /// <param name="field">A FormFields enum value</param>
        /// <returns>The value of the field, OR, and error message</returns>
        private string GetLocFieldAsString(FormFields field)
        {
            CountryCityStateLocationSearchParams cLoc = LocationParams as CountryCityStateLocationSearchParams;
            HospitalLocationSearchParams hLoc = LocationParams as HospitalLocationSearchParams;
            ZipCodeLocationSearchParams zLoc = LocationParams as ZipCodeLocationSearchParams;

            switch (field)
            {
                case FormFields.AtNIH:
                    {
                        if (Location == LocationType.AtNIH)
                        {
                            return "Yes";
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                case FormFields.City:
                    {
                        return cLoc.City;
                    }
                case FormFields.Country:
                    {
                        return cLoc.Country;
                    }
                case FormFields.State:
                    {
                        return String.Join(", ", cLoc.State.Select(st => st.Label));
                    }
                case FormFields.Hospital:
                    {
                        return hLoc.Hospital;
                    }

                case FormFields.ZipCode:
                    {
                        return zLoc.ZipCode;
                    }
                case FormFields.ZipRadius:
                    {
                        return zLoc.ZipRadius.ToString();
                    }
                default:
                    {
                        throw new Exception();
                    }
            }
        }


        /// <summary>
        /// Gets or sets the main cancer type that was selected.
        /// </summary>
        public TerminologyFieldSearchParam MainType
        {
            get { return _mainType; }
            set { _mainType = value; _usedFields |= FormFields.MainType; }
        }

        /// <summary>
        /// Gets or sets an array of the subtypes for this search definition
        /// </summary>
        public TerminologyFieldSearchParam[] SubTypes {
            get { return _subTypes; }
            set { _subTypes = value; _usedFields |= FormFields.SubTypes; }
        }

        /// <summary>
        /// Gets or sets an array of the stages for this search definition
        /// </summary>
        public TerminologyFieldSearchParam[] Stages {
            get { return _stages; }
            set { _stages = value; _usedFields |= FormFields.Stages; }
        }

        /// <summary>
        /// Gets or sets an array of the findings for this search definition
        /// </summary>
        public TerminologyFieldSearchParam[] Findings {
            get { return _findings; }
            set { _findings = value; _usedFields |= FormFields.Findings; }
        }

        /// <summary>
        /// Gets or sets the age for this search definition
        /// </summary>
        public int Age {
            get { return _age; }
            set { _age = value; _usedFields |= FormFields.Age; }
        }

        /// <summary>
        /// Gets or sets the gender for this search definition
        /// </summary>
        public String Gender {
            get { return _gender;  }
            set { _gender = value; _usedFields |= FormFields.Gender; }
        }

        /// <summary>
        /// Gets or sets the Phrase/Keyword used in the search
        /// </summary>
        public String Phrase {
            get { return _phrase; }
            set { _phrase = value; _usedFields |= FormFields.Phrase; } 
        }

        public HealthyVolunteerType HealthyVolunteer
        {
            get { return _healthyVols; }
            set { _healthyVols = value; _usedFields |= FormFields.HealthyVolunteers; }
        }

        /// <summary>
        /// Gets or sets an indicator if we need to search for trials with VA sites
        /// </summary>
        public bool IsVAOnly
        {
            get { return _isVAOnly; }
            set { _isVAOnly = value; _usedFields |= FormFields.IsVAOnly; }
        }

        /// <summary>
        /// Gets or sets the location type value
        /// </summary>
        public LocationType Location {
            get 
            { 
                return _locationType; 
            }
            set 
            { 
                _locationType = value; 
                if(value != LocationType.None)
                {
                    _usedFields |= FormFields.Location;
                }
            } 
        }

        /// <summary>
        /// Gets or sets the parameters for the selected location type
        /// </summary>
        public LocationSearchParams LocationParams
        {
            get { return _locationParams; }
            set { _locationParams = value; }
        }

        /// <summary>
        /// Gets or sets an array of the trial types in the search
        /// </summary>
        public LabelledSearchParam[] TrialTypes {
            get { return _trialTypes; }
            set { _trialTypes = value; _usedFields |= FormFields.TrialTypes; } 
        }

        /// <summary>
        /// Gets or sets an array of the drugs for this search definition
        /// </summary>
        public TerminologyFieldSearchParam[] Drugs {
            get { return _drugs; }
            set { _drugs = value; _usedFields |= FormFields.Drugs; } 
        }

        /// <summary>
        /// Gets or sets an array of the other treatments for this search definition
        /// </summary>
        public TerminologyFieldSearchParam[] OtherTreatments {
            get { return _otherTreatments; }
            set { _otherTreatments = value; _usedFields |= FormFields.OtherTreatments; } 
        }

        /// <summary>
        /// Gets or sets an array of the trial phases in the search
        /// </summary>
        public LabelledSearchParam[] TrialPhases {
            get { return _trialPhases; }
            set { _trialPhases = value; _usedFields |= FormFields.TrialPhases; }
        }

        /// <summary>
        /// Gets or sets an array of the trial IDs in the search
        /// </summary>
        public string[] TrialIDs {
            get { return _trialIDs; }
            set { _trialIDs = value; _usedFields |= FormFields.TrialIDs; }
        }

        /// <summary>
        /// Gets or sets the Investigator used in the search
        /// </summary>
        public String Investigator {
            get { return _investigator; }
            set { _investigator = value; _usedFields |= FormFields.Investigator; }
        }

        /// <summary>
        /// Gets or sets the lead org used in the search
        /// </summary>
        public String LeadOrg {
            get { return _leadOrg; }
            set { _leadOrg = value; _usedFields |= FormFields.LeadOrg; }
        }

        /// <summary>
        /// Gets or sets the results link flag for the search
        /// </summary>
        public ResultsLinkType ResultsLinkFlag
        {
            get { return _resultsLinkFlag; }
            set { _resultsLinkFlag = value; }
        }

        /// <summary>
        /// Gets or sets the redirect flag for the results/view
        /// </summary>
        public bool RedirectFlag
        {
            get { return _redirectFlag; }
            set { _redirectFlag = value; }
        }

        /// <summary>
        /// Gets or sets an array of CTS Search Param Errors to identify when a parse error occurred.
        /// </summary>
        public List<CTSSearchParamError> ParseErrors { 
            get { return _parseErrors;}
            set { _parseErrors = value; }
        }

    }
}
