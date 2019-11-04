using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Infra.Test.Comperers;

namespace Infra.Test
{
    public class TopologyUtlTests
    {
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
        public void Can_Find_NeighbourhoodSystem()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c', 'd', 'e'};
            var t = new HashSet<HashSet<char>>
            {
                new HashSet<char>(),
                new HashSet<char> {'a'},
                new HashSet<char> {'a', 'b'},
                new HashSet<char> {'a', 'c', 'd'},
                new HashSet<char> {'a', 'b', 'c', 'd'},
                new HashSet<char> {'a', 'b', 'c', 'd', 'e'},
            };

            // Act
            var result = TopologyUtl.NeighbourhoodSystem(set, t, 'c');

            // Assert
            Assert.Equal(new HashSet<HashSet<char>>
            {
                new HashSet<char> {'a', 'c', 'd'},
                new HashSet<char> {'a', 'b', 'c', 'd'},
                new HashSet<char> {'a', 'c', 'd', 'e'},
                new HashSet<char> {'a', 'b', 'c', 'd', 'e'},
            }, result, SetComparer);
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
            Assert.Equal(expected, result, SetOfSetComparer);
            Assert.Equal(29, result.Count);
        }
    }
}