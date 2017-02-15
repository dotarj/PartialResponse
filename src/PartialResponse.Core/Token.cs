// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;

namespace PartialResponse.Core
{
    /// <summary>
    /// Represents a token.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class Token
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="value">The value of the token.</param>
        /// <param name="type">The <see cref="TokenType"/> of the token.</param>
        public Token(string value, TokenType type)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
            this.Type = type; 
        }

        /// <summary>
        /// Gets the value of the token.
        /// </summary>
        /// <returns>The value of the token.</returns>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the <see cref="TokenType"/> of the token.
        /// </summary>
        /// <returns>The <see cref="TokenType"/> of the token.</returns>
        public TokenType Type { get; private set; }
    }
}