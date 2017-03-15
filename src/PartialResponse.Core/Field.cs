// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;

namespace PartialResponse.Core
{
    /// <summary>
    /// Represents a field.
    /// </summary>
    /// <remarks>This type supports the <see cref="Fields"/> infrastructure and is not intended to be used directly
    /// from your code.</remarks>
    public struct Field
    {
        private const string Wildcard = "*";

        /// <summary>
        /// Initializes a new instance of the <see cref="Field"/> structure.
        /// </summary>
        /// <param name="value">The value of the field.</param>
        public Field(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Parts = value.Split('/');
        }

        /// <summary>
        /// Gets the value parts of the field.
        /// </summary>
        /// <returns>The value parts of the field.</returns>
        public string[] Parts { get; }

        /// <summary>
        /// Indicates whether the field matches the specified value.
        /// </summary>
        /// <param name="parts">The parts to match, which is an array whose elements contain the substrings from the value that are delimited by '/'.</param>
        /// <returns>true if the field matches the specified value; otherwise, false.</returns>
        public bool Matches(string[] parts)
        {
            return this.Matches(parts, false);
        }

        /// <summary>
        /// Indicates whether the field matches the specified value.
        /// </summary>
        /// <param name="parts">The parts to match, which is an array whose elements contain the substrings from the value that are delimited by '/'.</param>
        /// <param name="ignoreCase">A value which indicates whether matching should be case-insensitive.</param>
        /// <returns>true if the field matches the specified value; otherwise, false.</returns>
        public bool Matches(string[] parts, bool ignoreCase)
        {
            if (parts == null)
            {
                throw new ArgumentNullException(nameof(parts));
            }

            var stringComparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            for (var index = 0; index < parts.Length && index < this.Parts.Length; index++)
            {
                if (!string.Equals(parts[index], this.Parts[index], stringComparison) && this.Parts[index] != Wildcard)
                {
                    return false;
                }

                // If the last part of the given value is reached and the field has more parts than the given value,
                // there is no match, eg. the field value is foo/bar and the given value is foo. If the field parts and
                // the given value parts have equal lengths, there is a match, eg. the field value is foo/bar and the
                // given value is foo/bar.
                if (index == parts.Length - 1)
                {
                    return parts.Length <= this.Parts.Length;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return string.Join("/", this.Parts);
        }
    }
}