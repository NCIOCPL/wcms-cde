using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Static class containing constants for Clinical Trials Search
    /// </summary>
    public static class CTSConstants
    {
        // CTRP trial statuses that qualify as "active"
        // These are used as filter criteria for returning trials on the results/view/listing pages.
        public static readonly string[] ActiveTrialStatuses = {
            // These CTRP statuses appear in results:
            "Active",
            "Approved",
            "Enrolling by Invitation",
            "In Review",
            "Temporarily Closed to Accrual",
            "Temporarily Closed to Accrual and Intervention" 
            // These CTRP statuses DO NOT appear in results:
            /// "Administratively Complete",
            /// "Closed to Accrual",
            /// "Closed to Accrual and Intervention",
            /// "Complete",
            /// "Withdrawn"
        };

        // Site-specific recruitment statuses that qualify as "active" (not to be confused with the Trial Status).
        // These are used to filter available study sites on results/view/listing pages.
        public static readonly string[] ActiveRecruitmentStatuses = {
            // These statuses appear in results:
            "active",
            "approved",
            "enrolling_by_invitation",
            "in_review",
            "temporarily_closed_to_accrual"
            // These statuses DO NOT appear in results:
            /// "closed_to_accrual",
            /// "completed",
            /// "administratively_complete",
            /// "closed_to_accrual_and_intervention",
            /// "withdrawn"
        };

        //Fields to include on returned search results list
        public static readonly string[] IncludeFields = {
            "nct_id",
            "nci_id",
            "brief_title",
            "sites.org_name",
            "sites.org_postal_code",
            "eligibility.structured",
            "current_trial_status",
            "sites.org_va",
            "sites.org_country",
            "sites.org_state_or_province",
            "sites.org_city",
            "sites.org_coordinates",
            "sites.recruitment_status",
            "diseases"
        };
    }
}
