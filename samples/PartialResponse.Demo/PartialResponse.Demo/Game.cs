// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System.Collections.Generic;

namespace PartialResponse.Demo
{
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
}