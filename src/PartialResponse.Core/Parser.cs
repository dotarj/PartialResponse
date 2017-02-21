using System;
using System.Collections.Generic;

namespace PartialResponse.Core
{
    public class Parser
    {
        private readonly Stack<string> prefixes = new Stack<string>();
        private readonly Dictionary<TokenType, Action> handlers;
        private readonly ParserContext context;
        private readonly Tokenizer tokenizer;

        private Token currentToken;
        private int depth;

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
            };
        }

        public void Parse()
        {
            // TODO: Reset stuff.

            this.NextToken();

            this.HandleIdentifier(acceptEnd: true);
        }

        private void HandleIdentifier(bool acceptEnd)
        {
            if (this.IsEndReached())
            {
                if (!acceptEnd)
                {
                    this.context.Error = "";
                }

                if (this.depth > 0)
                {
                    this.context.Error = "";
                }

                return;
            }

            if (currentToken.Type != TokenType.Identifier)
            {
                this.context.Error = "";
            }

            var prefix = this.prefixes.Count > 0 ? $"{this.prefixes.Pop()}/{this.currentToken.Value}" : this.currentToken.Value;

            this.prefixes.Push(prefix);

            this.NextToken();

            if (this.IsEndReached())
            {
                if (this.depth > 0)
                {
                    this.context.Error = "";
                }
                else
                {
                    var value = this.prefixes.Pop();

                    this.context.Values.Add(new Field(value));
                }

                return;
            }

            Action handler;

            if (!this.handlers.TryGetValue(this.currentToken.Type, out handler))
            {
                this.context.Error = "";
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
                    this.context.Error = "";

                    return;
                }

                this.NextToken();
            }
            while (!this.IsEndReached() && this.currentToken.Type == TokenType.RightParenthesis);

            if (!this.IsEndReached())
            {
                if (this.currentToken.Type != TokenType.Comma)
                {
                    this.context.Error = "";

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

        private void NextToken()
        {
            while (true)
            {
                if (this.tokenizer.IsEndReached())
                {
                    this.currentToken = null;

                    return;
                }

                var token = this.tokenizer.NextToken();

                if (token.Type != TokenType.WhiteSpace)
                {
                    this.currentToken = token;

                    return;
                }
            }
        }

        private bool IsEndReached()
        {
            return this.currentToken == null;
        }
    }
}