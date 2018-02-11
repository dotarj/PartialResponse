// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

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
            var position = 1;

            // Act
            var token = new Token(value, type, position);

            // Assert
            Assert.Equal(value, token.Value);
        }

        [Fact]
        public void TheConstructorShouldSetType()
        {
            // Arrange
            var value = "foo";
            var type = TokenType.Identifier;
            var position = 1;

            // Act
            var token = new Token(value, type, position);

            // Assert
            Assert.Equal(type, token.Type);
        }

        [Fact]
        public void TheConstructorShouldSetPosition()
        {
            // Arrange
            var value = "foo";
            var type = TokenType.Identifier;
            var position = 1;

            // Act
            var token = new Token(value, type, position);

            // Assert
            Assert.Equal(position, token.Position);
        }
    }
}