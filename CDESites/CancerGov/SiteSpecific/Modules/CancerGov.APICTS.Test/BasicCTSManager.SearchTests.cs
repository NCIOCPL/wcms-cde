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
        public static IEnumerable<object[]> OtherFieldsMappingData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //This array of objects maps to the parameters of the create method.
                    //URL at index 0, Expected object at index 1.
                    //TODO: fill out the rest of these tests
                    //TODO: get the tests to actually work - still having the equals/equivalent
                    //      errors with array comparer 
                    
                    
                    // TEST 0 - No parameters.
                    new object[] { 
                        new CTSSearchParams(),
                        new Dictionary<string, object> {
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    // TEST 5 - Age
                    new object[] {
                        new CTSSearchParams() {
                            Age = 35
                        },
                        new Dictionary<string,object> {
                            {"eligibility.structured.max_age_in_years_gte", 35},
                            {"eligibility.structured.min_age_in_years_lte", 35},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    
                    // TEST 6 - Gender
                    new object[] {
                        new CTSSearchParams() {
                            Gender = "male"
                        },
                        new Dictionary<string,object> {
                            {"eligibility.structured.gender", "male" },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    

                    // TEST 7 - Phrase/keyword
                    new object[] {
                        new CTSSearchParams() {
                            Phrase = "chicken"
                        },
                        new Dictionary<string, object>() {
                            { "_fulltext", "chicken" },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    // TEST 16 - Trial type
                    new object[] {
                        new CTSSearchParams() {
                            TrialTypes = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "basic_science",
                                    Label = "Basic science"
                                }
                            }
                        },
                        new Dictionary<string, object>() {
                            { "primary_purpose.primary_purpose_code", new string[] { "basic_science" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialTypes = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "basic_science",
                                    Label = "Basic science"
                                },
                                new LabelledSearchParam() {
                                    Key = "treatment",
                                    Label = "Treatment"
                                },
                            }
                        },
                        new Dictionary<string, object>() {
                            { "primary_purpose.primary_purpose_code", new string[] { "basic_science", "treatment" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    // TEST 21 - Principal investigator 
                    new object[] {
                        new CTSSearchParams() {
                            Investigator = "Sophia Smith"
                        },
                        new Dictionary<string, object>() {
                            { "principal_investigator_fulltext", "Sophia Smith"},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },

                    // TEST 22 - Lead organization
                    new object[] {
                        new CTSSearchParams() {
                            LeadOrg = "Mayo Clinic"
                        },
                        new Dictionary<string, object>() {
                            { "lead_org_fulltext", "Mayo Clinic"},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialIDs = new string[] { "NCI-2015-00054" }
                        },
                        new Dictionary<string, object>() {
                            { "_trialids", new string[] {"NCI-2015-00054"} },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialIDs = new string[] { "SWOG", "CCOG" }
                        },
                        new Dictionary<string, object>() {
                            { "_trialids", new string[] { "SWOG", "CCOG" } },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            IsVAOnly = true
                        },
                        new Dictionary<string, object>() {
                            { "sites.org_va", true },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },                    
                    new object[] {
                        new CTSSearchParams() {
                            IsVAOnly = false
                        },
                        new Dictionary<string, object>() {
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Any
                        },
                        new Dictionary<string, object>() {
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Healthy
                        },
                        new Dictionary<string, object>() {
                            { "accepts_healthy_volunteers_indicator", "YES" },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Infirmed
                        },
                        new Dictionary<string, object>() {
                            { "accepts_healthy_volunteers_indicator", "NO" },
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    /*


                    // TEST 20 - Trial ID 
                    new object[] {"tid=NCI-2014-01509", new CTSSearchParams() {
                        TrialIDs = new string[] {"NCI-2014-01509"}
                    }},


                    // TEST 23 - Page number
                    new object[] { "?pn=3", new CTSSearchParams() {
                        Page = 3
                    }},

                    // TEST 24 - Items per page
                    new object[] { "?ni=25", new CTSSearchParams() {
                        ItemsPerPage = 25
                    }}
                    */
                };
            }
        }

        /// <summary>
        /// CTSSearchParams -> filterCriterea mapping tests
        /// </summary>
        /// <param name="searchParams">An instance of a CTSSearchParams object</param>
        /// <param name="expectedCriteria">The expected criteria for the search</param>
        [Theory, MemberData("OtherFieldsMappingData")]
        public void MappingOtherFields(CTSSearchParams searchParams, Dictionary<string,object> expectedCriteria)
        {
            this.MappingTest(searchParams, expectedCriteria);
        }

        #region Phase Field Tests

        public static IEnumerable<object[]> PhaseFieldMappingData
        {
            get
            {
                return new[] {
                    // TEST 19 - Trial phase 
                    new object[] {
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "i",
                                    Label = "I"
                                }
                            }
                        },
                        new Dictionary<string, object>() {
                            { "phase.phase", new string[] { "i", "i_ii" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "ii",
                                    Label = "II"
                                }
                            }
                        },
                        new Dictionary<string, object>() {
                            { "phase.phase", new string[] { "ii", "i_ii", "ii_iii" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "iii",
                                    Label = "III"
                                }
                            }
                        },
                        new Dictionary<string, object>() {
                            { "phase.phase", new string[] { "iii", "ii_iii" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "iv",
                                    Label = "IV"
                                }
                            }
                        },
                        new Dictionary<string, object>() {
                            { "phase.phase", new string[] { "iv" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    new object[] {
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "i",
                                    Label = "I"
                                },
                                new LabelledSearchParam() {
                                    Key = "ii",
                                    Label = "II"
                                },
                                new LabelledSearchParam() {
                                    Key = "iii",
                                    Label = "III"
                                },
                                new LabelledSearchParam() {
                                    Key = "iv",
                                    Label = "IV"
                                }
                            }
                        },
                        new Dictionary<string, object>() {
                            { "phase.phase", new string[] { "i", "i_ii", "ii", "ii_iii", "iii", "iv" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// CTSSearchParams -> filterCriterea mapping tests
        /// </summary>
        /// <param name="searchParams">An instance of a CTSSearchParams object</param>
        /// <param name="expectedCriteria">The expected criteria for the search</param>
        [Theory, MemberData("PhaseFieldMappingData")]
        public void MappingPhaseFields(CTSSearchParams searchParams, Dictionary<string, object> expectedCriteria)
        {
            this.MappingTest(searchParams, expectedCriteria);
        }

        #endregion

    }
}
