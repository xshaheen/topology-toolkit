using System.Collections.Generic;
using System.Linq;
using Topology.Infra;
using Xunit;


namespace Topology.Test
{
    public class TopologyUtlTests
    {
        #region Comparer

        private readonly Infra.Comparer<HashSet<char>> _setComparer = Comparer.GetIEqualityComparer
            ((HashSet<char> x, HashSet<char> y) => x.SetEquals(y));

        private readonly Infra.Comparer<HashSet<HashSet<char>>> _setOfSetComparer = Comparer
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

        #endregion

        [Fact]
        public void Can_Determine_Valid_Topology()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char>(),
                new HashSet<char> {'a', 'b', 'c'},
                new HashSet<char> {'a'},
                new HashSet<char> {'b'},
                new HashSet<char> {'a', 'b'},
            };

            // Act
            var result = TopologyUtl.IsTopology(t, set);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Can_Determine_Invalid_Topology()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'b', 'c'},
                new HashSet<char> {'a'},
                new HashSet<char> {'b'},
            };

            // Act
            var result = TopologyUtl.IsTopology(t, set);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Can_Generate_PowerSet()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd'};
            var expected = new HashSet<HashSet<char>>
            {
                new HashSet<char>(),
                new HashSet<char> {'a'},                // 0001
                new HashSet<char> {'b'},                // 0010
                new HashSet<char> {'a', 'b'},           // 0011
                new HashSet<char> {'c'},                // 0100 
                new HashSet<char> {'a', 'c'},           // 0101
                new HashSet<char> {'b', 'c'},           // 0110
                new HashSet<char> {'a', 'b', 'c'},      // 0111
                new HashSet<char> {'d'},                // 1000
                new HashSet<char> {'d', 'a'},           // 1001
                new HashSet<char> {'d', 'b'},           // 1010
                new HashSet<char> {'d', 'a', 'b'},      // 1011
                new HashSet<char> {'d', 'c'},           // 1100
                new HashSet<char> {'d', 'a', 'c'},      // 1101
                new HashSet<char> {'d', 'b', 'c'},      // 1110
                new HashSet<char> {'a', 'b', 'c', 'd'}, // 1111
            };
            // Act
            var result = TopologyUtl.PowerSet(set);

            // Assert
            Assert.Equal(expected, result, _setComparer);
            Assert.Equal(16, result.Count);
        }

        [Fact]
        public void Can_Generate_ClosedSet()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd'};
            var subset1 = new HashSet<char> {'b', 'a', 'd'};
            var expected1 = new HashSet<char> {'c'};

            var subset2 = new HashSet<char>();

            // Act
            var result1 = TopologyUtl.ClosedSet(set, subset1);
            var result2 = TopologyUtl.ClosedSet(set, subset2);
            var result3 = TopologyUtl.ClosedSet(set, set);

            // Assert
            Assert.Equal(expected1, result1, _setComparer);
            // Assert - can generate the closure for empty set
            Assert.Equal(set, result2, _setComparer);
            // Assert - can generate the closure for the set itself.
            Assert.Equal(subset2, result3, _setComparer);
        }

        [Fact]
        public void Can_Generate_Topologies()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c'};

            var expected = new HashSet<HashSet<HashSet<char>>>
            {
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                },
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},
                    new HashSet<char> {'b'},
                    new HashSet<char> {'a', 'b'},
                    new HashSet<char> {'c'},
                    new HashSet<char> {'a', 'c'},
                    new HashSet<char> {'b', 'c'},
                    new HashSet<char> {'a', 'b', 'c'},
                }
            };

            // Act
            var result = TopologyUtl.Topologies(set).ToHashSet();

            // Assert - the two sets is equals
            Assert.Equal(expected, result, _setOfSetComparer);
            Assert.Equal(29, result.Count);
        }

        [Fact]
        public void Can_Find_Limit_Points()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd', 'e'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'b', 'c', 'd', 'e'},
                new HashSet<char>(),
                new HashSet<char> {'a'},
                new HashSet<char> {'c', 'd'},
                new HashSet<char> {'a', 'c', 'd'},
                new HashSet<char> {'b', 'c', 'd', 'e'}
            };
            var subset = new HashSet<char> {'a', 'b', 'c'};
            var expected = new HashSet<char> {'b', 'd', 'e'};

            // Act
            var result = TopologyUtl.LimitPoints(set, subset, t);

            // Assert
            Assert.Equal(expected, result, _setComparer);
        }

        [Fact]
        public void Can_Find_Closure_Points()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'b', 'c', 'd'},
                new HashSet<char>(),
                new HashSet<char> {'a'},
                new HashSet<char> {'b', 'c'},
                new HashSet<char> {'a', 'b', 'c'},
            };
            var subset = new HashSet<char> {'b', 'd'};
            var expected = new HashSet<char> {'b', 'c', 'd'};

            // Act
            var result = TopologyUtl.ClosurePoints(set, subset, t);
            var result2 = TopologyUtl.ClosurePoints(set, new HashSet<char>(), t);
            var result3 = TopologyUtl.ClosurePoints(set, set, t);

            // Assert
            Assert.Equal(expected, result, _setComparer);
            Assert.Equal(new HashSet<char>(), result2);
            Assert.Equal(set, result3);
        }

        [Fact]
        public void Can_Find_Interior_Points()
        {
            // Arrange
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'b', 'c'},
                new HashSet<char>(),
                new HashSet<char> {'a'},
                new HashSet<char> {'a', 'b'},
            };
            var subset = new HashSet<char> {'a', 'c'};
            var expected = new HashSet<char> {'a'};

            // Act
            var result = TopologyUtl.InteriorPoints(subset, t);

            // Assert
            Assert.Equal(expected, result, _setComparer);
        }

        [Fact]
        public void Can_Find_Exterior_Points()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'b', 'c', 'd'},
                new HashSet<char>(),
                new HashSet<char> {'a'},
                new HashSet<char> {'b', 'c'},
                new HashSet<char> {'a', 'b', 'c'},
            };
            var subset = new HashSet<char> {'b', 'd'};
            var expected = new HashSet<char> {'a'};

            // Act
            var result = TopologyUtl.ExteriorPoints(set, subset, t);

            // Assert
            Assert.Equal(expected, result, _setComparer);
        }

        [Fact]
        public void Can_Find_Boundary_Points()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'b', 'c', 'd'},
                new HashSet<char>(),
                new HashSet<char> {'a'},
                new HashSet<char> {'b', 'c'},
                new HashSet<char> {'a', 'b', 'c'},
            };
            var subset = new HashSet<char> {'b', 'd'};
            var expected = new HashSet<char> {'b', 'c', 'd'};

            // Act
            var result = TopologyUtl.BoundaryPoints(set, subset, t);

            // Assert
            Assert.Equal(expected, result, _setComparer);
        }
    }
}