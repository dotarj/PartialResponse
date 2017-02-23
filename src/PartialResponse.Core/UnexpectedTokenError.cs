using System;

namespace PartialResponse.Core
{
    /// <summary>
    /// Contains information about an unexpected token.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class UnexpectedTokenError
    {
        private readonly Token token;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedTokenError"/> class.
        /// </summary>
        /// <param name="token">The unexpected token.</param>
        public UnexpectedTokenError(Token token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            this.token = token;
        }

        /// <summary>
        /// Gets the value of the unexpected token.
        /// </summary>
        /// <returns>The value of the unexpected token.</returns>
        public string Value { get { return this.token.Value; } }

        /// <summary>
        /// Gets the <see cref="TokenType"/> of the unexpected token.
        /// </summary>
        /// <returns>The <see cref="TokenType"/> of the unexpected token.</returns>
        public TokenType Type { get { return this.token.Type; } }

        /// <summary>
        /// Gets the start position of the unexpected token.
        /// </summary>
        /// <returns>The start position of the unexpected token.</returns>
        public int Position { get { return this.token.Position; } }
    }
}