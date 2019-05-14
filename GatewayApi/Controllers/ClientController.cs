using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtManagement;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RestSharp;

namespace GatewayApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
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
