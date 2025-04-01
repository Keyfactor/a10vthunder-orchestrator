using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class DeleteCertBaseRequest
    {
        [JsonProperty("delete", NullValueHandling = NullValueHandling.Ignore)]
        public DeleteCertRequest DeleteCert { get; set; }
       
    }
}
