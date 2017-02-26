// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace PartialResponse.Core
{
    /// <summary>
    /// Represents a collection of fields.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public struct Fields
    {
        /// <summary>
        /// An empty instance of the <see cref="Fields"/> structure.
        /// </summary>
        public static Fields Empty = new Fields(new List<Field>());

        private Fields(List<Field> values)
        {
            this.Values = values.AsReadOnly();
        }

        /// <summary>
        /// Gets a collection containing the fields.
        /// </summary>
        /// <returns>A collection containing the fields</returns>
        public ReadOnlyCollection<Field> Values { get; }

        /// <summary>
        /// Indicates whether a field matches the specified value.
        /// </summary>
        /// <param name="value">The value to match.</param>
        /// <returns>true if a field matches the specified value; otherwise, false.</returns>
        public bool Matches(string value)
        {
            return this.Matches(value, false);
        }

        /// <summary>
        /// Indicates whether a field matches the specified value.
        /// </summary>
        /// <param name="value">The value to match.</param>
        /// <param name="ignoreCase">A value which indicates whether matching should be case-insensitive.</param>
        /// <returns>true if a field matches the specified value; otherwise, false.</returns>
        public bool Matches(string value, bool ignoreCase)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return this.Values.Any(v => v.Matches(value, ignoreCase));
        }

        /// <summary>
        /// Converts to value to a <see cref="Fields"/> object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">When this method returns, contains the <see cref="Fields"/> equivalent of the value,
        /// if the conversion succeeded, or null if the conversion failed.</param>
        /// <returns>true if value was converted successfully; otherwise, false.</returns>
        public static bool TryParse(string value, out Fields result)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            using (var reader = new StringReader(value))
            {
                var context = new ParserContext(reader);
                var parser = new Parser(context);

                parser.Parse();

                if (context.Error != null)
                {
                    result = Fields.Empty;

                    return false;
                }

                result = new Fields(context.Values);

                return true;
            }
        }
    }
}
