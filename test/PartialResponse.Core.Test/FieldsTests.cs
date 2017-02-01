using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class FieldsTests
    {
        [Fact]
        public void ShouldThrowIfValueIfNull()
        {
            // Arrange
            string value = null;
            Collection<string> fields = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => Fields.TryParse(value, out fields));
        }

        [Fact]
        public void ShouldReturnEmptyCollectionIfValueIsEmpty()
        {
            // Arrange
            var value = "";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Empty(fields);
        }

        [Fact]
        public void ShouldReturnFalseIfInvalidValue()
        {
            // Arrange
            var value = "foo(";
            Collection<string> fields = null;

            // Act
            var result = Fields.TryParse(value, out fields);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldReturnTrueIfValidValue()
        {
            // Arrange
            var value = "foo";
            Collection<string> fields = null;

            // Act
            var result = Fields.TryParse(value, out fields);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldParseProperty()
        {
            // Arrange
            var value = "foo";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo", fields.Single());
        }

        [Fact]
        public void ShouldParseMultipleProperties()
        {
            // Arrange
            var value = "foo,bar";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo", "bar" }, fields);
        }

        [Fact]
        public void ShouldParseGroupedProperty()
        {
            // Arrange
            var value = "foo(bar)";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar", fields.Single());
        }

        [Fact]
        public void ShouldParseGroupedMultipleProperties()
        {
            // Arrange
            var value = "foo(bar,baz)";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo/bar", "foo/baz" }, fields);
        }

        [Fact]
        public void ShouldParseNestedGroupedProperty()
        {
            // Arrange
            var value = "foo(bar(baz))";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar/baz", fields.Single());
        }

        [Fact]
        public void ShouldParseMultipleGroupedMultipleProperties()
        {
            // Arrange
            var value = "foo(bar(baz,qux))";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo/bar/baz", "foo/bar/qux" }, fields);
        }

        [Fact]
        public void ShouldParseNestedProperty()
        {
            // Arrange
            var value = "foo/bar";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar", fields.Single());
        }

        [Fact]
        public void ShouldParseGoupedNestedProperty()
        {
            // Arrange
            var value = "foo(bar/baz)";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal("foo/bar/baz", fields.Single());
        }

        [Fact]
        public void ShouldParseGoupedNestedMultipleProperties()
        {
            // Arrange
            var value = "foo(bar/baz,qux/quux)";
            Collection<string> fields = null;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.Equal(new [] { "foo/bar/baz", "foo/qux/quux" }, fields);
        }
    }
}