using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;

namespace PartialResponse.Net.Http.Demo
{
    public class HttpResponseExceptionMiddleware : OwinMiddleware
    {
        public HttpResponseExceptionMiddleware(OwinMiddleware next)
         : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            try
            {
                await Next.Invoke(context);
            }
            catch (HttpResponseException httpResponseException)
            {
                context.Response.StatusCode = (int)httpResponseException.Response.StatusCode;
            }
        }
    }
}