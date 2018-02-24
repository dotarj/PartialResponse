// Copyright (c) Arjen Post. See LICENSE and NOTICE in the project root for license information.

using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace PartialResponse
{
    public class FormatterController : ApiController
    {
        public IHttpActionResult Get(int statusCode = 200)
        {
            var content = new List<dynamic>
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

            return this.Content((HttpStatusCode)statusCode, content);
        }
    }
}
