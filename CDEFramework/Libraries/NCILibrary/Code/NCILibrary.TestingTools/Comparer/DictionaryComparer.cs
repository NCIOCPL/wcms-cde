using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCI.Test
{
    /// <summary>
    /// Dictionary Comparer
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class DictionaryComparer<TKey, TValue> :
        IEqualityComparer<Dictionary<TKey, TValue>>
    {
        private IEqualityComparer<TValue> valueComparer;

        /// <summary>
        /// Create a new instance of a DictionaryComparer
        /// </summary>
        /// <param name="valueComparer">A comparer to use for comparing the values</param>
        public DictionaryComparer(IEqualityComparer<TValue> valueComparer = null)
        {
            this.valueComparer = valueComparer ?? EqualityComparer<TValue>.Default;
        }

        /// <summary>
        /// Checks if the two dictionaries are equivelent
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(Dictionary<TKey, TValue> x, Dictionary<TKey, TValue> y)
        {
            if (x.Count != y.Count)
                return false;
            if (x.Keys.Except(y.Keys).Any())
                return false;
            if (y.Keys.Except(x.Keys).Any())
                return false;
            foreach (var pair in x)
                if (!valueComparer.Equals(pair.Value, y[pair.Key]))
                    return false;
            return true;
        }

        public int GetHashCode(Dictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }
}
