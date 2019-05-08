using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using JwtManagement;
using Newtonsoft.Json;

namespace WebApi.Jwt.Controllers
{
    public class ClientController : ApiController
    {
        [AllowAnonymous]
        public async Task<IEnumerable<string>> Get(string username)
        {
            var token = new JwtManager().GenerateToken(username);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:55385/api/");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await client.GetAsync("values");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();
                var results = JsonConvert.DeserializeObject<IEnumerable<string>>(result);
                return results;
            }
        }
    }
}
