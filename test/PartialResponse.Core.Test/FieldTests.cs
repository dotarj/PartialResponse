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
            var field = new Field("foo");

            // Act
            var result = field.Matches(new [] { "bar" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentNestedValues()
        {
            // Arrange
            var field = new Field("foo/bar");

            // Act
            var result = field.Matches(new [] { "foo", "baz" });

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSameValues()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new [] { "foo" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSamePrefixValues()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new [] { "foo", "bar" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForOtherSuffixValue()
        {
            // Arrange
            var field = new Field("foo/bar");

            // Act
            var result = field.Matches(new [] { "foo" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForWildcard()
        {
            // Arrange
            var field = new Field("*");

            // Act
            var result = field.Matches(new [] { "foo" });

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheMatchesMethodShouldIgnoreCase()
        {
            // Arrange
            var field = new Field("foo");

            // Act
            var result = field.Matches(new [] { "FOO" }, true);

            // Assert
            Assert.True(result);
        }
    }
}