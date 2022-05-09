using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class SslCertificateRequest
    {
        [JsonProperty("ssl-cert")] public SslCert SslCertificate { get; set; }
    }
}