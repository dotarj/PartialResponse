using System.Collections.Generic ;
using Microsoft.AspNetCore.Mvc;

namespace PartialResponse.AspNetCore.Mvc.Demo.Controllers
{
    public class ExecutorController : Controller
    {
        public IActionResult Index()
        {
            var response = new List<dynamic>()
            {
                new
                {
                    Foo = 1,
                    Bar = new
                    {
                        Baz = 2,
                        Qux = 3
                    }
                },
                new
                {
                    Foo = 2,
                    Bar = new
                    {
                        Baz = 3,
                        Qux = 4
                    }
                },
                new
                {
                    Foo = 3,
                    Bar = new
                    {
                        Baz = 5,
                        Qux = 6
                    }
                }
            };

            return this.PartialJson(response);
        }
    }
}
