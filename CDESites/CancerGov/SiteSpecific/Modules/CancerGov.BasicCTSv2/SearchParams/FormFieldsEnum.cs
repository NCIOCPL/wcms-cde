using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Enumeration representing a bitmap for the fields that are set.
    /// </summary>
    [Flags]
    public enum FormFields
    {
        None            = 0,
        MainType        = 1 << 0,
        SubTypes        = 1 << 1,
        Stages          = 1 << 2,
        Findings        = 1 << 3,
        Age             = 1 << 4,
        Gender          = 1 << 5,
        Phrase          = 1 << 6,
        Location        = 1 << 7,
        ZipCode         = 1 << 8,
        ZipRadius       = 1 << 9,
        Country         = 1 << 10,
        State           = 1 << 11,
        City            = 1 << 12,
        Hospital        = 1 << 13,
        AtNIH           = 1 << 14,
        TrialTypes      = 1 << 15,
        Drugs           = 1 << 16,
        OtherTreatments = 1 << 17,
        TrialPhases     = 1 << 18,
        TrialIDs        = 1 << 19,
        Investigator    = 1 << 20,
        LeadOrg         = 1 << 21,
        IsVAOnly        = 1 << 22,
        HealthyVolunteers = 1 << 23
    }
}
