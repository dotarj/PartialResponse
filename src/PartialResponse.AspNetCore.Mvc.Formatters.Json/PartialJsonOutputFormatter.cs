// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Buffers;
using System.IO;
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
        internal const string BypassPartialResponseKey = "BypassPartialResponse";

        private readonly IArrayPool<char> charPool;
        private readonly bool ignoreCase;

        // Perf: JsonSerializers are relatively expensive to create, and are thread safe. We cache
        // the serializer and invalidate it when the settings change.
        private JsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialJsonOutputFormatter"/> class.
        /// </summary>
        /// <param name="serializerSettings">
        /// The <see cref="JsonSerializerSettings"/>. Should be either the application-wide settings
        /// (<see cref="MvcPartialJsonOptions.SerializerSettings"/>) or an instance
        /// <see cref="JsonSerializerSettingsProvider.CreateSerializerSettings"/> initially returned.
        /// </param>
        /// <param name="charPool">The <see cref="ArrayPool{Char}"/>.</param>
        /// <param name="ignoreCase">A value that indicates whether partial response allows case-insensitive matching.</param>
        public PartialJsonOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, bool ignoreCase)
        {
            if (serializerSettings == null)
            {
                throw new ArgumentNullException(nameof(serializerSettings));
            }

            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }

            this.SerializerSettings = serializerSettings;
            this.charPool = new JsonArrayPool<char>(charPool);
            this.ignoreCase = ignoreCase;

            this.SupportedEncodings.Add(Encoding.UTF8);
            this.SupportedEncodings.Add(Encoding.Unicode);
            this.SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJson);
            this.SupportedMediaTypes.Add(MediaTypeHeaderValues.TextJson);
        }

        /// <summary>
        /// Gets the <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <remarks>
        /// Any modifications to the <see cref="JsonSerializerSettings"/> object after this
        /// <see cref="PartialJsonOutputFormatter"/> has been used will have no effect.
        /// </remarks>
        protected JsonSerializerSettings SerializerSettings { get; }

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

            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            Fields? fields = null;

            if (!this.ShouldBypassPartialResponse(context.HttpContext))
            {
                if (!request.TryGetFields(out fields))
                {
                    response.StatusCode = 400;

                    return;
                }
            }

            using (var writer = context.WriterFactory(response.Body, selectedEncoding))
            {
                this.WriteObject(writer, context.Object, fields);

                // Perf: call FlushAsync to call WriteAsync on the stream with any content left in the TextWriter's
                // buffers. This is better than just letting dispose handle it (which would result in a synchronous
                // write).
                await writer.FlushAsync();
            }
        }

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

            var jsonWriter = new JsonTextWriter(writer) { ArrayPool = this.charPool, CloseOutput = false, };

            return jsonWriter;
        }

        /// <summary>
        /// Called during serialization to create the <see cref="JsonSerializer"/>.
        /// </summary>
        /// <returns>The <see cref="JsonSerializer"/> used during serialization and deserialization.</returns>
        protected virtual JsonSerializer CreateJsonSerializer()
        {
            if (this.serializer == null)
            {
                this.serializer = JsonSerializer.Create(this.SerializerSettings);
            }

            return this.serializer;
        }

        private bool ShouldBypassPartialResponse(HttpContext httpContext)
        {
            if (httpContext.Items.ContainsKey(BypassPartialResponseKey))
            {
                return true;
            }

            return httpContext.Response.StatusCode != 200;
        }

        private void WriteObject(TextWriter writer, object value, Fields? fields)
        {
            using (var jsonWriter = this.CreateJsonWriter(writer))
            {
                var jsonSerializer = this.CreateJsonSerializer();

                if (fields.HasValue)
                {
                    jsonSerializer.Serialize(jsonWriter, value, path => fields.Value.Matches(path, this.ignoreCase));
                }
                else
                {
                    jsonSerializer.Serialize(jsonWriter, value);
                }
            }
        }
    }
}
