using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class DeleteCertBaseRequest
    {
        [JsonProperty("delete", NullValueHandling = NullValueHandling.Ignore)]
        public DeleteCertRequest DeleteCert { get; set; }
       
    }
}
