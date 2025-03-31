using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class SslCertificate
    {
        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("serial")] public string Serial { get; set; }

        [JsonProperty("notbefore")] public string NotBefore { get; set; }

        [JsonProperty("notafter")] public string NotAfter { get; set; }

        [JsonProperty("common-name")] public string CommonName { get; set; }

        [JsonProperty("organization")] public string Organization { get; set; }

        [JsonProperty("subject")] public string Subject { get; set; }

        [JsonProperty("issuer")] public string Issuer { get; set; }

        [JsonProperty("status")] public string Status { get; set; }
    }
}