// Copyright (c) Arjen Post. See LICENSE and NOTICE in the project root for license information.

using System.Net.Http.Headers;

namespace PartialResponse.Net.Http.Formatting
{
    /// <summary>
    /// Constants related to media types.
    /// </summary>
    internal static class MediaTypeConstants
    {
        private static readonly MediaTypeHeaderValue DefaultApplicationJsonMediaType = new MediaTypeHeaderValue("application/json");
        private static readonly MediaTypeHeaderValue DefaultTextJsonMediaType = new MediaTypeHeaderValue("text/json");

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>application/json</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>application/json</c>.
        /// </value>
        public static MediaTypeHeaderValue ApplicationJsonMediaType
        {
            get { return DefaultApplicationJsonMediaType.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="MediaTypeHeaderValue"/> instance representing <c>text/json</c>.
        /// </summary>
        /// <value>
        /// A new <see cref="MediaTypeHeaderValue"/> instance representing <c>text/json</c>.
        /// </value>
        public static MediaTypeHeaderValue TextJsonMediaType
        {
            get { return DefaultTextJsonMediaType.Clone(); }
        }
    }
}
