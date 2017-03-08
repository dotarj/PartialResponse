// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using PartialResponse.Net.Http.Formatting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Xunit;

namespace PartialResponse.Test
{
    public class JsonMediaTypeFormatterTests
    {
        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption1()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Name(");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption2()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer(Id)Publisher");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption3()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Console(Manufacturer(Id)))");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption4()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Console(Manufacturer//))");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption5()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=(");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption6()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=)");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public void TheWriteToStreamAsyncMethodShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption7()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=,");
            var value = GetData();

            // Act
            Assert.ThrowsAsync<HttpResponseException>(() => Test(request, value));
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeGamePropertiesOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Name");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(value.First().Id, result.First().Id);
            Assert.Equal(value.First().Name, result.First().Name);
            Assert.Null(result.First().Characters);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Developer);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Links);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeGamePropertiesOnlyOfSingleInstance()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Name");
            var value = GetData().First();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Id, result.Id);
            Assert.Equal(value.Name, result.Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeGamePropertiesUsingAsteriskOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=*");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(value.First().Id, result.First().Id);
            Assert.Equal(value.First().Name, result.First().Name);
            Assert.NotNull(result.First().Console);
            Assert.Equal(value.First().Console.Id, result.First().Console.Id);
            Assert.Equal(value.First().Console.Name, result.First().Console.Name);
            Assert.NotNull(result.First().Console.Manufacturer);
            Assert.Equal(value.First().Console.Manufacturer.Id, result.First().Console.Manufacturer.Id);
            Assert.Equal(value.First().Console.Manufacturer.Name, result.First().Console.Manufacturer.Name);
            Assert.NotNull(result.First().Publisher);
            Assert.Equal(value.First().Publisher.Id, result.First().Publisher.Id);
            Assert.Equal(value.First().Publisher.Name, result.First().Publisher.Name);
            Assert.NotNull(result.First().Developer);
            Assert.Equal(value.First().Developer.Id, result.First().Developer.Id);
            Assert.Equal(value.First().Developer.Name, result.First().Developer.Name);
            Assert.NotNull(result.First().Characters);
            Assert.Equal(value.First().Characters.Count(), result.First().Characters.Count());
            Assert.Equal(value.First().Characters.First().Id, result.First().Characters.First().Id);
            Assert.Equal(value.First().Characters.First().Name, result.First().Characters.First().Name);
            Assert.NotNull(result.First().Links);
            Assert.Equal(value.First().Links.First().Key, result.First().Links.First().Key);
            Assert.Equal(value.First().Links.First().Value, result.First().Links.First().Value);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeDeveloperNamePropertyOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer(Name)");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(0, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Characters);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Developer);
            Assert.Equal(value.First().Developer.Name, result.First().Developer.Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeDeveloperAndCharactersPropertiesOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer(Id,Name),Characters/*");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(0, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Developer);
            Assert.Equal(value.First().Developer.Id, result.First().Developer.Id);
            Assert.Equal(value.First().Developer.Name, result.First().Developer.Name);
            Assert.NotNull(result.First().Characters);
            Assert.Equal(value.First().Characters.Count(), result.First().Characters.Count());
            Assert.Equal(value.First().Characters.First().Id, result.First().Characters.First().Id);
            Assert.Equal(value.First().Characters.First().Name, result.First().Characters.First().Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeIdAndPublisherPropertiesOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Publisher(Id,Name)");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Developer);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Publisher);
            Assert.Equal(value.First().Publisher.Id, result.First().Publisher.Id);
            Assert.Equal(value.First().Publisher.Name, result.First().Publisher.Name);
            Assert.Null(result.First().Characters);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeIdAndPublisherNamePropertiesOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Publisher(Name)");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(1, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Developer);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Publisher);
            Assert.Equal(0, result.First().Publisher.Id);
            Assert.Equal(value.First().Publisher.Name, result.First().Publisher.Name);
            Assert.Null(result.First().Characters);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeDeveloperPropertiesOnlyUsingAsterisk()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer/*");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(0, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Characters);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Developer);
            Assert.Equal(value.First().Developer.Id, result.First().Developer.Id);
            Assert.Equal(value.First().Developer.Name, result.First().Developer.Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldIgnoreCase()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=developer/*");
            var value = GetData();

            // Act
            var result = await Test(request, value, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(0, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Characters);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Developer);
            Assert.Equal(value.First().Developer.Id, result.First().Developer.Id);
            Assert.Equal(value.First().Developer.Name, result.First().Developer.Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeCharactersIdPropertyOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Characters/Id");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(0, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Console);
            Assert.Null(result.First().Developer);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Characters);
            Assert.Equal(value.First().Characters.Count(), result.First().Characters.Count());
            Assert.Equal(value.First().Characters.First().Id, result.First().Characters.First().Id);
            Assert.Null(result.First().Characters.First().Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeConsolePropertiesAndDescendantsOnly()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Console/*");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(0, result.First().Id);
            Assert.Equal(null, result.First().Name);
            Assert.Null(result.First().Developer);
            Assert.Null(result.First().Publisher);
            Assert.Null(result.First().Characters);
            Assert.Null(result.First().Links);
            Assert.NotNull(result.First().Console);
            Assert.Equal(value.First().Console.Id, result.First().Console.Id);
            Assert.Equal(value.First().Console.Name, result.First().Console.Name);
            Assert.NotNull(result.First().Console.Manufacturer);
            Assert.Equal(value.First().Console.Manufacturer.Id, result.First().Console.Manufacturer.Id);
            Assert.Equal(value.First().Console.Manufacturer.Name, result.First().Console.Manufacturer.Name);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeAllProperties()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Count(), result.Count());
            Assert.Equal(value.First().Id, result.First().Id);
            Assert.Equal(value.First().Name, result.First().Name);
            Assert.NotNull(result.First().Console);
            Assert.Equal(value.First().Console.Id, result.First().Console.Id);
            Assert.Equal(value.First().Console.Name, result.First().Console.Name);
            Assert.NotNull(result.First().Console.Manufacturer);
            Assert.Equal(value.First().Console.Manufacturer.Id, result.First().Console.Manufacturer.Id);
            Assert.Equal(value.First().Console.Manufacturer.Name, result.First().Console.Manufacturer.Name);
            Assert.NotNull(result.First().Publisher);
            Assert.Equal(value.First().Publisher.Id, result.First().Publisher.Id);
            Assert.Equal(value.First().Publisher.Name, result.First().Publisher.Name);
            Assert.NotNull(result.First().Developer);
            Assert.Equal(value.First().Developer.Id, result.First().Developer.Id);
            Assert.Equal(value.First().Developer.Name, result.First().Developer.Name);
            Assert.NotNull(result.First().Characters);
            Assert.Equal(value.First().Characters.Count(), result.First().Characters.Count());
            Assert.Equal(value.First().Characters.First().Id, result.First().Characters.First().Id);
            Assert.Equal(value.First().Characters.First().Name, result.First().Characters.First().Name);
            Assert.NotNull(result.First().Links);
            Assert.Equal(value.First().Links.First().Key, result.First().Links.First().Key);
            Assert.Equal(value.First().Links.First().Value, result.First().Links.First().Value);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeNoProperties()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=");
            var value = GetData();

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.Count());
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeNull()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=");
            IEnumerable<Game> value = null;

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldSerializeValueTypes()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=");
            var value = 1;

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldBypassPartialResponseOnNon200Responses()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id");
            var httpContext = new MockHttpContext();
            httpContext.Response.StatusCode = 404;

            request.Properties["MS_HttpContext"] = httpContext;

            var value = new Error() { Message = "Error!" };

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Message, result.Message);
        }

        [Fact]
        public async Task TheWriteToStreamAsyncMethodShouldBypassPartialResponse()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id");
            request.SetBypassPartialResponse(true);

            var value = new Error() { Message = "Error!" };

            // Act
            var result = await Test(request, value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Message, result.Message);
        }

        private async static Task<T> Test<T>(HttpRequestMessage request, T value, bool ignoreCase = false)
        {
            var formatter = new PartialJsonMediaTypeFormatter() { IgnoreCase = ignoreCase }.GetPerRequestFormatterInstance(null, request, null);

            using (var memoryStream = new MemoryStream())
            {
                await formatter.WriteToStreamAsync(typeof(T), value, memoryStream, null, null);

                memoryStream.Flush();
                memoryStream.Position = 0;

                return (T)await formatter.ReadFromStreamAsync(typeof(T), memoryStream, null, null);
            }
        }

        private static IEnumerable<Game> GetData()
        {
            yield return new Game()
            {
                Id = 1,
                Name = "Star Fox",
                Console = new Console()
                {
                    Id = 1,
                    Name = "SNES",
                    Manufacturer = new Manufacturer()
                    {
                        Id = 1,
                        Name = "Nintendo"
                    }
                },
                Developer = new Developer() { Id = 1, Name = "Nintendo EAD" },
                Publisher = new Publisher() { Id = 1, Name = "Nintendo" },
                Characters = new List<Character>()
                {
                    new Character() { Id = 1, Name = "Fox McCloud" },
                    new Character() { Id = 2, Name = "Falco Lombardi" },
                    new Character() { Id = 3, Name = "Slippy Toad" },
                    new Character() { Id = 4, Name = "Peppy Hare" }
                },
                Links = new Dictionary<string, string>()
                {
                    { "self", "http://www.giantbomb.com/star-fox/3030-3984/" }
                }
            };

            yield return new Game()
            {
                Id = 2,
                Name = "Super Metroid",
                Console = new Console()
                {
                    Id = 1,
                    Name = "SNES",
                    Manufacturer = new Manufacturer()
                    {
                        Id = 1,
                        Name = "Nintendo"
                    }
                },
                Developer = new Developer() { Id = 2, Name = "Nintendo R&D1" },
                Publisher = new Publisher() { Id = 1, Name = "Nintendo" },
                Characters = new List<Character>()
                {
                    new Character() { Id = 5, Name = "Samus Aran" }
                },
                Links = new Dictionary<string, string>()
                {
                    { "self", "http://www.giantbomb.com/super-metroid/3030-8292/" }
                }
            };

            yield return new Game()
            {
                Id = 3,
                Name = "The Legend of Zelda: A Link to the Past",
                Console = new Console()
                {
                    Id = 1,
                    Name = "SNES",
                    Manufacturer = new Manufacturer()
                    {
                        Id = 1,
                        Name = "Nintendo"
                    }
                },
                Developer = new Developer() { Id = 1, Name = "Nintendo EAD" },
                Publisher = new Publisher() { Id = 1, Name = "Nintendo" },
                Characters = new List<Character>()
                {
                    new Character() { Id = 6, Name = "Link" },
                    new Character() { Id = 7, Name = "Princess Zelda" },
                    new Character() { Id = 8, Name = "Agahnim" },
                    new Character() { Id = 9, Name = "Ganon" }
                },
                Links = new Dictionary<string, string>()
                {
                    { "self", "http://www.giantbomb.com/the-legend-of-zelda-a-link-to-the-past/3030-10276/" }
                }
            };
        }
    }

    public class Publisher
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Developer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Console
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Manufacturer Manufacturer { get; set; }
    }

    public class Manufacturer
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class Game
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Console Console { get; set; }

        public Developer Developer { get; set; }

        public Publisher Publisher { get; set; }

        public IEnumerable<Character> Characters { get; set; }

        public Dictionary<string, string> Links { get; set; }
    }

    public class Error
    {
        public string Message { get; set; }
    }

    public class MockHttpContext : HttpContextBase
    {
        private HttpResponseBase response;

        public override HttpResponseBase Response
        {
            get
            {
                if (response == null)
                {
                    response = new MockHttpResponse();
                }

                return response;
            }
        }
    }

    public class MockHttpResponse : HttpResponseBase
    {
        public override int StatusCode { get; set; }
    }
}
