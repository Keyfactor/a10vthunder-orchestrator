using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Keyfactor.Extensions.Orchestrator.vThunder.api;
using Keyfactor.Extensions.Orchestrator.vThunder.Exceptions;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;

using Microsoft.Extensions.Logging;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace Keyfactor.Extensions.Orchestrator.vThunder
{
    public class Management : IManagementJobExtension
    {
        public string ExtensionName => "";

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

        public override JobResult processJob(ManagementJobConfiguration config)
        {
            ILogger logger = LogHandler.GetClassLogger<Management>();

            CertManager = new CertManager();
            AppConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            Protocol = AppConfig.AppSettings.Settings["Protocol"].Value;
            logger.LogTrace($"Ending Management Constructor Protocol is {Protocol}");

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
                                logger.LogTrace($"Starting Replace Job for {config.Job.Alias}");
                                Replace(config, InventoryResult, ApiClient);
                                logger.LogTrace($"Finishing Replace Job for {config.Job.Alias}");
                            }
                            else
                            {
                                logger.LogTrace($"Starting Add Job for {config.Job.Alias}");
                                Add(config, ApiClient);
                                logger.LogTrace($"Finishing Add Job for {config.Job.Alias}");
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
                            logger.LogTrace($"Starting Remove Job for {config.Job.Alias}");
                            Remove(config, InventoryResult, ApiClient);
                            logger.LogTrace($"Finishing Remove Job for {config.Job.Alias}");
                        }
                        catch (Exception e)
                        {
                            return ThrowError(e, "Error Removing Certificate");
                        }

                        break;
                    default:
                        return ThrowError(new UnSupportedOperationException(), "Management");
                }

                logger.LogTrace($"Finishing Process Job for {config.Job.Alias}");
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
            logger.LogTrace($"Start Delete the {configInfo.Job.Alias} Private Key");
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
            logger.LogTrace($"Successful Delete of the {configInfo.Job.Alias} Private Key");
        }

        protected internal virtual void Add(AnyJobConfigInfo configInfo, ApiClient apiClient)
        {
            logger.LogTrace($"Entering Add Function for {configInfo.Job.Alias}");
            var privateKeyString = "";
            string certPem;

            if (!string.IsNullOrEmpty(configInfo.Job.PfxPassword))
            {
                logger.LogTrace($"Pfx Password exists getting Private Key string for {configInfo.Job.Alias}");
                var certData = Convert.FromBase64String(configInfo.Job.EntryContents);
                var store = new Pkcs12Store(new MemoryStream(certData),
                    configInfo.Job.PfxPassword.ToCharArray());

                using (var memoryStream = new MemoryStream())
                {
                    using (TextWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        var pemWriter = new PemWriter(streamWriter);
                        logger.LogTrace($"Getting Public Key for {configInfo.Job.Alias}");
                        Alias = store.Aliases.Cast<string>().SingleOrDefault(a => store.IsKeyEntry(a));
                        var publicKey = store.GetCertificate(Alias).Certificate.GetPublicKey();
                        logger.LogTrace($"Getting Private Key for {configInfo.Job.Alias}");
                        var privateKey = store.GetKey(Alias).Key;
                        var keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);
                        logger.LogTrace($"Writing Private Key for {configInfo.Job.Alias}");
                        pemWriter.WriteObject(keyPair.Private);
                        streamWriter.Flush();
                        privateKeyString = Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim()
                            .Replace("\r", "").Replace("\0", "");
                        memoryStream.Close();
                        streamWriter.Close();
                        logger.LogTrace($"Private Key String Retrieved for {configInfo.Job.Alias}");
                    }
                }

                // Extract server certificate
                var beginCertificate = "-----BEGIN CERTIFICATE-----\n";
                var endCertificate = "\n-----END CERTIFICATE-----";

                logger.LogTrace($"Start getting Server Certificate for {configInfo.Job.Alias}");
                certPem = beginCertificate +
                          Pemify(Convert.ToBase64String(store.GetCertificate(Alias).Certificate.GetEncoded())) +
                          endCertificate;
                logger.LogTrace($"Finished getting Server Certificate for {configInfo.Job.Alias}");
            }
            else
            {
                logger.LogTrace($"No Private Key get Cert Pem {configInfo.Job.Alias}");
                certPem = CertStart + Pemify(configInfo.Job.EntryContents) + CertEnd;
            }

            logger.LogTrace($"Creating Cert API Add Request for {configInfo.Job.Alias}");
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

            logger.LogTrace($"Making API Call to Add Certificate For {configInfo.Job.Alias}");
            apiClient.AddCertificate(sslCertRequest, certPem);
            logger.LogTrace($"Finished API Call to Add Certificate For {configInfo.Job.Alias}");

            if (!string.IsNullOrEmpty(configInfo.Job.PfxPassword))
            {
                logger.LogTrace($"Creating Key API Add Request for {configInfo.Job.Alias}");
                var sslKeyRequest = new SslKeyRequest
                {
                    SslKey = new SslCertKey
                    {
                        Action = "import",
                        File = configInfo.Job.Alias.Replace(".pem", ".pem"),
                        FileHandle = configInfo.Job.Alias.Replace(".pem", ".pem")
                    }
                };

                logger.LogTrace($"Making Add Key API Call for {configInfo.Job.Alias}");
                apiClient.AddPrivateKey(sslKeyRequest, privateKeyString);
                logger.LogTrace($"Finished Add Key API Call for {configInfo.Job.Alias}");
            }

            logger.LogTrace($"Starting Log Off for Add {configInfo.Job.Alias}");
            logger.LogTrace($"Finished Log Off for Add {configInfo.Job.Alias}");
        }
    }
}