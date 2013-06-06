// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using PartialResponse.Net.Http.Formatting;
using System.Web;

namespace System.Net.Http
{
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Sets a value indicating whether partial response should be bypassed.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="request">The HTTP request.</param>
        public static void SetBypassPartialResponse(this HttpRequestMessage request, bool value)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            request.Properties.Add(PartialJsonMediaTypeFormatter.BypassPartialResponse, value);
        }

        /// <summary>
        /// Gets a value indicating whether partial response should be bypassed.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <returns>True if partial response should be bypassed, otherwise false.</returns>
        public static bool GetBypassPartialResponse(this HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            object value;

            if (request.Properties.TryGetValue(PartialJsonMediaTypeFormatter.BypassPartialResponse, out value))
            {
                return (bool)value;
            }

            return false;
        }

        internal static HttpContextBase GetHttpContext(this HttpRequestMessage request)
        {
            object value;

            if (request.Properties.TryGetValue("MS_HttpContext", out value))
            {
                return (HttpContextBase)value;
            }

            return null;
        }
    }
}
