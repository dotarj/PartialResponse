using System;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class UnexpectedTokenErrorTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfTokenIsNull()
        {
            // Arrange
            Token token = null;

            // Act
            Assert.Throws<ArgumentNullException>("token", () => new UnexpectedTokenError(token));
        }

        [Fact]
        public void TheConstructorShouldSetValue()
        {
            // Arrange
            var token = new Token("foo", TokenType.Identifier, 0);

            // Act
            var error = new UnexpectedTokenError(token);

            // Assert
            Assert.Equal(token.Value, error.Value);
        }

        [Fact]
        public void TheConstructorShouldSetType()
        {
            // Arrange
            var token = new Token("foo", TokenType.Identifier, 0);

            // Act
            var error = new UnexpectedTokenError(token);

            // Assert
            Assert.Equal(token.Type, error.Type);
        }

        [Fact]
        public void TheConstructorShouldSetPosition()
        {
            // Arrange
            var token = new Token("foo", TokenType.Identifier, 0);

            // Act
            var error = new UnexpectedTokenError(token);

            // Assert
            Assert.Equal(token.Position, error.Position);
        }
    }
}