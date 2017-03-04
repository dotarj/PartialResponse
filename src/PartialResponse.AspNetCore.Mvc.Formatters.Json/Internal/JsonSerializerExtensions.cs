// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal
{
    public static class JsonSerializerExtensions
    {
        public static void Serialize(this JsonSerializer jsonSerializer, JsonWriter jsonWriter, object value, Func<string, bool> shouldSerialize)
        {
            if (value == null)
            {
                jsonSerializer.Serialize(jsonWriter, value);
            }
            else
            {
                var context = new SerializerContext(shouldSerialize);
                var token = JToken.FromObject(value, jsonSerializer);

                var array = token as JArray;

                if (array != null)
                {
                    RemoveArrayElements(array, null, context);

                    array.WriteTo(jsonWriter);
                }
                else
                {
                    var @object = token as JObject;

                    if (@object != null)
                    {
                        RemoveObjectProperties(@object, null, context);

                        @object.WriteTo(jsonWriter);
                    }
                    else
                    {
                        token.WriteTo(jsonWriter);
                    }
                }
            }
        }

        private static void RemoveArrayElements(JArray array, string currentPath, SerializerContext context)
        {
            array.OfType<JObject>()
                .ToList()
                .ForEach(childObject => RemoveObjectProperties(childObject, currentPath, context));

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

        private static void RemoveObjectProperties(JObject @object, string currentPath, SerializerContext context)
        {
            @object.Properties()
                .Where(property =>
                {
                    var path = CombinePath(currentPath, property.Name);

                    return context.ShouldSerialize(path);
                })
                .ToList()
                .ForEach(property => property.Remove());

            @object.Properties()
                .Where(property => property.Value is JObject)
                .ToList()
                .ForEach(property =>
                {
                    var path = CombinePath(currentPath, property.Name);

                    RemoveObjectProperties((JObject)property.Value, path, context);
                });

            @object.Properties()
                .Where(property => property.Value is JArray)
                .ToList()
                .ForEach(property =>
                {
                    var path = CombinePath(currentPath, property.Name);

                    RemoveArrayElements((JArray)property.Value, path, context);
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

        private static string CombinePath(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
            {
                return name;
            }

            return string.Format("{0}/{1}", path, name);
        }
    }
}
