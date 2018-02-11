// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

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
            var source = new StringReader(string.Empty);
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Empty(context.Values);
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        [InlineData(")", TokenType.RightParenthesis)]
        [InlineData(",", TokenType.Comma)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAtStart(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader(value);
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
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
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
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
            Assert.Equal(new[] { "foo/bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        [InlineData(")", TokenType.RightParenthesis)]
        [InlineData(",", TokenType.Comma)]
        [InlineData("", TokenType.Eof)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterForwardSlash(string value, TokenType tokenType)
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
            Assert.Equal(new[] { "foo", "bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        [InlineData(")", TokenType.RightParenthesis)]
        [InlineData(",", TokenType.Comma)]
        [InlineData("", TokenType.Eof)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterComma(string value, TokenType tokenType)
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
            Assert.Equal(new[] { "foo/bar" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseGroupedMultipleIdentifiers()
        {
            // Arrange
            var source = new StringReader("foo(bar,baz)");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar", "foo/baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseIdentifierAfterGroupedMultipleIdentifiers()
        {
            // Arrange
            var source = new StringReader("foo(bar,baz),qux");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar", "foo/baz", "qux" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseGroupedNestedIdentifier()
        {
            // Arrange
            var source = new StringReader("foo(bar/baz)");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar/baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseGroupedMultipleNestedIdentifier()
        {
            // Arrange
            var source = new StringReader("foo(bar/baz,qux/quux)");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar/baz", "foo/qux/quux" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldSetErrorIfTooManyLeftParenthesis()
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
        public void TheParseMethodShouldSetErrorIfTooManyRightParenthesis()
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
        [InlineData(")", TokenType.RightParenthesis)]
        [InlineData(",", TokenType.Comma)]
        [InlineData("", TokenType.Eof)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterLeftParenthesis(string value, TokenType tokenType)
        {
            // Arrange
            var source = new StringReader($"foo({value}");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(tokenType, context.Error.Type);
        }

        [Theory]
        [InlineData("/", TokenType.ForwardSlash)]
        [InlineData("(", TokenType.LeftParenthesis)]
        public void TheParseMethodShouldSetErrorIfIllegalTokenAfterRightParenthesis(string value, TokenType tokenType)
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
            Assert.Equal(new[] { "foo/bar", "baz" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldParseIdentifierAfterNestedGroupedIdentifiers()
        {
            // Arrange
            var source = new StringReader("foo(bar(baz)),qux");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo/bar/baz", "qux" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreSpace()
        {
            // Arrange
            var source = new StringReader(" foo");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreTab()
        {
            // Arrange
            var source = new StringReader("\tfoo");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreCarriageReturn()
        {
            // Arrange
            var source = new StringReader("\rfoo");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }

        [Fact]
        public void TheParseMethodShouldIgnoreNewLine()
        {
            // Arrange
            var source = new StringReader("\nfoo");
            var context = new ParserContext(source);
            var parser = new Parser(context);

            // Act
            parser.Parse();

            // Assert
            Assert.Equal(new[] { "foo" }, context.Values.Select(value => string.Join("/", value.Parts)));
        }
    }
}