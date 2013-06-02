using System.Collections.Generic;

namespace PartialResponse.Demo
{
    internal static class Games
    {
        internal static IEnumerable<Game> Get()
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
}