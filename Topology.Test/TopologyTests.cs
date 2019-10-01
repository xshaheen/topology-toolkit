using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Topology.Test
{
    public class TopologyTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TopologyTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

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
            var result = Topology.IsTopology(t, set);

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
            var result = Topology.IsTopology(t, set);

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
            var result = Topology.PowerSet(set);

            // _testOutputHelper.WriteLine(Topology.SetToString(result.Except(expected).ToHashSet()));

            // Assert
            Assert.Equal(expected, result,
                Comparer.GetIEqualityComparer((HashSet<char> x, HashSet<char> y)
                    => x.SetEquals(y)));
            Assert.Equal(8, result.Count);

        }

        [Fact]
        public void Can_Generate_Topologies()
        {
            // Arrange
            var set = new HashSet<char> {'a', 'b', 'c'};
            
            var setComparer = Comparer.GetIEqualityComparer(
                (HashSet<HashSet<char>> x, HashSet<HashSet<char>> y) =>
                {
                    if (x.Count != y.Count) return false;
                    
                    var exist = false;
                    
                    foreach (var s in x)
                    {
                        exist = y.Contains(s, Comparer.GetIEqualityComparer(
                            (HashSet<char> a, HashSet<char> b) => a.SetEquals(b)));
                        if (!exist) break;
                    }
                    
                    return exist;
                });

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
            var result = Topology.Topologies(set);
            
            // _testOutputHelper.WriteLine(Topology.SetToString(result.Except(expected, setComparer).ToHashSet()));

            // Assert - the two sets is equals
            Assert.Equal(expected, result, setComparer);
            Assert.Equal(29, result.Count);
        }
    }
}