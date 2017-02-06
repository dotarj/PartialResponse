// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal;
using PartialResponse.Core;

namespace PartialResponse.AspNetCore.Mvc.Formatters
{
    /// <summary>
    /// A <see cref="TextOutputFormatter"/> for JSON content.
    /// </summary>
    public class PartialJsonOutputFormatter : TextOutputFormatter
    {
        private readonly IArrayPool<char> _charPool;

        // Perf: JsonSerializers are relatively expensive to create, and are thread safe. We cache
        // the serializer and invalidate it when the settings change.
        private JsonSerializer _serializer;

        /// <summary>
        /// Initializes a new <see cref="PartialJsonOutputFormatter"/> instance.
        /// </summary>
        /// <param name="serializerSettings">
        /// The <see cref="JsonSerializerSettings"/>. Should be either the application-wide settings
        /// (<see cref="MvcJsonOptions.SerializerSettings"/>) or an instance
        /// <see cref="JsonSerializerSettingsProvider.CreateSerializerSettings"/> initially returned.
        /// </param>
        /// <param name="charPool">The <see cref="ArrayPool{Char}"/>.</param>
        public PartialJsonOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool)
        {
            if (serializerSettings == null)
            {
                throw new ArgumentNullException(nameof(serializerSettings));
            }

            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }

            SerializerSettings = serializerSettings;
            _charPool = new JsonArrayPool<char>(charPool);

            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextJson);
        }

        /// <summary>
        /// Gets the <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <remarks>
        /// Any modifications to the <see cref="JsonSerializerSettings"/> object after this
        /// <see cref="PartialJsonOutputFormatter"/> has been used will have no effect.
        /// </remarks>
        protected JsonSerializerSettings SerializerSettings { get; }

        /// <summary>
        /// Called during serialization to create the <see cref="JsonWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="TextWriter"/> used to write.</param>
        /// <returns>The <see cref="JsonWriter"/> used during serialization.</returns>
        protected virtual JsonWriter CreateJsonWriter(TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }

            var jsonWriter = new JsonTextWriter(writer)
            {
                ArrayPool = _charPool,
                CloseOutput = false,
            };

            return jsonWriter;
        }

        /// <summary>
        /// Called during serialization to create the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <returns>The <see cref="JsonSerializer"/> used during serialization and deserialization.</returns>
        protected virtual JsonSerializer CreateJsonSerializer()
        {
            if (_serializer == null)
            {
                _serializer = JsonSerializer.Create(SerializerSettings);
            }

            return _serializer;
        }

        /// <summary>
        /// Gets a list of partial response fields for the current request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>A <see cref="System.Collections.ObjectModel.Collection{string}"/> that contains the specified fields for the
        /// current request, or null if all fields should by serialized.</returns>
        protected virtual Fields GetPartialResponseFields(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            var queryOption = request.Query["fields"].FirstOrDefault();

            if (queryOption != null)
            {
                Fields fields;

                if (!Fields.TryParse(queryOption, out fields))
                {
                    // TODO: No more HttpResponseException in ASP.NET Core.
                    throw new Exception();
                }

                return fields;
            }

            return null;
        }

        /// <summary>
        /// Returns a value that indicates whether partial response should be bypassed.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>True if the partial response should be bypassed, otherwise false.</returns>
        protected virtual bool ShouldBypassPartialResponse(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.GetBypassPartialResponse())
            {
                return true;
            }

            if (request.HttpContext != null)
            {
                return request.HttpContext.Response.StatusCode != 200;
            }

            return false;
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (selectedEncoding == null)
            {
                throw new ArgumentNullException(nameof(selectedEncoding));
            }

            var response = context.HttpContext.Response;
            using (var writer = context.WriterFactory(response.Body, selectedEncoding))
            {
                using (var jsonWriter = CreateJsonWriter(writer))
                {
                    var jsonSerializer = CreateJsonSerializer();

                    Fields fields = null;

                    if (!ShouldBypassPartialResponse(context.HttpContext.Request))
                    {
                        fields = GetPartialResponseFields(context.HttpContext.Request);
                    }

                    if (fields == null)
                    {
                        jsonSerializer.Serialize(jsonWriter, context.Object);
                    }
                    else
                    {
                        PartialJsonUtilities.RemovePropertiesAndArrayElements(context.Object, jsonWriter, jsonSerializer, value => fields.Matches(value));
                    }
                }

                // Perf: call FlushAsync to call WriteAsync on the stream with any content left in the TextWriter's
                // buffers. This is better than just letting dispose handle it (which would result in a synchronous
                // write).
                await writer.FlushAsync();
            }
        }
    }
}
