using System;
using System.Collections.Generic;
using System.Linq;
using static Core.TopologyUtl;
using static Core.SetUtl;

namespace Core
{
    public class SubsetCategories<T>
    {
        public SubsetCategories(
            HashSet<T> set,
            HashSet<T> subset,
            HashSet<HashSet<T>> topology)
        {
            // assert that the topology is a valid topology on the set
            if (!subset.IsSubsetOf(set))
                throw new Exception(
                    "The given subset is not a valid subset of the set.");

            // assert that the topology is a valid topology on the set
            if (!IsTopology(topology, set))
                throw new Exception(
                    "The given topology is not a valid topology on the set.");

            Set = set;
            Subset = subset;
            Topology = topology;
        }

        public HashSet<T> Set { get; }
        public HashSet<T> Subset { get; }
        public HashSet<HashSet<T>> Topology { get; }

        /// <remarks>
        /// Finds limit points (aka accumulation points or cluster points)
        /// Definition: Let A be a subset of a topological space (X, t). A point p ∈ X is said to
        /// be a limit point (or accumulation point or cluster point) of A if every open set G
        /// containing p contains a point of A different from p.
        /// Mathematical form: { p ∈ X: ∀G ∈ t, p ∈ G, A∩(G - p) != {} }
        /// </remarks>>
        public HashSet<T> LimitPoints 
        {
            get
            {
                var limitPoints = new HashSet<T>();

                foreach (var point in Set)
                {
                    var isLimit = true;
                    foreach (var e in Topology.Where(g => g.Contains(point)))
                    {
                        var except = e.Except(new[] {point}).ToArray();

                        if (!except.Any())
                        {
                            isLimit = false;
                            break;
                        }

                        if (Subset.Intersect(except).Any()) continue;

                        isLimit = false;
                        break;
                    }

                    if (isLimit) limitPoints.Add(point);
                }

                return limitPoints;
            } 
        }

        /// <remarks>
        /// Definition:
        /// Let (X,τ) be a topological space and A ⊆ X, then the closure of A
        /// is the intersection of all closed sets containing A
        /// i.e. the smallest closed set containing A.
        /// Example
        /// Let X={a,b,c,d} with topology τ={{},{a},{b,c},{a,b,c},X} and A={b,d}.
        /// - Open sets are   {},  {a}  {b,c},{a,b,c},X
        /// - Closed sets are X,{b,c,d},{a,d},{d}    ,{}
        /// - Closed sets containing A are X,{b,c,d}
        /// - Now closure(A) = {b,c,d} ∩ X = {b,c,d}
        /// </remarks>>
        public HashSet<T> ClosurePoints
        {
            get
            {
                // Generate all closed set for every element in t
                var allClosedSets = new HashSet<HashSet<T>>(Topology.Count);
                foreach (var e in Topology)
                    allClosedSets.Add(ClosedSet(Set, e));

                // Get the closed set that containing the subset.
                var closedSets = allClosedSets.Where(Subset.IsSubsetOf).ToArray();

                var closurePoints = closedSets.First();

                for (var i = 1; i < closedSets.Length; i++)
                    closurePoints.IntersectWith(closedSets[i]);

                return closurePoints;
            }
        }

        /// <remarks>
        /// Definition:
        /// * Let (X,t) be the topological space and A ⊆ X, then a point p∈A is said
        ///   to be an interior point of set A, if there exists an open set O such that p ∈ O ⊆ A
        /// * Interior of a set: is defined to be the union of all open sets contained in A.
        /// </remarks>>
        public HashSet<T> InteriorPoints
        {
            get
            {
                var interiors = new HashSet<T>();

                foreach (var g in Topology.Where(e => e.IsSubsetOf(Subset)))
                    interiors.UnionWith(g);

                return interiors;
            }
        }

        /// <remarks>
        /// Definition:
        ///   Let (X,τ) be a topological space and A ⊆ X, then a point p ∈ X,
        ///   is said to be an exterior point of A if there exists an open set O,
        ///   such that p ∈ O ∈ clo(A)
        /// Exterior of a Set: A - clo(A)
        /// </remarks>>
        public HashSet<T> ExteriorPoints
            => Set.Except(ClosurePoints).ToHashSet();

        /// <remarks>
        /// Find boundary points (aka frontier points)
        /// Theorems: If A is a subset of a topological space X,
        /// then boundary(A)= closure(A) − int(T)
        /// </remarks>>
        public HashSet<T> BoundaryPoints
        {
            get
            {
                var closurePoints = ClosurePoints;
                closurePoints.ExceptWith(InteriorPoints);
                return closurePoints;
            }
        }

        /// <remarks>
        /// Accuracy: Number of interior points divided by number of closure points.
        /// </remarks>>
        public double Accuracy
            => (double) InteriorPoints.Count / ClosurePoints.Count;
    }
}