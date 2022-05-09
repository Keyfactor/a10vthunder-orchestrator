using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class SslCollectionResponse
    {
        [JsonProperty("ssl-cert")] public Operation SslCertificate { get; set; }
    }
}