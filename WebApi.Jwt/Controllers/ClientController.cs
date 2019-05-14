using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using JwtManagement;
using RestSharp;

namespace WebApi.Jwt.Controllers
{
    public class ClientController : ApiController
    {
        [AllowAnonymous]
        public async Task<IEnumerable<string>> Get()
        {
            var accessToken = Request.Headers.Authorization.Parameter;
            var token = await new JwtManager().GenerateToken(accessToken);
            var client = new RestClient("http://localhost:55385/api/");
            var request = new RestRequest("values");
            request.AddHeader("Authorization", $"Bearer {token}");
            var results = await client.GetAsync<List<string>>(request);
            return results;
        }
    }
}
