// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using PartialResponse.Core;

namespace PartialResponse.Net.Http.Formatting
{
    /// <summary>
    /// <see cref="MediaTypeFormatter"/> class to handle Json.
    /// </summary>
    public class PartialJsonMediaTypeFormatter : MediaTypeFormatter
    {
        private readonly HttpRequestMessage _request;

        private JsonSerializerSettings _jsonSerializerSettings;
        private int _maxDepth = FormattingUtilities.DefaultMaxDepth;
        private Fields? _fields;

#if !NETFX_CORE // DataContractJsonSerializer and DataContractResolver are not supported in the portable library version.
        private readonly IContractResolver _defaultContractResolver;
        private RequestHeaderMapping _requestHeaderMapping;
#endif

        internal const string BypassPartialResponse = "BypassPartialResponse";

        /// <summary>
        /// Initializes a new instance of the <see cref="PartialResponse.Net.Http.Formatting.PartialJsonMediaTypeFormatter"/>
        /// class.
        /// </summary>
        public PartialJsonMediaTypeFormatter()
        {
            // Set default supported media types
            SupportedMediaTypes.Add(MediaTypeConstants.ApplicationJsonMediaType);
            SupportedMediaTypes.Add(MediaTypeConstants.TextJsonMediaType);

            // Initialize serializer
#if !NETFX_CORE // We don't support JsonContractResolver is not supported in the portable library portable library version.
            _defaultContractResolver = new JsonContractResolver(this);
#endif
            _jsonSerializerSettings = CreateDefaultSerializerSettings();

            // Set default supported character encodings
            SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true));
            SupportedEncodings.Add(new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true));

#if !NETFX_CORE // MediaTypeMappings are not supported in portable library
            _requestHeaderMapping = new XmlHttpRequestHeaderMapping();
            MediaTypeMappings.Add(_requestHeaderMapping);
#endif
        }

        private PartialJsonMediaTypeFormatter(HttpRequestMessage request)
            : this()
        {
            _request = request;
        }

        /// <summary>
        /// Returns a specialized instance of the System.Net.Http.Formatting.MediaTypeFormatter
        /// that can format a response for the given parameters.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <param name="request">The request.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns>Returns System.Net.Http.Formatting.MediaTypeFormatter.</returns>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new PartialJsonMediaTypeFormatter(request)
            {
                IgnoreCase = IgnoreCase,
                Indent = Indent,
                MaxDepth = MaxDepth,
                SerializerSettings = SerializerSettings
            };
        }

        /// <summary>
        /// Gets the default media type for Json, namely "application/json".
        /// </summary>
        /// <remarks>
        /// The default media type does not have any <c>charset</c> parameter as 
        /// the <see cref="Encoding"/> can be configured on a per <see cref="PartialJsonMediaTypeFormatter"/> 
        /// instance basis.
        /// </remarks>
        /// <value>
        /// Because <see cref="MediaTypeHeaderValue"/> is mutable, the value
        /// returned will be a new instance every time.
        /// </value>
        public static MediaTypeHeaderValue DefaultMediaType
        {
            get { return MediaTypeConstants.ApplicationJsonMediaType; }
        }

        /// <summary>
        /// Gets or sets the <see cref="JsonSerializerSettings"/> used to configure the <see cref="JsonSerializer"/>.
        /// </summary>
        public JsonSerializerSettings SerializerSettings
        {
            get { return _jsonSerializerSettings; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _jsonSerializerSettings = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fields should be compared ignoring or honoring their case.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to indent elements when writing data. 
        /// </summary>
        public bool Indent { get; set; }

#if !NETFX_CORE // MaxDepth not supported in portable library
        /// <summary>
        /// Gets or sets the maximum depth allowed by this formatter.
        /// </summary>
        public int MaxDepth
        {
            get
            {
                return _maxDepth;
            }
            set
            {
                if (value < FormattingUtilities.DefaultMinDepth)
                {
                    throw new ArgumentOutOfRangeException("value", value, string.Format("Value must be greater than or equal to {0}.", FormattingUtilities.DefaultMinDepth));
                }

                _maxDepth = value;
            }
        }
#endif

        /// <summary>
        /// Creates a <see cref="JsonSerializerSettings"/> instance with the default settings used by the <see cref="PartialJsonMediaTypeFormatter"/>.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This could only be static half the time.")]
        public JsonSerializerSettings CreateDefaultSerializerSettings()
        {
            return new JsonSerializerSettings()
            {
#if !NETFX_CORE // no Contract resolver in portable libraries
                ContractResolver = _defaultContractResolver,
#endif
                MissingMemberHandling = MissingMemberHandling.Ignore,

                // Do not change this setting
                // Setting this to None prevents Json.NET from loading malicious, unsafe, or security-sensitive types
                TypeNameHandling = TypeNameHandling.None
            };
        }

        /// <summary>
        /// Determines whether this <see cref="PartialJsonMediaTypeFormatter"/> can read objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be read.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be read, otherwise <c>false</c>.</returns>
        public override bool CanReadType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        /// <summary>
        /// Determines whether this <see cref="PartialJsonMediaTypeFormatter"/> can write objects
        /// of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type of object that will be written.</param>
        /// <returns><c>true</c> if objects of this <paramref name="type"/> can be written, otherwise <c>false</c>.</returns>
        public override bool CanWriteType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return true;
        }

        /// <summary>
        /// Called during deserialization to read an object of the specified <paramref name="type"/>
        /// from the specified <paramref name="readStream"/>.
        /// </summary>
        /// <param name="type">The type of object to read.</param>
        /// <param name="readStream">The <see cref="Stream"/> from which to read.</param>
        /// <param name="content">The <see cref="HttpContent"/> for the content being written.</param>
        /// <param name="formatterLogger">The <see cref="IFormatterLogger"/> to log events to.</param>
        /// <returns>A <see cref="Task"/> whose result will be the object instance that has been read.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (readStream == null)
            {
                throw new ArgumentNullException("readStream");
            }

            try
            {
                return TaskHelpers.FromResult(ReadFromStream(type, readStream, content, formatterLogger));
            }
            catch (Exception e)
            {
                return TaskHelpers.FromError<object>(e);
            }
        }

        private object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            HttpContentHeaders contentHeaders = content == null ? null : content.Headers;

            // If content length is 0 then return default value for this type
            if (contentHeaders != null && contentHeaders.ContentLength == 0)
            {
                return GetDefaultValueForType(type);
            }

            // Get the character encoding for the content
            Encoding effectiveEncoding = SelectCharacterEncoding(contentHeaders);

            try
            {
                using (JsonTextReader jsonTextReader = new JsonTextReader(new StreamReader(readStream, effectiveEncoding)) { CloseInput = false, MaxDepth = _maxDepth })
                {
                    JsonSerializer jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);
                    if (formatterLogger != null)
                    {
                        // Error must always be marked as handled
                        // Failure to do so can cause the exception to be rethrown at every recursive level and overflow the stack for x64 CLR processes
                        jsonSerializer.Error += (sender, e) =>
                        {
                            Exception exception = e.ErrorContext.Error;
                            formatterLogger.LogError(e.ErrorContext.Path, exception);
                            e.ErrorContext.Handled = true;
                        };
                    }
                    return jsonSerializer.Deserialize(jsonTextReader, type);
                }
            }
            catch (Exception e)
            {
                if (formatterLogger == null)
                {
                    throw;
                }
                formatterLogger.LogError(String.Empty, e);
                return GetDefaultValueForType(type);
            }
        }

        /// <summary>
        /// Called during serialization to write an object of the specified <paramref name="type"/>
        /// to the specified <paramref name="writeStream"/>.
        /// </summary>
        /// <param name="type">The type of object to write.</param>
        /// <param name="value">The object to write.</param>
        /// <param name="writeStream">The <see cref="Stream"/> to which to write.</param>
        /// <param name="content">The <see cref="HttpContent"/> for the content being written.</param>
        /// <param name="transportContext">The <see cref="TransportContext"/>.</param>
        /// <returns>A <see cref="Task"/> that will write the value to the stream.</returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The caught exception type is reflected into a faulted task.")]
        public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (writeStream == null)
            {
                throw new ArgumentNullException("writeStream");
            }

            try
            {
                WriteToStream(type, value, writeStream, content);
                return TaskHelpers.Completed();
            }
            catch (Exception e)
            {
                return TaskHelpers.FromError(e);
            }
        }

        private Fields? GetPartialResponseFields(HttpRequestMessage request)
        {
            var queryOption = HttpUtility.ParseQueryString(request.RequestUri.Query)["fields"];

            if (queryOption != null)
            {
                Fields fields;

                if (!Fields.TryParse(queryOption, out fields))
                {
                    // Not much choice but to throw a HttpResponseException inside a MediaTypeFormatter (no access 
                    // to the HttpResponseMessage).
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                return fields;
            }

            return null;
        }

        /// <summary>
        /// Returns a value that indicates whether the field should be serialized.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="tokenType">The type.</param>
        /// <returns>True if the value should be serialized, otherwise false.</returns>
        protected virtual bool ShouldSerialize(string field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }

            return _fields.HasValue ? _fields.Value.Matches(field, IgnoreCase) : true;
        }

        /// <summary>
        /// Returns a value that indicates whether partial response should be bypassed.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>True if the partial response should be bypassed, otherwise false.</returns>
        protected virtual bool ShouldBypassPartialResponse(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (request.GetBypassPartialResponse())
            {
                return true;
            }

            var httpContext =  request.GetHttpContext();

            if (httpContext != null)
            {
                return httpContext.Response.StatusCode != 200;
            }

            return false;
        }

        private void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            Encoding effectiveEncoding = SelectCharacterEncoding(content == null ? null : content.Headers);

            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(writeStream, effectiveEncoding)) { CloseOutput = false })
            {
                if (Indent)
                {
                    jsonTextWriter.Formatting = Newtonsoft.Json.Formatting.Indented;
                }

                JsonSerializer jsonSerializer = JsonSerializer.Create(_jsonSerializerSettings);

                if (!ShouldBypassPartialResponse(_request))
                {
                    _fields = GetPartialResponseFields(_request);
                }

                if (_fields == null)
                {
                    jsonSerializer.Serialize(jsonTextWriter, value);
                }
                else
                {
                    jsonSerializer.Serialize(jsonTextWriter, value, ShouldSerialize);
                }

                jsonTextWriter.Flush();
            }
        }
    }
}