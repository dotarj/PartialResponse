// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace PartialResponse.Core
{
    public static class Fields
    {
        private const string ValidationPattern = @"^\s*(\*|([^\/&^*&^,&^\s]+(/[^\/&^*&^,&^\s]+)*(/\*)?))(\s*,\s*(\*|([^\/&^*&^,&^\s]+(/[^\/&^*&^,&^\s]+)*(/\*)?)))*\s*$";

        public static bool TryParse(string value, out Collection<string> result)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var temp = new Collection<string>();

            if (value.Trim() != "")
            {
                if (!GetFields(null, value, temp))
                {
                    result = null;

                    return false;
                }
            }

            result = temp;

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

        private static bool GetFields(string basePath, string fields, Collection<string> result)
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
                        result.Add(PathUtilities.CombinePath(basePath, fields.Substring(pathStart, i - pathStart).Trim()));
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
                        result.Add(PathUtilities.CombinePath(basePath, fields.Substring(pathStart, i - pathStart + 1).Trim()));
                    }
                }
            }

            return true;
        }
    }
}
