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
                                ZipRadius = 10
                            }
                        },
                        //Order matters for the study sites.
                        new ClinicalTrial.StudySite[] {
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
