using System;
using System.Collections.Generic;
using System.IO;

namespace PartialResponse.Core
{
    /// <summary>
    /// Contains information used by a <see cref="Parser"/>.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public class ParserContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserContext"/> class.
        /// </summary>
        /// <param name="source">A <see cref="TextReader"/> representing the input string.</param>
        public ParserContext(TextReader source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            this.Source = source;
            this.Values = new List<Field>();
        }

        /// <summary>
        /// Gets the error that occured while parsing.
        /// </summary>
        /// <returns>The error if an error occured while parsing; otherwise, null.</returns>
        public UnexpectedTokenError Error { get; internal set; }

        /// <summary>
        /// Gets the <see cref="TextReader"/> representing the input string.
        /// </summary>
        /// <returns>The <see cref="TextReader"/> representing the input string.</returns>
        public TextReader Source { get; private set; }

        /// <summary>
        /// Gets the values that are extracted while parsing.
        /// </summary>
        /// <returns>The values that are extracted while parsing.</returns>
        public List<Field> Values { get; private set; }
    }
}