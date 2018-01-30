// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using PartialResponse.Demo;

namespace PartialResponse
{
    class Program
    {
        static void Main(string[] args)
        {
            //OwinHostDemo.Run("http://localhost:9000/");
            HttpSelfHostDemo.Run("http://localhost:9001/");
        }
    }
}
