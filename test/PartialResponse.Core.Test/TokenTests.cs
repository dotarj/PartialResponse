using System;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class TokenTests
    {
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