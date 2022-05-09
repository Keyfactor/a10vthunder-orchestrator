using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class SslKeyRequest
    {
        [JsonProperty("ssl-key")] public SslCertKey SslKey { get; set; }
    }
}