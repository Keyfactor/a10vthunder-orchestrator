using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class AuthResponse
    {
        [JsonProperty("signature")] public string Signature { get; set; }

        [JsonProperty("description")] public string Description { get; set; }
    }
}