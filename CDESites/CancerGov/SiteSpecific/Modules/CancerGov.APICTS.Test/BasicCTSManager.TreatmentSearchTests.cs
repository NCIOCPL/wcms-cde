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
        public static IEnumerable<object[]> TreatmentFieldsMappingData
        {
            get
            {
                // Array of tests
                return new[]
                {

                    // TEST 1 - One Drug
                    new object[] {
                        new CTSSearchParams() {
                            Drugs = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C1674" },
                                    Label = "Trastuzumab"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "arms.interventions.intervention_code", new string[] { "C1674" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //Two Drug
                    new object[] {
                        new CTSSearchParams() {
                            Drugs = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C1674" },
                                    Label = "Trastuzumab"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C1411" },
                                    Label = "Paclitaxel"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "arms.interventions.intervention_code", new string[] { "C1674", "C1411" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    // TEST 1 - One Other
                    new object[] {
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C17173" },
                                    Label = "Surgery"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "arms.interventions.intervention_code", new string[] { "C17173" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //Two Other
                    new object[] {
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C17173" },
                                    Label = "Surgery"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C15313" },
                                    Label = "Radiation Therapy"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "arms.interventions.intervention_code", new string[] { "C17173", "C15313" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //Combo
                    new object[] {
                        new CTSSearchParams() {
                            Drugs = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C1674" },
                                    Label = "Trastuzumab"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C1411" },
                                    Label = "Paclitaxel"
                                }
                            },
                            OtherTreatments = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C17173" },
                                    Label = "Surgery"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[]{ "C15313" },
                                    Label = "Radiation Therapy"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "arms.interventions.intervention_code", new string[] { "C1674", "C1411", "C17173", "C15313" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },                    
                    
                    //,
                    //TODO: Handle Cancer subtypes, stages and findings
                    // with multiple IDs too!
                    /*
                    // TEST 2 - Cancer subtype
                    new object[] {"?st=C7771", new CTSSearchParams() {
                        SubTypes = new TerminologyFieldSearchParam[] { 
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C7771" },
                                Label = "Recurrent Breast Cancer"
                            }
                        }
                    }},

                    // TEST 3 - Cancer stage
                    new object[] {"?stg=C88375", new CTSSearchParams() {
                        Stages = new TerminologyFieldSearchParam[] { 
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C88375" },
                                Label = "Stage I Breast Cancer"
                            }
                        }
                    }},

                    // TEST 4 - Cancer findings 
                    new object[] {"?fin=C26696", new CTSSearchParams() {
                        Findings = new TerminologyFieldSearchParam[] { 
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C26696" },
                                Label = "Anxiety"
                            }
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
        [Theory, MemberData("TreatmentFieldsMappingData")]
        public void MappingTreatmentFields(CTSSearchParams searchParams, Dictionary<string, object> expectedCriteria)
        {
            this.MappingTest(searchParams, expectedCriteria);
        }

    }
}
