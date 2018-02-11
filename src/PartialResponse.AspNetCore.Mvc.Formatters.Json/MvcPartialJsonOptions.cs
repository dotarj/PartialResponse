// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Newtonsoft.Json;
using PartialResponse.AspNetCore.Mvc.Formatters;

namespace PartialResponse.AspNetCore.Mvc
{
    /// <summary>
    /// Provides programmatic configuration for JSON in the MVC framework.
    /// </summary>
    public class MvcPartialJsonOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether partial response allows case-insensitive matching.
        /// </summary>
        public bool IgnoreCase { get; set; }

        /// <summary>
        /// Gets the <see cref="JsonSerializerSettings"/> that are used by this application.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get; } = JsonSerializerSettingsProvider.CreateSerializerSettings();
    }
}