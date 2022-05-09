using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class SslCertKey
    {
        [JsonProperty("action")] public string Action { get; set; }

        [JsonProperty("file")] public string File { get; set; }

        [JsonProperty("file-handle")] public string FileHandle { get; set; }
    }
}