// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using Owin; 
using PartialResponse.Net.Http.Formatting;
using System.Web.Http;

namespace PartialResponse.Net.Http.Demo
{
    public class Startup 
    {
        public void Configuration(IAppBuilder appBuilder)
        { 
            var configuration = new HttpConfiguration();
            
            configuration.Formatters.Clear();
            configuration.Formatters.Add(new PartialJsonMediaTypeFormatter() { IgnoreCase = true });
            configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional }); 

            appBuilder.Use<HttpResponseExceptionMiddleware>().UseWebApi(configuration);
        } 
    } 
}