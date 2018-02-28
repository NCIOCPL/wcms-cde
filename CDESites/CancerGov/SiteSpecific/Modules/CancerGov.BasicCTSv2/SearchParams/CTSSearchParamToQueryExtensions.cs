using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    public static class CTSSearchParamToQueryExtensions
    {
        /// <summary>
        /// Helper function to convert between CTSSearchParams and filter criteria for the API
        /// </summary>
        /// <param name="searchParams">The search parameters</param>
        /// <returns>Filter criteria to perform that search</returns>
        public static Dictionary<string, object> ToFilterCriteria(this CTSSearchParams searchParams)
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

                foreach (string phase in searchParams.TrialPhases.Select(tp => tp.Key))
                {
                    phases.Add(phase);

                    switch (phase)
                    {
                        case "i":
                            {
                                if (!phases.Contains("i_ii"))
                                {
                                    phases.Add("i_ii");
                                }
                                break;
                            }
                        case "ii":
                            {
                                if (!phases.Contains("i_ii"))
                                {
                                    phases.Add("i_ii");
                                }
                                if (!phases.Contains("ii_iii"))
                                {
                                    phases.Add("ii_iii");
                                }
                                break;
                            }
                        case "iii":
                            {
                                if (!phases.Contains("ii_iii"))
                                {
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
            filterCriteria.Add("current_trial_status", CTSConstants.ActiveTrialStatuses);

            return filterCriteria;
        }

        /// <summary>
        /// Adds criteria to only match locations that are actively recruiting sites.  Only adds the filter if it has not been added before.
        /// </summary>
        /// <param name="filterCriteria"></param>
        private static void FilterActiveSites(Dictionary<string, object> filterCriteria)
        {
            if (!filterCriteria.ContainsKey("sites.recruitment_status"))
            {
                filterCriteria.Add("sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses);
            }
        }

    }
}
