// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal
{
    /// <summary>
    /// Sets up JSON formatter options for <see cref="MvcOptions"/>.
    /// </summary>
    public class MvcPartialJsonMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly MvcPartialJsonOptions partialJsonOptions;
        private readonly ArrayPool<char> charPool;
        private readonly ObjectPoolProvider objectPoolProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MvcPartialJsonMvcOptionsSetup"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="partialJsonOptions">The options.</param>
        /// <param name="charPool">The character array pool.</param>
        /// <param name="objectPoolProvider">The object pool provider.</param>
        public MvcPartialJsonMvcOptionsSetup(ILoggerFactory loggerFactory, IOptions<MvcPartialJsonOptions> partialJsonOptions, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (partialJsonOptions == null)
            {
                throw new ArgumentNullException(nameof(partialJsonOptions));
            }

            if (charPool == null)
            {
                throw new ArgumentNullException(nameof(charPool));
            }

            if (objectPoolProvider == null)
            {
                throw new ArgumentNullException(nameof(objectPoolProvider));
            }

            this.loggerFactory = loggerFactory;
            this.partialJsonOptions = partialJsonOptions.Value;
            this.charPool = charPool;
            this.objectPoolProvider = objectPoolProvider;
        }

        /// <summary>
        /// Configures the <see cref="MvcOptions"/> by adding the <see cref="PartialJsonOutputFormatter"/>.
        /// </summary>
        /// <param name="options">The MVC options.</param>
        public void Configure(MvcOptions options)
        {
            options.OutputFormatters.Add(new PartialJsonOutputFormatter(this.partialJsonOptions.SerializerSettings, this.charPool, this.partialJsonOptions.IgnoreCase));
            options.FormatterMappings.SetMediaTypeMappingForFormat("json", MediaTypeHeaderValue.Parse("application/json"));
            options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider(typeof(JToken)));
        }
    }
}
