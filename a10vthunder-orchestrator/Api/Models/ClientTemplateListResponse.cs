using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public class WebCategory
    {
        [JsonProperty("uncategorized")]
        public int? Uncategorized { get; set; }

        [JsonProperty("real-estate")]
        public int? RealEstate { get; set; }

        [JsonProperty("computer-and-internet-security")]
        public int? ComputerAndInternetSecurity { get; set; }

        [JsonProperty("financial-services")]
        public int? FinancialServices { get; set; }

        [JsonProperty("business-and-economy")]
        public int? BusinessAndEconomy { get; set; }

        [JsonProperty("computer-and-internet-info")]
        public int? ComputerAndInternetInfo { get; set; }

        [JsonProperty("auctions")]
        public int? Auctions { get; set; }

        [JsonProperty("shopping")]
        public int? Shopping { get; set; }

        [JsonProperty("cult-and-occult")]
        public int? CultAndOccult { get; set; }

        [JsonProperty("travel")]
        public int? Travel { get; set; }

        [JsonProperty("drugs")]
        public int? Drugs { get; set; }

        [JsonProperty("adult-and-pornography")]
        public int? AdultAndPornography { get; set; }

        [JsonProperty("home-and-garden")]
        public int? HomeAndGarden { get; set; }

        [JsonProperty("military")]
        public int? Military { get; set; }

        [JsonProperty("social-network")]
        public int? SocialNetwork { get; set; }

        [JsonProperty("dead-sites")]
        public int? DeadSites { get; set; }

        [JsonProperty("stock-advice-and-tools")]
        public int? StockAdviceAndTools { get; set; }

        [JsonProperty("training-and-tools")]
        public int? TrainingAndTools { get; set; }

        [JsonProperty("dating")]
        public int? Dating { get; set; }

        [JsonProperty("sex-education")]
        public int? SexEducation { get; set; }

        [JsonProperty("religion")]
        public int? Religion { get; set; }

        [JsonProperty("entertainment-and-arts")]
        public int? EntertainmentAndArts { get; set; }

        [JsonProperty("personal-sites-and-blogs")]
        public int? PersonalSitesAndBlogs { get; set; }

        [JsonProperty("legal")]
        public int? Legal { get; set; }

        [JsonProperty("local-information")]
        public int? LocalInformation { get; set; }

        [JsonProperty("streaming-media")]
        public int? StreamingMedia { get; set; }

        [JsonProperty("job-search")]
        public int? JobSearch { get; set; }

        [JsonProperty("gambling")]
        public int? Gambling { get; set; }

        [JsonProperty("translation")]
        public int? Translation { get; set; }

        [JsonProperty("reference-and-research")]
        public int? ReferenceAndResearch { get; set; }

        [JsonProperty("shareware-and-freeware")]
        public int? SharewareAndFreeware { get; set; }

        [JsonProperty("peer-to-peer")]
        public int? PeerToPeer { get; set; }

        [JsonProperty("marijuana")]
        public int? Marijuana { get; set; }

        [JsonProperty("hacking")]
        public int? Hacking { get; set; }

        [JsonProperty("games")]
        public int? Games { get; set; }

        [JsonProperty("philosophy-and-politics")]
        public int? PhilosophyAndPolitics { get; set; }

        [JsonProperty("weapons")]
        public int? Weapons { get; set; }

        [JsonProperty("pay-to-surf")]
        public int? PayToSurf { get; set; }

        [JsonProperty("hunting-and-fishing")]
        public int? HuntingAndFishing { get; set; }

        [JsonProperty("society")]
        public int? Society { get; set; }

        [JsonProperty("educational-institutions")]
        public int? EducationalInstitutions { get; set; }

        [JsonProperty("online-greeting-cards")]
        public int? OnlineGreetingCards { get; set; }

        [JsonProperty("sports")]
        public int? Sports { get; set; }

        [JsonProperty("swimsuits-and-intimate-apparel")]
        public int? SwimsuitsAndIntimateApparel { get; set; }

        [JsonProperty("questionable")]
        public int? Questionable { get; set; }

        [JsonProperty("kids")]
        public int? Kids { get; set; }

        [JsonProperty("hate-and-racism")]
        public int? HateAndRacism { get; set; }

        [JsonProperty("personal-storage")]
        public int? PersonalStorage { get; set; }

        [JsonProperty("violence")]
        public int? Violence { get; set; }

        [JsonProperty("keyloggers-and-monitoring")]
        public int? KeyloggersAndMonitoring { get; set; }

        [JsonProperty("search-engines")]
        public int? SearchEngines { get; set; }

        [JsonProperty("internet-portals")]
        public int? InternetPortals { get; set; }

        [JsonProperty("web-advertisements")]
        public int? WebAdvertisements { get; set; }

        [JsonProperty("cheating")]
        public int? Cheating { get; set; }

        [JsonProperty("gross")]
        public int? Gross { get; set; }

        [JsonProperty("web-based-email")]
        public int? WebBasedEmail { get; set; }

        [JsonProperty("malware-sites")]
        public int? MalwareSites { get; set; }

        [JsonProperty("phishing-and-other-fraud")]
        public int? PhishingAndOtherFraud { get; set; }

        [JsonProperty("proxy-avoid-and-anonymizers")]
        public int? ProxyAvoidAndAnonymizers { get; set; }

        [JsonProperty("spyware-and-adware")]
        public int? SpywareAndAdware { get; set; }

        [JsonProperty("music")]
        public int? Music { get; set; }

        [JsonProperty("government")]
        public int? Government { get; set; }

        [JsonProperty("nudity")]
        public int? Nudity { get; set; }

        [JsonProperty("news-and-media")]
        public int? NewsAndMedia { get; set; }

        [JsonProperty("illegal")]
        public int? Illegal { get; set; }

        [JsonProperty("cdns")]
        public int? Cdns { get; set; }

        [JsonProperty("internet-communications")]
        public int? InternetCommunications { get; set; }

        [JsonProperty("bot-nets")]
        public int? BotNets { get; set; }

        [JsonProperty("abortion")]
        public int? Abortion { get; set; }

        [JsonProperty("health-and-medicine")]
        public int? HealthAndMedicine { get; set; }

        [JsonProperty("confirmed-spam-sources")]
        public int? ConfirmedSpamSources { get; set; }

        [JsonProperty("spam-urls")]
        public int? SpamUrls { get; set; }

        [JsonProperty("unconfirmed-spam-sources")]
        public int? UnconfirmedSpamSources { get; set; }

        [JsonProperty("open-http-proxies")]
        public int? OpenHttpProxies { get; set; }

        [JsonProperty("dynamic-comment")]
        public int? DynamicComment { get; set; }

        [JsonProperty("parked-domains")]
        public int? ParkedDomains { get; set; }

        [JsonProperty("alcohol-and-tobacco")]
        public int? AlcoholAndTobacco { get; set; }

        [JsonProperty("private-ip-addresses")]
        public int? PrivateIpAddresses { get; set; }

        [JsonProperty("image-and-video-search")]
        public int? ImageAndVideoSearch { get; set; }

        [JsonProperty("fashion-and-beauty")]
        public int? FashionAndBeauty { get; set; }

        [JsonProperty("recreation-and-hobbies")]
        public int? RecreationAndHobbies { get; set; }

        [JsonProperty("motor-vehicles")]
        public int? MotorVehicles { get; set; }

        [JsonProperty("web-hosting-sites")]
        public int? WebHostingSites { get; set; }

        [JsonProperty("food-and-dining")]
        public int? FoodAndDining { get; set; }
    }

    public class WebReputation
    {
        [JsonProperty("bypass-trustworthy")]
        public int? BypassTrustworthy { get; set; }
    }

    public class ExceptionWebReputation
    {
        [JsonProperty("exception-trustworthy")]
        public int? ExceptionTrustworthy { get; set; }
    }

    public class ClientSslList
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("auth-username")]
        public string AuthUsername { get; set; }

        // v4 direct properties (will be null in v6)
        [JsonProperty("cert")]
        public string Cert { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("local-logging")]
        public int? LocalLogging { get; set; }

        [JsonProperty("ocsp-stapling")]
        public int? OcspStapling { get; set; }

        // v6 specific properties (will be null in v4)
        [JsonProperty("require-sni-cert-matched")]
        public int? RequireSniCertMatched { get; set; }

        [JsonProperty("ssli-inbound-enable")]
        public int? SsliInboundEnable { get; set; }

        [JsonProperty("ssli-logging")]
        public int? SsliLogging { get; set; }

        [JsonProperty("client-certificate")]
        public string ClientCertificate { get; set; }

        [JsonProperty("close-notify")]
        public int? CloseNotify { get; set; }

        [JsonProperty("forward-proxy-alt-sign")]
        public int? ForwardProxyAltSign { get; set; }

        [JsonProperty("enable-tls-alert-logging")]
        public int? EnableTlsAlertLogging { get; set; }

        [JsonProperty("forward-proxy-verify-cert-fail-action")]
        public int? ForwardProxyVerifyCertFailAction { get; set; }

        [JsonProperty("verify-cert-fail-action")]
        public string VerifyCertFailAction { get; set; }

        [JsonProperty("forward-proxy-cert-revoke-action")]
        public int? ForwardProxyCertRevokeAction { get; set; }

        [JsonProperty("cert-revoke-action")]
        public string CertRevokeAction { get; set; }

        [JsonProperty("forward-proxy-no-shared-cipher-action")]
        public int? ForwardProxyNoSharedCipherAction { get; set; }

        [JsonProperty("no-shared-cipher-action")]
        public string NoSharedCipherAction { get; set; }

        // v6 specific
        [JsonProperty("forward-proxy-esni-action")]
        public int? ForwardProxyEsniAction { get; set; }

        [JsonProperty("forward-proxy-cert-unknown-action")]
        public int? ForwardProxyCertUnknownAction { get; set; }

        [JsonProperty("cert-unknown-action")]
        public string CertUnknownAction { get; set; }

        [JsonProperty("notbefore")]
        public int? NotBefore { get; set; }

        [JsonProperty("notafter")]
        public int? NotAfter { get; set; }

        [JsonProperty("forward-proxy-ssl-version")]
        public int? ForwardProxySslVersion { get; set; }

        [JsonProperty("forward-proxy-ocsp-disable")]
        public int? ForwardProxyOcspDisable { get; set; }

        [JsonProperty("forward-proxy-crl-disable")]
        public int? ForwardProxyCrlDisable { get; set; }

        [JsonProperty("forward-proxy-cert-cache-timeout")]
        public int? ForwardProxyCertCacheTimeout { get; set; }

        [JsonProperty("forward-proxy-cert-cache-limit")]
        public int? ForwardProxyCertCacheLimit { get; set; }

        [JsonProperty("forward-proxy-cert-expiry")]
        public int? ForwardProxyCertExpiry { get; set; }

        // v6 specific - missing from v4
        [JsonProperty("forward-proxy-enable")]
        public int? ForwardProxyEnable { get; set; }

        [JsonProperty("handshake-logging-enable")]
        public int? HandshakeLoggingEnable { get; set; }

        [JsonProperty("session-key-logging-enable")]
        public int? SessionKeyLoggingEnable { get; set; }

        [JsonProperty("forward-proxy-selfsign-redir")]
        public int? ForwardProxySelfsignRedir { get; set; }

        [JsonProperty("forward-proxy-failsafe-disable")]
        public int? ForwardProxyFailsafeDisable { get; set; }

        [JsonProperty("forward-proxy-log-disable")]
        public int? ForwardProxyLogDisable { get; set; }

        [JsonProperty("forward-proxy-no-sni-action")]
        public string ForwardProxyNoSniAction { get; set; }

        [JsonProperty("case-insensitive")]
        public int? CaseInsensitive { get; set; }

        [JsonProperty("client-auth-case-insensitive")]
        public int? ClientAuthCaseInsensitive { get; set; }

        [JsonProperty("forward-proxy-cert-not-ready-action")]
        public string ForwardProxyCertNotReadyAction { get; set; }

        // v4 has web-category, v6 has web-reputation objects
        [JsonProperty("web-category")]
        public WebCategory WebCategory { get; set; }

        [JsonProperty("web-reputation")]
        public WebReputation WebReputation { get; set; }

        [JsonProperty("exception-web-reputation")]
        public ExceptionWebReputation ExceptionWebReputation { get; set; }

        [JsonProperty("require-web-category")]
        public int? RequireWebCategory { get; set; }

        // v6 specific
        [JsonProperty("central-cert-pin-list")]
        public int? CentralCertPinList { get; set; }

        [JsonProperty("server-name-auto-map")]
        public int? ServerNameAutoMap { get; set; }

        [JsonProperty("sni-bypass-missing-cert")]
        public int? SniBypassMissingCert { get; set; }

        [JsonProperty("sni-bypass-expired-cert")]
        public int? SniBypassExpiredCert { get; set; }

        [JsonProperty("sni-bypass-enable-log")]
        public int? SniBypassEnableLog { get; set; }

        [JsonProperty("direct-client-server-auth")]
        public int? DirectClientServerAuth { get; set; }

        [JsonProperty("session-cache-size")]
        public int? SessionCacheSize { get; set; }

        [JsonProperty("session-cache-timeout")]
        public int? SessionCacheTimeout { get; set; }

        [JsonProperty("session-ticket-disable")]
        public int? SessionTicketDisable { get; set; }

        [JsonProperty("session-ticket-lifetime")]
        public int? SessionTicketLifetime { get; set; }

        [JsonProperty("ssl-false-start-disable")]
        public int? SslFalseStartDisable { get; set; }

        [JsonProperty("disable-sslv3")]
        public int? DisableSslV3 { get; set; }

        [JsonProperty("version")]
        public int? Version { get; set; }

        [JsonProperty("dgversion")]
        public int? DgVersion { get; set; }

        [JsonProperty("renegotiation-disable")]
        public int? RenegotiationDisable { get; set; }

        [JsonProperty("authorization")]
        public int? Authorization { get; set; }

        // v6 specific
        [JsonProperty("early-data")]
        public int? EarlyData { get; set; }

        [JsonProperty("ja3-enable")]
        public int? Ja3Enable { get; set; }

        [JsonProperty("ja4-enable")]
        public int? Ja4Enable { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        // v6 has certificate-list, v4 has direct cert/key
        [JsonProperty("certificate-list")]
        public List<CertificateListItem> CertificateList { get; set; }

        [JsonProperty("a10-url")]
        public string A10Url { get; set; }

        // Helper methods to work with both formats
        public string GetCertificate()
        {
            // v4 format - direct cert property
            if (!string.IsNullOrEmpty(Cert))
                return Cert;

            // v6 format - certificate-list
            if (CertificateList?.Count > 0)
                return CertificateList[0].Cert;

            return null;
        }

        public string GetKey()
        {
            // v4 format - direct key property
            if (!string.IsNullOrEmpty(Key))
                return Key;

            // v6 format - certificate-list
            if (CertificateList?.Count > 0)
                return CertificateList[0].Key;

            return null;
        }

        public List<CertificateListItem> GetAllCertificates()
        {
            // v6 format - return certificate list directly
            if (CertificateList?.Count > 0)
                return CertificateList;

            // v4 format - create a certificate list item from direct properties
            if (!string.IsNullOrEmpty(Cert) || !string.IsNullOrEmpty(Key))
            {
                return new List<CertificateListItem>
                {
                    new CertificateListItem
                    {
                        Cert = Cert,
                        Key = Key,
                        Uuid = Uuid,
                        A10Url = A10Url
                    }
                };
            }

            return new List<CertificateListItem>();
        }

        // Helper method to check if this template uses a specific certificate
        public bool UsesCertificate(string certName)
        {
            if (string.IsNullOrEmpty(certName))
                return false;

            // v4 format - check direct cert property
            if (!string.IsNullOrEmpty(Cert) &&
                string.Equals(Cert, certName, StringComparison.OrdinalIgnoreCase))
                return true;

            // v6 format - check certificate-list
            if (CertificateList?.Any(c =>
                string.Equals(c.Cert, certName, StringComparison.OrdinalIgnoreCase)) == true)
                return true;

            return false;
        }
    }

    public class ClientTemplateListResponse
    {
        [JsonProperty("client-ssl-list")]
        public List<ClientSslList> ClientSslList { get; set; }
    }
}