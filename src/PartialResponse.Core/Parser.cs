using System;
using System.Collections.Generic;

namespace PartialResponse.Core
{
    /// <summary>
    /// A parser for fields parameter parsing.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class Parser
    {
        private readonly Stack<string> prefixes = new Stack<string>();
        private readonly Dictionary<TokenType, Action> handlers;
        private readonly ParserContext context;
        private readonly Tokenizer tokenizer;

        private Token currentToken;
        private int depth;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="context">A <see cref="ParserContext"/> used by the parser.</param>
        public Parser(ParserContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.context = context;

            this.tokenizer = new Tokenizer(context.Source);
            this.handlers = new Dictionary<TokenType, Action>
            {
                { TokenType.ForwardSlash, this.HandleForwardSlash },
                { TokenType.LeftParenthesis, this.HandleLeftParenthesis },
                { TokenType.RightParenthesis, this.HandleRightParenthesis },
                { TokenType.Comma, this.HandleComma },
                { TokenType.Eof, this.HandleEof }
            };
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        public void Parse()
        {
            this.NextToken();

            this.HandleIdentifier(acceptEnd: true);
        }

        private void HandleIdentifier(bool acceptEnd)
        {
            if (this.currentToken.Type == TokenType.Eof)
            {
                if (!acceptEnd)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);
                }

                if (this.depth > 0)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);
                }

                return;
            }

            if (currentToken.Type != TokenType.Identifier)
            {
                this.context.Error = new UnexpectedTokenError(this.currentToken);
            }

            var prefix = this.prefixes.Count > 0 ? $"{this.prefixes.Pop()}/{this.currentToken.Value}" : this.currentToken.Value;

            this.prefixes.Push(prefix);

            this.NextToken();

            Action handler;

            if (!this.handlers.TryGetValue(this.currentToken.Type, out handler))
            {
                this.context.Error = new UnexpectedTokenError(this.currentToken);
            }

            handler();
        }

        private void HandleForwardSlash()
        {
            this.NextToken();
            this.HandleIdentifier(acceptEnd: false);
        }

        private void HandleLeftParenthesis()
        {
            this.depth++;

            var value = this.prefixes.Peek();

            this.prefixes.Push(value);

            this.NextToken();
            this.HandleIdentifier(acceptEnd: false);
        }

        private void HandleRightParenthesis()
        {
            var value = this.prefixes.Pop();

            this.context.Values.Add(new Field(value));

            do
            {
                this.depth--;

                if (this.depth < 0)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);

                    return;
                }

                this.NextToken();
            }
            while (this.currentToken.Type == TokenType.RightParenthesis);

            if (this.currentToken.Type != TokenType.Eof)
            {
                if (this.currentToken.Type != TokenType.Comma)
                {
                    this.context.Error = new UnexpectedTokenError(this.currentToken);

                    return;
                }

                this.NextToken();

                this.HandleIdentifier(acceptEnd: false);
            }
        }

        private void HandleComma()
        {
            var value = this.prefixes.Pop();

            this.context.Values.Add(new Field(value));

            this.NextToken();
            this.HandleIdentifier(acceptEnd: false);
        }

        private void HandleEof()
        {
            if (this.depth > 0)
            {
                this.context.Error = new UnexpectedTokenError(this.currentToken);

                return;
            }

            var value = this.prefixes.Pop();

            this.context.Values.Add(new Field(value));
        }

        private void NextToken()
        {
            while (true)
            {
                var token = this.tokenizer.NextToken();

                if (token.Type != TokenType.WhiteSpace)
                {
                    this.currentToken = token;

                    return;
                }
            }
        }
    }
}