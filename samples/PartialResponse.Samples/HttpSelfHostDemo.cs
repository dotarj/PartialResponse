// Copyright (c) Arjen Post. See LICENSE and NOTICE in the project root for license information.

using System;
using System.Web.Http;
using System.Web.Http.SelfHost;
using PartialResponse.Net.Http.Formatting;

namespace PartialResponse.Demo
{
    public static class HttpSelfHostDemo
    {
        public static void Run(string baseAddress)
        {
            var configuration = new HttpSelfHostConfiguration(baseAddress);

            configuration.Filters.Add(new PartialJsonActionFilter());
            configuration.Formatters.Clear();
            configuration.Formatters.Add(new PartialJsonMediaTypeFormatter() { IgnoreCase = true });
            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

            using (var server = new HttpSelfHostServer(configuration))
            {
                server.OpenAsync().Wait();

                Console.WriteLine($"Now listening on: {baseAddress}");
                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }
    }
}
