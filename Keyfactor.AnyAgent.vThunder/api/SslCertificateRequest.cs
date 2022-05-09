using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class SslCertificateRequest
    {
        [JsonProperty("ssl-cert")] public SslCert SslCertificate { get; set; }
    }
}