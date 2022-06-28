using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class SslCertKey
    {
        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("file")] public string File { get; set; }

        [JsonProperty("file-handle")] public string FileHandle { get; set; }
    }
}