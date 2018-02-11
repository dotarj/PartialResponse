// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using PartialResponse.AspNetCore.Mvc.Formatters;
using PartialResponse.Core;

namespace Microsoft.AspNetCore.Http
{
    /// <summary>
    /// Provides extension methods for the <see cref="HttpRequest"/> class.
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        /// Indicates that partial response should be bypassed for the current request.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        public static void BypassPartialResponse(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            request.HttpContext.Items[PartialJsonOutputFormatter.BypassPartialResponseKey] = null;
        }

        internal static bool TryGetFields(this HttpRequest request, out Fields? result)
        {
            if (!request.Query.ContainsKey("fields"))
            {
                result = null;

                return true;
            }

            Fields fields;

            if (!Fields.TryParse(request.Query["fields"][0], out fields))
            {
                result = null;

                return false;
            }

            result = fields;

            return true;
        }
    }
}
