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
        public static IEnumerable<object[]> NoneFilteringData
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
                            Location = LocationType.None
                        },
                        new ClinicalTrial.StudySite[] { }
                    },
                    //No other filter
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetAllLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.None
                        },
                        GetAllLocData().ToArray()
                    },
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetAllLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.None
                        },
                        new ClinicalTrial.StudySite[]
                        {
                            new ClinicalTrial.StudySite()
                            {
                                Name = "Hospital C",
                                City = "Berlin",
                                Country = "United States",
                                IsVA = true,
                                StateOrProvinceAbbreviation = "MD"
                            }
                        }
                    },
                };
            }
        }

        private static List<ClinicalTrial.StudySite> GetAllLocData()
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
                    }
                }
            );

            return sites;
        }



        [Theory, MemberData("NoneFilteringData")]
        public void FilterByNone(ClinicalTrial trial, CTSSearchParams searchParams, IEnumerable<ClinicalTrial.StudySite> expectedSites)
        {
            SnippetControls.TrialVelocityTools tvt = new SnippetControls.TrialVelocityTools();
            IEnumerable<ClinicalTrial.StudySite> actual = tvt.GetFilteredLocations(trial, searchParams);

            //TODO: Implement This
            //Assert.Equal(true, false);
            Assert.Equal(expectedSites, actual, new ClinicalTrialsAPI.Test.StudySiteComparer());
        }

    }
}
