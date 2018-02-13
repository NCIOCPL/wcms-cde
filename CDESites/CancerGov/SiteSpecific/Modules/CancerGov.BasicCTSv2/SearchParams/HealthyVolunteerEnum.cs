using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2
{
    /// <summary>
    /// Defines the possible choices for Healthy Volunteer filtering.
    /// </summary>
    public enum HealthyVolunteerType
    {
        /// <summary>
        /// Any trial is ok, e.g. this is not filtering by healthy volunteers 
        /// </summary>
        Any = 0,
        /// <summary>
        /// Use this parameter to only match trials that are accepting healthy volunteers
        /// </summary>
        Healthy = 1,
        /// <summary>
        /// Use this parameter to only match trials that are NOT accepting healthy volunteers 
        /// </summary>
        Infirmed = 2
    }
}
