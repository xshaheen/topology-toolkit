using System;
using System.Collections.Generic;

namespace Topology.Infra.Infrastructure
{
    public class Comparer
    {
        public static Comparer<T> GetIEqualityComparer<T>(Func<T, T, bool> func) 
            => new Comparer<T>(func);
    }

    public class Comparer<T> : Comparer, IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparisonFunc;

        public Comparer(Func<T, T, bool> func) { _comparisonFunc = func; }

        public bool Equals(T x, T y) => _comparisonFunc(x, y);

        public int GetHashCode(T obj) => obj.GetHashCode();
    }
}