using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class SslCertificateCollection
    {
        [JsonProperty("ssl-certs")] public SslCertificate[] SslCertificates { get; set; }
    }
}