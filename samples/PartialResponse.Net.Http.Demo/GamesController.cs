// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PartialResponse.Demo
{
    public class GamesController : ApiController
    {
        // api/games
        public IEnumerable<Game> Get()
        {
            return Games.Get();
        }

        // api/games/{id}
        public HttpResponseMessage Get(int id)
        {
            var result = Games.Get().FirstOrDefault(game => game.Id == id);

            if (result == null)
            {
                return Request.CreateResponse<Error>(HttpStatusCode.NotFound, new Error() { Message = string.Format("No game found with id {0}.", id) });
            }

            return Request.CreateResponse<Game>(HttpStatusCode.OK, result);
        }
    }
}