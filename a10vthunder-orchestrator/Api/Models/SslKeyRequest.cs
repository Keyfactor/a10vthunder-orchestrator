using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class SslKeyRequest
    {
        [JsonProperty("ssl-key")] public SslCertKey SslKey { get; set; }
    }
}