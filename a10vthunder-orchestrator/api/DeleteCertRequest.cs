using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class DeleteCertRequest
    {
        [JsonProperty("cert-name")]
        public string CertName { get; set; }

        [JsonProperty("private-key",NullValueHandling=NullValueHandling.Ignore)]
        public string PrivateKey { get; set; }
    }
}
