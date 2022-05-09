using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class SslCertificateCollection
    {
        [JsonProperty("ssl-certs")] public SslCertificate[] SslCertificates { get; set; }
    }
}