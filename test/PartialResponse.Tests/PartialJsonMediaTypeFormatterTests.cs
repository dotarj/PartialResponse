// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Moq;
using PartialResponse.Net.Http.Formatting;
using Xunit;

namespace PartialResponse.Tests
{
    public class PartialJsonMediaTypeFormatterTests
    {
        private readonly PartialJsonMediaTypeFormatter formatter;
        private readonly HttpRequestMessage httpRequest = new HttpRequestMessage();
        private readonly HttpResponseBase httpResponse = Mock.Of<HttpResponseBase>();

        public PartialJsonMediaTypeFormatterTests()
        {
            this.httpRequest.Method = HttpMethod.Get;

            this.formatter = (PartialJsonMediaTypeFormatter)new PartialJsonMediaTypeFormatter().GetPerRequestFormatterInstance(null, this.httpRequest, null);
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowIfFieldsMalformed()
        {
            // Arrange
            this.httpRequest.RequestUri = new Uri("http://localhost?fields=foo/");

            var value = new { };

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => this.WriteAsync(value));
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldNotApplyFieldsIfNotSupplied()
        {
            // Arrange
            this.httpRequest.RequestUri = new Uri("http://localhost");

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldApplyFieldsIfSupplied()
        {
            // Arrange
            Mock.Get(this.httpResponse)
                .SetupGet(httpResponse => httpResponse.StatusCode)
                .Returns(200);

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=foo");

            var value = new { foo = "bar", baz = "qux" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldIgnoreCase()
        {
            // Arrange
            Mock.Get(this.httpResponse)
                .SetupGet(httpResponse => httpResponse.StatusCode)
                .Returns(200);

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=FOO");
            this.formatter.IgnoreCase = true;

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldNotIgnoreCase()
        {
            // Arrange
            Mock.Get(this.httpResponse)
                .SetupGet(httpResponse => httpResponse.StatusCode)
                .Returns(200);

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=FOO");
            this.formatter.IgnoreCase = false;

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldBypassPartialResponseIfConfigured()
        {
            // Arrange
            Mock.Get(this.httpResponse)
                .SetupGet(httpResponse => httpResponse.StatusCode)
                .Returns(200);

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=");
            this.httpRequest.SetBypassPartialResponse(true);

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldBypassPartialResponseIfHttpResponseMessageStatusCodeIsNot200()
        {
            // Arrange
            var httpResponseMessage = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };

            this.httpRequest.Properties["PR_HttpResponseMessage"] = httpResponseMessage;

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=");

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldNotBypassPartialResponseIfHttpContextResponseIsNull()
        {
            // Arrange
            var httpContext = Mock.Of<HttpContextBase>();

            Mock.Get(httpContext)
                .SetupGet(context => context.Response)
                .Returns((HttpResponseBase)null);

            this.httpRequest.Properties["MS_HttpContext"] = httpContext;

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=foo");

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldBypassPartialResponseIfHttpContextStatusCodeIsNot200()
        {
            // Arrange
            var httpContext = Mock.Of<HttpContextBase>();

            Mock.Get(httpContext)
                .SetupGet(context => context.Response)
                .Returns(this.httpResponse);

            this.httpRequest.Properties["MS_HttpContext"] = httpContext;

            Mock.Get(this.httpResponse)
                .SetupGet(httpResponse => httpResponse.StatusCode)
                .Returns(500);

            this.httpRequest.RequestUri = new Uri("http://localhost?fields=");

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldNotBypassPartialResponseIfHttpContextAndHttpResponseMessageAreNotSet()
        {
            // Arrange
            this.httpRequest.RequestUri = new Uri("http://localhost?fields=");

            var value = new { foo = "bar" };

            // Act
            var body = await this.WriteAsync(value);

            // Assert
            Assert.Equal("{}", body);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldAcceptNullRequest()
        {
            // Arrange
            var formatter = new PartialJsonMediaTypeFormatter();

            Mock.Get(this.httpResponse)
                .SetupGet(httpResponse => httpResponse.StatusCode)
                .Returns(200);

            // Act
            using (var memoryStream = new MemoryStream())
            {
                await formatter.WriteToStreamAsync(typeof(object), new { }, memoryStream, null, null);
            }
        }

        private async Task<string> WriteAsync(object value)
        {
            using (var memoryStream = new MemoryStream())
            {
                await this.formatter.WriteToStreamAsync(typeof(object), value, memoryStream, null, null);

                memoryStream.Flush();
                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}
