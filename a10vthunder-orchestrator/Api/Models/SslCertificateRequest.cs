using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class SslCertificateRequest
    {
        [JsonProperty("ssl-cert")] public SslCert SslCertificate { get; set; }
    }
}