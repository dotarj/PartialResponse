// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Provides extension methods for the <see cref="ControllerBase"/> class.
    /// </summary>
    public static class ControllerExtensions
    {
        /// <summary>
        /// Creates a <see cref="PartialJsonResult"/> object that serializes the specified <paramref name="data"/> object
        /// to JSON.
        /// </summary>
        /// <param name="controller">The controller instance.</param>
        /// <param name="data">The object to serialize.</param>
        /// <returns>The created <see cref="PartialJsonResult"/> that serializes the specified <paramref name="data"/>
        /// to JSON format for the response.</returns>
        public static PartialJsonResult PartialJson(this ControllerBase controller, object data)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            return new PartialJsonResult(data);
        }

        /// <summary>
        /// Creates a <see cref="PartialJsonResult"/> object that serializes the specified <paramref name="data"/> object
        /// to JSON.
        /// </summary>
        /// <param name="controller">The controller instance.</param>
        /// <param name="data">The object to serialize.</param>
        /// <param name="serializerSettings">The <see cref="JsonSerializerSettings"/> to be used by
        /// the formatter.</param>
        /// <returns>The created <see cref="PartialJsonResult"/> that serializes the specified <paramref name="data"/>
        /// as JSON format for the response.</returns>
        /// <remarks>Callers should cache an instance of <see cref="JsonSerializerSettings"/> to avoid
        /// recreating cached data with each call.</remarks>
        public static PartialJsonResult PartialJson(this ControllerBase controller, object data, JsonSerializerSettings serializerSettings)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            if (serializerSettings == null)
            {
                throw new ArgumentNullException(nameof(serializerSettings));
            }

            return new PartialJsonResult(data, serializerSettings);
        }
    }
}