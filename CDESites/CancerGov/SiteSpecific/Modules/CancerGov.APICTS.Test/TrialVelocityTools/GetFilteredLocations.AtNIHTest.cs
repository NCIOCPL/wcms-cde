using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CancerGov.ClinicalTrials.Basic.v2.SnippetControls;
using CancerGov.ClinicalTrialsAPI;

using Xunit;
using Moq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

namespace CancerGov.ClinicalTrials.Basic.v2.Test.TrialVelocityTools
{
    public partial class GetFilteredLocations
    {
        static readonly string AssemblyFileName;
        static readonly string AssemblyPath;

        static GetFilteredLocations()
        {
            AssemblyFileName = Assembly.GetExecutingAssembly().CodeBase;
            Uri fileNameURI = new Uri(AssemblyFileName);
            AssemblyPath = Path.GetDirectoryName(fileNameURI.LocalPath);
        } 

        /// <summary>
        /// Loads a test trial from TrialExamples folder.  
        /// </summary>
        /// <param name="trialName">The name of the file to load.</param>
        /// <returns></returns>
        public static ClinicalTrial LoadTrial(string trialName)
        {
            string path = Path.Combine(AssemblyPath, "TrialExamples", trialName);
            ClinicalTrial trial = JsonConvert.DeserializeObject<ClinicalTrial>(
                File.ReadAllText(path)
            );

            return trial;
        }

        public static IEnumerable<object[]> AtNIHFilteringData 
        {
            get
            {
                // Array of tests
                return new[]
                {
                    //Empty Set
                    new object[] {
                        new ClinicalTrial(),
                        new ClinicalTrial.StudySite[] { }
                    },
                    //One NIH site, lots of others.
                    new object[] { 
                        LoadTrial("NCIMatch.json"), 
                        new ClinicalTrial.StudySite[] {
                           new ClinicalTrial.StudySite() {
                               AddressLine1 = "10 Center Drive",
                               AddressLine2 = null,
                               City ="Bethesda",
                               ContactEmail = null,
                               ContactName = "A P Chen",
                               ContactPhone = "800-411-1222",
                               Coordinates = new ClinicalTrial.StudySite.GeoLocation() {
                                   Latitude = 39.0003, Longitude = -77.1056
                               },
                               Country = "United States",
                               Family = null,
                               LocalSiteIdentifier = string.Empty,
                               Name = "National Institutes of Health Clinical Center",
                               OrgEmail = null, OrgFax = null, 
                               OrgPhone = "800-411-1222",
                               OrgToFamilyRelationship = null,
                               OrgTTY = null,
                               PostalCode = "20892",
                               RecruitmentStatus = "ACTIVE",
                               StateOrProvinceAbbreviation = "MD"
                           }
                        }
                    },
                    //Two NIH sites, lots of others.
                    new object[] {
                        LoadTrial("2AtNIH.json"), 
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
                    },
                    
                };
            }
        }

        [Theory, MemberData("AtNIHFilteringData")]
        public void FilterByAtNIH(ClinicalTrial trial, IEnumerable<ClinicalTrial.StudySite> expectedSites)
        {
            SnippetControls.TrialVelocityTools tvt = new SnippetControls.TrialVelocityTools();
            CTSSearchParams searchParams = new CTSSearchParams() { Location = LocationType.AtNIH };
            IEnumerable<ClinicalTrial.StudySite> actual = tvt.GetFilteredLocations(trial, searchParams);

            Assert.Equal(expectedSites, actual, new ClinicalTrialsAPI.Test.StudySiteComparer());
        }

        
    }
}
