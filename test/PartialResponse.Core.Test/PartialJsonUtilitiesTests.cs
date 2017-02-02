using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class PartialJsonUtilitiesTests
    {
        [Fact]
        public void ShouldThrowIfValueIfNull()
        {
            // Arrange
            string value = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => PartialJsonUtilities.GetRegexPatternForField(value));
        }

        [Fact]
        public void ShouldParseSingleLevelValue()
        {
            // Arrange
            var value = "foo";

            // Act
            var pattern = PartialJsonUtilities.GetRegexPatternForField(value);

            // Assert
            Assert.Equal(@"^(((foo(/.+)?)|\*))$", pattern);
        }

        [Fact]
        public void ShouldParseMultipleLevelValue()
        {
            // Arrange
            var value = "foo/bar";

            // Act
            var pattern = PartialJsonUtilities.GetRegexPatternForField(value);

            // Assert
            Assert.Equal(@"^((foo(/((bar(/.+)?)|\*))?)|\*)$", pattern);
        }
    }
}