using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class AuthResponse
    {
        [JsonProperty("signature")] public string Signature { get; set; }

        [JsonProperty("description")] public string Description { get; set; }
    }
}