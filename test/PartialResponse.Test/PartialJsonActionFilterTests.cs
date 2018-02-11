// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Moq;
using Xunit;

namespace PartialResponse.Test
{
    public class PartialJsonActionFilterTests
    {
        private readonly PartialJsonActionFilter partialJsonActionFilter = new PartialJsonActionFilter();

        [Fact]
        public void TheExecuteActionFilterAsyncMethodShouldThrowIfActionContextIsNull()
        {
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => this.partialJsonActionFilter.ExecuteActionFilterAsync(null, CancellationToken.None, () => null));
        }

        [Fact]
        public void TheExecuteActionFilterAsyncMethodShouldThrowIfContinuationIsNull()
        {
            // Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => this.partialJsonActionFilter.ExecuteActionFilterAsync(new HttpActionContext(), CancellationToken.None, null));
        }

        [Fact]
        public async Task TheExecuteActionFilterAsyncMethodShouldSetHttpResponseMessage()
        {
            // Arrange
            var httpActionContext = new HttpActionContext(new HttpControllerContext { Request = new HttpRequestMessage() }, Mock.Of<HttpActionDescriptor>());
            var httpResponeMessage = new HttpResponseMessage();

            // Act
            await this.partialJsonActionFilter.ExecuteActionFilterAsync(httpActionContext, CancellationToken.None, () => Task.FromResult(httpResponeMessage));

            // Assert
            Assert.Equal(httpResponeMessage, httpActionContext.Request.Properties["PR_HttpResponseMessage"]);
        }
    }
}
