// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using PartialResponse.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace PartialResponse.AspNetCore.Mvc
{
    /// <summary>
    /// Provides programmatic configuration for JSON in the MVC framework.
    /// </summary>
    public class MvcPartialJsonOptions
    {
        /// <summary>
        /// Gets the <see cref="JsonSerializerSettings"/> that are used by this application.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; } =
            JsonSerializerSettingsProvider.CreateSerializerSettings();
    }
}