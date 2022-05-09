using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder.api
{
    public class Operation
    {
        [JsonProperty("oper")] public SslCertificateCollection Oper { get; set; }
    }
}