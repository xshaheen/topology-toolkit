using System;
using System.Collections.Generic;

namespace Topology
{
    public class Comparer
    {
        public static Comparer<T> GetIEqualityComparer<T>(Func<T, T, bool> func)
        {
            return new Comparer<T>(func);
        }
    }

    public class Comparer<T> : Comparer, IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparisonFunc;

        public Comparer(Func<T, T, bool> func) { _comparisonFunc = func; }

        public bool Equals(T x, T y)
        {
            return _comparisonFunc(x, y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}