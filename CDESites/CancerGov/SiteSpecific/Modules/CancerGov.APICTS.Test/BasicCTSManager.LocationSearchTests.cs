using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Moq;
using CancerGov.ClinicalTrialsAPI;

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{
    public partial class BasicCTSManager_Test
    {
        //Placeholder for testing CTSSearchParam -> FilterCriteria mapping.
        public static IEnumerable<object[]> LocationFieldsMappingData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    // TEST 15 - Is location NIH?  
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.AtNIH,
                            LocationParams = new AtNIHLocationSearchParams()
                        },
                        new Dictionary<string, object> {
                            { "sites.org_postal_code", "20892" },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    // TEST 14 - Hospital                     
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "M D Anderson Cancer Center"
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_name_fulltext", "M D Anderson Cancer Center" },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    // TEST 11 - Country
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                Country = "United States"
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_country", "United States" },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    // TEST 12 - State 
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                }
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_state_or_province", new string[] { "MD" } },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    }, 
                    // TEST 13 - City
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Baltimore"
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_city", "Baltimore" },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    // TEST 13 - City & State
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Baltimore",
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                }
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_city", "Baltimore" },
                            { "sites.org_state_or_province", new string[] { "MD" } },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    // TEST 13 - City & State & country
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Baltimore",
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                },
                                Country = "United States"
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_country", "United States" },
                            { "sites.org_city", "Baltimore" },
                            { "sites.org_state_or_province", new string[] { "MD" } },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                ZipRadius = 500,
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            }
                        },
                        new Dictionary<string, object> {
                            { "sites.org_coordinates_lat", 39.0897 },
                            { "sites.org_coordinates_lon", -77.1798 },
                            { "sites.org_coordinates_dist", "500mi" },
                            { "sites.recruitment_status", CTSConstants.ActiveRecruitmentStatuses },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    }


                    /*
                    // TEST 9 - Zip code
                    new object[] { "?loc=1&z=20850", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams() {
                            ZipCode = "20850"
                        }
                    }},

                    // TEST 10 - Zip radius
                    new object[] { "?loc=1&z=20850&zp=500", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams() {
                            ZipCode = "20850",
                            ZipRadius = 500
                        }
                    }},



                    

                    */

                };
            }
        }

        /// <summary>
        /// CTSSearchParams -> filterCriterea mapping tests
        /// </summary>
        /// <param name="searchParams">An instance of a CTSSearchParams object</param>
        /// <param name="expectedCriteria">The expected criteria for the search</param>
        [Theory, MemberData("LocationFieldsMappingData")]
        public void MappingLocationFields(CTSSearchParams searchParams, Dictionary<string, object> expectedCriteria)
        {
            this.MappingTest(searchParams, expectedCriteria);
        }
    }
}
