using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Topology
{
    public class Topology
    {
        static void Main()
        { 
            
        }


        public static string PrintHashSet<T>(HashSet<T> set)
        {
            var sb = new StringBuilder("{");

            foreach (var e in set) sb.Append(e + ", ");

            var len = sb.Length;
            // remove the extra ", "
            if (len > 1) sb.Remove(len - 2, 2);

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a all topology defined on a given set.
        /// </summary>
        /// Example:
        /// - Input: S = {'c', 'b', 'a'} // See unit test.
        ///   Output Pattern: any pattern also have 000 and 111
        ///   Single And Double
        ///   001
        ///       010
        ///           011
        ///               100
        ///                   101
        ///                       110
        /// 
        ///   Single-double (disjoint)
        ///   001                 110
        ///       010         101
        ///           011 100
        /// 
        ///   Single-double
        ///   001     011
        ///   001             101
        ///       010 011
        ///       010             110
        ///               100 101
        ///               100     110
        /// 
        ///   Single-single-double
        ///   001 010 011
        ///   001         100 101
        ///       010     100     110
        ///
        ///   Single-double-double
        ///   001     011     101
        ///       010 011         110
        ///               100 101 110
        ///
        ///   single-single-double-double 
        ///   001 010 011     101
        ///   001 010 011         110
        ///       010 011 100     110
        ///       010     100 101 110
        ///   001     011 100 101
        ///   001         100 101 110
        ///
        ///   Power set
        ///   001 010 011 100 101 110
        /// - From this pattern the elements of the power set is exist or not
        ///   so by using a brute force tests we can get all topologies.
        /// - Fact:
        ///   Let T(n) denote the number of distinct topologies on a set with n points.
        ///   There is no known simple formula to compute T(n) for arbitrary n.
        /// - Theorem:
        ///   The number of subsets of size r (or r-combinations) that can be chosen
        ///   from a set of n elements, is given by the formula: nCr = !n / r!(n - r)!
        /// - Theorem:
        ///   The number of subsets of all size is 2^n
        /// <typeparam name="T">Set elements type.</typeparam>
        /// <param name="set">The set that a topologies dif</param>
        /// <returns>Set of all topologies that defined on <paramref name="set"/>.</returns>
        public static HashSet<HashSet<HashSet<T>>> Topologies<T>(T[] set)
        {
            var setAsHashSet = set.ToHashSet();
            var powerSet = PowerSet(set);

            var topologies = new HashSet<HashSet<HashSet<T>>>{
                powerSet, 
                new HashSet<HashSet<T>> {new HashSet<T>(), setAsHashSet}
            };

            powerSet.Remove(new HashSet<T>());
            powerSet.Remove(setAsHashSet);
            var n = powerSet.Count;

            // loop to get all 2^n subset.
            for (var i = 0; i < (1 << n); i++)
            {
                var t = new HashSet<HashSet<T>> { new HashSet<T>(), setAsHashSet };

                

                if (IsTopology(t, setAsHashSet)) topologies.Add(t);

            }


            return null;
        }


        /// <summary>
        /// Determine if the <paramref name="t"/> is a topology of the <paramref name="set"/>.
        /// </summary>
        /// Topology Definition:
        /// let X be a set and let τ be a family of subsets of X. Then τ is called a topology on X if:
        /// * Both the empty set and X are elements of τ.
        /// * Any union of elements of τ is an element of τ.
        /// * Any intersection of finitely many elements of τ is an element of τ.
        /// <typeparam name="T">Type of <paramref name="set"/> elements.</typeparam>
        /// <param name="t">The candidate topology.</param>
        /// <param name="set">The set that candidate topology <paramref name="t"/> defined on.</param>
        /// <returns>Returns true if the <paramref name="t"/> if topology on the <paramref name="set"/>, otherwise return false.</returns>
        public static bool IsTopology<T>(HashSet<HashSet<T>> t, HashSet<T> set)
        {
            var setComparer = Comparer.GetIEqualityComparer((IEnumerable<T> x, IEnumerable<T> y)
                => ((HashSet<T>)x).SetEquals(y));

            if (!t.Contains(set, setComparer) ||
                !t.Contains(new HashSet<T>(), setComparer)) return false;

            // Todo: find a better data structure that support indexer can improve performance a little bit.
            foreach (var e1 in t)
            {
                foreach (var e2 in t)
                {
                    var union = e1.Union(e2);
                    var intersection = e1.Intersect(e2);
                    if (!t.Contains(union, setComparer) || !t.Contains(intersection, setComparer))
                        return false;
                }
            }

            return true;
        }


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
        public static HashSet<HashSet<T>> PowerSet<T>(T[] set)
        {
            var n = set.Length;
            var powerSet = new HashSet<HashSet<T>>();
            
            // Fact: The number of subsets of a set contains n elements is 2^n.
            // loop to get all 2^n subset
            for (var i = 0; i < (1 << n); i++)
            {
                var subset = new HashSet<T>();

                // loop though every element in the set and determine with number 
                // should present in the current subset.
                for (var j = 0; j < n; j++)
                    // if the jth element (bit) in the ith subset (binary number of i) add it.
                    if (((1 << j) & i) > 0) subset.Add(set[j]);

                powerSet.Add(subset);
            }

            return powerSet;
        }

    }

}