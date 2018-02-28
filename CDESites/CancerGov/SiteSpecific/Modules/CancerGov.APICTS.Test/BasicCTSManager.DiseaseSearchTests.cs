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
        public static IEnumerable<object[]> DiseaseFieldsMappingData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    // TEST 1 - Main Cancer Type
                    new object[] {
                        new CTSSearchParams() {
                            MainType = new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C4872" },
                                Label = "Breast Cancer"
                            }
                        },
                        new Dictionary<string, object>{
                            { "_maintypes", new string[] { "C4872" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One maintype, one code
                    new object[] {
                        new CTSSearchParams() {
                            MainType = new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C4872", "C6789" },
                                Label = "TEST Cancer"
                            }
                        },
                        new Dictionary<string, object>{
                            { "_maintypes", new string[] { "C4872", "C6789" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One Subtype/One Code
                    new object[] {
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C4001" },
                                    Label = "Inflammatory Breast Cancer"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_subtypes", new string[] { "C4001" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One Subtype/Two Code
                    new object[] {
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C5678", "C1234" },
                                    Label = "Test Subtype Cancer"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_subtypes", new string[] { "C5678", "C1234" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //Two Subtype/One and Two
                    new object[] {
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C4001" },
                                    Label = "Inflammatory Breast Cancer"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C5678", "C1234" },
                                    Label = "Test Subtype Cancer"
                                }

                            }
                        },
                        new Dictionary<string, object>{
                            { "_subtypes", new string[] { "C4001", "C5678", "C1234" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One Stage/One Code
                    new object[] {
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C9246" },
                                    Label = "Stage IIIB Inflammatory Breast Cancer"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_stages", new string[] { "C9246" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One Stage/Two Code
                    new object[] {
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C12345", "C678910" },
                                    Label = "Test Cancer Stage"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_stages", new string[] { "C12345", "C678910" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //Two Stage/One and Two
                    new object[] {
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C9246" },
                                    Label = "Stage IIIB Inflammatory Breast Cancer"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C12345", "C678910" },
                                    Label = "Test Cancer Stage"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_stages", new string[] { "C9246", "C12345", "C678910" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One Finding/One Code
                    new object[] {
                        new CTSSearchParams() {
                            Findings = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C1234" },
                                    Label = "Test Finding"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_findings", new string[] { "C1234" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //One Finding/Two Code
                    new object[] {
                        new CTSSearchParams() {
                            Findings = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C2345", "C3456" },
                                    Label = "Test Finding 2"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_findings", new string[] { "C2345", "C3456" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //Two Findings/One and Two
                    new object[] {
                        new CTSSearchParams() {
                            Findings = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C1234" },
                                    Label = "Test Finding"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C2345", "C3456" },
                                    Label = "Test Finding 2"
                                }
                            }
                        },
                        new Dictionary<string, object>{
                            { "_findings", new string[] { "C1234", "C2345", "C3456" }},
                            { "current_trial_status", CTSConstants.ActiveTrialStatuses }
                        }
                    },
                    //TODO: Handle combo

                    //TODO: Test case for Subtype same as Stage, e.g. DCIS
                };
            }
        }


        /// <summary>
        /// CTSSearchParams -> filterCriterea mapping tests
        /// </summary>
        /// <param name="searchParams">An instance of a CTSSearchParams object</param>
        /// <param name="expectedCriteria">The expected criteria for the search</param>
        [Theory, MemberData("DiseaseFieldsMappingData")]
        public void MappingDiseaseFields(CTSSearchParams searchParams, Dictionary<string, object> expectedCriteria)
        {
            this.MappingTest(searchParams, expectedCriteria);
        }

    }
}
