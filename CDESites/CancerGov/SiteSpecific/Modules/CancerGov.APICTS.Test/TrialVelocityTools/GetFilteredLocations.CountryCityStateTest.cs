using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CancerGov.ClinicalTrials.Basic.v2.SnippetControls;
using CancerGov.ClinicalTrialsAPI;

using Xunit;
using Moq;

namespace CancerGov.ClinicalTrials.Basic.v2.Test.TrialVelocityTools
{
    public partial class GetFilteredLocations
    {
        public static IEnumerable<object[]> CCSFilteringData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //Empty Set
                    new object[] {
                        new ClinicalTrial(),
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Berlin"
                            }
                        },
                        new ClinicalTrial.StudySite[] { }
                    },
                    //City Only
                    new object[] { 
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Berlin"
                            }
                        },
                        GetExpectedCityOnlySites()
                    },
                    //City Only + VA
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Berlin"
                            }
                        },
                        GetExpectedVASites()
                    },
                    //State Only
                    new object[] { 
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
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
                        GetExpectedStateOnlySites()
                    },
                    //State Only + VA
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
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
                        GetExpectedVASites()
                    },
                    //2 States Only
                    new object[] { 
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "PA",
                                        Label = "Pennsylvania"
                                    },
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                }
                            }
                        },
                        GetExpected2StateOnlySites()
                    },
                    //2 States Only + VA
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "PA",
                                        Label = "Pennsylvania"
                                    },
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                }
                            }
                        },
                        GetExpectedVASites()
                    },
                    //Country Only
                    new object[] { 
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                Country = "Germany"
                            }
                        },
                        GetExpectedCountryOnlySites()
                    },
                    //Country Only + VA
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                Country = "Germany"
                            }
                        },
                        new ClinicalTrial.StudySite[] { }
                    },

                    //City State Only
                    new object[] { 
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Berlin",
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                }
                            }
                        },
                        GetExpectedCityStateSites()
                    },
                    //City State Only + VA
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetCSSLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.CountryCityState,
                            LocationParams = new CountryCityStateLocationSearchParams() {
                                City = "Berlin",
                                State = new LabelledSearchParam[] {
                                    new LabelledSearchParam() {
                                        Key = "MD",
                                        Label = "Maryland"
                                    }
                                }
                            }
                        },
                        GetExpectedVASites()
                    }

                    //TODO: Country only, State Only, combos
                };
            }
        }

        private static List<ClinicalTrial.StudySite> GetCSSLocData()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital A",
                        City = "Berlin",
                        Country = "Germany",
                        IsVA = false,
                        StateOrProvinceAbbreviation = null
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital Skip A",
                        City = "Paris",
                        Country = "France",
                        IsVA = false,
                        StateOrProvinceAbbreviation = null
                    },

                    //Interesting fact.  Kitchener, On was once Berlin, ON up until WW1.
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital O",
                        City = "Berlin",
                        Country = "Canada",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "ON"
                    },

                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital B",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    },

                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital P",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "PA"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = true,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Another Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "AK"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital Skip B",
                        City = "Montreal",
                        Country = "Canada",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "QC"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital Skip C",
                        City = "Bethesda",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    }
                }
            );

            return sites;
        }

        private static IEnumerable<ClinicalTrial.StudySite> GetExpectedCityOnlySites()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital A",
                        City = "Berlin",
                        Country = "Germany",
                        IsVA = false,
                        StateOrProvinceAbbreviation = null
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital O",
                        City = "Berlin",
                        Country = "Canada",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "ON"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital B",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital P",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "PA"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = true,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Another Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "AK"
                    }                    
                }
            );

            return sites;          
        }

        private static IEnumerable<ClinicalTrial.StudySite> GetExpectedStateOnlySites()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital B",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = true,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital Skip C",
                        City = "Bethesda",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    }
                }
            );

            return sites;
        }

        private static IEnumerable<ClinicalTrial.StudySite> GetExpected2StateOnlySites()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital B",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital P",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "PA"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = true,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital Skip C",
                        City = "Bethesda",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    }
                }
            );

            return sites;
        }

        private static IEnumerable<ClinicalTrial.StudySite> GetExpectedCountryOnlySites()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital A",
                        City = "Berlin",
                        Country = "Germany",
                        IsVA = false,
                        StateOrProvinceAbbreviation = null
                    }
                }
            );

            return sites;
        }

        private static IEnumerable<ClinicalTrial.StudySite> GetExpectedCityStateSites()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital B",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = false,
                        StateOrProvinceAbbreviation = "MD"
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = true,
                        StateOrProvinceAbbreviation = "MD"
                    }
                }
            );

            return sites;
        }

        private static IEnumerable<ClinicalTrial.StudySite> GetExpectedVASites()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Hospital C",
                        City = "Berlin",
                        Country = "United States",
                        IsVA = true,
                        StateOrProvinceAbbreviation = "MD"
                    }
                }
            );

            return sites;
        }

        [Theory, MemberData("CCSFilteringData")]
        public void FilterByCCS(ClinicalTrial trial, CTSSearchParams searchParams, IEnumerable<ClinicalTrial.StudySite> expectedSites)
        {
            SnippetControls.TrialVelocityTools tvt = new SnippetControls.TrialVelocityTools();
            IEnumerable<ClinicalTrial.StudySite> actual = tvt.GetFilteredLocations(trial, searchParams);

            Assert.Equal(expectedSites, actual, new ClinicalTrialsAPI.Test.StudySiteComparer());
        }

    }
}
