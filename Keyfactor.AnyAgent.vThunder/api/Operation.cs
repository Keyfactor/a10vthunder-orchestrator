using Newtonsoft.Json;

namespace Keyfactor.AnyAgent.vThunder.api
{
    public class Operation
    {
        [JsonProperty("oper")] public SslCertificateCollection Oper { get; set; }
    }
}