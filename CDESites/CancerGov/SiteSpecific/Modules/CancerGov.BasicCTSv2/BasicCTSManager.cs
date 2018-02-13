using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CancerGov.ClinicalTrialsAPI;
using Newtonsoft.Json.Linq;
using MoreLinq;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public class BasicCTSManager
    {
        /// <summary>
        /// Gets the API client this instance of a BasicCTSManager will use
        /// </summary>
        private IClinicalTrialsAPIClient Client { get; set; }

        // CTRP trial statuses that qualify as "active"
        // These are used as filter criteria for returning trials on the results/view/listing pages.
        public static readonly string[] ActiveTrialStatuses = {
            // These CTRP statuses appear in results:
            "Active",
            "Approved", 
            "Enrolling by Invitation",
            "In Review",
            "Temporarily Closed to Accrual",
            "Temporarily Closed to Accrual and Intervention" 
            // These CTRP statuses DO NOT appear in results:
            /// "Administratively Complete",
            /// "Closed to Accrual",
            /// "Closed to Accrual and Intervention",
            /// "Complete",
            /// "Withdrawn"
        };

        // Site-specific recruitment statuses that qualify as "active" (not to be confused with the Trial Status).
        // These are used to filter available study sites on results/view/listing pages.
        public static readonly string[] ActiveRecruitmentStatuses = {
            // These statuses appear in results:
            "active",
            "approved", 
            "enrolling_by_invitation",
            "in_review",
            "temporarily_closed_to_accrual"
            // These statuses DO NOT appear in results:
            /// "closed_to_accrual",
            /// "completed",
            /// "administratively_complete",
            /// "closed_to_accrual_and_intervention",
            /// "withdrawn"
        };

        //Fields to include on returned search results list
        private static readonly string[] IncludeFields = {
            "nct_id",
            "nci_id",
            "brief_title",
            "sites.org_name",
            "sites.org_postal_code",
            "eligibility.structured",
            "current_trial_status",
            "sites.org_va",
            "sites.org_country",
            "sites.org_state_or_province",
            "sites.org_city",
            "sites.org_coordinates",
            "sites.recruitment_status",
            "diseases"
        };

        /// <summary>
        /// Creates a new instance of a BasicCTSManager
        /// </summary>
        /// <param name="host">The hostname of the API</param>        
        [Obsolete("This will need to be retired before SDS")]
        public BasicCTSManager(string host)
            : this(APIClientHelper.GetV1ClientInstance()){}

        /// <summary>
        /// Creates a new instance of a BasicCTSManager
        /// </summary>
        /// <param name="client"></param>
        public BasicCTSManager(IClinicalTrialsAPIClient client)
        {
            this.Client = client;
        }

        #region Public Methods

        /// <summary>
        /// Returns a Clinical Trial
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ClinicalTrial Get(string id)
        {
            ClinicalTrial trial = Client.Get(id);
            RemoveNonRecruitingSites(trial);
            return trial;
        }

        /// <summary>
        /// Performs a search against the Clinical Trials API
        /// </summary>
        /// <param name="searchParams">Search paramesters</param>
        /// <param name="dynamicFilterParams">Deserialized dynamic search parameters</param>
        /// <returns>Clinical Trials collection</returns>
        [Obsolete("This should be replaced with a more appropriate GetList for the listing pages.")]
        public ClinicalTrialsCollection Search(BaseCTSSearchParam searchParams, JObject dynamicFilterParams = null) {
            //Set page
            //Set size
            //Get only the fields we want
            //Get only "active" statuses
            //Then get additional filters based on type of search.

            //From starts at 0
            int from = 0;

            if (searchParams.Page > 1)
            {
                from = (searchParams.Page - 1) * searchParams.ItemsPerPage;
            }

            Dictionary<string, object> filterCriteria = new Dictionary<string, object>();

            //This is for only searching open trials.
            filterCriteria.Add("current_trial_status", ActiveTrialStatuses);

            if (searchParams.ZipLookup != null)
            {
                filterCriteria.Add("sites.org_coordinates_lat", searchParams.ZipLookup.GeoCode.Lat);
                filterCriteria.Add("sites.org_coordinates_lon", searchParams.ZipLookup.GeoCode.Lon);
                filterCriteria.Add("sites.org_coordinates_dist", searchParams.ZipRadius.ToString() + "mi");
                FilterActiveSites(filterCriteria);                
            }

            //Add Age Filter
            //<field>_gte, <field>_lte
            if (searchParams.Age != null)
            {
                filterCriteria.Add("eligibility.structured.max_age_in_years_gte", searchParams.Age);
                filterCriteria.Add("eligibility.structured.min_age_in_years_lte", searchParams.Age);
            }

            if (!String.IsNullOrEmpty(searchParams.Query))
            {
                filterCriteria.Add("_fulltext", searchParams.Query);
            }

            if (!String.IsNullOrEmpty(searchParams.Country))
            {
                filterCriteria.Add("sites.org_country", searchParams.Country);
                FilterActiveSites(filterCriteria);
            }

            if (!String.IsNullOrEmpty(searchParams.City))
            {
                filterCriteria.Add("sites.org_city", searchParams.City);
                FilterActiveSites(filterCriteria);
            }

            if (!String.IsNullOrEmpty(searchParams.State))
            {
                filterCriteria.Add("sites.org_state_or_province", searchParams.State);
                FilterActiveSites(filterCriteria);
            }
            
            // TBD
            if (!String.IsNullOrEmpty(searchParams.HospitalOrInstitution))
            {
                filterCriteria.Add("sites.org_name_fulltext", searchParams.HospitalOrInstitution);
                FilterActiveSites(filterCriteria);
            }
                              
            if (searchParams.TrialTypeArray != null)
            {
                filterCriteria.Add("primary_purpose.primary_purpose_code", searchParams.TrialTypeArray);
            }

            // Drug and Trial ID's are sent under the same key and should be grouped.
            List<string> drugAndTrialIds = new List<string>();
            if(searchParams.DrugIDs != null)
                drugAndTrialIds.AddRange(searchParams.DrugIDs);
            if(searchParams.TreatmentInterventionCodes != null)
                drugAndTrialIds.AddRange(searchParams.TreatmentInterventionCodes);
            if (drugAndTrialIds.Count > 0)
            {
                filterCriteria.Add("arms.interventions.intervention_code", drugAndTrialIds.ToArray());
            }

            // Array of strings
            if (searchParams.TrialPhaseArray != null)
            {
                filterCriteria.Add("phase.phase", searchParams.TrialPhaseArray);
            }

            if (searchParams.NewTrialsOnly)
            {
                filterCriteria.Add("start_date_gte", searchParams.NewTrialsOnly);
            }

            if (!String.IsNullOrEmpty(searchParams.PrincipalInvestigator))
            {
                filterCriteria.Add("principal_investigator_fulltext", searchParams.PrincipalInvestigator);
            }

            if (!String.IsNullOrEmpty(searchParams.LeadOrganization))
            {
                filterCriteria.Add("lead_org_fulltext", searchParams.LeadOrganization);
            }

            //Add Gender Filter
            if (!String.IsNullOrWhiteSpace(searchParams.Gender))
            {
                filterCriteria.Add("eligibility.structured.gender", searchParams.Gender);
            }

            if (searchParams.TrialIDs != null)
            {
                filterCriteria.Add("_trialids", searchParams.TrialIDs);
            }

            if (searchParams.CancerFindings != null)
            {
                filterCriteria.Add("diseases.display_name", searchParams.CancerFindings);
            }


            //Add phrase if this is a phrase search
            if (searchParams is PhraseSearchParam)
            {
                filterCriteria.Add("_fulltext", ((PhraseSearchParam)searchParams).Phrase);
            }
            else if (searchParams is CancerTypeSearchParam)
            {
                //This is now an array of codes.
                filterCriteria.Add("diseases.nci_thesaurus_concept_id", ((CancerTypeSearchParam)searchParams).CancerTypeIDs);
            }

            //Get our list of trials from the API client
            ClinicalTrialsCollection rtnResults = new ClinicalTrialsCollection();
            if (dynamicFilterParams != null) // get results with passed in params
            {
                //JObject ddSearchParams = GetDeserializedJSON(dynamicFilterParams);
                rtnResults = Client.GetTrialsList(
                    size: searchParams.ItemsPerPage,
                    from: from,
                    searchParams: filterCriteria,
                    dynamicSearchParams: dynamicFilterParams
                );
            }
            else // get default results
            { 
                rtnResults = Client.List(
                    size: searchParams.ItemsPerPage,
                    from: from,
                    includeFields: IncludeFields,
                    searchParams: filterCriteria
                );
            }

            foreach(ClinicalTrial trial in rtnResults.Trials)
            {
                RemoveNonRecruitingSites(trial);
            }

            return rtnResults;

        }

        /// <summary>
        /// Performs a search against the Clinical Trials API
        /// </summary>
        /// <param name="searchParams">Search paramesters</param>
        /// <param name="dynamicFilterParams">Deserialized dynamic search parameters</param>
        /// <returns>Clinical Trials collection</returns>
        public ClinicalTrialsCollection Search(CTSSearchParams searchParams, int pageNumber = 0, int itemsPerPage = 10)
        {
            //TODO: Determine if the searchParams really are the best place for the pager.  I am thinking NO.
            int from = 0;

            if (pageNumber > 1)
            {
                from = (pageNumber - 1) * itemsPerPage;
            }

            Dictionary<string, object> filterCriteria = MapSearchParamsToFilterCriteria(searchParams);

            //Get our list of trials from the API client
            ClinicalTrialsCollection rtnResults = new ClinicalTrialsCollection();

            //Fetch results
            rtnResults = Client.List(
                size: itemsPerPage,
                from: from,
                includeFields: IncludeFields,
                searchParams: filterCriteria
            );

            //Remove all the inactive sites from all the trials.
            foreach (ClinicalTrial trial in rtnResults.Trials)
            {
                RemoveNonRecruitingSites(trial);
            }

            return rtnResults;
        }

        /// <summary>
        /// Helper function to convert between CTSSearchParams and filter criteria for the API
        /// </summary>
        /// <param name="searchParams">The search parameters</param>
        /// <returns>Filter criteria to perform that search</returns>
        private Dictionary<string, object> MapSearchParamsToFilterCriteria(CTSSearchParams searchParams)
        {
            Dictionary<string, object> filterCriteria = new Dictionary<string, object>();

            //Diseases
            if (searchParams.IsFieldSet(FormFields.MainType))
            {
                filterCriteria.Add("_maintypes", searchParams.MainType.Codes);
            }

            if (searchParams.IsFieldSet(FormFields.SubTypes))
            {
                filterCriteria.Add("_subtypes", searchParams.SubTypes.SelectMany(st => st.Codes).ToArray());
            }

            if (searchParams.IsFieldSet(FormFields.Stages))
            {
                filterCriteria.Add("_stages", searchParams.Stages.SelectMany(st => st.Codes).ToArray());
            }

            if (searchParams.IsFieldSet(FormFields.Findings))
            {
                filterCriteria.Add("_findings", searchParams.Findings.SelectMany(st => st.Codes).ToArray());
            }

            //For Sept 2017 SDS release we will combine drug and other using an OR query.  Future releases should
            //use AND between Drugs and Other.
            if (searchParams.IsFieldSet(FormFields.Drugs) || searchParams.IsFieldSet(FormFields.OtherTreatments))
            {
                // Drug and Trial ID's are sent under the same key and should be grouped.
                List<string> drugAndTrialIds = new List<string>();

                if (searchParams.IsFieldSet(FormFields.Drugs))
                {
                    drugAndTrialIds.AddRange(searchParams.Drugs.SelectMany(d => d.Codes));
                }
                if (searchParams.IsFieldSet(FormFields.OtherTreatments))
                {
                    drugAndTrialIds.AddRange(searchParams.OtherTreatments.SelectMany(ot => ot.Codes));
                }

                if (drugAndTrialIds.Count > 0)
                {
                    filterCriteria.Add("arms.interventions.intervention_code", drugAndTrialIds.ToArray());
                }
            }

            //Add Age Filter
            //<field>_gte, <field>_lte
            if (searchParams.IsFieldSet(FormFields.Age))
            {
                filterCriteria.Add("eligibility.structured.max_age_in_years_gte", searchParams.Age);
                filterCriteria.Add("eligibility.structured.min_age_in_years_lte", searchParams.Age);
            }

            if (searchParams.IsFieldSet(FormFields.Phrase))
            {
                filterCriteria.Add("_fulltext", searchParams.Phrase);
            }

            if (searchParams.IsFieldSet(FormFields.TrialTypes))
            {
                filterCriteria.Add("primary_purpose.primary_purpose_code", searchParams.TrialTypes.Select(tt => tt.Key).ToArray());
            }

            // Array of strings
            if (searchParams.IsFieldSet(FormFields.TrialPhases))
            {
                //We must expand the phases into the i_ii and ii_iii trials.
                List<string> phases = new List<string>();

                foreach(string phase in searchParams.TrialPhases.Select(tp => tp.Key)) 
                {
                    phases.Add(phase);

                    switch(phase) {                        
                        case "i" : {
                            if (!phases.Contains("i_ii")) {
                                phases.Add("i_ii");
                            }
                            break;
                        }
                        case "ii" : {
                            if (!phases.Contains("i_ii")) {
                                phases.Add("i_ii");
                            }
                            if (!phases.Contains("ii_iii")) {
                                phases.Add("ii_iii");
                            }
                            break;
                        }
                        case "iii" : {
                            if (!phases.Contains("ii_iii")) {
                                phases.Add("ii_iii");
                            }
                                break;
                        }
                    }
                }
                
                filterCriteria.Add("phase.phase", phases.ToArray());
            }

            if (searchParams.IsFieldSet(FormFields.Investigator))
            {
                filterCriteria.Add("principal_investigator_fulltext", searchParams.Investigator);
            }

            if (searchParams.IsFieldSet(FormFields.LeadOrg))
            {
                filterCriteria.Add("lead_org_fulltext", searchParams.LeadOrg);
            }

            //Add Healthy Volunteers Filter
            if (searchParams.IsFieldSet(FormFields.HealthyVolunteers) && (searchParams.HealthyVolunteer != HealthyVolunteerType.Any))
            {
                if (searchParams.HealthyVolunteer == HealthyVolunteerType.Healthy)
                {
                    filterCriteria.Add("accepts_healthy_volunteers_indicator", "YES");
                }
                else if (searchParams.HealthyVolunteer == HealthyVolunteerType.Infirmed)
                {
                    filterCriteria.Add("accepts_healthy_volunteers_indicator", "NO");
                }
                //Else there is probably an issue, but we will assume not.
            }

            //Add Gender Filter
            if (searchParams.IsFieldSet(FormFields.Gender))
            {
                filterCriteria.Add("eligibility.structured.gender", searchParams.Gender);
            }

            if (searchParams.IsFieldSet(FormFields.TrialIDs))
            {
                filterCriteria.Add("_trialids", searchParams.TrialIDs);
            }

            //Add VA Only Filter
            if (searchParams.IsFieldSet(FormFields.IsVAOnly) && searchParams.IsVAOnly)
            {
                filterCriteria.Add("sites.org_va", true);
            }

            if (searchParams.IsFieldSet(FormFields.Location) && searchParams.Location != LocationType.None)
            {
                switch (searchParams.Location)
                {
                    case LocationType.AtNIH:
                        {
                            //NIH has their own postal code, so this means @NIH
                            filterCriteria.Add("sites.org_postal_code", "20892");
                            break;
                        }
                    case LocationType.Hospital:
                        {
                            filterCriteria.Add("sites.org_name_fulltext", ((HospitalLocationSearchParams)searchParams.LocationParams).Hospital);
                            break;
                        }
                    case LocationType.CountryCityState:
                        {
                            CountryCityStateLocationSearchParams locParams = (CountryCityStateLocationSearchParams)searchParams.LocationParams;

                            if (locParams.IsFieldSet(FormFields.Country))
                            {
                                filterCriteria.Add("sites.org_country", locParams.Country);
                            }

                            if (locParams.IsFieldSet(FormFields.City))
                            {
                                filterCriteria.Add("sites.org_city", locParams.City);
                            }
                            if (locParams.IsFieldSet(FormFields.State))
                            {
                                filterCriteria.Add("sites.org_state_or_province", locParams.State.Select(lst => lst.Key).ToArray());
                            }
                            break;
                        }
                    case LocationType.Zip: 
                        {
                            ZipCodeLocationSearchParams locParams = (ZipCodeLocationSearchParams)searchParams.LocationParams;
                   
                            if (locParams.IsFieldSet(FormFields.ZipCode))
                            {
                                filterCriteria.Add("sites.org_coordinates_lat", locParams.GeoLocation.Lat);
                                filterCriteria.Add("sites.org_coordinates_lon", locParams.GeoLocation.Lon);
                                filterCriteria.Add("sites.org_coordinates_dist", locParams.ZipRadius.ToString() + "mi");
                            }
                            break;
                        }
                    default:
                        {
                            throw new Exception(String.Format("Location type, {0} not supported.", searchParams.Location));
                        }
                }
                //All locations need filtering of active sites.
                FilterActiveSites(filterCriteria);
            }

            //This is for only searching open trials.
            filterCriteria.Add("current_trial_status", ActiveTrialStatuses);

            return filterCriteria;
        }

        /// <summary>
        /// Adds criteria to only match locations that are actively recruiting sites.  Only adds the filter if it has not been added before.
        /// </summary>
        /// <param name="filterCriteria"></param>
        private void FilterActiveSites(Dictionary<string, object> filterCriteria)
        {
            if (!filterCriteria.ContainsKey("sites.recruitment_status"))
            {
                filterCriteria.Add("sites.recruitment_status", ActiveRecruitmentStatuses);
            }
        }

        /// <summary>
        /// Gets the Geo Location for a ZipCode
        /// </summary>
        /// <param name="zipCode"></param>
        /// <returns></returns>
        //Removing as part of CTS SDS.  This comment should be removed once the old code is purged
        //public ZipLookup GetZipLookupForZip(string zipCode)
        //{
        //    // Setting the default latitude and longitude values for the middle of Antarctica.
        //    // This way, if a user enters a properly formatted, but invalid zip, we won't hit a 
        //    // null exception error when retrieving the coordinates.
        //    double latitude = -89.9999;
        //    double longitude = -89.9999;

        //    // Look up our ZipCodeGeoEntry object from the JSON reference file and pass in the 
        //    // coordinates if the mapping exists.
        //    ZipCodeGeoEntry zipEntry = ZipCodeGeoLookup.GetZipCodeGeoEntry(zipCode);
        //    if (zipEntry != null)
        //    {
        //        if (!Double.IsNaN(zipEntry.Latitude) && !Double.IsNaN(zipEntry.Longitude))
        //        {
        //            latitude = zipEntry.Latitude;
        //            longitude = zipEntry.Longitude;
        //        }
        //    }

        //    return new ZipLookup()
        //    {
        //        PostalCode_ZIP = zipCode,
        //        GeoCode = new GeoLocation(latitude, longitude)
        //    };
        //}

        /// <summary>
        /// Gets the cancer type display name the user selected
        /// </summary>
        /// <param name="cancertypeids">The IDs of the term</param>
        /// <param name="key">The term key of the unique menu name</param>
        /// <returns></returns>
        [Obsolete("With the new SDS release this will be obsolete.")]
        public string GetCancerTypeDisplayName(string[] cancertypeids, string key)
        {
            string displayName = string.Empty;

            if (!String.IsNullOrWhiteSpace(key))
            {
                Term term = Client.GetTerm(key);
                if (term == null)
                {
                    return GetCancerTypeDisplayName(cancertypeids, null);
                }
                else
                {
                    return term.DisplayText;
                }
            }
            else
            {
                //If we did not have a key, OR we did not find an entry
                //in either case we need to try and find it by ID.
                Dictionary<string, object> filterCriteria = new Dictionary<string, object>();

                //If we have a key, then look it up by key
                filterCriteria.Add("codes", cancertypeids);

                TermCollection rtnResults = Client.Terms(
                    size: 100,
                    from: 0,
                    searchParams: filterCriteria
                );

                if (rtnResults.TotalResults > 0)
                {
                    //This is hacky!!
                    //TODO: Clean this up and check for errors
                    //TODO: Some terms may have 2 IDs, this query will return anything with either term,
                    //not just the terms with both.  We should iterate through the results and find the first
                    //with both.
                    return rtnResults.Terms[0].DisplayText;
                }

            }

            return string.Empty; //Nothing found
        }

        /// <summary>
        /// Gets a collection of trials based on trial ids, batchVal at a time.
        /// </summary>
        /// <param name="ids">An array of trial IDs to fetch</param>
        /// <param name="batchVal">The number of trials to retrieve at a time</param>
        /// <returns>An enumerable list of ClinicalTrial objects</returns>
        public IEnumerable<ClinicalTrial> GetMultipleTrials(List<String> ids, int batchVal = 5)
        {
            foreach (IEnumerable<string> batch in ids.Batch(batchVal))
            {
                Dictionary<string, object> filterCriteria = new Dictionary<string, object>();
                filterCriteria.Add("nci_id", batch.ToArray());
                ClinicalTrialsCollection ctColl = new ClinicalTrialsCollection();

                ctColl = Client.List(
                    size: 100,
                    //from: 0,
                    searchParams: filterCriteria
                );

                foreach (ClinicalTrial c in ctColl.Trials)                
                {
                    //Remove all the inactive sites from the trial.
                    RemoveNonRecruitingSites(c);
                    yield return c;

                }
            }            
        }


        

        #endregion

        #region Private Members

        /// <summary>
        /// Creates a list of actively recruiting sites only 
        /// </summary>
        /// <param name="trial">Clinical trial</param>
        private void RemoveNonRecruitingSites(ClinicalTrial trial)
        {
            if (trial.Sites != null)
            {
                trial.Sites = new List<ClinicalTrial.StudySite>(trial.Sites.Where(site => IsActivelyRecruiting(site)));
            }
        }

        /// <summary>
        /// Set to true if site status matches an item in ActiveRecruitmentStatuses
        /// </summary>
        /// <param name="site">Study site</param>
        private bool IsActivelyRecruiting(ClinicalTrial.StudySite site)
        {
            return ActiveRecruitmentStatuses.Any(status => status.ToLower() == site.RecruitmentStatus.ToLower());
        }

        #endregion

        


    }
}
