// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PartialResponse.Core
{
    public class Fields
    {
        private Fields(List<Field> values)
        {
            this.Values = values.AsReadOnly();
        }

        public ReadOnlyCollection<Field> Values { get; private set; }

        public bool Matches(string value)
        {
            return this.Matches(value, false);
        }

        public bool Matches(string value, bool ignoreCase)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return this.Values.Any(v => v.Matches(value, ignoreCase));
        }

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
                    result = null;

                    return false;
                }

                result = new Fields(context.Values);

                return true;
            }
        }
    }
}
