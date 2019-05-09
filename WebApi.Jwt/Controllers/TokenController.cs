using System.Threading.Tasks;
using System.Web.Http;
using JwtManagement;

namespace WebApi.Jwt.Controllers
{
    public class TokenController : ApiController
    {
        [AllowAnonymous]
        public async Task<string> Get()
        {
            return await new JwtManager().GenerateToken("");
        }
    }
}
