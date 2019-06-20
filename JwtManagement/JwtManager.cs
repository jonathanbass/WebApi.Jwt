using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace JwtManagement
{
    public class JwtManager
    {
        private readonly string _key;

        public JwtManager()
        {
            var redis = ConnectionMultiplexer.Connect("localhost");

            var db = redis.GetDatabase();
            var keysString = db.StringGet("SecurityKeys");
            var key = JsonConvert.DeserializeObject<IEnumerable<string>>(keysString).First();
            _key = key;
        }

        public async Task<string> GenerateToken(string accessToken, int expireMinutes = 20)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var symmetricSecurityKey = new SymmetricSecurityKey(Convert.FromBase64String(_key));
            var now = DateTime.UtcNow;

            // http://docs.identityserver.io/en/latest/endpoints/userinfo.html
            var userInfoClient = new UserInfoClient("http://localhost:5000/connect/userinfo");
            var userInfo = await userInfoClient.GetAsync(accessToken.Replace("Bearer ", string.Empty));

            var claims = new [] { new Claim(ClaimTypes.Name, userInfo.Claims.First().Value) };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            return token;
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token);

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(_key);
                var key = new SymmetricSecurityKey(symmetricKey);

                var validationParameters = new TokenValidationParameters()
                {
                   RequireExpirationTime = true,
                   ValidateIssuer = false,
                   ValidateAudience = false,
                   IssuerSigningKey = key
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}