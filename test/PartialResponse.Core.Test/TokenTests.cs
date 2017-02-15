using System;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class TokenTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfValueIsNull()
        {
            // Arrange
            string value = null;
            var type = TokenType.Identifier;

            // Act
            Assert.Throws<ArgumentNullException>("value", () => new Token(value, type));
        }

        [Fact]
        public void TheConstructorShouldSetValue()
        {
            // Arrange
            var value = "foo";
            var type = TokenType.Identifier;

            // Act
            var token = new Token(value, type);

            // Assert
            Assert.Equal(value, token.Value);
        }

        [Fact]
        public void TheConstructorShouldSetType()
        {
            // Arrange
            var value = "foo";
            var type = TokenType.Identifier;

            // Act
            var token = new Token(value, type);

            // Assert
            Assert.Equal(type, token.Type);
        }
    }
}