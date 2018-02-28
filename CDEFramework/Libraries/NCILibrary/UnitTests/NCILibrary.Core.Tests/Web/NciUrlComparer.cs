using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NCI.Test;

namespace NCI.Web.Test
{
    /// <summary>
    /// Comparer to determine NciUrl Equality
    /// </summary>
    public class NciUrlComparer : IEqualityComparer<NciUrl>
    {
        /// <summary>
        /// Determines if the two NciUrls are Equivelent 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(NciUrl x, NciUrl y)
        {
            DictionaryComparer<string, string> comparer = new DictionaryComparer<string, string>();

            return x.UriStem == y.UriStem &&
                comparer.Equals(x.QueryParameters, y.QueryParameters);
        }

        /// <summary>
        /// NOT IMPLEMENTED
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(NciUrl obj)
        {
            throw new NotImplementedException();
        }
    }
}
