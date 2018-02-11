// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using Microsoft.Extensions.Logging;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json.Internal
{
    internal static class MvcPartialJsonLoggerExtensions
    {
        private static readonly Action<ILogger, string, Exception> LogMessage;

        static MvcPartialJsonLoggerExtensions()
        {
            LogMessage = LoggerMessage.Define<string>(LogLevel.Information, 1, "Executing PartialJsonResult, writing value {Value}.");
        }

        public static void PartialJsonResultExecuting(this ILogger logger, object value)
        {
            LogMessage(logger, Convert.ToString(value), null);
        }
    }
}
