using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class AuthSignatureResponse
    {
        [JsonProperty("authresponse")] public AuthResponse Response { get; set; }
    }
}