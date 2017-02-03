using System;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class FieldTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfValueIsNull()
        {
            // Arrange
            string value = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => new Field(value));
        }

        [Fact]
        public void ThePartsPropertyShouldContainSingleValue()
        {
            // Arrange
            var value = "foo";

            // Act
            var field = new Field(value);

            // Assert
            Assert.Equal(new [] { "foo" }, field.Parts);
        }

        [Fact]
        public void ThePartsPropertyShouldContainMultipleValues()
        {
            // Arrange
            var value = "foo/bar";

            // Act
            var field = new Field(value);

            // Assert
            Assert.Equal(new [] { "foo", "bar" }, field.Parts);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentValues()
        {
            // Arrange
            var value = "foo";

            // Act
            var field = new Field(value);

            // Assert
            Assert.False(field.Matches("bar"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentNestedValues()
        {
            // Arrange
            var value = "foo/bar";

            // Act
            var field = new Field(value);

            // Assert
            Assert.False(field.Matches("foo/baz"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSameValues()
        {
            // Arrange
            var value = "foo";

            // Act
            var field = new Field(value);

            // Assert
            Assert.True(field.Matches("foo"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSamePrefixValues()
        {
            // Arrange
            var value = "foo";

            // Act
            var field = new Field(value);

            // Assert
            Assert.True(field.Matches("foo/bar"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForOtherSuffixValue()
        {
            // Arrange
            var value = "foo/bar";

            // Act
            var field = new Field(value);

            // Assert
            Assert.False(field.Matches("foo"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForWildcard()
        {
            // Arrange
            var value = "*";

            // Act
            var field = new Field(value);

            // Assert
            Assert.True(field.Matches("foo"));
        }
    }
}