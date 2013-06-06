// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using PartialResponse.Net.Http.Formatting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PartialResponse.Test
{
    [TestClass]
    public class JsonMediaTypeFormatterTests
    {
        [TestClass]
        public class TheWriteToStreamAsyncMethod
        {
            [TestMethod, ExpectedException(typeof(HttpResponseException))]
            public async Task ShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption1()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Name(");
                var value = GetData();

                // Act
                await Test(request, value);
            }

            [TestMethod, ExpectedException(typeof(HttpResponseException))]
            public async Task ShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption2()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer(Id)Publisher");
                var value = GetData();

                // Act
                await Test(request, value);
            }

            [TestMethod, ExpectedException(typeof(HttpResponseException))]
            public async Task ShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption3()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Console(Manufacturer(Id)))");
                var value = GetData();

                // Act
                await Test(request, value);
            }

            [TestMethod, ExpectedException(typeof(HttpResponseException))]
            public async Task ShouldThrowAnHttpResponseExceptionOnAnInvalidFieldsQueryOption4()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Console(Manufacturer//))");
                var value = GetData();

                // Act
                await Test(request, value);
            }

            [TestMethod]
            public async Task ShouldSerializeGamePropertiesOnly()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Name");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(value.First().Id, result.First().Id);
                Assert.AreEqual(value.First().Name, result.First().Name);
                Assert.IsNull(result.First().Characters);
                Assert.IsNull(result.First().Console);
                Assert.IsNull(result.First().Developer);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Links);
            }

            [TestMethod]
            public async Task ShouldSerializeGamePropertiesOnlyOfSingleInstance()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id,Name");
                var value = GetData().First();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Id, result.Id);
                Assert.AreEqual(value.Name, result.Name);
            }

            [TestMethod]
            public async Task ShouldSerializeGamePropertiesUsingAsteriskOnly()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=*");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(value.First().Id, result.First().Id);
                Assert.AreEqual(value.First().Name, result.First().Name);
                Assert.IsNotNull(result.First().Console);
                Assert.AreEqual(value.First().Console.Id, result.First().Console.Id);
                Assert.AreEqual(value.First().Console.Name, result.First().Console.Name);
                Assert.IsNotNull(result.First().Console.Manufacturer);
                Assert.AreEqual(value.First().Console.Manufacturer.Id, result.First().Console.Manufacturer.Id);
                Assert.AreEqual(value.First().Console.Manufacturer.Name, result.First().Console.Manufacturer.Name);
                Assert.IsNotNull(result.First().Publisher);
                Assert.AreEqual(value.First().Publisher.Id, result.First().Publisher.Id);
                Assert.AreEqual(value.First().Publisher.Name, result.First().Publisher.Name);
                Assert.IsNotNull(result.First().Developer);
                Assert.AreEqual(value.First().Developer.Id, result.First().Developer.Id);
                Assert.AreEqual(value.First().Developer.Name, result.First().Developer.Name);
                Assert.IsNotNull(result.First().Characters);
                Assert.AreEqual(value.First().Characters.Count(), result.First().Characters.Count());
                Assert.AreEqual(value.First().Characters.First().Id, result.First().Characters.First().Id);
                Assert.AreEqual(value.First().Characters.First().Name, result.First().Characters.First().Name);
                Assert.IsNotNull(result.First().Links);
                Assert.AreEqual(value.First().Links.First().Key, result.First().Links.First().Key);
                Assert.AreEqual(value.First().Links.First().Value, result.First().Links.First().Value);
            }

            [TestMethod]
            public async Task ShouldSerializeDeveloperNamePropertyOnly()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer(Name)");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(0, result.First().Id);
                Assert.AreEqual(null, result.First().Name);
                Assert.IsNull(result.First().Characters);
                Assert.IsNull(result.First().Console);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Links);
                Assert.IsNotNull(result.First().Developer);
                Assert.AreEqual(value.First().Developer.Name, result.First().Developer.Name);
            }

            [TestMethod]
            public async Task ShouldSerializeDeveloperAndCharactersPropertiesOnly()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer(Id,Name),Characters/*");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(0, result.First().Id);
                Assert.AreEqual(null, result.First().Name);
                Assert.IsNull(result.First().Console);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Links);
                Assert.IsNotNull(result.First().Developer);
                Assert.AreEqual(value.First().Developer.Id, result.First().Developer.Id);
                Assert.AreEqual(value.First().Developer.Name, result.First().Developer.Name);
                Assert.IsNotNull(result.First().Characters);
                Assert.AreEqual(value.First().Characters.Count(), result.First().Characters.Count());
                Assert.AreEqual(value.First().Characters.First().Id, result.First().Characters.First().Id);
                Assert.AreEqual(value.First().Characters.First().Name, result.First().Characters.First().Name);
            }

            [TestMethod]
            public async Task ShouldSerializeDeveloperPropertiesOnlyUsingAsterisk()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Developer/*");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(0, result.First().Id);
                Assert.AreEqual(null, result.First().Name);
                Assert.IsNull(result.First().Characters);
                Assert.IsNull(result.First().Console);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Links);
                Assert.IsNotNull(result.First().Developer);
                Assert.AreEqual(value.First().Developer.Id, result.First().Developer.Id);
                Assert.AreEqual(value.First().Developer.Name, result.First().Developer.Name);
            }

            [TestMethod]
            public async Task ShouldIgnoreCase()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=developer/*");
                var value = GetData();

                // Act
                var result = await Test(request, value, true);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(0, result.First().Id);
                Assert.AreEqual(null, result.First().Name);
                Assert.IsNull(result.First().Characters);
                Assert.IsNull(result.First().Console);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Links);
                Assert.IsNotNull(result.First().Developer);
                Assert.AreEqual(value.First().Developer.Id, result.First().Developer.Id);
                Assert.AreEqual(value.First().Developer.Name, result.First().Developer.Name);
            }

            [TestMethod]
            public async Task ShouldSerializeCharactersIdPropertyOnly()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Characters/Id");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(0, result.First().Id);
                Assert.AreEqual(null, result.First().Name);
                Assert.IsNull(result.First().Console);
                Assert.IsNull(result.First().Developer);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Links);
                Assert.IsNotNull(result.First().Characters);
                Assert.AreEqual(value.First().Characters.Count(), result.First().Characters.Count());
                Assert.AreEqual(value.First().Characters.First().Id, result.First().Characters.First().Id);
                Assert.IsNull(result.First().Characters.First().Name);
            }

            [TestMethod]
            public async Task ShouldSerializeConsolePropertiesAndDescendantsOnly()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Console/*");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(0, result.First().Id);
                Assert.AreEqual(null, result.First().Name);
                Assert.IsNull(result.First().Developer);
                Assert.IsNull(result.First().Publisher);
                Assert.IsNull(result.First().Characters);
                Assert.IsNull(result.First().Links);
                Assert.IsNotNull(result.First().Console);
                Assert.AreEqual(value.First().Console.Id, result.First().Console.Id);
                Assert.AreEqual(value.First().Console.Name, result.First().Console.Name);
                Assert.IsNotNull(result.First().Console.Manufacturer);
                Assert.AreEqual(value.First().Console.Manufacturer.Id, result.First().Console.Manufacturer.Id);
                Assert.AreEqual(value.First().Console.Manufacturer.Name, result.First().Console.Manufacturer.Name);
            }

            [TestMethod]
            public async Task ShouldSerializeAllProperties()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Count(), result.Count());
                Assert.AreEqual(value.First().Id, result.First().Id);
                Assert.AreEqual(value.First().Name, result.First().Name);
                Assert.IsNotNull(result.First().Console);
                Assert.AreEqual(value.First().Console.Id, result.First().Console.Id);
                Assert.AreEqual(value.First().Console.Name, result.First().Console.Name);
                Assert.IsNotNull(result.First().Console.Manufacturer);
                Assert.AreEqual(value.First().Console.Manufacturer.Id, result.First().Console.Manufacturer.Id);
                Assert.AreEqual(value.First().Console.Manufacturer.Name, result.First().Console.Manufacturer.Name);
                Assert.IsNotNull(result.First().Publisher);
                Assert.AreEqual(value.First().Publisher.Id, result.First().Publisher.Id);
                Assert.AreEqual(value.First().Publisher.Name, result.First().Publisher.Name);
                Assert.IsNotNull(result.First().Developer);
                Assert.AreEqual(value.First().Developer.Id, result.First().Developer.Id);
                Assert.AreEqual(value.First().Developer.Name, result.First().Developer.Name);
                Assert.IsNotNull(result.First().Characters);
                Assert.AreEqual(value.First().Characters.Count(), result.First().Characters.Count());
                Assert.AreEqual(value.First().Characters.First().Id, result.First().Characters.First().Id);
                Assert.AreEqual(value.First().Characters.First().Name, result.First().Characters.First().Name);
                Assert.IsNotNull(result.First().Links);
                Assert.AreEqual(value.First().Links.First().Key, result.First().Links.First().Key);
                Assert.AreEqual(value.First().Links.First().Value, result.First().Links.First().Value);
            }

            [TestMethod]
            public async Task ShouldSerializeNoProperties()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=");
                var value = GetData();

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(0, result.Count());
            }

            [TestMethod]
            public async Task ShouldSerializeNull()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=");
                IEnumerable<Game> value = null;

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNull(result);
            }

            [TestMethod]
            public async Task ShouldSerializeValueTypes()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=");
                var value = 1;

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.AreEqual(value, result);
            }

            [TestMethod]
            public async Task ShouldBypassPartialResponseOnNon200Responses()
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
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Message, result.Message);
            }

            [TestMethod]
            public async Task ShouldBypassPartialResponse()
            {
                // Arrange
                var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost?fields=Id");
                request.SetBypassPartialResponse(true);

                var value = new Error() { Message = "Error!" };

                // Act
                var result = await Test(request, value);

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(value.Message, result.Message);
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
                        response = new MochHttpResponse();
                    }

                    return response;
                }
            }
        }

        public class MochHttpResponse : HttpResponseBase
        {
            public override int StatusCode { get; set; }
        }
    }
}
