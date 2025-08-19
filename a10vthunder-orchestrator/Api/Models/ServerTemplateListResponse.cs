// Copyright 2025 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Keyfactor.PKI.X509;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace a10vthunder.Api.Models
{
    public class TemplateCertificate
    {
        public string cert { get; set; }
        public string key { get; set; }
        public string uuid { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }
    }

    public class ServerTemplateListResponse
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

        [JsonProperty("forward-proxy-enable")]
        public int? forwardproxyenable { get; set; }  // optional

        [JsonProperty("session-ticket-enable")]
        public int sessionticketenable { get; set; }

        public int version { get; set; }
        public int dgversion { get; set; }

        [JsonProperty("ssli-logging")]
        public int? ssliLogging { get; set; }  // optional

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

        [JsonProperty("certificate")]
        [JsonConverter(typeof(CertificateConverter))]
        public TemplateCertificate certificate { get; set; }

        [JsonProperty("cert")]
        public string certFlat { get; set; }

        [JsonProperty("key")]
        public string keyFlat { get; set; }

        [JsonProperty("a10-url")]
        public string a10url { get; set; }

        [JsonIgnore]
        public TemplateCertificate MergedCertificate => certificate ?? (
            certFlat != null && keyFlat != null
            ? new TemplateCertificate { cert = certFlat, key = keyFlat, uuid = uuid, a10url = a10url + "/certificate" }
            : null
        );
    }

    public class CertificateConverter : JsonConverter<TemplateCertificate>
    {
        public override TemplateCertificate ReadJson(JsonReader reader, Type objectType, TemplateCertificate existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            // Case 1: certificate is a full object
            if (obj["cert"] != null && obj["key"] != null)
            {
                return obj.ToObject<TemplateCertificate>();
            }

            // If the certificate node is not an object (e.g., null or primitive), fallback
            return null;
        }

        public override void WriteJson(JsonWriter writer, TemplateCertificate value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

}
