// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;

namespace PartialResponse.Core
{
    public class Field
    {
        private const string Wildcard = "*";

        public Field(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Parts = value.Split('/');
        }

        public string[] Parts { get; private set; }

        public bool Matches(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var parts = value.Split('/');

            for (var index = 0; index < parts.Length && index < this.Parts.Length; index++)
            {
                if (parts[index] != this.Parts[index] && this.Parts[index] != Wildcard)
                {
                    return false;
                }

                // If the last part of the given value is reached and the field has more parts than the given value,
                // there is no match, eg. the field value is foo/bar and the given value is foo. If the field parts and
                // the given value parts have equal lengths, there is a match, eg. the field value is foo/bar and the
                // given value is foo/bar.
                if (index == parts.Length - 1)
                {
                    return parts.Length == this.Parts.Length;
                }
            }

            return true;
        }

        public override string ToString()
        {
            return string.Join("/", this.Parts);
        }
    }
}