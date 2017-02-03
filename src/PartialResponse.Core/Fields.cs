// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace PartialResponse.Core
{
    public class Fields
    {
        private const string ValidationPattern = @"^\s*(\*|([^\/&^*&^,&^\s]+(/[^\/&^*&^,&^\s]+)*(/\*)?))(\s*,\s*(\*|([^\/&^*&^,&^\s]+(/[^\/&^*&^,&^\s]+)*(/\*)?)))*\s*$";

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

            var values = new List<Field>();

            if (value.Trim() != "")
            {
                if (!GetFields(null, value, values))
                {
                    result = null;

                    return false;
                }
            }

            result = new Fields(values);

            return true;
        }

        private static bool Validate(string fields)
        {
            if (!Regex.IsMatch(fields, ValidationPattern))
            {
                return false;
            }

            return true;
        }

        private static bool GetFields(string basePath, string fields, List<Field> result)
        {
            if (!Validate(fields))
            {
                return false;
            }

            var parenthesisCount = 0;
            var firstParenthesis = -1;
            var pathStart = 0;
            var parenthesisClosed = false;

            for (var i = 0; i < fields.Length; i++)
            {
                var character = fields[i];

                if (parenthesisClosed)
                {
                    if (character != ',' && character != ')' && character != ' ' && character != '\t')
                    {
                        return false;
                    }
                }

                if (character == '(')
                {
                    if (parenthesisCount == 0)
                    {
                        firstParenthesis = i;
                    }

                    parenthesisCount++;
                }

                if (character == ')')
                {
                    parenthesisCount--;

                    if (parenthesisCount < 0)
                    {
                        return false;
                    }

                    parenthesisClosed = true;
                }

                if (character == ',' && parenthesisCount == 0)
                {
                    if (firstParenthesis > -1)
                    {
                        var newBasePath = PathUtilities.CombinePath(basePath, fields.Substring(pathStart, firstParenthesis - pathStart));

                        if (!GetFields(newBasePath, fields.Substring(firstParenthesis + 1, i - firstParenthesis - 2), result))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        result.Add(new Field(PathUtilities.CombinePath(basePath, fields.Substring(pathStart, i - pathStart).Trim())));
                    }

                    firstParenthesis = -1;
                    pathStart = i + 1;
                    parenthesisClosed = false;
                }

                if (i == fields.Length - 1)
                {
                    if (parenthesisCount != 0)
                    {
                        return false;
                    }

                    if (firstParenthesis > -1)
                    {
                        var newBasePath = PathUtilities.CombinePath(basePath, fields.Substring(pathStart, firstParenthesis - pathStart));

                        if (!GetFields(newBasePath, fields.Substring(firstParenthesis + 1, i - firstParenthesis - 1), result))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        result.Add(new Field(PathUtilities.CombinePath(basePath, fields.Substring(pathStart, i - pathStart + 1).Trim())));
                    }
                }
            }

            return true;
        }
    }
}
