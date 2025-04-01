using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class SslCertificateCollection
    {
        [JsonProperty("ssl-certs")] public SslCertificate[] SslCertificates { get; set; }
    }
}