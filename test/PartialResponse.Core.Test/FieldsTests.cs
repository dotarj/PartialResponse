using System;
using System.Linq;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class FieldsTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfValueIsNull()
        {
            // Arrange
            string value = null;
            Fields fields = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => Fields.TryParse(value, out fields));
        }

        [Fact]
        public void TheTryParseMethodShouldReturnEmptyCollectionIfValueIsEmpty()
        {
            // Arrange
            var value = "";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Empty(fields.Values);
        }

        [Fact]
        public void TheTryParseMethodShouldReturnFalseIfInvalidValue()
        {
            // Arrange
            var value = "foo(";
            Fields fields = null;

            // Act
            var result = Fields.TryParse(value, out fields);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TheTryParseMethodShouldReturnTrueIfValidValue()
        {
            // Arrange
            var value = "foo";
            Fields fields = null;

            // Act
            var result = Fields.TryParse(value, out fields);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void TheTryParseMethodShouldParseProperty()
        {
            // Arrange
            var value = "foo";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo", fields.Values.Single().ToString());
        }

        [Fact]
        public void TheTryParseMethodShouldParseMultipleProperties()
        {
            // Arrange
            var value = "foo,bar";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo", "bar" }, fields.Values.Select(v => v.ToString()));
        }

        [Fact]
        public void TheTryParseMethodShouldParseGroupedProperty()
        {
            // Arrange
            var value = "foo(bar)";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar", fields.Values.Single().ToString());
        }

        [Fact]
        public void TheTryParseMethodShouldParseGroupedMultipleProperties()
        {
            // Arrange
            var value = "foo(bar,baz)";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo/bar", "foo/baz" }, fields.Values.Select(v => v.ToString()));
        }

        [Fact]
        public void TheTryParseMethodShouldParseNestedGroupedProperty()
        {
            // Arrange
            var value = "foo(bar(baz))";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar/baz", fields.Values.Single().ToString());
        }

        [Fact]
        public void TheTryParseMethodShouldParseMultipleGroupedMultipleProperties()
        {
            // Arrange
            var value = "foo(bar(baz,qux))";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo/bar/baz", "foo/bar/qux" }, fields.Values.Select(v => v.ToString()));
        }

        [Fact]
        public void TheTryParseMethodShouldParseNestedProperty()
        {
            // Arrange
            var value = "foo/bar";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar", fields.Values.Single().ToString());
        }

        [Fact]
        public void TheTryParseMethodShouldParseGoupedNestedProperty()
        {
            // Arrange
            var value = "foo(bar/baz)";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar/baz", fields.Values.Single().ToString());
        }

        [Fact]
        public void TheTryParseMethodShouldParseGoupedNestedMultipleProperties()
        {
            // Arrange
            var value = "foo(bar/baz,qux/quux)";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo/bar/baz", "foo/qux/quux" }, fields.Values.Select(v => v.ToString()));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentValues()
        {
            // Arrange
            var value = "foo";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.False(fields.Matches("bar"));
        }

        [Fact]
        public void TheMatchesMethodShouldReturnTrueForSameValues()
        {
            // Arrange
            var value = "foo";
            Fields fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.True(fields.Matches("foo"));
        }
    }
}