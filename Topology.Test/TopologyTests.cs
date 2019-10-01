using System.Collections.Generic;
using Xunit;

namespace Topology.Test
{
    public class TopologyTests
    {
        [Fact]
        public void Can_Determine_Valid_Topology()
        {
            // Arrange
            var set = new HashSet<char> { 'a', 'b', 'c' };
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
            var set = new HashSet<char> { 'a', 'b', 'c' };
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
            var set = new[] { 'a', 'b', 'c', 'd' };

            // Act
            var result = Topology.PowerSet(set);

            // Assert
            Assert.Equal(new HashSet<HashSet<char>>
            {
                new HashSet<char>(),
                new HashSet<char> {'a'},                // 0001
                new HashSet<char> {'b'},                // 0010
                new HashSet<char> {'a', 'b'},           // 0011
                new HashSet<char> {'c'},                // 0100 
                new HashSet<char> {'c', 'a'},           // 0101
                new HashSet<char> {'c', 'b'},           // 0110
                new HashSet<char> {'c', 'b', 'a'},      // 0111
                new HashSet<char> {'d'},                // 1000
                new HashSet<char> {'d', 'a'},           // 1001
                new HashSet<char> {'d', 'b'},           // 1010
                new HashSet<char> {'d', 'b', 'a'},      // 1011
                new HashSet<char> {'d', 'c'},           // 1100
                new HashSet<char> {'d', 'c', 'a'},      // 1101
                new HashSet<char> {'d', 'c', 'b'},      // 1110
                new HashSet<char> {'a', 'b', 'c', 'd'}, // 1111
            }, result, Comparer.GetIEqualityComparer((IEnumerable<char> x, IEnumerable<char> y)
                => ((HashSet<char>)x).SetEquals(y)));
        }

        [Fact]
        public void Can_Generate_Topologies()
        {
            // Arrange
            var set = new[] { 'a', 'b', 'c' };

            // Act
            var result = Topology.Topologies(set);

            // Assert
            Assert.Equal(new HashSet<HashSet<HashSet<char>>>
            {
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 1 // trivial topology
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 2 // singles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 3 // singles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 4 // doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},     // 100
                    new HashSet<char> {'a', 'b', 'c'},
                }, // 5 // single
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 6 // doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 7  // doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b', 'a'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 8 // single-doubles (disjoint)
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 9 // single-doubles (disjoint)
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 10 // single-doubles (disjoint)
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 11 // single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 12 // single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 13 // single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 14 // single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 15 // single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 16 // single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 17 // single-single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 18 // single-single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 19 // single-single-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 20 // single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 21 // single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 22 // single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 23 // single-single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'b', 'a'}, // 011
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 24 // single-single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b'}, // 011
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 25 // single-single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b'},      // 100
                    new HashSet<char> {'b', 'a'}, // 101
                    new HashSet<char> {'c', 'a'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 26 // single-single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 27 // single-single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'c'},      // 100
                    new HashSet<char> {'b', 'a'}, // 101
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                }, // 28 // single-single-double-doubles
                new HashSet<HashSet<char>>
                {
                    new HashSet<char>(),
                    new HashSet<char> {'a'},      // 001
                    new HashSet<char> {'b'},      // 010
                    new HashSet<char> {'a', 'b'}, // 011
                    new HashSet<char> {'c'},      // 100 
                    new HashSet<char> {'c', 'a'}, // 101
                    new HashSet<char> {'c', 'b'}, // 110
                    new HashSet<char> {'c', 'b', 'a'},
                } // 29 // power set
            }, result); // set comparer
        }

    }
}

// new HashSet{ new HashSet<char>{}, new HashSet<char>{'a', 'b', 'c'} }