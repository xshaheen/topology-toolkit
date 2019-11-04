using System;
using System.Collections.Generic;
using System.Linq;

namespace Infra
{
    public class SetUtl
    {
        /// <summary>
        /// Generates the power set of the given set in O(2^set.length).
        /// </summary>
        /// Examples:
        /// - Input: S = {a, b}, Output: {}, {a}, {b}, S
        ///   Note the pattern:          00   01   10  11
        /// - Input: S = {a, b, c}, Output: {}, {a}, {b}, {a, b} {c}, {c, a}, {b, c}, S
        ///   Note the pattern:             000 001  010  011    100  101      110    111 
        /// - Input: S = {a, b, c, d}, Output: {}, {a} , {b}, {c}, {d}, {a,b}, {a,c}, {a,d},
        ///   {b,c}, {b,d}, {c,d}, {a,b,c}, {a,b,d}, {a,c,d}, {b,c,d}, S
        /// <typeparam name="T">The type of set elements.</typeparam>
        /// <param name="set">The target set.</param>
        /// <returns>The power set of the set.</returns>
        public static HashSet<HashSet<T>> PowerSet<T>(HashSet<T> set)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));
            if (set.Count > 32) throw new Exception("Max set elements is 32 element");

            var n = 1 << set.Count;
            var powerSet = new HashSet<HashSet<T>>(n);

            // loop to get all 2^set.Count subset
            for (var i = 0; i < n; i++)
            {
                var subset = new HashSet<T>();

                // loop though every element in the set and determine with number 
                // should present in the current subset.
                var j = 0;
                foreach (var e in set)
                    // if the jth element (bit) in the ith subset (binary number of i) add it.
                    if (((1L << j++) & i) > 0)
                        subset.Add(e);

                powerSet.Add(subset);
            }

            return powerSet;
        }

        /// <summary>
        /// Find the closure of a <paramref name="subset"/>.
        /// </summary>
        /// <typeparam name="T">Type of <paramref name="set"/> elements.</typeparam>
        public static HashSet<T> ClosedSet<T>(HashSet<T> set, HashSet<T> subset)
        {
            if (set == null) throw new ArgumentNullException(nameof(set));
            if (subset == null) throw new ArgumentNullException(nameof(subset));

            if (!subset.IsSubsetOf(set)) 
                throw new Exception("The given subset not a valid subset of the set.");

            return set.Where(e => !subset.Contains(e)).ToHashSet();
        }
    }
}