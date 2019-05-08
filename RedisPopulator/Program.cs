using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace RedisPopulator
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var hmac = new HMACSHA256();
            var securityKey = Convert.ToBase64String(hmac.Key);

            var redis = await ConnectionMultiplexer.ConnectAsync("localhost");

            var db = redis.GetDatabase();
            const string securityKeysKey = "SecurityKeys";
            var securityKeys = new List<string>();
            if (db.KeyExists(securityKeysKey))
            {
                var keysString = db.StringGet("SecurityKeys");
                var keys = JsonConvert.DeserializeObject<IEnumerable<string>>(keysString);
                securityKeys.AddRange(keys);
            }
            
            securityKeys.Add(securityKey);

            db.StringSet(securityKeysKey, JsonConvert.SerializeObject(securityKeys));

            Console.WriteLine(securityKeys.Count);
        }
    }
}
