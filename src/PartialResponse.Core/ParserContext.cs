using System;
using System.Collections.Generic;
using System.IO;

namespace PartialResponse.Core
{
    public class ParserContext
    {
        public ParserContext(TextReader source)
        {
            if (source == null)
            {
                throw new ArgumentNullException();
            }

            this.Source = source;
            this.Values = new List<Field>();
        }

        public string Error { get; internal set; }

        public TextReader Source { get; private set; }

        public List<Field> Values { get; private set; }
    }
}