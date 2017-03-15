using System;
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
            Fields fields;

            // Act
            Assert.Throws<ArgumentNullException>(() => Fields.TryParse(value, out fields));
        }

        [Fact]
        public void TheValuesPropertyShouldReturnEmptyValuesIfDefault()
        {
            // Arrange
            var fields = default(Fields);

            // Assert
            Assert.Empty(fields.Values);
        }

        [Fact]
        public void TheMatchesMethodShouldReturnFalseForDifferentValues()
        {
            // Arrange
            var value = "foo";
            Fields fields;

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
            Fields fields;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.True(fields.Matches("foo"));
        }

        [Fact]
        public void TheMatchesMethodShouldIgnoreCase()
        {
            // Arrange
            var value = "foo";
            Fields fields;

            // Act
            Fields.TryParse(value, out fields);

            // Assert
            Assert.True(fields.Matches("FOO", true));
        }
    }
}