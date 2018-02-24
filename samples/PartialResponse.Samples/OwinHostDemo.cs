// Copyright (c) Arjen Post. See LICENSE and NOTICE in the project root for license information.

using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using PartialResponse.Net.Http.Formatting;

namespace PartialResponse
{
    public static class OwinHostDemo
    {
        public static void Run(string baseAddress)
        {
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine($"Now listening on: {baseAddress}");
                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

        private class Startup
        {
            public void Configuration(IAppBuilder appBuilder)
            {
                var configuration = new HttpConfiguration();

                configuration.Filters.Add(new PartialJsonActionFilter());
                configuration.Formatters.Clear();
                configuration.Formatters.Add(new PartialJsonMediaTypeFormatter() { IgnoreCase = true });
                configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });

                appBuilder.UseWebApi(configuration);
            }
        }
    }
}
