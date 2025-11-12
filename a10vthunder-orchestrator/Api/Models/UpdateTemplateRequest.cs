// Copyright 2025 Keyfactor
// Licensed under the Apache License, Version 2.0

using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class UpdateTemplateCertificate
    {
        [JsonProperty("cert")]
        public string Cert { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

    public class UpdateTemplateRequest
    {
        [JsonProperty("certificate")]
        public UpdateTemplateCertificate Certificate { get; set; }
    }
}
