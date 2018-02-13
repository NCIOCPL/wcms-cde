using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace CancerGov.ClinicalTrials.Basic.v2.Test.SearchParams
{
    public class CTSSearchParams_Test
    {
        #region FieldAsString Tests

        #region Simple Fields

        [Fact()]
        public void FieldAsString_Age()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { Age = 29 };
            Assert.Equal("29", searchParams.GetFieldAsString("Age"));
        }

        [Fact()]
        public void FieldAsString_Investigator()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { Investigator = "The Investigator" };
            Assert.Equal("The Investigator", searchParams.GetFieldAsString("Investigator"));
        }

        [Fact()]
        public void FieldAsString_LeadOrg()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { LeadOrg = "The Lead Org" };
            Assert.Equal("The Lead Org", searchParams.GetFieldAsString("LeadOrg"));
        }

        [Fact()]
        public void FieldAsString_Phrase()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { Phrase = "Keyword" };
            Assert.Equal("Keyword", searchParams.GetFieldAsString("Phrase"));
        }

        [Fact()]
        public void FieldAsString_TrialIDsX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { TrialIDs = new string[] { "NCT12345" } };
            Assert.Equal("NCT12345", searchParams.GetFieldAsString("TrialIDs"));
        }

        [Fact()]
        public void FieldAsString_TrialIDsX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { TrialIDs = new string[] { "NCT12345", "SWOG" } };
            Assert.Equal("NCT12345, SWOG", searchParams.GetFieldAsString("TrialIDs"));
        }

        [Fact()]
        public void FieldAsString_Gender()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { Gender = "female" };
            Assert.Equal("female", searchParams.GetFieldAsString("Gender"));
        }

        [Fact()]
        public void FieldAsString_HealthyVolunteer_Healthy()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { HealthyVolunteer = HealthyVolunteerType.Healthy };
            Assert.Equal("Only Accepting Healthy Volunteers", searchParams.GetFieldAsString("HealthyVolunteers"));
        }

        [Fact()]
        public void FieldAsString_HealthyVolunteer_Unhealthy()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { HealthyVolunteer = HealthyVolunteerType.Infirmed };
            Assert.Equal("Not Accepting Healthy Volunteers", searchParams.GetFieldAsString("HealthyVolunteers"));
        }

        [Fact()]
        public void FieldAsString_HealthyVolunteer_Any()
        {
            CTSSearchParams searchParams = new CTSSearchParams() { HealthyVolunteer = HealthyVolunteerType.Any };
            Assert.Equal("Accepting All Volunteers", searchParams.GetFieldAsString("HealthyVolunteers"));
        }

        #endregion

        #region Locations

        //TODO: Finish locations
        /*
            case FormFields.AtNIH:
            case FormFields.ZipCode:
            case FormFields.ZipRadius:
        */

        [Fact]
        public void FieldAsString_Hospital()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Location = LocationType.Hospital,
                LocationParams = new HospitalLocationSearchParams()
                {
                    Hospital = "Mayo Clinic"
                }
            };
            Assert.Equal("Mayo Clinic", searchParams.GetFieldAsString("Hospital"));
        }

        [Fact]
        public void FieldAsString_Country()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Location = LocationType.CountryCityState,
                LocationParams = new CountryCityStateLocationSearchParams()
                {
                    Country = "United States"
                }
            };
            Assert.Equal("United States", searchParams.GetFieldAsString("Country"));
        }

        [Fact]
        public void FieldAsString_City()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Location = LocationType.CountryCityState,
                LocationParams = new CountryCityStateLocationSearchParams()
                {
                    City = "Baltimore"
                }
            };
            Assert.Equal("Baltimore", searchParams.GetFieldAsString("City"));
        }

        [Fact]
        public void FieldAsString_StatesX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Location = LocationType.CountryCityState,
                LocationParams = new CountryCityStateLocationSearchParams()
                {
                    State = new LabelledSearchParam[] { 
                        new LabelledSearchParam() {
                            Key = "MD",
                            Label = "Maryland"
                        }
                    }
                }
            };
            Assert.Equal("Maryland", searchParams.GetFieldAsString("State"));
        }

        [Fact()]
        public void FieldAsString_StatesX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Location = LocationType.CountryCityState,
                LocationParams = new CountryCityStateLocationSearchParams()
                {
                    State = new LabelledSearchParam[] { 
                        new LabelledSearchParam() {
                            Key = "MD",
                            Label = "Maryland"
                        },
                        new LabelledSearchParam() {
                            Key = "DE",
                            Label = "Delaware"
                        }                    }
                }
            };
            Assert.Equal("Maryland, Delaware", searchParams.GetFieldAsString("State"));
        }


        #endregion

        #region Labelled Fields

        [Fact()]
        public void FieldAsString_PhasesX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams() {
                TrialPhases = new LabelledSearchParam[] { 
                    new LabelledSearchParam() {
                        Key = "i",
                        Label = "Phase I"
                    }
                }
            };
            Assert.Equal("Phase I", searchParams.GetFieldAsString("TrialPhases"));
        }

        [Fact()]
        public void FieldAsString_PhasesX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                TrialPhases = new LabelledSearchParam[] { 
                    new LabelledSearchParam() {
                        Key = "i",
                        Label = "Phase I"
                    },
                    new LabelledSearchParam() {
                        Key = "ii",
                        Label = "Phase II"
                    }
                }
            };
            Assert.Equal("Phase I, Phase II", searchParams.GetFieldAsString("TrialPhases"));
        }

        [Fact()]
        public void FieldAsString_TrialTypesX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                TrialTypes = new LabelledSearchParam[] { 
                    new LabelledSearchParam() {
                        Key = "basic_science",
                        Label = "Basic Science"
                    }
                }
            };
            Assert.Equal("Basic Science", searchParams.GetFieldAsString("TrialTypes"));
        }

        [Fact()]
        public void FieldAsString_TrialTypesX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                TrialTypes = new LabelledSearchParam[] { 
                    new LabelledSearchParam() {
                        Key = "basic_science",
                        Label = "Basic Science"
                    },
                    new LabelledSearchParam() {
                        Key = "treatment",
                        Label = "Treatment"
                    }
                }
            };
            Assert.Equal("Basic Science, Treatment", searchParams.GetFieldAsString("TrialTypes"));
        }

        #endregion

        #region Term Fields

        #region Treatments

        [Fact()]
        public void FieldAsString_DrugX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams() {
                Drugs = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Test Drug 1"
                    }
                }
            };
            Assert.Equal("Test Drug 1", searchParams.GetFieldAsString("Drugs"));
        }

        [Fact()]
        public void FieldAsString_DrugX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Drugs = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Test Drug 1"
                    },
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C6789" },
                        Label = "Test Drug 2"
                    }
                }
            };
            Assert.Equal("Test Drug 1, Test Drug 2", searchParams.GetFieldAsString("Drugs"));
        }

        [Fact()]
        public void FieldAsString_OtherX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                OtherTreatments = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Test Other Treatment 1"
                    }
                }
            };
            Assert.Equal("Test Other Treatment 1", searchParams.GetFieldAsString("OtherTreatments"));
        }

        [Fact()]
        public void FieldAsString_OtherX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                OtherTreatments = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Test Other Treatment 1"
                    },
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C6789" },
                        Label = "Test Other Treatment 2"
                    }
                }
            };
            Assert.Equal("Test Other Treatment 1, Test Other Treatment 2", searchParams.GetFieldAsString("OtherTreatments"));
        }

        #endregion

        #region Diseases

        [Fact()]
        public void FieldAsString_MainTypeX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                MainType = new TerminologyFieldSearchParam() 
                {
                    Codes = new string[] { "C12345" },
                    Label = "Maintype 1"
                }
                
            };
            Assert.Equal("Maintype 1", searchParams.GetFieldAsString("MainType"));
        }


        [Fact()]
        public void FieldAsString_SubTypesX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                SubTypes = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Subtype 1"
                    }
                }
            };
            Assert.Equal("Subtype 1", searchParams.GetFieldAsString("SubTypes"));
        }

        [Fact()]
        public void FieldAsString_SubTypesX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                SubTypes = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Subtype 1"
                    },
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Subtype 2"
                    }
                }
            };
            Assert.Equal("Subtype 1, Subtype 2", searchParams.GetFieldAsString("SubTypes"));
        }

        [Fact()]
        public void FieldAsString_StagesX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Stages = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Stage 1"
                    }
                }
            };
            Assert.Equal("Stage 1", searchParams.GetFieldAsString("Stages"));
        }

        [Fact()]
        public void FieldAsString_StagesX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Stages = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Stage 1"
                    },
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Stage 2"
                    }
                }
            };
            Assert.Equal("Stage 1, Stage 2", searchParams.GetFieldAsString("Stages"));
        }

        [Fact()]
        public void FieldAsString_FindingsX1()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Findings = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Finding 1"
                    }
                }
            };
            Assert.Equal("Finding 1", searchParams.GetFieldAsString("Findings"));
        }

        [Fact()]
        public void FieldAsString_FindingsX2()
        {
            CTSSearchParams searchParams = new CTSSearchParams()
            {
                Findings = new TerminologyFieldSearchParam[] { 
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Finding 1"
                    },
                    new TerminologyFieldSearchParam() {
                        Codes = new string[] { "C12345" },
                        Label = "Finding 2"
                    }
                }
            };
            Assert.Equal("Finding 1, Finding 2", searchParams.GetFieldAsString("Findings"));
        }

        #endregion

        #endregion

        #endregion

    }
}
