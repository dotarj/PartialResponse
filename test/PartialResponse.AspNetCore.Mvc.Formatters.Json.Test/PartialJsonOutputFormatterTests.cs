using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace PartialResponse.AspNetCore.Mvc.Formatters.Json
{
    public class PartialJsonOutputFormatterTests
    {
        private readonly PartialJsonOutputFormatter formatter;
        private readonly HttpContext httpContext = Mock.Of<HttpContext>();
        private readonly HttpRequest httpRequest = Mock.Of<HttpRequest>();
        private readonly HttpResponse httpResponse = Mock.Of<HttpResponse>();
        private readonly IQueryCollection queryCollection = Mock.Of<IQueryCollection>();
        private readonly StringBuilder body = new StringBuilder();

        public PartialJsonOutputFormatterTests()
        {
            Mock.Get(this.httpRequest)
                .SetupGet(httpRequest => httpRequest.Query)
                .Returns(this.queryCollection);

            Mock.Get(this.httpContext)
                .SetupGet(httpContext => httpContext.Request)
                .Returns(this.httpRequest);

            Mock.Get(this.httpContext)
                .SetupGet(httpContext => httpContext.Response)
                .Returns(this.httpResponse);

            this.formatter = new PartialJsonOutputFormatter(new JsonSerializerSettings(), Mock.Of<ArrayPool<char>>());
        }

        [Fact]
        public async Task TheWriteResponseBodyAsyncMethodShouldReturnStatusCode400IfFieldsMalformed()
        {
            // Arrange
            Mock.Get(this.queryCollection)
                .Setup(queryCollection => queryCollection.ContainsKey("fields"))
                .Returns(true);

            Mock.Get(this.queryCollection)
                .SetupGet(queryCollection => queryCollection["fields"])
                .Returns("foo/");

            var writeContext = new OutputFormatterWriteContext(this.httpContext, (stream, encoding) => new StringWriter(this.body), typeof(object), new {});

            // Act
            await this.formatter.WriteResponseBodyAsync(writeContext, Encoding.UTF8);

            // Assert
            Mock.Get(this.httpResponse)
                .VerifySet(httpResponse => httpResponse.StatusCode = 400);
        }

        [Fact]
        public async Task TheWriteResponseBodyAsyncMethodShouldNotWriteBodyIfFieldsMalformed()
        {
            // Arrange
            Mock.Get(this.queryCollection)
                .Setup(queryCollection => queryCollection.ContainsKey("fields"))
                .Returns(true);

            Mock.Get(this.queryCollection)
                .SetupGet(queryCollection => queryCollection["fields"])
                .Returns("foo/");

            var writeContext = new OutputFormatterWriteContext(this.httpContext, (stream, encoding) => new StringWriter(this.body), typeof(object), new {});

            // Act
            await this.formatter.WriteResponseBodyAsync(writeContext, Encoding.UTF8);

            // Assert
            Assert.Equal(0, this.body.Length);
        }

        [Fact]
        public async Task TheWriteResponseBodyAsyncMethodShouldNotApplyFieldsIfNotSupplied()
        {
            // Arrange
            Mock.Get(this.queryCollection)
                .Setup(queryCollection => queryCollection.ContainsKey("fields"))
                .Returns(false);

            var value = new { foo = "bar" };

            var writeContext = new OutputFormatterWriteContext(this.httpContext, (stream, encoding) => new StringWriter(this.body), typeof(object), value);

            // Act
            await this.formatter.WriteResponseBodyAsync(writeContext, Encoding.UTF8);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", this.body.ToString());
        }

        [Fact]
        public async Task TheWriteResponseBodyAsyncMethodShouldApplyFieldsIfSupplied()
        {
            // Arrange
            Mock.Get(this.queryCollection)
                .Setup(queryCollection => queryCollection.ContainsKey("fields"))
                .Returns(true);

            Mock.Get(this.queryCollection)
                .SetupGet(queryCollection => queryCollection["fields"])
                .Returns("foo");

            var value = new { foo = "bar", baz = "qux" };

            var writeContext = new OutputFormatterWriteContext(this.httpContext, (stream, encoding) => new StringWriter(this.body), typeof(object), value);

            // Act
            await this.formatter.WriteResponseBodyAsync(writeContext, Encoding.UTF8);

            // Assert
            Assert.Equal("{\"foo\":\"bar\"}", this.body.ToString());
        }
    }
}