using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    internal class Credentials
    {
        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("password")] public string Password { get; set; }
    }
}