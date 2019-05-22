using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using JwtManagement;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace GatewayApi.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        [Authorize]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var accessToken = Request.Headers["Authorization"].First();
            var jwtManager = new JwtManager();
            var token = await jwtManager.GenerateToken(accessToken);
            var client = new RestClient("http://localhost:55385/api/");
            var request = new RestRequest("values");
            request.AddHeader("Authorization", $"Bearer {token}");
            var results = await client.GetAsync<List<string>>(request);
            return results;
        }
    }
}
