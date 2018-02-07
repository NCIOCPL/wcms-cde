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
        public static IEnumerable<object[]> HospitalFilteringData
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
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "Mayo"
                            }
                        },
                        new ClinicalTrial.StudySite[] { }
                    },
                    // Mayo -- As of now this should return everything because we do not filter                   
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetHospitalLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "Mayo"
                            }
                        },
                        GetHospitalLocData() 
                    },
                    // (Negative test) Chicken -- As of now this should return everything because we do not filter                   
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetHospitalLocData()
                        },
                        new CTSSearchParams() {
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "Chicken"
                            }
                        },
                        GetHospitalLocData()
                    },
                    // VA Only test -- As of now this should return everything because we do not filter                   
                    new object[] {
                        new ClinicalTrial() {
                            Sites = GetHospitalLocData()
                        },
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.Hospital,
                            LocationParams = new HospitalLocationSearchParams() {
                                Hospital = "Veterans"
                            }
                        },
                        GetHospitalLocData()
                    }
                };
            }
        }

        private static List<ClinicalTrial.StudySite> GetHospitalLocData()
        {
            List<ClinicalTrial.StudySite> sites = new List<ClinicalTrial.StudySite>();

            sites.AddRange(
                new ClinicalTrial.StudySite[] {
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Kansas City Veterans Affairs Medical Center",
                        IsVA = true
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Mayo Clinic",
                        IsVA = false
                    },
                    new ClinicalTrial.StudySite()
                    {
                        Name = "Mayo Clinic in Florida",
                        IsVA = false
                    }
                }
           );
            return sites;
        }

        [Theory, MemberData("HospitalFilteringData")]
        public void FilterByHospital(ClinicalTrial trial, CTSSearchParams searchParams, IEnumerable<ClinicalTrial.StudySite> expectedSites)
        {
            SnippetControls.TrialVelocityTools tvt = new SnippetControls.TrialVelocityTools();
            IEnumerable<ClinicalTrial.StudySite> actual = tvt.GetFilteredLocations(trial, searchParams);
            
            Assert.Equal(expectedSites, actual, new ClinicalTrialsAPI.Test.StudySiteComparer());
        }

    }
}
