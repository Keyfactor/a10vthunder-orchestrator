using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class DeleteCertBaseRequest
    {
        [JsonProperty("delete", NullValueHandling = NullValueHandling.Ignore)]
        public DeleteCertRequest DeleteCert { get; set; }
       
    }
}
