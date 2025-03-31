using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class PrivateKey
    {
        public int load { get; set; }

        [JsonProperty("file-url")]
        public string fileurl { get; set; }
    }

    public class ManagementPrivateKeyRequest
    {
        [JsonProperty("private-key")]
        public PrivateKey privatekey { get; set; }
    }

}
