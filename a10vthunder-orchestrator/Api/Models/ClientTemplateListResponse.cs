using Newtonsoft.Json;
using System.Collections.Generic;

namespace a10vthunder.Api.Models
{
    public class CertificateListItem
    {
        [JsonProperty("cert")]
        public string Cert { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }
    }

    public class ClientSslList
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("auth-username")]
        public string AuthUsername { get; set; }

        [JsonProperty("local-logging")]
        public int LocalLogging { get; set; }

        [JsonProperty("ocsp-stapling")]
        public int OcspStapling { get; set; }

        [JsonProperty("client-certificate")]
        public string ClientCertificate { get; set; }

        [JsonProperty("close-notify")]
        public int CloseNotify { get; set; }

        [JsonProperty("enable-tls-alert-logging")]
        public int EnableTlsAlertLogging { get; set; }

        [JsonProperty("handshake-logging-enable")]
        public int HandshakeLoggingEnable { get; set; }

        [JsonProperty("session-key-logging-enable")]
        public int SessionKeyLoggingEnable { get; set; }

        [JsonProperty("server-name-auto-map")]
        public int ServerNameAutoMap { get; set; }

        [JsonProperty("sni-bypass-missing-cert")]
        public int SniBypassMissingCert { get; set; }

        [JsonProperty("sni-bypass-expired-cert")]
        public int SniBypassExpiredCert { get; set; }

        [JsonProperty("sni-bypass-enable-log")]
        public int SniBypassEnableLog { get; set; }

        [JsonProperty("direct-client-server-auth")]
        public int DirectClientServerAuth { get; set; }

        [JsonProperty("session-cache-size")]
        public int SessionCacheSize { get; set; }

        [JsonProperty("session-cache-timeout")]
        public int SessionCacheTimeout { get; set; }

        [JsonProperty("session-ticket-disable")]
        public int SessionTicketDisable { get; set; }

        [JsonProperty("session-ticket-lifetime")]
        public int SessionTicketLifetime { get; set; }

        [JsonProperty("ssl-false-start-disable")]
        public int SslFalseStartDisable { get; set; }

        [JsonProperty("disable-sslv3")]
        public int DisableSslV3 { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("dgversion")]
        public int DgVersion { get; set; }

        [JsonProperty("renegotiation-disable")]
        public int RenegotiationDisable { get; set; }

        [JsonProperty("authorization")]
        public int Authorization { get; set; }

        [JsonProperty("early-data")]
        public int EarlyData { get; set; }

        [JsonProperty("ja3-enable")]
        public int Ja3Enable { get; set; }

        [JsonProperty("ja4-enable")]
        public int Ja4Enable { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("certificate-list")]
        public List<CertificateListItem> CertificateList { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }
    }

    public class ClientTemplateListResponse
    {
        [JsonProperty("client-ssl-list")]
        public List<ClientSslList> ClientSslList { get; set; }
    }
}
