// Copyright 2025 Keyfactor
// SPDX-License-Identifier: Apache-2.0

using Newtonsoft.Json;
using System.Collections.Generic;

namespace a10vthunder.Api.Models
{
    public class ServerCertificate
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

    public class ServerSslList
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        // v4 direct properties (will be null in v6)
        [JsonProperty("cert")]
        public string Cert { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("enable-tls-alert-logging")]
        public int? EnableTlsAlertLogging { get; set; }

        [JsonProperty("handshake-logging-enable")]
        public int? HandshakeLoggingEnable { get; set; }

        // v6 specific property (will be null in v4)
        [JsonProperty("session-key-logging-enable")]
        public int? SessionKeyLoggingEnable { get; set; }

        [JsonProperty("close-notify")]
        public int? CloseNotify { get; set; }

        [JsonProperty("forward-proxy-enable")]
        public int? ForwardProxyEnable { get; set; }

        [JsonProperty("session-ticket-enable")]
        public int? SessionTicketEnable { get; set; }

        [JsonProperty("version")]
        public int? Version { get; set; }

        [JsonProperty("dgversion")]
        public int? DgVersion { get; set; }

        [JsonProperty("ssli-logging")]
        public int? SsliLogging { get; set; }

        // v4 specific property (will be null in v6)
        [JsonProperty("dh-short-key-action")]
        public string DhShortKeyAction { get; set; }

        [JsonProperty("ocsp-stapling")]
        public int? OcspStapling { get; set; }

        [JsonProperty("use-client-sni")]
        public int? UseClientSni { get; set; }

        [JsonProperty("renegotiation-disable")]
        public int? RenegotiationDisable { get; set; }

        [JsonProperty("session-cache-size")]
        public int? SessionCacheSize { get; set; }

        // v6 specific property (will be null in v4)
        [JsonProperty("early-data")]
        public int? EarlyData { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        // v6 has certificate object, v4 has direct cert/key
        [JsonProperty("certificate")]
        public ServerCertificate Certificate { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }

        // Helper methods to work with both formats (consistent with client model)
        public string GetCertificate()
        {
            // v4 format - direct cert property
            if (!string.IsNullOrEmpty(Cert))
                return Cert;

            // v6 format - certificate object
            if (Certificate != null && !string.IsNullOrEmpty(Certificate.Cert))
                return Certificate.Cert;

            return null;
        }

        public string GetKey()
        {
            // v4 format - direct key property
            if (!string.IsNullOrEmpty(Key))
                return Key;

            // v6 format - certificate object
            if (Certificate != null && !string.IsNullOrEmpty(Certificate.Key))
                return Certificate.Key;

            return null;
        }

        public ServerCertificate GetCertificateObject()
        {
            // v6 format - return certificate object directly
            if (Certificate != null)
                return Certificate;

            // v4 format - create certificate object from direct properties
            if (!string.IsNullOrEmpty(Cert) || !string.IsNullOrEmpty(Key))
            {
                return new ServerCertificate
                {
                    Cert = Cert,
                    Key = Key,
                    Uuid = Uuid,
                    A10Url = A10Url?.EndsWith("/certificate") == true
                        ? A10Url
                        : A10Url + "/certificate"
                };
            }

            return null;
        }

        // Check if this is v4 format
        public bool IsV4Format()
        {
            return !string.IsNullOrEmpty(Cert) || !string.IsNullOrEmpty(Key) || !string.IsNullOrEmpty(DhShortKeyAction);
        }

        // Check if this is v6 format  
        public bool IsV6Format()
        {
            return Certificate != null || SessionKeyLoggingEnable.HasValue || EarlyData.HasValue;
        }
    }

    public class ServerTemplateListResponse
    {
        [JsonProperty("server-ssl-list")]
        public List<ServerSslList> ServerSslList { get; set; }
    }
}