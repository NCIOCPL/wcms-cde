using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Moq;
using NCI.Web;

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{
    /// <summary>
    /// Tests for the CTSearchParamFactory
    /// </summary>
    public partial class CTSSearchParamFactory_Test
    {

        //Placeholder for testing CTSSearchParam -> FilterCriteria mapping.
        public static IEnumerable<object[]> SerializeParamsData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //This array of objects maps to the parameters of the create method.
                    //URL at index 0, Expected object at index 1.
                    
                    // TEST 0 - No parameters.
                    new object[] { 
                        new CTSSearchParams() {
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "rl", "1" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 1 - Main Cancer Type
                    new object[] { 
                        new CTSSearchParams() {
                            MainType = new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C4872" },
                                Label = "Breast Cancer"
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "t", "C4872"},
                                { "rl", "1" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 2 - Main Cancer Type
                    new object[] { 
                        new CTSSearchParams() {
                            MainType = new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C3995", "C4872" },
                                Label = "Stage IV Breast Cancer"
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "t", "C3995|C4872"},
                                { "rl", "1" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 3 - Cancer SubType
                    new object[] { 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771" },
                                    Label = "Recurrent Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "st", "C7771"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 4 - Cancer SubType
                    new object[] { 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C4001", "C7771" },
                                    Label = "Recurrent Inflammatory Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "st", "C4001|C7771"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 5 - Cancer SubType
                    new object[] { 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C4001", "C7771" },
                                    Label = "Recurrent Inflammatory Breast Cancer"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771" },
                                    Label = "Recurrent Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "st", "C4001|C7771,C7771"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 6 - Cancer Stage
                    new object[] { 
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7768" },
                                    Label = "Stage II Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "stg", "C7768"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 7 - Cancer Stages
                    new object[] { 
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C5454", "C7768" },
                                    Label = "Stage II Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "stg", "C5454|C7768"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 8 - Cancer Stages
                    new object[] { 
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C3641" },
                                    Label = "Stage 0 Breast Cancer"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C5454", "C7768" },
                                    Label = "Stage II Breast Cancer"
                                },
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "stg", "C3641,C5454|C7768"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 9 - Findings
                    new object[] { 
                        new CTSSearchParams() {
                            Findings = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C26696" },
                                    Label = "Anxiety"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "fin", "C26696"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 10 - Findings
                    new object[] { 
                        new CTSSearchParams() {
                            Findings = new TerminologyFieldSearchParam[] {
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C26696" },
                                    Label = "Anxiety"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C35014" },
                                    Label = "Separation Anxiety Disorder"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "fin", "C26696,C35014"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 11 - Age
                    new object[] {
                        new CTSSearchParams() {
                            Age = 35,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },                        
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "a", "35" },
                                { "rl", "1" },
                                { "loc", "0" }
                            }
                        }
                    },
                     
                    // TEST 12 - Phrase/keyword
                    new object[] {
                        new CTSSearchParams() {
                            Phrase = "chicken",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "q", "chicken" },
                                { "rl", "1" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 13 - Gender
                    new object[] {
                        new CTSSearchParams() {
                            Gender = "male",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "g", "male" },
                                { "rl", "1" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 14 - Location
                    new object[] {
                        new CTSSearchParams() {
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "rl", "1" }
                            }
                        }
                    },
                    
                    // TEST 15 - Zip code
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "1" },
                                { "z", "20850" },
                                { "zp", "100" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 16 - Zip radius
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                ZipRadius = 500,
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "1" },
                                { "z", "20850" },
                                { "zp", "500" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 17 - Country
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                Country = "United States"
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "2" },
                                { "lcnty", "United+States" },
                                { "rl", "2" }
                            }
                        }
                    },

                    // TEST 18 - State 
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
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "2" },
                                { "lst", "MD" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 19 - States
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    },
                                    new LabelledSearchParam() {
                                        Key = "VA",
                                        Label = "Virginia"
                                    }
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "2" },
                                { "lst", "MD,VA" },
                                { "rl", "2" }
                            }
                        }
                    }, 
                    
                    // TEST 20 - City 
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Baltimore"
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "2" },
                                { "lcty", "Baltimore" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 21 - Hospital 
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "M D Anderson Cancer Center"
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "3" },
                                { "hos", "M+D+Anderson+Cancer+Center" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 22 - Is location NIH? 
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.AtNIH,
                            LocationParams = new AtNIHLocationSearchParams(),
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "4" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 23 - Zip code on basic page
                    new object[] { 
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                ZipRadius = 100,
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                        },
                        new NciUrl () {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "1" },
                                { "z", "20850" },
                                { "zp", "100" },
                                { "rl", "1" }
                            }
                        }
                    },
                    
                    // TEST 24 - Trial type
                    new object[] {
                        new CTSSearchParams() {
                            TrialTypes = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "basic_science",
                                    Label = "Basic science"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "tt", "basic_science"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 25 - Trial types
                    new object[] {
                        new CTSSearchParams() {
                            TrialTypes = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "basic_science",
                                    Label = "Basic science"
                                },
                                new LabelledSearchParam() {
                                    Key = "supportive_care",
                                    Label = "Supportive Care"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "tt", "basic_science,supportive_care"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 26 - Drugs
                    new object[] {
                        new CTSSearchParams() {
                            Drugs = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C1647" },
                                    Label = "Trastuzumab"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "d", "C1647"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },

                    // TEST 27 - Drugs
                    new object[] {
                        new CTSSearchParams() {
                            Drugs = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C1647" },
                                    Label = "Trastuzumab"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C2039" },
                                    Label = "Bevacizumab"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "d", "C1647,C2039"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 28 - Other treatments/interventions
                    new object[] {
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C131060" },
                                    Label = "Checkpoint Blockade Immunotherapy"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced,
                            Location = LocationType.None
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "i", "C131060"},
                                { "rl", "2" },
                                { "loc", "0" }
                            }
                        }
                    },
                    
                    // TEST 29 - Other treatments/interventions
                    new object[] {
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C131060" },
                                    Label = "Checkpoint Blockade Immunotherapy"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C26665" },
                                    Label = "Pomegranate Juice"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C107350", "C26665" },
                                    Label = "Pomegranate"
                                }
                            },
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {  
                                { "loc", "0" },
                                { "i", "C131060,C26665,C107350|C26665"},
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 30 - Trial phase 
                    new object[] {
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "i",
                                    Label = "I"
                                }
                            },
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "tp", "i" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 31 - Trial phase 
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
                                }
                            },
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "tp", "i,ii" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 32 - Trial ID
                    new object[] {
                        new CTSSearchParams() {
                            TrialIDs = new string[] {"NCI-2014-01509"},
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "tid", "NCI-2014-01509" },
                                { "rl", "2" }
                            }
                        }
                    },

                    // TEST 33 - Trial IDs
                    new object[] {
                        new CTSSearchParams() {
                            TrialIDs = new string[] {"NCI-2014-01509", "NCI-2014-0157"},
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "tid", "NCI-2014-01509,NCI-2014-0157" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 34 - Principal investigator 
                    new object[] { 
                        new CTSSearchParams() {
                            Investigator = "Sophia Smith",
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "in", "Sophia+Smith" },
                                { "rl", "2" }
                            }
                        }
                    },
                    
                    // TEST 35 - Lead organization
                    new object[] { 
                        new CTSSearchParams() {
                            LeadOrg = "Mayo Clinic",
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "loc", "0" },
                                { "lo", "Mayo+Clinic" },
                                { "rl", "2" }
                            }
                        }
                    },

                    // TEST 36a - Location
                    new object[] {
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "va", "1" },
                                { "loc", "0" },
                                { "rl", "1" }
                            }
                        }
                    },

                    // TEST 36b - Location
                    new object[] {
                        new CTSSearchParams() {
                            IsVAOnly = false,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "va", "0" },
                                { "loc", "0" },
                                { "rl", "1" }
                            }
                        }
                    },
                    // TEST 37a - Healthy Volunteers (any)
                    new object[] {
                        new CTSSearchParams() {
                            //Since this was set, a param will be generated. Not setting it and leaving the default will not, and
                            //that is tested by the empty test at the top of this file
                            HealthyVolunteer = HealthyVolunteerType.Any,  
                            IsVAOnly = false,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "hv", "0" },
                                { "va", "0" },
                                { "loc", "0" },
                                { "rl", "1" }
                            }
                        }
                    },
                    // TEST 37b - Healthy Volunteers (healthy only)
                    new object[] {
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Healthy,
                            IsVAOnly = false,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "hv", "1"},
                                { "va", "0" },
                                { "loc", "0" },
                                { "rl", "1" }
                            }
                        }
                    },
                    // TEST 37c - Healthy Volunteers (unhealthy only)
                    new object[] {
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Infirmed,
                            IsVAOnly = false,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        new NciUrl() {
                            QueryParameters = new Dictionary<string,string>() {
                                { "hv", "2"},
                                { "va", "0" },
                                { "loc", "0" },
                                { "rl", "1" }
                            }
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
        [Theory, MemberData("SerializeParamsData")]
        public void SerializeParams(CTSSearchParams searchParams, NciUrl expected)
        {
            //Get the results of parsing the URL
            NciUrl actual = CTSSearchParamFactory.ConvertParamsToUrl(searchParams);

            //Test the actual result to the expected.  NOTE: If you add fields to the CTSSearchParams, you need
            //to also modify the comparer
            Assert.Equal(expected.QueryParameters, actual.QueryParameters);
        }

          


    }
}
