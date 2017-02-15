// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PartialResponse.Core
{
    /// <summary>
    /// A tokenizer for fields parameter tokenization.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class Tokenizer
    {
        private readonly TextReader reader;
        private readonly StringBuilder buffer = new StringBuilder();
        private readonly Dictionary<char, TokenType> tokens = new Dictionary<char, TokenType>()
        {
            ['/'] = TokenType.ForwardSlash,
            ['('] = TokenType.LeftParenthesis,
            [')'] = TokenType.RightParenthesis,
            [','] = TokenType.Comma
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokenizer"/> class.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> representing the input string.</param>
        public Tokenizer(TextReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            this.reader = reader;
        }

        /// <summary>
        /// Returns the next token from the input string.
        /// </summary>
        /// <returns>The next token from the input string, or null if the end has been reached.</returns>
        public Token NextToken()
        {
            if (this.IsEndReached())
            {
                return null;
            }

            Token token;

            TokenType tokenType;

            if (tokens.TryGetValue(this.GetCurrentCharacter(), out tokenType))
            {
                this.TakeCharacter();

                token = new Token(this.buffer.ToString(), tokenType);
            }
            else if (this.IsWhiteSpace(this.GetCurrentCharacter()))
            {
                this.TakeCharactersWhile(character => this.IsWhiteSpace(character));

                token = new Token(this.buffer.ToString(), TokenType.WhiteSpace);
            }
            else
            {
                this.TakeCharactersWhile(character => !this.tokens.ContainsKey(character) && !this.IsWhiteSpace(character));

                token = new Token(this.buffer.ToString(), TokenType.Identifier);
            }

            this.buffer.Clear();

            return token;
        }

        private void TakeCharactersWhile(Func<char, bool> predicate)
        {
            while (!this.IsEndReached() && predicate(this.GetCurrentCharacter()))
            {
                this.TakeCharacter();
            }
        }

        private void TakeCharacter()
        {
            if (this.IsEndReached())
            {
                return;
            }

            this.buffer.Append(this.GetCurrentCharacter());

            this.reader.Read();
        }

        private bool IsWhiteSpace(char character)
        {
            return
                character == ' ' ||
                character == '\t' ||
                character == '\r' ||
                character == '\n';
        }

        private char GetCurrentCharacter()
        {
            var value = this.reader.Peek();

            return value == -1 ? '\0' : (char)value;
        }

        private bool IsEndReached()
        {
            return this.reader.Peek() == -1;
        }
    }
}