using Newtonsoft.Json;
using System.Collections.Generic;

namespace a10vthunder_orchestrator.Api.Models
{
    public class TemplateCertificate
    {
        public string cert { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }

    public class TemplateListResponse
    {
        [JsonProperty("server-ssl-list")]
        public List<ServerSslList> serverssllist { get; set; }
    }

    public class ServerSslList
    {
        public string name { get; set; }

        [JsonProperty("enable-tls-alert-logging")]
        public int enabletlsalertlogging { get; set; }

        [JsonProperty("handshake-logging-enable")]
        public int handshakeloggingenable { get; set; }

        [JsonProperty("session-key-logging-enable")]
        public int sessionkeyloggingenable { get; set; }

        [JsonProperty("close-notify")]
        public int closenotify { get; set; }

        [JsonProperty("session-ticket-enable")]
        public int sessionticketenable { get; set; }
        public int version { get; set; }
        public int dgversion { get; set; }

        [JsonProperty("ocsp-stapling")]
        public int ocspstapling { get; set; }

        [JsonProperty("use-client-sni")]
        public int useclientsni { get; set; }

        [JsonProperty("renegotiation-disable")]
        public int renegotiationdisable { get; set; }

        [JsonProperty("session-cache-size")]
        public int sessioncachesize { get; set; }

        [JsonProperty("early-data")]
        public int earlydata { get; set; }
        public string uuid { get; set; }
        public Certificate certificate { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }
}
