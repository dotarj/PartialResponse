using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PartialResponse.Net.Http.Formatting
{
    internal static class PartialJsonMediaTypeFormatterUtilities
    {
        internal static string GetRegexPatternForField(string value)
        {
            var patternBuilder = new StringBuilder();
            var parts = value.Split('/');

            patternBuilder.Append("^(");

            for (int i = 0; i < parts.Length - 1; i++)
            {
                patternBuilder.Append(string.Format(@"({0}(/", parts[i]));
            }

            patternBuilder.Append(string.Format(@"(({0}(/.+)?)|\*)", parts[parts.Length - 1]));

            for (int i = 0; i < parts.Length - 2; i++)
            {
                patternBuilder.Append(@")?|\*)");
            }

            if (parts.Length > 1)
            {
                patternBuilder.Append(@")?)|\*");
            }

            patternBuilder.Append(")$");

            return patternBuilder.ToString();
        }

        internal static void RemovePropertiesAndArrayElements(object value, JsonTextWriter jsonTextWriter, JsonSerializer jsonSerializer, Func<string, JsonTokenType, bool> shouldSerialize)
        {
            if (value == null)
            {
                jsonSerializer.Serialize(jsonTextWriter, value);
            }
            else
            {
                var token = JToken.FromObject(value, jsonSerializer);

                var array = token as JArray;

                if (array != null)
                {
                    RemoveArrayElements(array, null, shouldSerialize, new Dictionary<string, bool>());

                    array.WriteTo(jsonTextWriter);
                }
                else
                {
                    var @object = token as JObject;

                    if (@object != null)
                    {
                        RemoveObjectProperties(@object, null, shouldSerialize, new Dictionary<string, bool>());

                        @object.WriteTo(jsonTextWriter);
                    }
                    else
                    {
                        token.WriteTo(jsonTextWriter);
                    }
                }
            }
        }

        private static void RemoveArrayElements(JArray array, string currentPath, Func<string, JsonTokenType, bool> shouldSerialize, Dictionary<string, bool> cache)
        {
            array.OfType<JObject>()
                .ToList()
                .ForEach(childObject => RemoveObjectProperties(childObject, currentPath, shouldSerialize, cache));

            RemoveArrayIfEmpty(array);
        }

        private static void RemoveArrayIfEmpty(JArray array)
        {
            if (array.Count == 0)
            {
                if (array.Parent is JProperty && array.Parent.Parent != null)
                {
                    array.Parent.Remove();
                }
                else if (array.Parent is JArray)
                {
                    array.Remove();
                }
            }
        }

        private static void RemoveObjectProperties(JObject @object, string currentPath, Func<string, JsonTokenType, bool> shouldSerialize, Dictionary<string, bool> cache)
        {
            @object.Properties()
                .Where(property =>
                {
                    var path = PathUtilities.CombinePath(currentPath, property.Name);

                    if (cache.ContainsKey(path))
                    {
                        return cache[path];
                    }

                    var result = !shouldSerialize(path, GetTokenType(property.Value));

                    cache.Add(path, result);

                    return result;
                })
                .ToList()
                .ForEach(property => property.Remove());

            @object.Properties()
                .Where(property => property.Value is JObject)
                .ToList()
                .ForEach(property =>
                {
                    var path = PathUtilities.CombinePath(currentPath, property.Name);

                    RemoveObjectProperties((JObject)property.Value, path, shouldSerialize, cache);
                });

            @object.Properties()
                .Where(property => property.Value is JArray)
                .ToList()
                .ForEach(property =>
                {
                    var path = PathUtilities.CombinePath(currentPath, property.Name);

                    RemoveArrayElements((JArray)property.Value, path, shouldSerialize, cache);
                });

            RemoveObjectIfEmpty(@object);
        }

        private static void RemoveObjectIfEmpty(JObject @object)
        {
            if (!@object.Properties().Any())
            {
                if (@object.Parent is JProperty && @object.Parent.Parent != null)
                {
                    @object.Parent.Remove();
                }
                else if (@object.Parent is JArray)
                {
                    @object.Remove();
                }
            }
        }

        private static JsonTokenType GetTokenType(JToken token)
        {
            if (token is JArray)
            {
                return JsonTokenType.Array;
            }
            else if (token is JObject)
            {
                return JsonTokenType.Object;
            }
            else
            {
                return JsonTokenType.Value;
            }
        }
    }
}
