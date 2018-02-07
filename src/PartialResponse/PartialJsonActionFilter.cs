// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using PartialResponse.Net.Http.Formatting;

namespace PartialResponse
{
    /// <summary>
    /// Ensures that the status code can be determined by the <see cref="PartialJsonMediaTypeFormatter"/> by placing
    /// the <see cref="HttpResponseMessage"/> in the <see cref="HttpRequestMessage.Properties"/>.
    /// </summary>
    public class PartialJsonActionFilter : IActionFilter
    {
        internal const string HttpResponseMessageKey = "PR_HttpResponseMessage";

        /// <summary>
        /// Gets a value indicating whether more than one instance of the indicated attribute can be specified
        /// for a single program element.
        /// </summary>
        public bool AllowMultiple => false;

        /// <summary>
        /// Executes the filter action asynchronously.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <param name="cancellationToken">The cancellation token assigned for this task.</param>
        /// <param name="continuation">The delegate function to continue after the action method is invoked.</param>
        /// <returns>The newly created task for this operation.</returns>
        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext == null)
            {
                throw new ArgumentNullException(nameof(actionContext));
            }

            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            var response = await continuation().ConfigureAwait(false);

            actionContext.Request.Properties[HttpResponseMessageKey] = response;

            return response;
        }
    }
}
