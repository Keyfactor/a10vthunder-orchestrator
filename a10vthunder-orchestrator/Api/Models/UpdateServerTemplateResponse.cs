using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class UpdateTemplateResposneCertificate
    {
        public string cert { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }

    public class UpdateServerTemplateResponse
    {
        public UpdateTemplateResposneCertificate certificate { get; set; }
    }
}
