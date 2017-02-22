using System;
using System.IO;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class TokenizerTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfReaderIsNull()
        {
            // Arrange
            TextReader reader = null;

            // Act
            Assert.Throws<ArgumentNullException>("reader", () => new Tokenizer(reader));
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeForwardSlash()
        {
            // Arrange
            var reader = new StringReader("/");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.ForwardSlash, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueForwardSlash()
        {
            // Arrange
            var reader = new StringReader("/");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("/", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeLeftParenthesis()
        {
            // Arrange
            var reader = new StringReader("(");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.LeftParenthesis, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueLeftParenthesis()
        {
            // Arrange
            var reader = new StringReader("(");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("(", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeRightParenthesis()
        {
            // Arrange
            var reader = new StringReader(")");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.RightParenthesis, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueRightParenthesis()
        {
            // Arrange
            var reader = new StringReader(")");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(")", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeComma()
        {
            // Arrange
            var reader = new StringReader(",");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Comma, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueComma()
        {
            // Arrange
            var reader = new StringReader(",");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(",", token.Value);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void TheNextTokenMethodShouldReturnTokenTypeWhiteSpace(string value)
        {
            // Arrange
            var reader = new StringReader(value);
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.WhiteSpace, token.Type);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\r")]
        [InlineData("\n")]
        public void TheNextTokenMethodShouldReturnTokenValueWhiteSpace(string value)
        {
            // Arrange
            var reader = new StringReader(value);
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(value, token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeIdentifier()
        {
            // Arrange
            var reader = new StringReader("foo");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueIdentifier()
        {
            // Arrange
            var reader = new StringReader("foo");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeEofIfEndReached()
        {
            // Arrange
            var reader = new StringReader("foo");
            var tokenizer = new Tokenizer(reader);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Eof, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueIdentifierBeforeForwardSlash()
        {
            // Arrange
            var reader = new StringReader("foo/");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeIdentifierBeforeForwardSlash()
        {
            // Arrange
            var reader = new StringReader("foo/");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueIdentifierAfterForwardSlash()
        {
            // Arrange
            var reader = new StringReader("/foo");
            var tokenizer = new Tokenizer(reader);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeIdentifierAfterForwardSlash()
        {
            // Arrange
            var reader = new StringReader("/foo");
            var tokenizer = new Tokenizer(reader);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueIdentifierBeforeWhiteSpace()
        {
            // Arrange
            var reader = new StringReader("foo ");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeIdentifierBeforeWhiteSpace()
        {
            // Arrange
            var reader = new StringReader("foo ");
            var tokenizer = new Tokenizer(reader);

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenValueIdentifierAfterWhiteSpace()
        {
            // Arrange
            var reader = new StringReader(" foo");
            var tokenizer = new Tokenizer(reader);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal("foo", token.Value);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenTypeIdentifierAfterWhiteSpace()
        {
            // Arrange
            var reader = new StringReader(" foo");
            var tokenizer = new Tokenizer(reader);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Fact]
        public void TheNextTokenMethodShouldReturnTokenPosition()
        {
            // Arrange
            var reader = new StringReader("foo/");
            var tokenizer = new Tokenizer(reader);

            tokenizer.NextToken();

            // Act
            var token = tokenizer.NextToken();

            // Assert
            Assert.Equal(3, token.Position);
        }
    }
}