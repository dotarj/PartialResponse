// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System;
using Microsoft.Owin.Hosting;

namespace PartialResponse
{
    public static class OwinHostDemo
    {
        public static void Run(string baseAddress)
        {
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }
    }
}
