using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Keyfactor.AnyAgent.vThunder.api;
using Keyfactor.AnyAgent.vThunder.Exceptions;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;
using Keyfactor.Platform.Extensions.Agents.Enums;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace Keyfactor.AnyAgent.vThunder.Jobs
{
    [Job(JobTypes.Management)]
    public class Management : AgentJob
    {
        protected internal static Func<string, string> Pemify = ss =>
            ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + Pemify(ss.Substring(64));

        protected internal virtual string Protocol { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual CertManager CertManager { get; set; }
        protected internal virtual Configuration AppConfig { get; set; }
        protected internal virtual InventoryResult InventoryResult { get; set; }
        protected internal virtual bool ExistingCert { get; set; }
        protected internal virtual string CertStart { get; set; } = "-----BEGIN CERTIFICATE-----\n";
        protected internal virtual string CertEnd { get; set; } = "\n-----END CERTIFICATE-----";
        protected internal virtual string Alias { get; set; }

        public override AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory,
            SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr)
        {
            CertManager = new CertManager();
            AppConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            Protocol = AppConfig.AppSettings.Settings["Protocol"].Value;
            Logger.Trace($"Ending Management Constructor Protocol is {Protocol}");

            using (ApiClient = new ApiClient(config.Server.Username, config.Server.Password,
                $"{Protocol}://{config.Store.ClientMachine.Trim()}"))
            {
                if (string.IsNullOrEmpty(config.Job.Alias))
                    return ThrowError(new ArgumentException("Missing Alias/Overwrite, Operation Cannot Be Completed"),
                        "Management Add/Replace");

                ApiClient.Logon();
                InventoryResult = CertManager.GetCert(ApiClient, config.Job.Alias);
                ExistingCert = InventoryResult != null && InventoryResult?.InventoryList?.Count == 1;

                switch (config.Job.OperationType)
                {
                    case AnyJobOperationType.Add:
                        try
                        {
                            if (ExistingCert)
                            {
                                Logger.Trace($"Starting Replace Job for {config.Job.Alias}");
                                Replace(config, InventoryResult, ApiClient);
                                Logger.Trace($"Finishing Replace Job for {config.Job.Alias}");
                            }
                            else
                            {
                                Logger.Trace($"Starting Add Job for {config.Job.Alias}");
                                Add(config, ApiClient);
                                Logger.Trace($"Finishing Add Job for {config.Job.Alias}");
                            }
                        }
                        catch (Exception e)
                        {
                            return ThrowError(e, "Error Adding Certificate");
                        }

                        break;
                    case AnyJobOperationType.Remove:
                        try
                        {
                            Logger.Trace($"Starting Remove Job for {config.Job.Alias}");
                            Remove(config, InventoryResult, ApiClient);
                            Logger.Trace($"Finishing Remove Job for {config.Job.Alias}");
                        }
                        catch (Exception e)
                        {
                            return ThrowError(e, "Error Removing Certificate");
                        }

                        break;
                    default:
                        return ThrowError(new UnSupportedOperationException(), "Management");
                }

                Logger.Trace($"Finishing Process Job for {config.Job.Alias}");
                return Success("Management Job Completed");
            }
        }

        protected internal virtual void Replace(AnyJobConfigInfo config, InventoryResult inventoryResult,
            ApiClient apiClient)
        {
            Remove(config, inventoryResult, apiClient);
            Add(config, apiClient);
        }

        protected internal virtual void Remove(AnyJobConfigInfo configInfo, InventoryResult inventoryResult,
            ApiClient apiClient)
        {
            Logger.Trace($"Start Delete the {configInfo.Job.Alias} Private Key");
            DeleteCertBaseRequest deleteKeyRoot;
            if (inventoryResult.InventoryList[0].PrivateKeyEntry)
                deleteKeyRoot = new DeleteCertBaseRequest
                {
                    DeleteCert = new DeleteCertRequest
                    {
                        CertName = configInfo.Job.Alias,
                        PrivateKey = configInfo.Job.Alias
                    }
                };
            else
                deleteKeyRoot = new DeleteCertBaseRequest
                {
                    DeleteCert = new DeleteCertRequest
                    {
                        CertName = configInfo.Job.Alias
                    }
                };

            apiClient.RemoveCertificate(deleteKeyRoot);
            Logger.Trace($"Successful Delete of the {configInfo.Job.Alias} Private Key");
        }

        protected internal virtual void Add(AnyJobConfigInfo configInfo, ApiClient apiClient)
        {
            Logger.Trace($"Entering Add Function for {configInfo.Job.Alias}");
            var privateKeyString = "";
            string certPem;

            if (!string.IsNullOrEmpty(configInfo.Job.PfxPassword))
            {
                Logger.Trace($"Pfx Password exists getting Private Key string for {configInfo.Job.Alias}");
                var certData = Convert.FromBase64String(configInfo.Job.EntryContents);
                var store = new Pkcs12Store(new MemoryStream(certData),
                    configInfo.Job.PfxPassword.ToCharArray());

                using (var memoryStream = new MemoryStream())
                {
                    using (TextWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        var pemWriter = new PemWriter(streamWriter);
                        Logger.Trace($"Getting Public Key for {configInfo.Job.Alias}");
                        Alias = store.Aliases.Cast<string>().SingleOrDefault(a => store.IsKeyEntry(a));
                        var publicKey = store.GetCertificate(Alias).Certificate.GetPublicKey();
                        Logger.Trace($"Getting Private Key for {configInfo.Job.Alias}");
                        var privateKey = store.GetKey(Alias).Key;
                        var keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);
                        Logger.Trace($"Writing Private Key for {configInfo.Job.Alias}");
                        pemWriter.WriteObject(keyPair.Private);
                        streamWriter.Flush();
                        privateKeyString = Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim()
                            .Replace("\r", "").Replace("\0", "");
                        memoryStream.Close();
                        streamWriter.Close();
                        Logger.Trace($"Private Key String Retrieved for {configInfo.Job.Alias}");
                    }
                }

                // Extract server certificate
                var beginCertificate = "-----BEGIN CERTIFICATE-----\n";
                var endCertificate = "\n-----END CERTIFICATE-----";

                Logger.Trace($"Start getting Server Certificate for {configInfo.Job.Alias}");
                certPem = beginCertificate +
                          Pemify(Convert.ToBase64String(store.GetCertificate(Alias).Certificate.GetEncoded())) +
                          endCertificate;
                Logger.Trace($"Finished getting Server Certificate for {configInfo.Job.Alias}");
            }
            else
            {
                Logger.Trace($"No Private Key get Cert Pem {configInfo.Job.Alias}");
                certPem = CertStart + Pemify(configInfo.Job.EntryContents) + CertEnd;
            }

            Logger.Trace($"Creating Cert API Add Request for {configInfo.Job.Alias}");
            var sslCertRequest = new SslCertificateRequest
            {
                SslCertificate = new SslCert
                {
                    Action = "import",
                    CertificateType = "pem",
                    File = configInfo.Job.Alias.Replace(".pem", ".pem"),
                    FileHandle = configInfo.Job.Alias.Replace(".pem", ".pem")
                }
            };

            Logger.Trace($"Making API Call to Add Certificate For {configInfo.Job.Alias}");
            apiClient.AddCertificate(sslCertRequest, certPem);
            Logger.Trace($"Finished API Call to Add Certificate For {configInfo.Job.Alias}");

            if (!string.IsNullOrEmpty(configInfo.Job.PfxPassword))
            {
                Logger.Trace($"Creating Key API Add Request for {configInfo.Job.Alias}");
                var sslKeyRequest = new SslKeyRequest
                {
                    SslKey = new SslCertKey
                    {
                        Action = "import",
                        File = configInfo.Job.Alias.Replace(".pem", ".pem"),
                        FileHandle = configInfo.Job.Alias.Replace(".pem", ".pem")
                    }
                };

                Logger.Trace($"Making Add Key API Call for {configInfo.Job.Alias}");
                apiClient.AddPrivateKey(sslKeyRequest, privateKeyString);
                Logger.Trace($"Finished Add Key API Call for {configInfo.Job.Alias}");
            }

            Logger.Trace($"Starting Log Off for Add {configInfo.Job.Alias}");
            Logger.Trace($"Finished Log Off for Add {configInfo.Job.Alias}");
        }
    }
}