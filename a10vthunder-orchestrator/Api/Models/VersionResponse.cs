using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class VersionResponse
    {
        [JsonProperty("version")]
        public VersionContainer Version { get; set; }
    }

    public class VersionContainer
    {
        [JsonProperty("oper")]
        public VersionOperational Oper { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }
    }

    public class VersionOperational
    {
        [JsonProperty("hw-platform")]
        public string HwPlatform { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("sw-version")]
        public string SwVersion { get; set; }

        [JsonProperty("plat-features")]
        public string PlatFeatures { get; set; }

        [JsonProperty("boot-from")]
        public string BootFrom { get; set; }

        [JsonProperty("serial-number")]
        public string SerialNumber { get; set; }

        [JsonProperty("aflex-version")]
        public string AflexVersion { get; set; }

        [JsonProperty("axapi-version")]
        public string AxapiVersion { get; set; }

        [JsonProperty("pri-gui-version")]
        public string PriGuiVersion { get; set; }

        [JsonProperty("sec-gui-version")]
        public string SecGuiVersion { get; set; }

        [JsonProperty("cylance-version")]
        public string CylanceVersion { get; set; }

        [JsonProperty("hd-pri")]
        public string HdPri { get; set; }

        [JsonProperty("hd-sec")]
        public string HdSec { get; set; }

        [JsonProperty("last-config-saved-time")]
        public string LastConfigSavedTime { get; set; }

        [JsonProperty("virtualization-type")]
        public string VirtualizationType { get; set; }

        [JsonProperty("sys-poll-mode")]
        public string SysPollMode { get; set; }

        [JsonProperty("product")]
        public string Product { get; set; }

        [JsonProperty("hw-code")]
        public string HwCode { get; set; }

        [JsonProperty("current-time")]
        public string CurrentTime { get; set; }

        [JsonProperty("up-time")]
        public string UpTime { get; set; }

        [JsonProperty("nun-ctrl-cpus")]
        public int NunCtrlCpus { get; set; }

        [JsonProperty("buff-size")]
        public int BuffSize { get; set; }

        [JsonProperty("io-buff-enabled")]
        public string IoBuffEnabled { get; set; }

        [JsonProperty("build-type")]
        public string BuildType { get; set; }

        [JsonProperty("cots-sys-mfg")]
        public string CotsSysMfg { get; set; }

        [JsonProperty("cots-sys-name")]
        public string CotsSysName { get; set; }

        [JsonProperty("cots-sys-ver")]
        public string CotsSysVer { get; set; }

        [JsonProperty("series-name")]
        public string SeriesName { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }
    }
}