using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class AuthSignatureResponse
    {
        [JsonProperty("authresponse")] public AuthResponse Response { get; set; }
    }
}