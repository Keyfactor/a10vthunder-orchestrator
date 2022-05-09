using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class AuthSignatureResponse
    {
        [JsonProperty("authresponse")] public AuthResponse Response { get; set; }
    }
}