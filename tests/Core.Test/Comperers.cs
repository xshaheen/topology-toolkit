using System.Collections.Generic;
using System.Linq;
using Core.Infra;

namespace Core.Test
{
    public class Comperers
    {
        public static Infra.Comparer<HashSet<char>> SetComparer = Comparer.GetIEqualityComparer
            ((HashSet<char> x, HashSet<char> y) => x.SetEquals(y));

        public static Infra.Comparer<HashSet<HashSet<char>>> SetOfSetComparer = Comparer
            .GetIEqualityComparer((HashSet<HashSet<char>> x, HashSet<HashSet<char>> y) =>
            {
                if (x.Count != y.Count) return false;

                var exist = false;

                foreach (var s in x)
                {
                    // ReSharper disable once PossibleUnintendedLinearSearchInSet
                    exist = y.Contains(s, Comparer.GetIEqualityComparer(
                        (HashSet<char> a, HashSet<char> b) => a.SetEquals(b)));
                    if (!exist) break;
                }

                return exist;
            });
    }
}