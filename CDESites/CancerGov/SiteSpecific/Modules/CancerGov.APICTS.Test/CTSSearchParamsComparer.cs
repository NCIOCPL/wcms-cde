﻿using System;
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
            bool isEqual =
                _termComp.Equals(x.MainType, y.MainType) &&
                x.Phrase == y.Phrase;

            //ADD A FIELD TO SearchParams, NEED to add here.

            return isEqual;
        }

        public int GetHashCode(CTSSearchParams obj)
        {
            int hash = 0;
            hash ^= _termComp.GetHashCode(obj.MainType);
            hash ^= obj.Phrase.GetHashCode();

            //ADD A FIELD TO SearchParams, NEED to add here.

            return hash;
        }

        #endregion
    }
}
