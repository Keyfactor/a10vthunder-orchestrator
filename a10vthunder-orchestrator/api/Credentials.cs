using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    internal class Credentials
    {
        [JsonProperty("username")] public string Username { get; set; }

        [JsonProperty("password")] public string Password { get; set; }
    }
}