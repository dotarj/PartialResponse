// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PartialResponse.AspNetCore.Mvc;
using PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal;

namespace PartialResponse.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for the <see cref="IMvcBuilder"/> interface.
    /// </summary>
    public static class MvcPartialJsonMvcBuilderExtensions
    {
        /// <summary>
        /// Adds services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <returns>A reference to the current instance of <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddPartialJsonFormatters(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            AddPartialJsonFormatterServices(builder.Services);

            return builder;
        }

        /// <summary>
        /// Adds services to the <see cref="IServiceCollection"/> and the <paramref name="setupAction"/> to configure
        /// the <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="setupAction">An action to configure the <see cref="JsonSerializerSettings"/>.</param>
        /// <returns>A reference to the current instance of <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddPartialJsonFormatters(this IMvcBuilder builder, Action<JsonSerializerSettings> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            AddPartialJsonFormatterServices(builder.Services);

            builder.Services.Configure<MvcPartialJsonOptions>((options) => setupAction(options.SerializerSettings));

            return builder;
        }

        /// <summary>
        /// Adds the <paramref name="setupAction"/> to configure the <see cref="JsonSerializerSettings"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <param name="setupAction">An action to configure the <see cref="MvcPartialJsonOptions"/>.</param>
        /// <returns>A reference to the current instance of <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddPartialJsonOptions(this IMvcBuilder builder, Action<MvcPartialJsonOptions> setupAction)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            builder.Services.Configure<MvcPartialJsonOptions>(setupAction);

            return builder;
        }

        // Internal for testing.
        internal static void AddPartialJsonFormatterServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcPartialJsonMvcOptionsSetup>());
            services.TryAddSingleton<PartialJsonResultExecutor>();
        }
    }
}
