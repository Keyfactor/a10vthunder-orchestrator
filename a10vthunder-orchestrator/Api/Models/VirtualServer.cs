using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace a10vthunder.Api.Models
{
    public class VirtualServerListResponse
    {
        [JsonProperty("virtual-server-list")]
        public List<VirtualServer> VirtualServerList { get; set; }
    }

    public class VirtualServer
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ip-address")]
        public string IpAddress { get; set; }

        [JsonProperty("enable-disable-action")]
        public string EnableDisableAction { get; set; }

        [JsonProperty("redistribution-flagged")]
        public int? RedistributionFlagged { get; set; }

        [JsonProperty("arp-disable")]
        public int? ArpDisable { get; set; }

        [JsonProperty("stats-data-action")]
        public string StatsDataAction { get; set; }

        [JsonProperty("extended-stats")]
        public int? ExtendedStats { get; set; }

        [JsonProperty("disable-vip-adv")]
        public int? DisableVipAdv { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("port-list")]
        public List<VirtualServerPort> PortList { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }

        // Helper methods
        public List<VirtualServerPort> GetPortsUsingClientTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName) || PortList == null)
                return new List<VirtualServerPort>();

            return PortList.Where(p =>
                string.Equals(p.TemplateClientSsl, templateName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public List<VirtualServerPort> GetPortsUsingServerTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName) || PortList == null)
                return new List<VirtualServerPort>();

            return PortList.Where(p =>
                string.Equals(p.TemplateServerSsl, templateName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public class VirtualServerPort
    {
        [JsonProperty("port-number")]
        public int? PortNumber { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("range")]
        public int? Range { get; set; }

        // v6 specific property
        [JsonProperty("support-http2")]
        public int? SupportHttp2 { get; set; }

        // v4 has this more commonly, v6 may not always have it
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("conn-limit")]
        public int? ConnLimit { get; set; }

        [JsonProperty("reset")]
        public int? Reset { get; set; }

        [JsonProperty("no-logging")]
        public int? NoLogging { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("def-selection-if-pref-failed")]
        public string DefSelectionIfPrefFailed { get; set; }

        [JsonProperty("skip-rev-hash")]
        public int? SkipRevHash { get; set; }

        [JsonProperty("message-switching")]
        public int? MessageSwitching { get; set; }

        [JsonProperty("force-routing-mode")]
        public int? ForceRoutingMode { get; set; }

        [JsonProperty("reset-on-server-selection-fail")]
        public int? ResetOnServerSelectionFail { get; set; }

        [JsonProperty("clientip-sticky-nat")]
        public int? ClientipStickyNat { get; set; }

        [JsonProperty("extended-stats")]
        public int? ExtendedStats { get; set; }

        [JsonProperty("snat-on-vip")]
        public int? SnatOnVip { get; set; }

        [JsonProperty("stats-data-action")]
        public string StatsDataAction { get; set; }

        [JsonProperty("syn-cookie")]
        public int? SynCookie { get; set; }

        // v6 specific properties
        [JsonProperty("showtech-print-extended-stats")]
        public int? ShowtechPrintExtendedStats { get; set; }

        [JsonProperty("attack-detection")]
        public int? AttackDetection { get; set; }

        [JsonProperty("no-auto-up-on-aflex")]
        public int? NoAutoUpOnAflex { get; set; }

        // v4 specific property
        [JsonProperty("scaleout-bucket-count")]
        public int? ScaleoutBucketCount { get; set; }

        [JsonProperty("auto")]
        public int? Auto { get; set; }

        // v6 specific property
        [JsonProperty("use-cgnv6")]
        public int? UseCgnv6 { get; set; }

        [JsonProperty("ipinip")]
        public int? Ipinip { get; set; }

        [JsonProperty("rtp-sip-call-id-match")]
        public int? RtpSipCallIdMatch { get; set; }

        [JsonProperty("use-rcv-hop-for-resp")]
        public int? UseRcvHopForResp { get; set; }

        [JsonProperty("use-rcv-hop-group")]
        public int? UseRcvHopGroup { get; set; }

        // v6 specific property
        [JsonProperty("redirect-to-https")]
        public int? RedirectToHttps { get; set; }

        [JsonProperty("template-server-ssl")]
        public string TemplateServerSsl { get; set; }

        [JsonProperty("template-client-ssl")]
        public string TemplateClientSsl { get; set; }

        [JsonProperty("template-virtual-port")]
        public string TemplateVirtualPort { get; set; }

        [JsonProperty("use-default-if-no-server")]
        public int? UseDefaultIfNoServer { get; set; }

        [JsonProperty("no-dest-nat")]
        public int? NoDestNat { get; set; }

        [JsonProperty("cpu-compute")]
        public int? CpuCompute { get; set; }

        [JsonProperty("memory-compute")]
        public int? MemoryCompute { get; set; }

        [JsonProperty("substitute-source-mac")]
        public int? SubstituteSourceMac { get; set; }

        // v6 specific properties
        [JsonProperty("aflex-table-entry-syn-disable")]
        public int? AflexTableEntrySynDisable { get; set; }

        [JsonProperty("gtp-session-lb")]
        public int? GtpSessionLb { get; set; }

        [JsonProperty("reply-acme-challenge")]
        public int? ReplyAcmeChallenge { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }

        [JsonProperty("service-group")]
        public string ServiceGroup { get; set; }

        // Helper methods
        public bool UsesClientTemplate(string templateName)
        {
            return !string.IsNullOrEmpty(TemplateClientSsl) &&
                   string.Equals(TemplateClientSsl, templateName, StringComparison.OrdinalIgnoreCase);
        }

        public bool UsesServerTemplate(string templateName)
        {
            return !string.IsNullOrEmpty(TemplateServerSsl) &&
                   string.Equals(TemplateServerSsl, templateName, StringComparison.OrdinalIgnoreCase);
        }

        public string GetPortIdentifier()
        {
            return $"{PortNumber}+{Protocol}";
        }

        // Check if this is v4 format
        public bool IsV4Format()
        {
            return ScaleoutBucketCount.HasValue ||
                   (!SupportHttp2.HasValue && !ShowtechPrintExtendedStats.HasValue);
        }

        // Check if this is v6 format
        public bool IsV6Format()
        {
            return SupportHttp2.HasValue || ShowtechPrintExtendedStats.HasValue ||
                   AttackDetection.HasValue || UseCgnv6.HasValue || RedirectToHttps.HasValue ||
                   AflexTableEntrySynDisable.HasValue || GtpSessionLb.HasValue;
        }
    }

    public class VirtualServerPortUpdateRequest
    {
        [JsonProperty("port")]
        public VirtualServerPortUpdate Port { get; set; }
    }

    public class VirtualServerPortUpdate
    {
        [JsonProperty("port-number")]
        public int? PortNumber { get; set; }

        [JsonProperty("protocol")]
        public string Protocol { get; set; }

        [JsonProperty("template-server-ssl")]
        public string TemplateServerSsl { get; set; }

        [JsonProperty("template-client-ssl")]
        public string TemplateClientSsl { get; set; }
    }

    // Management certificate operations classes
    public class ManagementCertRequest
    {
        [JsonProperty("certificate")]
        public ManagementCertificate Certificate { get; set; }
    }

    public class ManagementCertificate
    {
        [JsonProperty("load")]
        public int? Load { get; set; }

        [JsonProperty("file-url")]
        public string FileUrl { get; set; }
    }

    public class ManagementPrivateKeyRequest
    {
        [JsonProperty("private-key")]
        public PrivateKey PrivateKey { get; set; }
    }

    public class PrivateKey
    {
        [JsonProperty("load")]
        public int? Load { get; set; }

        [JsonProperty("file-url")]
        public string FileUrl { get; set; }
    }

    public class ManagementCertRestartRequest
    {
        [JsonProperty("secure")]
        public Secure Secure { get; set; }
    }

    public class Secure
    {
        [JsonProperty("restart")]
        public int? Restart { get; set; }
    }
}