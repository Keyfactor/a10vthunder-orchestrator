using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class Operation
    {
        [JsonProperty("oper")] public SslCertificateCollection Oper { get; set; }
    }
}