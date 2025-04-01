using Newtonsoft.Json;
using System.Collections.Generic;

namespace a10vthunder.Api.Models
{
    public class CleintCertificateList
    {
        public string cert { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }

    public class UpdateClientTemplateResponse
    {
        [JsonProperty("certificate-list")]
        public List<CleintCertificateList> certificatelist { get; set; }
    }

}
