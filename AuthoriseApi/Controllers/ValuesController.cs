using System.Collections.Generic;
using System.Web.Http;
using JwtManagement.Filters;

namespace AuthoriseApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        [JwtAuthentication]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }
    }
}
