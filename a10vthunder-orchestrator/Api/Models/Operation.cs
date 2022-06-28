using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class Operation
    {
        [JsonProperty("oper")] public SslCertificateCollection Oper { get; set; }
    }
}