using System.Collections.Generic;
using Xunit;
using static Core.Test.Comperers;

namespace Core.Test
{
    public class SubsetCategoriesTests
    {
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
            var categories = new SubsetCategories<char>(set, subset, t);

            // Act
            var result = categories.LimitPoints;

            // Assert
            Assert.Equal(expected, result, SetComparer);
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

            var categories1 = new SubsetCategories<char>(set, subset, t);
            var categories2 = new SubsetCategories<char>(set, new HashSet<char>(), t);
            var categories3 = new SubsetCategories<char>(set, set, t);

            // Act
            var result = categories1.ClosurePoints;
            var result2 = categories2.ClosurePoints;
            var result3 = categories3.ClosurePoints;

            // Assert
            Assert.Equal(expected, result, SetComparer);
            Assert.Equal(new HashSet<char>(), result2);
            Assert.Equal(set, result3);
        }

        [Fact]
        public void Can_Find_Interior_Points()
        {
            var set = new HashSet<char> {'a', 'b', 'c'};
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
            var categories = new SubsetCategories<char>(set, subset, t);

            // Act
            var result = categories.InteriorPoints;

            // Assert
            Assert.Equal(expected, result, SetComparer);
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
            var categories = new SubsetCategories<char>(set, subset, t);

            // Act
            var result = categories.ExteriorPoints;

            // Assert
            Assert.Equal(expected, result, SetComparer);
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
            var categories = new SubsetCategories<char>(set, subset, t);

            // Act
            var result = categories.BoundaryPoints;

            // Assert
            Assert.Equal(expected, result, SetComparer);
        }
    }
}