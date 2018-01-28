using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.Implements
{
    /// <summary>
    /// Equality comparer based on delegate.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateEqualityComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> _equals;

        /// <summary>Ctor.</summary>
        public DelegateEqualityComparer(Func<T, T, bool> equals)
        {
            _equals = equals;
        }

        /// <summary>Equals.</summary>
        public bool Equals(T x, T y) => _equals == null ? false : _equals(x, y);

        /// <summary>GetHashCode.</summary>
        public int GetHashCode(T obj)
        {
            throw new NotImplementedException();
        }
    }
}
