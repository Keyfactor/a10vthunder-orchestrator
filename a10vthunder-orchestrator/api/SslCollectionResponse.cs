using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class SslCollectionResponse
    {
        [JsonProperty("ssl-cert")] public Operation SslCertificate { get; set; }
    }
}