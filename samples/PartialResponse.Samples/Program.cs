// Copyright (c) Arjen Post. See LICENSE and NOTICE in the project root for license information.

using PartialResponse.Demo;

namespace PartialResponse
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // OwinHostDemo.Run("http://localhost:9000/");
            HttpSelfHostDemo.Run("http://localhost:9001/");
        }
    }
}
