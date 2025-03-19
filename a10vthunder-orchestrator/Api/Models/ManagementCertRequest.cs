using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class Certificate
    {
        public int load { get; set; }

        [JsonProperty("file-url")]
        public string fileurl { get; set; }
    }

    public class ManagementCertRequest
    {
        public Certificate certificate { get; set; }
    }
}
