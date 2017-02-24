using System;
using System.IO;
using System.Linq;
using Xunit;

namespace PartialResponse.Core.Test
{
    public class ParserTests
    {
        [Fact]
        public void TheConstructorShouldThrowIfContextIsNull()
        {
            // Arrange
            ParserContext context = null;

            // Act
            Assert.Throws<ArgumentNullException>("context", () => new Parser(context));
        }

        [Fact]
        public void TheParseMethodShouldParseEmptySource()
        {
            // Arrange
            var source = new StringReader("");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Empty(context.Values);
        }

        [Fact]
        public void TheParseMethodShouldSetErrorIfParenthesisNotClosed()
        {
            // Arrange
            var source = new StringReader("foo(bar");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(TokenType.Eof, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldSetErrorIfIdentifierExpected()
        {
            // Arrange
            var source = new StringReader("/");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(TokenType.ForwardSlash, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldParseSingleIdentifier()
        {
            // Arrange
            var source = new StringReader("foo");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new [] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseNestedIdentifier()
        {
            // Arrange
            var source = new StringReader("foo/bar");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new [] { "foo/bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        [InlineData(")", TokenType.RightParenthesis)]
        [InlineData(",", TokenType.Comma)]
        [InlineData("", TokenType.Eof)]
        public void TheParseMethodShouldParseSetErrorIfIllegalTokenAfterForwardSlash(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo/{value}");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldParseMultipleIdentifiers()
        {
            // Arrange
            var source = new StringReader("foo,bar");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new [] { "foo", "bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        [InlineData(")", TokenType.RightParenthesis)]
        [InlineData(",", TokenType.Comma)]
        [InlineData("", TokenType.Eof)]
        public void TheParseMethodShouldParseSetErrorIfIllegalTokenAfterComma(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo,{value}");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldParseGroupedIdentifier()
        {
            // Arrange
            var source = new StringReader("foo(bar)");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new [] { "foo/bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldSetErrorIfTooManyClosingParenthesis()
        {
            // Arrange
            var source = new StringReader("foo(bar))");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(TokenType.RightParenthesis, context.Error.Type);
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        public void TheParseMethodShouldParseSetErrorIfIllegalTokenAfterRightParenthesis(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo(bar){value}");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Fact]
        public void TheParseMethodShouldParseIdentifierAfterGroupedIdentifier()
        {
            // Arrange
            var source = new StringReader("foo(bar),baz");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new [] { "foo/bar", "baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }
    }
}