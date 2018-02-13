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
        //Test cases for Create test method. 
        //All of these tests cases test invalid parameters and the resulting parsing.
        public static IEnumerable<object[]> URLParsingErrorsData {
            get
            {
                // Array of tests
                return new[]
                {
                    //This array of objects maps to the parameters of the create method.
                    //URL at index 0, Expected object at index 1. 
                    
                    // TEST 1.0 - Main Cancer Type with invalid param (not matching C-code pattern)
                    new object[] {"?t=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 1.1 - Main Cancer Type with C-code that has no lookup
                    new object[] {"?t=C4873", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 1.2 - Main Cancer Type with a multiple-coded param, one invalid
                    new object[] {"?t=C4872|chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                     
                    // TEST 1.3 - Main Cancer Type with multiple C-codes
                    new object[] {"?t=C4872,C4878", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 2.0 - Cancer Subtype with invalid param (not matching C-code pattern)
                    new object[] {"?st=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 2.1 - Cancer Subtype with C-code that has no lookup
                    new object[] {"?st=C7772", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 2.2 - Cancer Subtypes with same C-code multiple times
                    new object[] {"?st=C7771,C133092,C7771", new CTSSearchParams() {
                        SubTypes = new TerminologyFieldSearchParam[] { 
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C7771" },
                                Label = "Recurrent Breast Cancer"
                            },
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C133092" },
                                Label = "Recurrent Breast Angiosarcoma"
                            },
                        },
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 2.3 - Cancer Subtypes with a multiple-coded param, one invalid
                    new object[] {"?st=C133092|chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 2.4 - Cancer Subtypes with multiple codes, one with no lookup
                    new object[] {"?st=C4001|C7771,C7772", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 3.0 - Cancer stage with invalid param (not matching C-code pattern)
                    new object[] {"?stg=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 3.1 - Cancer stages with C-code that has no lookup
                    new object[] {"?stg=C88376", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 3.2 - Cancer stages with same C-code multiple times
                    new object[] {"?stg=C88375,C3641,C88375", new CTSSearchParams() {
                        Stages = new TerminologyFieldSearchParam[] { 
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C88375" },
                                Label = "Stage I Breast Cancer"
                            },
                            new TerminologyFieldSearchParam() {
                                Codes = new string[] { "C3641" },
                                Label = "Stage 0 Breast Cancer"
                            }
                        },
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 3.3 - Cancer Subtypes with a multiple-coded param, one invalid
                    new object[] {"?st=C88375|chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 3.4 - Cancer Subtypes with multiple codes, one with no lookup
                    new object[] {"?st=C3995|C4872,C88376", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 4.0 - Cancer findings with invalid param (not matching C-code pattern)
                    new object[] {"?fin=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 4.1 - Cancer findings with C-code that has no lookup
                    new object[] {"?fin=C35015", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 4.2 - Cancer findings with same C-code multiple times
                    new object[] {"?fin=C26696,C35014,C26696", new CTSSearchParams() {
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
                    }},

                    // TEST 4.3 - Cancer findings with a multiple-coded param, one invalid
                    new object[] {"?fin=C26696|chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 4.4 - Cancer Subtypes with multiple codes, one with no lookup
                    new object[] {"?fin=C2878|C35014,C35015", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 5.0 - Age not an int
                    new object[] { "?a=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 5.1 - Age invalid (>120)
                    new object[] { "?a=122", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 5.2 - Age invalid (<0)
                    new object[] { "?a=-1", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 6.0 - Gender not "male" or "female"
                    new object[] { "?g=1", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 6.1 - Gender not "male" or "female"
                    new object[] { "?g=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 7.0 - Location with invalid param (not int)
                    new object[] { "?loc=chicken", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},

                    // TEST 7.1 - Location with invalid param (not 0-4) 
                    new object[] { "?loc=5", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},
                    
                    // TEST 7.2 - Location all with zip code set
                    new object[] { "?loc=0&z=20850&rl=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 7.3 - Location all with state set
                    new object[] { "?loc=0&st=VA&rl=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 7.4 - Location all with city set
                    new object[] { "?loc=0&lcty=Arlington&rl=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 7.5 - Location all with country set
                    new object[] { "?loc=0&lcnty=United+States&rl=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 7.6 - Location all with hospital set
                    new object[] { "?loc=0&hos=Mayo+Clinic&rl=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 7.7 - Location none and zip
                    new object[] { "?loc=0&loc=1&rl=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 8.0 - Location zip without a zip code param
                    new object[] { "?loc=1&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 8.1 - Location zip with an invalid zip code
                    new object[] { "?loc=1&z=chicken&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 8.2 - Location zip with an invalid zip code
                    new object[] { "?loc=1&z=11111&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 8.3 - Location zip with invalid zip proximity
                    new object[] { "?loc=1&z=20850&zp=chicken&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 8.4 - Location zip with invalid zip proximity
                    new object[] { "?loc=1&z=20850&zp=-1&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 8.5 - Location zip with city/state/country
                    new object[] { "?loc=1&lcty=Arlington&lst=VA&lcnty=United+States&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 8.6 - Location zip with hospital
                    new object[] { "?loc=1&hos=Mayo+Clinic&rl=2", new CTSSearchParams() {
                        Location = LocationType.Zip,
                        LocationParams = new ZipCodeLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                
                    // TEST 8.7 - Location zip without Advanced search form
                    new object[] { "?loc=1", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Basic

                    }},
                    
                    // TEST 9.0 - Location City/State/Country without any values set
                    new object[] { "?loc=2&rl=2", new CTSSearchParams() {
                        Location = LocationType.CountryCityState,
                        LocationParams = new CountryCityStateLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 9.1 - Invalid State
                    new object[] { "?loc=2&lst=VI&rl=2", new CTSSearchParams() {
                        Location = LocationType.CountryCityState,
                        LocationParams = new CountryCityStateLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 9.2 - Multiple states, one invalid
                    new object[] { "?loc=2&lst=MD,chicken&rl=2", new CTSSearchParams() {
                        Location = LocationType.CountryCityState,
                        LocationParams = new CountryCityStateLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 9.3 - Location city/state/country with zip code
                    new object[] { "?loc=2&z=11111&rl=2", new CTSSearchParams() {
                        Location = LocationType.CountryCityState,
                        LocationParams = new CountryCityStateLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 9.4 - Location city/state/country with hospital
                    new object[] { "?loc=2&hos=Mayo+Clinic&rl=2", new CTSSearchParams() {
                        Location = LocationType.CountryCityState,
                        LocationParams = new CountryCityStateLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 9.5 - Location city/state/country without Advanced search form
                    new object[] { "?loc=2", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},
                    
                    // TEST 9.6 - Location city/state/country with only state (without country)
                    new object[] { "?loc=2&lst=Baltimore&rl=2", new CTSSearchParams() {
                        Location = LocationType.CountryCityState,
                        LocationParams = new CountryCityStateLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                    
                    // TEST 10.0 - Location hospital without hospital set
                    new object[] { "?loc=3&rl=2", new CTSSearchParams() {
                        Location = LocationType.Hospital,
                        LocationParams = new HospitalLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},
                     
                    // TEST 10.1 - Location hospital with zip code
                    new object[] { "?loc=3&z=11111&rl=2", new CTSSearchParams() {
                        Location = LocationType.Hospital,
                        LocationParams = new HospitalLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 10.2 - Location hospital with city/state/country
                    new object[] { "?loc=3&lcty=Arlington&lst=VA&lcnty=United+States&rl=2", new CTSSearchParams() {
                        Location = LocationType.Hospital,
                        LocationParams = new HospitalLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 10.3 - Location hospital without Advanced search form
                    new object[] { "?loc=3", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},
                    
                    // TEST 11.0 - Is location NIH with zip
                    new object[] { "?loc=4&z=11111&rl=2", new CTSSearchParams() {
                        Location = LocationType.AtNIH,
                        LocationParams = new AtNIHLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 11.1 - Location hospital with city/state/country
                    new object[] { "?loc=4&lcty=Arlington&lst=VA&lcnty=United+States&rl=2", new CTSSearchParams() {
                        Location = LocationType.AtNIH,
                        LocationParams = new AtNIHLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 11.2 - Location city/state/country with hospital
                    new object[] { "?loc=4&hos=Mayo+Clinic&rl=2", new CTSSearchParams() {
                        Location = LocationType.AtNIH,
                        LocationParams = new AtNIHLocationSearchParams(),
                        ResultsLinkFlag = ResultsLinkType.Advanced
                    }},

                    // TEST 11.3 - Location city/state/country with hospital
                    new object[] { "?loc=4", new CTSSearchParams() {
                        Location = LocationType.None,
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},
                    
                    // TEST 12.0 - Results link flag invalid (string)
                    new object[] { "?rl=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Unknown
                    }},
                    // TEST 12.1 - Results link flag (not 1 or 2)
                    new object[] { "?rl=-1", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Unknown
                    }},
                    
                    // TEST 13.0 - Trial type invalid
                    new object[] {"?tt=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 13.1 - Trial type all with type
                    new object[] {"?tt=all,basic_science", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 14.0 - Drug with invalid param
                    new object[] {"?d=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 14.1 - Drugs with invalid param (no lookup)
                    new object[] {"?d=C1648", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 14.2 - Drugs with multiple codes, one invalid
                    new object[] {"?d=C1647,chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 14.3 - Drugs with multiple codes, one repeated
                    new object[] {"?d=C1647,C2039,C1647", new CTSSearchParams() {
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
                    }},
                    
                    // TEST 15.0 - Other treatments/interventions with invalid param
                    new object[] {"?i=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                     
                    // TEST 15.1 - Other treatments/interventions with invalid param (no lookup)
                    new object[] {"?i=c131061", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 15.2 - Other treatments/interventions with multiple codes, one invalid
                    new object[] {"?i=c131060,c131061", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 15.3 - Other treatments/interventions with multiple codes, one repeated
                    new object[] {"?i=C131060,C107350|C26665,C131060", new CTSSearchParams() {
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
                    }},

                    // TEST 15.4 - Other treatments/interventions with multiple codes, one without lookup
                    new object[] {"?i=C131060,C107350|C26665,C131061", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                    
                    // TEST 16.0 - Trial phase with invalid param (no lookup)
                    new object[] {"?tp=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},
                   
                    // TEST 16.1 - Trial phases with all ("") and another value selected
                    new object[] {"?tp=,ii", new CTSSearchParams() {
                        TrialPhases = new LabelledSearchParam[] { 
                            new LabelledSearchParam() {
                                Key = "ii",
                                Label = "II"
                            }
                        },
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 16.2 - Trial phases with multiple values, one invalid (without lookup)
                    new object[] {"?tp=ii,chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic,
                        Location = LocationType.None
                    }},

                    // TEST 17.0 - Is VA only with invalid param (not int)
                    new object[] { "?va=chicken", new CTSSearchParams() {
                        IsVAOnly = false,
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},

                    // TEST 18.0 - Healthy Volunteers with invalid param (not int)
                    new object[] { "?hv=chicken", new CTSSearchParams() {
                        ResultsLinkFlag = ResultsLinkType.Basic
                    }},
                };
            }
        }

        /// <summary>
        /// Main test method.  Takes in a URL and an expected object and sees if Create(url) returns that object.
        /// </summary>
        /// <param name="url">The "URL" with query parameters</param>
        /// <param name="expected">An instance of a CTSSearchParams object that represents the expected result after parsing the URL.</param>
        [Theory, MemberData("URLParsingErrorsData")]
        public void CreateWithInvalidParams(string url, CTSSearchParams expected)
        {
            var mockLookupSvc = GetLookupMockForErrors();
            var mockZipLookupSvc = GetZipLookupMockForErrors();

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
        /// Gets a mock that can be used for a ITerminologyLookupService
        /// See https://github.com/moq/moq4 for more details on the mock library.
        /// (You can do cool thinks like make sure a method was called a certain number of times too...)
        /// (You can pretend to throw an exception if this are not right...)
        /// </summary>s
        /// <returns>A mock to be used as the service.</returns>
        private Mock<IZipCodeGeoLookupService> GetZipLookupMockForErrors()
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
        private Mock<ITerminologyLookupService> GetLookupMockForErrors()
        {
            Mock<ITerminologyLookupService> rtnMock = new Mock<ITerminologyLookupService>();

            //Handle the case when a string of C4872 is passed in to GetTitleCase and return the label "Breast Cancer"
            //This makes it so that we do not have to create a fake class that returns fake data.
            rtnMock.Setup(lookup => lookup.GetTitleCase("c4872"))
                .Returns("Breast Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c4872"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c4878"))
                .Returns("Lung Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c4878"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.MappingContainsKey("c4873"))
                .Returns(false);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c3995,c4872"))
                .Returns("Stage IV Breast Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c3995,c4872"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c7771"))
                .Returns("Recurrent Breast Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c7771"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c4001,c7771"))
                .Returns("Recurrent Inflammatory Breast Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c4001,c7771"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c133092"))
                .Returns("Recurrent Breast Angiosarcoma");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c133092"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.MappingContainsKey("c7772"))
                .Returns(false);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c88375"))
                .Returns("Stage I Breast Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c88375"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c3641"))
                .Returns("Stage 0 Breast Cancer");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c3641"))
                .Returns(true); 

            rtnMock.Setup(lookup => lookup.MappingContainsKey("c88376"))
                .Returns(false);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c26696"))
                .Returns("Anxiety");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c26696"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c35014"))
                .Returns("Separation Anxiety Disorder");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c35014"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c2878,c35014"))
                .Returns("Anxiety Disorder");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c2878,c35014"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.MappingContainsKey("c35015"))
                .Returns(false);

            rtnMock.Setup(lookup => lookup.Get("MD"))
                .Returns("Maryland");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("MD"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.Get("VA"))
                .Returns("Virginia");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("VA"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.MappingContainsKey("VI"))
                .Returns(false);

            rtnMock.Setup(lookup => lookup.Get("basic_science"))
                .Returns("Basic Science");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("basic_science"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.Get("supportive_care"))
                .Returns("Supportive Care");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("supportive_care"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c1647"))
                .Returns("Trastuzumab");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c1647"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c2039"))
                .Returns("Bevacizumab");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c2039"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c136282,c2039"))
                .Returns("Bevacizumab Regimen");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c136282,c2039"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c131060"))
                .Returns("Checkpoint Blockade Immunotherapy");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c131060"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.MappingContainsKey("c131061"))
                .Returns(false);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c26665"))
                .Returns("Pomegranate Juice");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c26665"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.GetTitleCase("c107350,c26665"))
                .Returns("Pomegranate");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("c107350,c26665"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.Get("i"))
                .Returns("I");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("i"))
                .Returns(true);

            rtnMock.Setup(lookup => lookup.Get("ii"))
                .Returns("II");
            rtnMock.Setup(lookup => lookup.MappingContainsKey("ii"))
                .Returns(true);

            return rtnMock;
        }
    }
}
