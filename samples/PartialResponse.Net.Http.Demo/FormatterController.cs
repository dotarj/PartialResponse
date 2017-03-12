// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System.Collections.Generic;
using System.Web.Http;

namespace PartialResponse.Net.Http.Demo
{
    public class FormatterController : ApiController
    {
        public List<dynamic> Get()
        {
            return new List<dynamic>()
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
        }
    }
}
