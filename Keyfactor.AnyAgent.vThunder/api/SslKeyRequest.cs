using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class SslKeyRequest
    {
        [JsonProperty("ssl-key")] public SslCertKey SslKey { get; set; }
    }
}