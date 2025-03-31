using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class ManagementCertificate
    {
        public int load { get; set; }

        [JsonProperty("file-url")]
        public string fileurl { get; set; }
    }

    public class ManagementCertRequest
    {
        public ManagementCertificate certificate { get; set; }
    }
}
