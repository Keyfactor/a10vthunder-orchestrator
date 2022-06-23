using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class SslKeyRequest
    {
        [JsonProperty("ssl-key")] public SslCertKey SslKey { get; set; }
    }
}