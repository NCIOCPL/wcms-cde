using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancerGov.ClinicalTrials.Basic.v2.Test
{


    /// <summary>
    /// Comparer for a CTSSearchParams class.
    /// </summary>
    public class CTSSearchParamsComparer : IEqualityComparer<CTSSearchParams>
    {

        private TerminologyFieldSearchParamComparer _termComp = new TerminologyFieldSearchParamComparer();
        private LabelledSearchParamComparer _labelledParamComp = new LabelledSearchParamComparer();
        private LocationSearchParamsComparer _locParamComp = new LocationSearchParamsComparer();

        #region IEqualityComparer<CTSSearchParams> Members
         
        public bool Equals(CTSSearchParams x, CTSSearchParams y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            //This should compare every single property.
            // "_termComp.Equals()" - compare two TerminologyFieldSearchParam objects
            // "_labelledParamComp.Equals() - compare two LabelledSearchParam objects
            // "this.AreTermParamListsEqual()" - compare two arrays of TerminologyFieldSearchParam objects
            // "this.AreLabelledParamListsEqual()" - compare two arrays of LabelledSearchParam objects
            // "this.AreParamArraysEqual()" - compare two String arrays of CTSSearchParams properties 
            // "x.<object> == y.<object>" - compare two CTSSearchParam properies
            bool isEqual =
                _termComp.Equals(x.MainType, y.MainType) &&
                this.AreTermParamListsEqual(x.SubTypes, y.SubTypes) &&
                this.AreTermParamListsEqual(x.Stages, y.Stages) &&
                this.AreTermParamListsEqual(x.Findings, y.Findings) &&
                this.AreTermParamListsEqual(x.Drugs, y.Drugs) &&
                this.AreTermParamListsEqual(x.OtherTreatments, y.OtherTreatments) &&
                this.AreLabelledParamListsEqual(x.TrialTypes, y.TrialTypes) &&
                this.AreLabelledParamListsEqual(x.TrialPhases, y.TrialPhases) &&
                this.AreParamArraysEqual(x.TrialIDs, y.TrialIDs) &&
                x.Age == y.Age &&
                x.Gender == y.Gender &&
                x.HealthyVolunteer == y.HealthyVolunteer &&
                x.Phrase == y.Phrase && // Keyword
                x.IsVAOnly == y.IsVAOnly &&
                x.Location == y.Location &&
                _locParamComp.Equals(x.LocationParams, y.LocationParams) &&
                x.Investigator == y.Investigator &&
                x.LeadOrg == y.LeadOrg &&
                x.ResultsLinkFlag == y.ResultsLinkFlag;
            //ADD A FIELD TO SearchParams, NEED to add here.

            return isEqual;
        }


        /// <summary>
        /// Helper function to determine param arrays are equal, order does not matter.
        /// </summary>
        /// <param name="x">Param array 1</param>
        /// <param name="y">Param array 2</param>
        /// <returns></returns>
        private bool AreParamArraysEqual(string[] x, string[] y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.

            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            //Generate a set of those values that are not in both lists.
            //if this is not 0, then there is an error.
            var diffxy = x.Except(y);

            return diffxy.Count() == 0;
        }


        /// <summary>
        /// Helper function to determine if two synonym lists are equal, order does not matter.
        /// </summary>
        /// <param name="x">Synonym list 1</param>
        /// <param name="y">Synonym list 2</param>
        /// <returns></returns>
        private bool AreTermParamListsEqual(TerminologyFieldSearchParam[] x, TerminologyFieldSearchParam[] y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.

            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            //Generate a set of those values that are not in both lists.
            //if this is not 0, then there is an error.
            var diffxy = x.Except(y, new TerminologyFieldSearchParamComparer());

            return diffxy.Count() == 0;
        }

        /// <summary>
        /// Helper function to determine if two labelled objects are equal.
        /// </summary>
        /// <param name="x">Labelled object 1</param>
        /// <param name="y">Labelled object 2</param>
        /// <returns></returns>
        private bool AreLabelledParamListsEqual(LabelledSearchParam[] x, LabelledSearchParam[] y)
        {
            // If the items are both null, or if one or the other is null, return 
            // the correct response right away.
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }

            //Generate a set of those values that are not in both lists.
            //if this is not 0, then there is an error.
            var diffxy = x.Except(y, new LabelledSearchParamComparer());

            return diffxy.Count() == 0;
        }

        //Used to make sure hash of codes is same for two arrays with same values.
        //Found at https://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order
        private int GetOrderIndependentHashCode<T>(IEnumerable<T> source)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            return GetOrderIndependentHashCode(source, comparer);
        }

        //Used to make sure hash of codes is same for two arrays with same codes.
        //Found at https://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order
        private int GetOrderIndependentHashCode<T>(IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            int hash = 0;
            int curHash;
            int bitOffset = 0;
            // Stores number of occurences so far of each value.
            var valueCounts = new Dictionary<T, int>();

            foreach (T element in source)
            {
                curHash = comparer.GetHashCode(element);
                if (valueCounts.TryGetValue(element, out bitOffset))
                    valueCounts[element] = bitOffset + 1;
                else
                    valueCounts.Add(element, bitOffset);

                // The current hash code is shifted (with wrapping) one bit
                // further left on each successive recurrence of a certain
                // value to widen the distribution.
                // 37 is an arbitrary low prime number that helps the
                // algorithm to smooth out the distribution.
                hash = unchecked(hash + ((curHash << bitOffset) |
                    (curHash >> (32 - bitOffset))) * 37);
            }

            return hash;
        }

        public int GetHashCode(CTSSearchParams obj)
        {
            
            throw new NotImplementedException("CTS Search Param Comparer Hash has not been implemented");
            /* TODO: code below does not work when there are arrays of objects. This needs to be updated to use the new helper functions.
            int hash = 0;
            hash ^= _termComp.GetHashCode(obj.MainType);
            hash ^= obj.SubTypes.GetHashCode();
            hash ^= obj.Stages.GetHashCode();
            hash ^= obj.Findings.GetHashCode();
            hash ^= obj.Age.GetHashCode();
            hash ^= obj.Gender.GetHashCode();
            hash ^= obj.Phrase.GetHashCode();
            hash ^= obj.Location.GetHashCode();
            hash ^= _locParamComp.GetHashCode(obj.LocationParams);
            hash ^= obj.TrialTypes.GetHashCode();
            hash ^= obj.Drugs.GetHashCode();
            hash ^= obj.OtherTreatments.GetHashCode();
            hash ^= obj.TrialPhases.GetHashCode();
            hash ^= obj.TrialIDs.GetHashCode();
            hash ^= obj.Investigator.GetHashCode();
            hash ^= obj.LeadOrg.GetHashCode();
            hash ^= obj.ResultsLinkFlag.GetHashCode();
            //ADD A FIELD TO SearchParams, NEED to add here.
            return hash;
            */
        }


        #endregion
    }
}
