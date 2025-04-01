﻿using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class DeleteCertRequest
    {
        [JsonProperty("cert-name")]
        public string CertName { get; set; }

        [JsonProperty("private-key",NullValueHandling=NullValueHandling.Ignore)]
        public string PrivateKey { get; set; }
    }
}
