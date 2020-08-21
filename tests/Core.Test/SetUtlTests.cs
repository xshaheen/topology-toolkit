using System.Collections.Generic;
using Xunit;
using static Core.SetUtl;
using static Core.Test.Comperers;

namespace Core.Test
{
    public class SetUtlTests
    {
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
            var result = PowerSet(set);

            // Assert
            Assert.Equal(expected, result, SetComparer);
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
            var result1 = ClosedSet(set, subset1);
            var result2 = ClosedSet(set, subset2);
            var result3 = ClosedSet(set, set);

            // Assert
            Assert.Equal(expected1, result1, SetComparer);
            // Assert - can generate the closure for empty set
            Assert.Equal(set, result2, SetComparer);
            // Assert - can generate the closure for the set itself.
            Assert.Equal(subset2, result3, SetComparer);
        }
    }
}