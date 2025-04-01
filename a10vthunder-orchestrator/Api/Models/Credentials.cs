using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    internal class Credentials
    {
        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("password")] public string Password { get; set; }
    }
}