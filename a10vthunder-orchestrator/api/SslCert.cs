using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class SslCert
    {
        [JsonProperty("certificate-type")] public string CertificateType { get; set; }

        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("file")] public string File { get; set; }

        [JsonProperty("file-handle")] public string FileHandle { get; set; }
    }
}