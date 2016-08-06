﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json;

namespace CancerGov.ClinicalTrialsAPI
{
    /// <summary>
    /// Represents a Clinical Trial as returned by http://clinicaltrialsapi.cancer.gov
    /// </summary>
    public class ClinicalTrial
    {

        #region Subclasses of this object

        /// <summary>
        /// Represents a unspecified protocol ID
        /// </summary>
        public class OtherId
        {
            /// <summary>
            /// The type of identifier
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// The ID
            /// </summary>
            [JsonProperty("value")]
            public string Value { get; set; }
        }

        /// <summary>
        /// Represents the primary purpose (Type of Trial) of this trial
        /// </summary>
        public class PrimaryPurposeInformation
        {
            /// <summary>
            /// Gets or sets the purpose code for this trial. (e.g. Treatment, Prevention)
            /// </summary>
            [JsonProperty("primary_purpose_code")]
            public string Code { get; set; }

            /// <summary>
            /// Gets or sets additional text of this purpose.  
            /// <remarks>This appears on trials with a Code equal to OTHER</remarks>
            /// </summary>
            [JsonProperty("primary_purpose_other_text")]
            public string OtherText { get; set; }

            /// <summary>
            /// Gets or Sets an additional qualifier code for this trial.  
            /// <remarks>This is not currently in use</remarks>
            /// </summary>
            [JsonProperty("primary_purpose_additional_qualifier_code")]
            public object AdditionalQualifierCode { get; set; }
        }

        public class Phase
        {
            [JsonProperty("")]
            public string phase { get; set; }
            [JsonProperty("")]
            public object phase_other_text { get; set; }
            [JsonProperty("")]
            public object phase_additional_qualifier_code { get; set; }
        }

        public class Masking
        {
            [JsonProperty("")]
            public object masking { get; set; }
            [JsonProperty("")]
            public object masking_allocation_code { get; set; }
            [JsonProperty("")]
            public object masking_role_investigator { get; set; }
            [JsonProperty("")]
            public object masking_role_outcome_assessor { get; set; }
            [JsonProperty("")]
            public object masking_role_subject { get; set; }
            [JsonProperty("")]
            public object masking_role_caregiver { get; set; }
        }

        /// <summary>
        /// Class representing an overall contact for the trial
        /// </summary>
        public class CentralContactInformation
        {
            /// <summary>
            /// Gets or sets the Email of this contact
            /// </summary>
            [JsonProperty("central_contact_email")]
            public string Email { get; set; }

            /// <summary>
            /// Gets or sets the name of this contact
            /// </summary>
            [JsonProperty("central_contact_name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the phone of this contact
            /// </summary>
            [JsonProperty("central_contact_phone")]
            public object Phone { get; set; }

            /// <summary>
            /// Gets or sets the type of this contact
            /// </summary>
            [JsonProperty("central_contact_type")]
            public object Type { get; set; }
        }

        /// <summary>
        /// Represents a collaborator of this trial
        /// </summary>
        public class Collaborator
        {
            /// <summary>
            /// Gets or sets the name of this collaborator
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the functional role of this collaborator
            /// </summary>
            [JsonProperty("functional_role")]
            public string FunctionalRole { get; set; }

            /// <summary>
            /// Gets or sets the status of this collaborator
            /// </summary>
            [JsonProperty("status")]
            public string status { get; set; }
        }

        public class Org
        {
            [JsonProperty("")]
            public string address_line_1 { get; set; }
            [JsonProperty("")]
            public string address_line_2 { get; set; }
            [JsonProperty("")]
            public string postal_code { get; set; }
            [JsonProperty("")]
            public string city { get; set; }
            [JsonProperty("")]
            public string state_or_province { get; set; }
            [JsonProperty("")]
            public string country { get; set; }
            [JsonProperty("")]
            public string name { get; set; }
            [JsonProperty("")]
            public string status { get; set; }
            [JsonProperty("")]
            public string status_date { get; set; }
            [JsonProperty("")]
            public string email { get; set; }
            [JsonProperty("")]
            public string fax { get; set; }
            [JsonProperty("")]
            public string phone { get; set; }
            [JsonProperty("")]
            public string tty { get; set; }
            [JsonProperty("")]
            public string family { get; set; }
            [JsonProperty("")]
            public string org_to_family_relationship { get; set; }
        }

        public class Site
        {
            [JsonProperty("")]
            public string contact_email { get; set; }
            [JsonProperty("")]
            public object contact_name { get; set; }
            [JsonProperty("")]
            public string contact_phone { get; set; }
            [JsonProperty("")]
            public object generic_contact { get; set; }
            [JsonProperty("")]
            public Org org { get; set; }
            [JsonProperty("")]
            public string recruitment_status { get; set; }
            [JsonProperty("")]
            public string recruitment_status_date { get; set; }
            [JsonProperty("")]
            public string local_site_identifier { get; set; }
        }

        public class Disease2
        {
            [JsonProperty("")]
            public string id { get; set; }
            [JsonProperty("")]
            public string display_name { get; set; }
            [JsonProperty("")]
            public object lead_disease_indicator { get; set; }
            [JsonProperty("")]
            public string date_last_created { get; set; }
            [JsonProperty("")]
            public string date_last_updated { get; set; }
        }

        public class Disease
        {
            [JsonProperty("")]
            public Disease2 disease { get; set; }
        }

        /// <summary>
        /// Represents the Eligibility Information of a trial
        /// </summary>
        public class EligibilityInformation
        {

            /// <summary>
            /// Represents the structured eligibility criteria of a trial
            /// </summary>
            public class StructuredCriteriaInformation
            {
                [JsonProperty("gender")]
                public string Gender { get; set; }

                [JsonProperty("max_age")]
                public string MaxAgeStr { get; set; }

                [JsonProperty("max_age_number")]
                public int MaxAgeInt { get; set; }

                [JsonProperty("max_age_unit")]
                public string MaxAgeUnits { get; set; }

                [JsonProperty("min_age")]
                public string MinAgeStr { get; set; }

                [JsonProperty("min_age_number")]
                public int MinAgeInt { get; set; }

                [JsonProperty("min_age_unit")]
                public string MinAgeUnits { get; set; }
            }

            /// <summary>
            /// Represents an unstructured eligibility criterion of a trial
            /// </summary>
            public class UnstructuredCriterion
            {
                /// <summary>
                /// Gets or sets a bool indicating if this criterion indications inclusion for the trial
                /// </summary>
                [JsonProperty("inclusion_indicator")]
                public bool IsInclusionCriterion { get; set; }

                /// <summary>
                /// Gets or sets the content of this criterion
                /// </summary>
                [JsonProperty("description")]
                public string Description { get; set; }
            }

            /// <summary>
            /// Gets or sets the Structured Eligibility Information for this trial (e.g. Age and Gender)
            /// </summary>
            [JsonProperty("structured")]
            public StructuredCriteriaInformation StructuredCriteria { get; set; }

            /// <summary>
            /// Gets and sets a list of unstructured eligibility criteria for this trial
            /// </summary>
            [JsonProperty("unstructured")]
            public List<UnstructuredCriterion> UnstructuredCriteria { get; set; }

            //Expose some helper properties and methods.  (Age, Gender, Inclusion and exclusion

            /// <summary>
            /// Gets the Gender Criterion
            /// </summary>
            public string Gender
            {
                get { return StructuredCriteria.Gender; }
            }
        }

        public class Intervention
        {
            [JsonProperty("")]
            public string intervention_name { get; set; }
            [JsonProperty("")]
            public string intervention_type { get; set; }
            [JsonProperty("")]
            public object intervention_description { get; set; }
            [JsonProperty("")]
            public string date_created_intervention { get; set; }
            [JsonProperty("")]
            public string date_updated_intervention { get; set; }
        }

        public class Arm
        {
            [JsonProperty("")]
            public string arm_name { get; set; }
            [JsonProperty("")]
            public object arm_type { get; set; }
            [JsonProperty("")]
            public string arm_description { get; set; }
            [JsonProperty("")]
            public string date_created_arm { get; set; }
            [JsonProperty("")]
            public string date_updated_arm { get; set; }
            [JsonProperty("")]
            public List<Intervention> interventions { get; set; }
        }

        #endregion


        #region Trial Identifiers

        /// <summary>
        /// Gets or sets the NCI ID for this trial
        /// </summary>
        [JsonProperty("nci_id")]
        public string NCIID { get; set; }

        /// <summary>
        /// Gets or sets the ClinicalTrials.gov ID
        /// </summary>
        [JsonProperty("nct_id")]
        public string NCTID { get; set; }

        /// <summary>
        /// Gets or sets the primary protocol ID of this trial
        /// </summary>
        [JsonProperty("protocol_id")]
        public string ProtocolID { get; set; }

        /// <summary>
        /// Gets or sets the NCI Center for Cancer Research identifier of this trial, if it exists
        /// </summary>
        [JsonProperty("ccr_id")]
        public object CCRID { get; set; }

        /// <summary>
        /// Gets or sets the NCI Cancer Therapy Evaluation Program identifier of this trial, if it exists
        /// </summary>
        [JsonProperty("ctep_id")]
        public string CTEPID { get; set; }

        /// <summary>
        /// Gets or sets the NCI Division of Cancer Prevention identifier of this trial, if it exists
        /// </summary>
        [JsonProperty("dcp_id")]
        public object DCPID { get; set; }

        /// <summary>
        /// Gets or sets a collection of additional unspecified trial identifiers
        /// </summary>
        [JsonProperty("other_ids")]
        public List<OtherId> OtherTrialIDs { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the shorter title for this trial
        /// </summary>
        [JsonProperty("brief_title")]
        public string BriefTitle { get; set; }

        /// <summary>
        /// Gets or sets the official title of this trial
        /// </summary>
        [JsonProperty("official_title")]
        public string OfficialTitle { get; set; }

        /// <summary>
        /// Gets or sets a brief summary of this trial
        /// </summary>
        [JsonProperty("brief_summary")]
        public string BriefSummary { get; set; }

        /// <summary>
        /// Gets or sets the detailed description for this trial
        /// <remarks>This will contain newline characters (\r\n) to indicate a new line
        /// </remarks>
        /// </summary>
        [JsonProperty("detail_description")]
        public string DetailedDescription { get; set; }

        /// <summary>
        /// Gets or sets the eligibility information for this trial
        /// </summary>
        [JsonProperty("eligibility")]
        public EligibilityInformation EligibilityInfo { get; set; }

        /// <summary>
        /// Gets or sets the primary purpose of this trial (Treatment, Screening, Prevention, etc.)
        /// </summary>
        [JsonProperty("primary_purpose")]
        public PrimaryPurposeInformation PrimaryPurpose { get; set; }

        /// <summary>
        /// Gets or sets the status of this trial
        /// </summary>
        [JsonProperty("current_trial_status")]
        public string CurrentTrialStatus { get; set; }


        #region lead organization and sponsor information

        /// <summary>
        /// Gets or sets the name of the lead organization of this trial
        /// </summary>
        [JsonProperty("lead_org")]
        public string LeadOrganizationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the overall principal investigator of this trial
        /// </summary>
        [JsonProperty("principal_investigator")]
        public string PrincipalInvestigator { get; set; }

        /// <summary>
        /// Gets or sets the overall contact information for this trial
        /// </summary>
        [JsonProperty("central_contact")]
        public CentralContactInformation CentralContact { get; set; }

        /// <summary>
        /// Gets or sets a list of the collaborators for this trial
        /// </summary>
        [JsonProperty("collaborators")]
        public List<Collaborator> Collaborators { get; set; }

        #endregion


        #region Composite Properties to Bubble Up information from embedded objects

        /// <summary>
        /// Gets the primary purpose code for this trial
        /// </summary>
        [JsonIgnore()]
        public string TrialType
        {
            get
            {
                return this.PrimaryPurpose.Code;
            }
        }

        #endregion

        /*
        [JsonProperty("")]
        public object associated_studies { get; set; }

        [JsonProperty("")]
        public string amendment_date { get; set; }

        [JsonProperty("")]
        public string date_last_created { get; set; }

        [JsonProperty("")]
        public object date_last_updated { get; set; }

        [JsonProperty("")]
        public string current_trial_status_date { get; set; }

        [JsonProperty("")]
        public string start_date { get; set; }

        [JsonProperty("")]
        public string start_date_type_code { get; set; }

        [JsonProperty("")]
        public object completion_date { get; set; }

        [JsonProperty("")]
        public object completion_date_type_code { get; set; }

        [JsonProperty("")]
        public object acronym { get; set; }

        [JsonProperty("")]
        public object keywords { get; set; }

        [JsonProperty("")]
        public object classification_code { get; set; }

        [JsonProperty("")]
        public object interventional_model { get; set; }

        [JsonProperty("")]
        public string accepts_healthy_volunteers_indicator { get; set; }

        [JsonProperty("")]
        public string study_protocol_type { get; set; }

        [JsonProperty("")]
        public string study_subtype_code { get; set; }

        [JsonProperty("")]
        public string study_population_description { get; set; }

        [JsonProperty("")]
        public string study_model_code { get; set; }

        [JsonProperty("")]
        public string study_model_other_text { get; set; }

        [JsonProperty("")]
        public string sampling_method_code { get; set; }

        [JsonProperty("")]
        public Phase phase { get; set; }

        [JsonProperty("")]
        public Masking masking { get; set; }

        [JsonProperty("")]
        public List<Site> sites { get; set; }

        [JsonProperty("")]
        public List<string> anatomic_sites { get; set; }

        [JsonProperty("")]
        public List<Disease> diseases { get; set; }

        [JsonProperty("")]
        public int minimum_target_accrual_number { get; set; }

        [JsonProperty("")]
        public object number_of_arms { get; set; }

        [JsonProperty("")]
        public List<Arm> arms { get; set; }

        [JsonProperty("")]
        public string date_last_updated_anything { get; set; }
         */
    }
}
