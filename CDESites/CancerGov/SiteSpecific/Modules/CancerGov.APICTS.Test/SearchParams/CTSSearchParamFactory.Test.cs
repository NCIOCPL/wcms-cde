using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using Moq;

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{
    /// <summary>
    /// Tests for the CTSearchParamFactory
    /// </summary>
    public partial class CTSSearchParamFactory_Test
    {
        //TODO: Test all combos, and ensure _usedFields is set correctly

        //Test cases for Create test method. 
        //@Sarina and @Dion - You will want to create your expected objects in another file at some point.
        public static IEnumerable<object[]> URLParsingData {
            get
            {
                // Array of tests
                return new[]
                {
                    //This array of objects maps to the parameters of the create method.
                    //URL at index 0, Expected object at index 1. 
                    
                    // TEST 0 - No parameters.
                    new object[] { "", 
                        new CTSSearchParams() {
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.None
                    },
                    
                    // TEST 1 - Main Cancer Type
                    new object[] {"?t=C4872", 
                        new CTSSearchParams() {
                            MainType = new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C4872" },
                                Label = "Breast Cancer"
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.MainType
                    },
                    
                    // TEST 2 - Main Cancer Type
                    new object[] {"?t=C4872|C3995", 
                        new CTSSearchParams() {
                            MainType = new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C3995", "C4872" },
                                Label = "Stage IV Breast Cancer"
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.MainType
                    },
                    
                    // TEST 3 - Cancer subtype
                    new object[] {"?st=C7771", 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771" },
                                    Label = "Recurrent Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.SubTypes
                    },
                    
                    // TEST 4 - Cancer subtype
                    new object[] {"?st=C7771|C4001", 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771", "C4001" },
                                    Label = "Recurrent Inflammatory Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.SubTypes
                    },
                    
                    // TEST 5 - Cancer subtypes
                    new object[] {"?st=C7771|C4001,C7771", 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771" },
                                    Label = "Recurrent Breast Cancer"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771", "C4001" },
                                    Label = "Recurrent Inflammatory Breast Cancer"
                                },
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.SubTypes
                    },
                    
                    // TEST 6 - Cancer stage
                    new object[] {"?stg=C7771|C4001", 
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C4001" , "C7771" },
                                    Label = "Recurrent Inflammatory Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Stages
                    },
                    
                    // TEST 7 - Cancer stages
                    new object[] {"?stg=c7771|c4001,c7771", 
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C4001" , "C7771" },
                                    Label = "Recurrent Inflammatory Breast Cancer"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7771" },
                                    Label = "Recurrent Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Stages
                    },
                    
                    // TEST 8 - Cancer stage
                    new object[] {"?stg=C88375", 
                        new CTSSearchParams() {
                            Stages = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C88375" },
                                    Label = "Stage I Breast Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Stages
                    },
                    
                    // TEST 9 - Cancer findings 
                    new object[] {"?fin=C26696", 
                        new CTSSearchParams() {
                            Findings = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C26696" },
                                    Label = "Anxiety"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Findings
                    },

                    // TEST 10 - Cancer findings 
                    new object[] {"?fin=C26696,C35014", 
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
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Findings
                    },

                    // TEST 11 - Age
                    new object[] { "?a=35", 
                        new CTSSearchParams() {
                            Age = 35,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Age
                    },

                    // TEST 12 - Gender
                    new object[] { "?g=male", 
                        new CTSSearchParams() {
                            Gender = "male",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Gender
                    },

                    // TEST 13 - Gender
                    new object[] { "?g=female", 
                        new CTSSearchParams() {
                            Gender = "female",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Gender
                    },

                    // TEST 14 - Phrase/keyword
                    new object[] { "?q=chicken", 
                        new CTSSearchParams() {
                            Phrase = "chicken",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Phrase
                    },

                    // TEST 15 - Location
                    new object[] { "?loc=0", 
                        new CTSSearchParams() {
                            Location = LocationType.None,
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        FormFields.None
                    },
                    
                    // TEST 16 - Zip code
                    new object[] { "?loc=1&z=20850&rl=2", 
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.ZipCode
                    },
                    
                    // TEST 17 - Zip radius
                    new object[] { "?loc=1&z=20850&zp=500&rl=2", 
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                ZipRadius = 500,
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.ZipRadius
                    },
                    
                    // TEST 18 - Country
                    new object[] { "?loc=2&lcnty=United+States&rl=2", 
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                Country = "United States"
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.Country
                    },

                    // TEST 19 - State 
                    new object[] { "?loc=2&lst=MD&rl=2", 
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
                        FormFields.State
                    },

                    // TEST 20 - States
                    new object[] { "?loc=2&lst=MD,VA&rl=2",
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
                        FormFields.State
                    }, 
                    
                    // TEST 21 - City 
                    new object[] { "?loc=2&lcty=Baltimore&lcnty=United+States&rl=2", 
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Baltimore",
                                Country = "United States"
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.City
                    },
                    
                    // TEST 22 - Hospital 
                    new object[] { "?loc=3&hos=M+D+Anderson+Cancer+Center&rl=2", 
                        new CTSSearchParams() {
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "M D Anderson Cancer Center"
                            },
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.Hospital
                    },
                    
                    // TEST 23 - Is location NIH? 
                    new object[] { "?loc=4&rl=2", 
                        new CTSSearchParams() {
                            Location = LocationType.AtNIH,
                            LocationParams = new AtNIHLocationSearchParams(),
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.AtNIH
                    },
                    
                    // TEST 24 - Zip code on basic page
                    new object[] { "?rl=1&z=20850", 
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20850",
                                ZipRadius = 100,
                                GeoLocation = new GeoLocation(39.0897, -77.1798)
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        FormFields.ZipCode
                    },

                    // TEST 25 - Results link flag
                    new object[] { "?rl=1", 
                        new CTSSearchParams() {
                            ResultsLinkFlag = ResultsLinkType.Basic
                        },
                        FormFields.None
                    },

                    // TEST 26 - Results link flag
                    new object[] { "?rl=2", 
                        new CTSSearchParams() {
                            ResultsLinkFlag = ResultsLinkType.Advanced
                        },
                        FormFields.None
                    },
                    
                    // TEST 27 - Trial type
                    new object[] {"?tt=basic_science", 
                        new CTSSearchParams() {
                            TrialTypes = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "basic_science",
                                    Label = "Basic Science"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialTypes
                    },
                    // TEST 28 - Trial types
                    new object[] {"?tt=basic_science,supportive_care", 
                        new CTSSearchParams() {
                            TrialTypes = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "basic_science",
                                    Label = "Basic Science"
                                },
                                new LabelledSearchParam() {
                                    Key = "supportive_care",
                                    Label = "Supportive Care"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialTypes
                    },
                    
                    // TEST 29 - Drug
                    new object[] {"?d=C1647", 
                        new CTSSearchParams() {
                            Drugs = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C1647" },
                                    Label = "Trastuzumab"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Drugs
                    },

                    // TEST 30 - Drugs
                    new object[] {"?d=C1647,C2039", 
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
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Drugs
                    },
                    
                    // TEST 31 - Other treatments/interventions
                    new object[] {"?i=C131060", 
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C131060" },
                                    Label = "Checkpoint Blockade Immunotherapy"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.OtherTreatments
                    },

                    // TEST 32 - Other treatments/interventions
                    new object[] {"?i=C131060,C26665", 
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C131060" },
                                    Label = "Checkpoint Blockade Immunotherapy"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C26665" },
                                    Label = "Pomegranate Juice"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.OtherTreatments
                    },
                    
                    // TEST 33 - Other treatments/interventions
                    new object[] {"?i=C131060,C107350|C26665", 
                        new CTSSearchParams() {
                            OtherTreatments = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C131060" },
                                    Label = "Checkpoint Blockade Immunotherapy"
                                },
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C26665" , "C107350" },
                                    Label = "Pomegranate"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.OtherTreatments
                    },
                    
                    // TEST 34 - Trial phase 
                    new object[] {"?tp=i", 
                        new CTSSearchParams() {
                            TrialPhases = new LabelledSearchParam[] { 
                                new LabelledSearchParam() {
                                    Key = "i",
                                    Label = "I"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialPhases
                    },
                   
                    // TEST 35 - Trial phases
                    new object[] {"?tp=i,ii",
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
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialPhases
                    },
                    
                    // TEST 36 - Trial ID 
                    new object[] {"?tid=NCI-2014-01509", 
                        new CTSSearchParams() {
                            TrialIDs = new string[] {"NCI-2014-01509"},
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialIDs
                    },

                    // TEST 37 - Trial IDs
                    new object[] {"?tid=NCI-2014-01509,NCI-2014-01507", 
                        new CTSSearchParams() {
                            TrialIDs = new string[] { "NCI-2014-01509", "NCI-2014-01507" },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialIDs
                    },

                    // TEST 38 - Trial IDs
                    new object[] {"?tid=NCI-2014-01509;NCI-2014-01507", 
                        new CTSSearchParams() {
                            TrialIDs = new string[] { "NCI-2014-01509", "NCI-2014-01507" },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.TrialIDs
                    },
                    
                    // TEST 39 - Principal investigator 
                    new object[] { "?in=Sophia+Smith",
                        new CTSSearchParams() {
                            Investigator = "Sophia Smith",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.Investigator
                    },

                    // TEST 40 - Lead organization
                    new object[] { "?lo=Mayo+Clinic",
                        new CTSSearchParams() {
                            LeadOrg = "Mayo Clinic",
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.LeadOrg
                    },
                    
                    // TEST 41 - Multiple C-codes that map to same label
                    new object[] { "?st=C7880,C7110,C7839", 
                        new CTSSearchParams() {
                            SubTypes = new TerminologyFieldSearchParam[] { 
                                new TerminologyFieldSearchParam() {
                                    Codes = new string[] { "C7110", "C7839", "C7880" },
                                    Label = "Recurrent Liver Cancer"
                                }
                            },
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.SubTypes
                    },

                    // TEST 42 - Redirect page flag
                    new object[] { "?r=1", 
                        new CTSSearchParams() {
                            RedirectFlag = true,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.SubTypes
                    },

                    // TEST 43 - Test VA Only
                    new object[] { "?va=1", 
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.IsVAOnly
                    },
                    //TEST 44a - Test Healthy Volunteers
                    new object[] { "?hv=1",
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Healthy,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.HealthyVolunteers
                    },
                    //TEST 44a - Test Healthy Volunteers
                    new object[] { "?hv=2",
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Infirmed,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.HealthyVolunteers
                    },
                    //TEST 44a - Test Healthy Volunteers
                    new object[] { "?hv=0",
                        new CTSSearchParams() {
                            HealthyVolunteer = HealthyVolunteerType.Any,
                            ResultsLinkFlag = ResultsLinkType.Basic,
                            Location = LocationType.None
                        },
                        FormFields.HealthyVolunteers
                    },


                };
            }
        }
        
        public static IEnumerable<object[]> FieldSetData {
            get
            {
                // Array of tests
                return new[]
                {
                    //This array of objects maps to the parameters of the create method.
                    //URL at index 0, Expected object at index 1. 
                    
                    // TEST 1 - all parameters with zip location
                    new object[] { "?t=C4872&st=C5214&stg=C36301&fin=C26696&a=25&q=PSA&loc=1&z=20850&zp=100&tt=treatment&d=C2039&i=C116464&tp=I&tid=NCI-2014-01509&in=Adam+David+Cohen&lo=Mayo+Clinic+Cancer+Center+LAO&va=1&rl=2",
                        new FormFields[] {
                            FormFields.MainType,
                            FormFields.SubTypes,
                            FormFields.Stages,
                            FormFields.Findings,
                            FormFields.Age,
                            FormFields.Phrase,
                            FormFields.Location,
                            FormFields.TrialTypes,
                            FormFields.Drugs,
                            FormFields.OtherTreatments,
                            FormFields.TrialPhases,
                            FormFields.LeadOrg,
                            FormFields.Investigator,
                            FormFields.IsVAOnly
                        },
                        new FormFields[] {
                            FormFields.ZipCode,
                            FormFields.ZipRadius
                        }
                    },

                    // TEST 2 - all parameters with country/city/state location
                    new object[] { "?t=C4872&st=C5214&stg=C36301&fin=C26696&a=25&q=PSA&loc=2&lcnty=United+States&lst=MD&lcty=Baltimore&tt=treatment&d=C2039&i=C116464&tp=I&tid=NCI-2014-01509&in=Adam+David+Cohen&lo=Mayo+Clinic+Cancer+Center+LAO&va=1&rl=2",
                        new FormFields[] {
                            FormFields.MainType,
                            FormFields.SubTypes,
                            FormFields.Stages,
                            FormFields.Findings,
                            FormFields.Age,
                            FormFields.Phrase,
                            FormFields.Location,
                            FormFields.TrialTypes,
                            FormFields.Drugs,
                            FormFields.OtherTreatments,
                            FormFields.TrialPhases,
                            FormFields.LeadOrg,
                            FormFields.Investigator,
                            FormFields.IsVAOnly
                        },
                        new FormFields[] {
                            FormFields.Country,
                            FormFields.City,
                            FormFields.State
                        }
                    },

                    // TEST 3 - all parameters with hospital location
                    new object[] { "?t=C4872&st=C5214&stg=C36301&fin=C26696&a=25&q=PSA&loc=3&hos=Mayo+Clinic&tt=treatment&d=C2039&i=C116464&tp=I&tid=NCI-2014-01509&in=Adam+David+Cohen&lo=Mayo+Clinic+Cancer+Center+LAO&va=1&rl=2",                         
                        new FormFields[] {
                            FormFields.MainType,
                            FormFields.SubTypes,
                            FormFields.Stages,
                            FormFields.Findings,
                            FormFields.Age,
                            FormFields.Phrase,
                            FormFields.Location,
                            FormFields.TrialTypes,
                            FormFields.Drugs,
                            FormFields.OtherTreatments,
                            FormFields.TrialPhases,
                            FormFields.LeadOrg,
                            FormFields.Investigator,
                            FormFields.IsVAOnly
                        },
                        new FormFields[] {
                            FormFields.Hospital
                        }
                    },

                    // TEST 4 - all parameters with At NIH location
                    new object[] { "?t=C4872&st=C5214&stg=C36301&fin=C26696&a=25&q=PSA&loc=4&tt=treatment&d=C2039&i=C116464&tp=I&tid=NCI-2014-01509&in=Adam+David+Cohen&lo=Mayo+Clinic+Cancer+Center+LAO&va=1&rl=2",
                        new FormFields[] {
                            FormFields.MainType,
                            FormFields.SubTypes,
                            FormFields.Stages,
                            FormFields.Findings,
                            FormFields.Age,
                            FormFields.Phrase,
                            FormFields.Location,
                            FormFields.TrialTypes,
                            FormFields.Drugs,
                            FormFields.OtherTreatments,
                            FormFields.TrialPhases,
                            FormFields.LeadOrg,
                            FormFields.Investigator,
                            FormFields.IsVAOnly
                        },
                        new FormFields[] {
                            FormFields.AtNIH
                        }
                    },
                };
            }
        }
                    
        
        /// <summary>
        /// Main test method.  Takes in a URL and an expected object and sees if Create(url) returns that object.
        /// </summary>
        /// <param name="url">The "URL" with query parameters</param>
        /// <param name="expected">An instance of a CTSSearchParams object that represents the expected result after parsing the URL.</param>
        /// <param name="field">The field that is set on the CTSSearchParams object returned in the specific test.</param>
        [Theory, MemberData("URLParsingData")]
        public void Create(string url, CTSSearchParams expected, FormFields field)
        {
            var mockLookupSvc = GetLookupMock();
            var mockZipLookupSvc = GetZipLookupMock();

            //Create a new instance of the factory, passing in the Mock's version of an implementation
            //of our ITerminologyLookupService interface.
            CTSSearchParamFactory factory = new CTSSearchParamFactory(mockLookupSvc.Object, mockZipLookupSvc.Object);

            //Get the results of parsing the URL
            CTSSearchParams actual = factory.Create(url);

            //Test the actual result to the expected.  NOTE: If you add fields to the CTSSearchParams, you need
            //to also modify the comparer
            Assert.Equal(expected, actual, new CTSSearchParamsComparer());
        }

        /// <summary>
        /// Second main test method.  Takes in a URL and an expected object and sees if Create(url) returns an object that has the correct fields set.
        /// </summary>
        /// <param name="url">The "URL" with query parameters</param>
        /// <param name="expected">An instance of a CTSSearchParams object that represents the expected result after parsing the URL.</param>
        /// <param name="field">The field that is set on the CTSSearchParams object returned in the specific test.</param>
        [Theory, MemberData("URLParsingData")]
        public void AreFieldsSet(string url, CTSSearchParams expected, FormFields field)
        {
            var mockLookupSvc = GetLookupMock();
            var mockZipLookupSvc = GetZipLookupMock();

            //Create a new instance of the factory, passing in the Mock's version of an implementation
            //of our ITerminologyLookupService interface.
            CTSSearchParamFactory factory = new CTSSearchParamFactory(mockLookupSvc.Object, mockZipLookupSvc.Object);

            //Get the results of parsing the URL
            CTSSearchParams actual = factory.Create(url);

            //Test the actual result to the expected.  NOTE: If you add fields to the CTSSearchParams, you need
            //to also modify the comparer
            Assert.Equal(expected.IsFieldSet(field), actual.IsFieldSet(field));
        }

        /// <summary>
        /// Third main test method.  Takes in a URL and an expected object and sees if Create(url) returns an object that has the correct fields set.
        /// </summary>
        /// <param name="url">The "URL" with query parameters</param>
        /// <param name="expected">An instance of a CTSSearchParams object that represents the expected result after parsing the URL.</param>
        /// <param name="field">The fields that are set on the CTSSearchParams object returned in the specific test.</param>
        [Theory, MemberData("FieldSetData")]
        public void AreAllFieldsSet(string url, FormFields[] fields, FormFields[] locFields)
        {
            var mockLookupSvc = GetLookupMock();
            var mockZipLookupSvc = GetZipLookupMock();

            //Create a new instance of the factory, passing in the Mock's version of an implementation
            //of our ITerminologyLookupService interface.
            CTSSearchParamFactory factory = new CTSSearchParamFactory(mockLookupSvc.Object, mockZipLookupSvc.Object);

            //Get the results of parsing the URL
            CTSSearchParams actual = factory.Create(url);

            //Test the actual result to the expected.
            foreach(FormFields field in fields)
            {
                Assert.Equal(true, actual.IsFieldSet(field), new IsFieldSetComparer());
            }

            //Test the actual result to the expected.
            foreach (FormFields field in locFields)
            {
                Assert.Equal(true, actual.LocationParams.IsFieldSet(field), new IsFieldSetComparer());
            }
        }

        /// <summary>
        /// Gets a mock that can be used for a ITerminologyLookupService
        /// See https://github.com/moq/moq4 for more details on the mock library.
        /// (You can do cool thinks like make sure a method was called a certain number of times too...)
        /// (You can pretend to throw an exception if this are not right...)
        /// </summary>s
        /// <returns>A mock to be used as the service.</returns>
        private Mock<IZipCodeGeoLookupService> GetZipLookupMock()
        {
            Mock<IZipCodeGeoLookupService> rtnMock = new Mock<IZipCodeGeoLookupService>();
            rtnMock.Setup(lookup => lookup.GetZipCodeGeoEntry("20850"))
                .Returns(new GeoLocation(39.0897, -77.1798)); 

            return rtnMock;
        }

        /// <summary>
        /// Gets a mock that can be used for a ITerminologyLookupService
        /// See https://github.com/moq/moq4 for more details on the mock library.
        /// (You can do cool thinks like make sure a method was called a certain number of times too...)
        /// (You can pretend to throw an exception if this are not right...)
        /// </summary>s
        /// <returns>A mock to be used as the service.</returns>
        private Mock<ITerminologyLookupService> GetLookupMock()
        {
            Mock<ITerminologyLookupService> rtnMock = new Mock<ITerminologyLookupService>();

            //Handle the case when a string of C4872 is passed in to GetTitleCase and return the label "Breast Cancer"
            //This makes it so that we do not have to create a fake class that returns fake data.
            rtnMock.Setup(lookup => lookup.GetTitleCase("c4872"))
                .Returns("Breast Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c4878"))
                .Returns("Lung Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c5214"))
                .Returns("Breast Adenocarcinoma");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c36301"))
                .Returns("Breast Carcinoma Metastatic in the Brain");
             
            rtnMock.Setup(lookup => lookup.GetTitleCase("c3995,c4872"))
                .Returns("Stage IV Breast Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c7110"))
                .Returns("Recurrent Liver Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c7880"))
                .Returns("Recurrent Liver Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c7839"))
                .Returns("Recurrent Liver Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c7771"))
                .Returns("Recurrent Breast Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c4001,c7771"))
                .Returns("Recurrent Inflammatory Breast Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c88375"))
                .Returns("Stage I Breast Cancer");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c26696"))
                .Returns("Anxiety");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c35014"))
                .Returns("Separation Anxiety Disorder");

            rtnMock.Setup(lookup => lookup.Get("MD"))
                .Returns("Maryland");

            rtnMock.Setup(lookup => lookup.Get("VA"))
                .Returns("Virginia");

            rtnMock.Setup(lookup => lookup.Get("basic_science"))
                .Returns("Basic Science");

            rtnMock.Setup(lookup => lookup.Get("treatment"))
                .Returns("Treatment");

            rtnMock.Setup(lookup => lookup.Get("supportive_care"))
                .Returns("Supportive Care");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c1647"))
                .Returns("Trastuzumab");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c2039"))
                .Returns("Bevacizumab");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c131060"))
                .Returns("Checkpoint Blockade Immunotherapy");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c26665"))
                .Returns("Pomegranate Juice");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c107350,c26665"))
                .Returns("Pomegranate");

            rtnMock.Setup(lookup => lookup.GetTitleCase("c116464"))
                .Returns("Bone Graft ");

            rtnMock.Setup(lookup => lookup.Get("i"))
                .Returns("I");

            rtnMock.Setup(lookup => lookup.Get("ii"))
                .Returns("II");

            rtnMock.Setup(lookup => lookup.MappingContainsKey(It.IsAny<string>()))
                .Returns(true);

            //@Sarina and @Dion - Add other instances for GetTitleCase to support your unit tests.

            return rtnMock;
        }
    }
}
