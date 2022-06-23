using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class AuthResponse
    {
        [JsonProperty("signature")] public string Signature { get; set; }

        [JsonProperty("description")] public string Description { get; set; }
    }
}