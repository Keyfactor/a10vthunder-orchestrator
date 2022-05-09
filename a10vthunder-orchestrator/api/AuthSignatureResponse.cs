using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class AuthSignatureResponse
    {
        [JsonProperty("authresponse")] public AuthResponse Response { get; set; }
    }
}