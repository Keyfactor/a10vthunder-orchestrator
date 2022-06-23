using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class SslCertificateCollection
    {
        [JsonProperty("ssl-certs")] public SslCertificate[] SslCertificates { get; set; }
    }
}