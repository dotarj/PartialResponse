using System.Collections.Generic ;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        public List<dynamic> Index()
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

            return response;
        }
    }
}
