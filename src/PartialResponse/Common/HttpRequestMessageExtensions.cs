// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System.Web;
using PartialResponse;
using PartialResponse.Net.Http.Formatting;

namespace System.Net.Http
{
    /// <summary>
    /// Provides a method for bypassing partial response.
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Sets a value indicating whether partial response should be bypassed.
        /// </summary>
        /// <param name="request">The HTTP request.</param>
        /// <param name="value">The value.</param>
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

            if (request.Properties.TryGetValue(PartialJsonMediaTypeFormatter.BypassPartialResponse, out object value))
            {
                return (bool)value;
            }

            return false;
        }

        internal static int? GetResponseStatusCode(this HttpRequestMessage request)
        {
            int? statusCode = null;

            if (request.Properties.TryGetValue(PartialJsonActionFilter.HttpResponseMessageKey, out var value))
            {
                var httpResponseMessage = (HttpResponseMessage)value;

                statusCode = (int)httpResponseMessage.StatusCode;
            }
            else if (request.Properties.TryGetValue("MS_HttpContext", out value))
            {
                var httpContext = (HttpContextBase)value;

                statusCode = httpContext.Response?.StatusCode;
            }

            return statusCode;
        }
    }
}
