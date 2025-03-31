using Newtonsoft.Json;
using System.Collections.Generic;

namespace a10vthunder_orchestrator.Api.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class CertificateList
    {
        public string cert { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }

    public class ClientSslList
    {
        public string name { get; set; }

        [JsonProperty("auth-username")]
        public string authusername { get; set; }

        [JsonProperty("local-logging")]
        public int locallogging { get; set; }

        [JsonProperty("ocsp-stapling")]
        public int ocspstapling { get; set; }

        [JsonProperty("client-certificate")]
        public string clientcertificate { get; set; }

        [JsonProperty("close-notify")]
        public int closenotify { get; set; }

        [JsonProperty("enable-tls-alert-logging")]
        public int enabletlsalertlogging { get; set; }

        [JsonProperty("handshake-logging-enable")]
        public int handshakeloggingenable { get; set; }

        [JsonProperty("session-key-logging-enable")]
        public int sessionkeyloggingenable { get; set; }

        [JsonProperty("server-name-auto-map")]
        public int servernameautomap { get; set; }

        [JsonProperty("sni-bypass-missing-cert")]
        public int snibypassmissingcert { get; set; }

        [JsonProperty("sni-bypass-expired-cert")]
        public int snibypassexpiredcert { get; set; }

        [JsonProperty("sni-bypass-enable-log")]
        public int snibypassenablelog { get; set; }

        [JsonProperty("direct-client-server-auth")]
        public int directclientserverauth { get; set; }

        [JsonProperty("session-cache-size")]
        public int sessioncachesize { get; set; }

        [JsonProperty("session-cache-timeout")]
        public int sessioncachetimeout { get; set; }

        [JsonProperty("session-ticket-disable")]
        public int sessionticketdisable { get; set; }

        [JsonProperty("session-ticket-lifetime")]
        public int sessionticketlifetime { get; set; }

        [JsonProperty("ssl-false-start-disable")]
        public int sslfalsestartdisable { get; set; }

        [JsonProperty("disable-sslv3")]
        public int disablesslv3 { get; set; }
        public int version { get; set; }
        public int dgversion { get; set; }

        [JsonProperty("renegotiation-disable")]
        public int renegotiationdisable { get; set; }
        public int authorization { get; set; }

        [JsonProperty("early-data")]
        public int earlydata { get; set; }

        [JsonProperty("ja3-enable")]
        public int ja3enable { get; set; }

        [JsonProperty("ja4-enable")]
        public int ja4enable { get; set; }
        public string uuid { get; set; }

        [JsonProperty("certificate-list")]
        public List<CertificateList> certificatelist { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }

    public class ClientTemplateListResponse
    {
        [JsonProperty("client-ssl-list")]
        public List<ClientSslList> clientssllist { get; set; }
    }


}
