// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Microsoft.Net.Http.Headers;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal
{
    internal class MediaTypeHeaderValues
    {
        public static readonly MediaTypeHeaderValue ApplicationJson
            = MediaTypeHeaderValue.Parse("application/json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue TextJson
            = MediaTypeHeaderValue.Parse("text/json").CopyAsReadOnly();
    }
}