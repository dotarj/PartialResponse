// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using PartialResponse.Net.Http.Formatting;
using System;
using System.Web;
using System.Web.Http;

namespace PartialResponse.Demo
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new PartialJsonMediaTypeFormatter() { IgnoreCase = true });

            GlobalConfiguration.Configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
        }
    }
}