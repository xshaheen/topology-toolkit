﻿using System.Collections.Generic;
using Infra.Infra;
using Xunit;
using static Infra.SetUtl;

namespace Infra.Test
{
    public class SetUtlTests
    {
        #region Comparer

        private readonly Infra.Comparer<HashSet<char>> _setComparer = Comparer.GetIEqualityComparer
            ((HashSet<char> x, HashSet<char> y) => x.SetEquals(y));

        #endregion

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
            var result1 = ClosedSet(set, subset1);
            var result2 = ClosedSet(set, subset2);
            var result3 = ClosedSet(set, set);

            // Assert
            Assert.Equal(expected1, result1, _setComparer);
            // Assert - can generate the closure for empty set
            Assert.Equal(set, result2, _setComparer);
            // Assert - can generate the closure for the set itself.
            Assert.Equal(subset2, result3, _setComparer);
        }
    }
}