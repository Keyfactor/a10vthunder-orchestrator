using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class SslCollectionResponse
    {
        [JsonProperty("ssl-cert")] public Operation SslCertificate { get; set; }
    }
}