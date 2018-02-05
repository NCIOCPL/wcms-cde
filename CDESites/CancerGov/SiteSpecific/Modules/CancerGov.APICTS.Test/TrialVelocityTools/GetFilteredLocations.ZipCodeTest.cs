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

        public static IEnumerable<object[]> ZipFilteringData
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //Empty Set
                    new object[] {
                        LoadTrial("2AtNIH.json"),
                        new CTSSearchParams() {
                            Location = LocationType.Zip,
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "20852",
                                ZipRadius = 10,
                                GeoLocation = new GeoLocation(
                                    38.984653,
                                    -77.094711
                                )

                            }
                        },
                        //Order matters for the study sites.
                        new ClinicalTrial.StudySite[] {
                            new ClinicalTrial.StudySite()
                            {
                                ContactEmail = "aekim@childrensnational.org",
                                ContactName = "AeRang Kim",
                                ContactPhone = "202-476-4744",
                                RecruitmentStatus = "ACTIVE",
                                LocalSiteIdentifier = "",
                                AddressLine1 = "111 Michigan Avenue Northwest",
                                City = "Washington",
                                Country = "United States",
                                Name = "Children's National Medical Center",
                                OrgPhone = "202-884-2549",
                                PostalCode = "20010",
                                StateOrProvinceAbbreviation = "DC",
                                Coordinates = new ClinicalTrial.StudySite.GeoLocation()
                                {
                                    Latitude = 38.9322,
                                    Longitude = -77.028
                                }
                            },
                            new ClinicalTrial.StudySite() {
                               AddressLine1 = "10 Center Drive",
                               AddressLine2 = null,
                               City ="Bethesda",
                               ContactEmail = "widemanb@pbmac.nci.nih.gov",
                               ContactName = "Brigitte C. Widemann",
                               ContactPhone = "301-496-7387",
                               Coordinates = new ClinicalTrial.StudySite.GeoLocation() {
                                   Latitude = 39.0003, Longitude = -77.1056
                               },
                               Country = "United States",
                               Family = null,
                               LocalSiteIdentifier = null,
                               Name = "National Institutes of Health Clinical Center",
                               OrgEmail = null, OrgFax = null,
                               OrgPhone = "800-411-1222",
                               OrgToFamilyRelationship = null,
                               OrgTTY = null,
                               PostalCode = "20892",
                               RecruitmentStatus = "ACTIVE",
                               StateOrProvinceAbbreviation = "MD"
                           },
                            new ClinicalTrial.StudySite() {
                               AddressLine1 = "9000 Rockville Pike",
                               AddressLine2 = null,
                               City ="Bethesda",
                               ContactEmail = "widemanb@mail.nih.gov",
                               ContactName = "Brigitte C. Widemann",
                               ContactPhone = "301-496-7387",
                               Coordinates = new ClinicalTrial.StudySite.GeoLocation() {
                                   Latitude = 39.0003, Longitude = -77.1056
                               },
                               Country = "United States",
                               Family = "NCI Center for Cancer Research (CCR)",
                               LocalSiteIdentifier = null,
                               Name = "National Cancer Institute Pediatric Oncology Branch",
                               OrgEmail = null, OrgFax = null,
                               OrgPhone = "877-624-4878",
                               OrgToFamilyRelationship = "ORGANIZATIONAL",
                               OrgTTY = null,
                               PostalCode = "20892",
                               RecruitmentStatus = "ACTIVE",
                               StateOrProvinceAbbreviation = "MD"
                           }
                        }
                    },
                    new object[] {
                        LoadTrial("NCIMatch.json"),
                        new CTSSearchParams() {
                            IsVAOnly = true,
                            Location = LocationType.Zip,
                            //Location as Norwalk CT.  Near VA hospital in CT, near NYC (with non VA sites)
                            LocationParams = new ZipCodeLocationSearchParams() {
                                ZipCode = "06850",
                                ZipRadius = 200,
                                GeoLocation = new GeoLocation(
                                    41.1173,
                                    -73.4593
                                )

                            }
                        },
                        //Order matters for the study sites.
                        new ClinicalTrial.StudySite[] {
                            new ClinicalTrial.StudySite()
                            {
                                ContactName = "Herta Huey-An Chao",
                                ContactPhone = "203-937-3421ext2832",
                                RecruitmentStatus = "ACTIVE",
                                LocalSiteIdentifier = "",
                                AddressLine1 = "950 Campbell Avenue",                                
                                City = "West Haven",
                                Country = "United States",
                                Name = "Veterans Affairs Connecticut Healthcare System-West Haven Campus",
                                OrgPhone = "203-932-5711",
                                PostalCode = "06516",
                                StateOrProvinceAbbreviation = "CT",
                                IsVA = true,
                                Coordinates = new ClinicalTrial.StudySite.GeoLocation() {
                                    Latitude = 41.2716,
                                    Longitude = -72.9666
                                }
                            }
                        }
                    }
                };
            }
        }


        [Theory, MemberData("ZipFilteringData")]
        public void FilterByZip(ClinicalTrial trial, CTSSearchParams searchParams, IEnumerable<ClinicalTrial.StudySite> expectedSites)
        {
            SnippetControls.TrialVelocityTools tvt = new SnippetControls.TrialVelocityTools();
            IEnumerable<ClinicalTrial.StudySite> actual = tvt.GetFilteredLocations(trial, searchParams);

            //TODO: Implement this.
            Assert.Equal(expectedSites, actual, new ClinicalTrialsAPI.Test.StudySiteComparer());
        }

    }
}
