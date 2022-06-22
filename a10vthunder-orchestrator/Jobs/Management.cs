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

using Newtonsoft.Json;

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
        protected internal virtual bool AllowInvalidCert { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual CertManager CertManager { get; set; }
        protected internal virtual InventoryResult InventoryResult { get; set; }
        protected internal virtual bool ExistingCert { get; set; }
        protected internal virtual string CertStart { get; set; } = "-----BEGIN CERTIFICATE-----\n";
        protected internal virtual string CertEnd { get; set; } = "\n-----END CERTIFICATE-----";
        protected internal virtual string Alias { get; set; }

        public JobResult ProcessJob(ManagementJobConfiguration config)
        {
            ILogger logger = LogHandler.GetClassLogger<Management>();

            dynamic properties = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties.ToString());
            Protocol = properties.protocol == null || string.IsNullOrEmpty(properties.protocol.Value) ? "https" : properties.protocol.Value;
            AllowInvalidCert = properties.allowInvalidCert == null || string.IsNullOrEmpty(properties.allowInvalidCert.Value) ? false : bool.Parse(properties.allowInvalidCert.Value);

            CertManager = new CertManager();
            logger.LogTrace($"Ending Management Constructor Protocol is {Protocol}");

            using (ApiClient = new ApiClient(config.ServerUsername, config.ServerPassword,
                $"{Protocol}://{config.CertificateStoreDetails.ClientMachine.Trim()}", AllowInvalidCert))
            {
                if (string.IsNullOrEmpty(config.JobCertificate.Alias))
                    return AnyErrors.ThrowError(logger, new ArgumentException("Missing Alias/Overwrite, Operation Cannot Be Completed"),
                        this.GetType().Name, "Management Add/Replace");

                ApiClient.Logon();
                InventoryResult = CertManager.GetCert(ApiClient, config.JobCertificate.Alias);
                ExistingCert = InventoryResult != null && InventoryResult?.InventoryList?.Count == 1;

                switch (config.OperationType)
                {
                    case CertStoreOperationType.Add:
                        try
                        {
                            if (ExistingCert)
                            {
                                logger.LogTrace($"Starting Replace Job for {config.JobCertificate.Alias}");
                                Replace(logger, config, InventoryResult, ApiClient);
                                logger.LogTrace($"Finishing Replace Job for {config.JobCertificate.Alias}");
                            }
                            else
                            {
                                logger.LogTrace($"Starting Add Job for {config.JobCertificate.Alias}");
                                Add(logger, config, ApiClient);
                                logger.LogTrace($"Finishing Add Job for {config.JobCertificate.Alias}");
                            }
                        }
                        catch (Exception e)
                        {
                            return AnyErrors.ThrowError(logger, e, this.GetType().Name, "Error Adding Certificate");
                        }

                        break;
                    case CertStoreOperationType.Remove:
                        try
                        {
                            logger.LogTrace($"Starting Remove Job for {config.JobCertificate.Alias}");
                            Remove(logger, config, InventoryResult, ApiClient);
                            logger.LogTrace($"Finishing Remove Job for {config.JobCertificate.Alias}");
                        }
                        catch (Exception e)
                        {
                            return AnyErrors.ThrowError(logger, e, this.GetType().Name, "Error Removing Certificate");
                        }

                        break;
                    default:
                        return AnyErrors.ThrowError(logger, new UnSupportedOperationException(), this.GetType().Name, "Management");
                }

                logger.LogTrace($"Finishing Process Job for {config.JobCertificate.Alias}");
                return new JobResult() { JobHistoryId = config.JobHistoryId, Result = OrchestratorJobStatusJobResult.Success };
            }
        }

        protected internal virtual void Replace(ILogger logger, ManagementJobConfiguration config, InventoryResult inventoryResult,
            ApiClient apiClient)
        {
            Remove(logger, config, inventoryResult, apiClient);
            Add(logger, config, apiClient);
        }

        protected internal virtual void Remove(ILogger logger, ManagementJobConfiguration configInfo, InventoryResult inventoryResult,
            ApiClient apiClient)
        {
            logger.LogTrace($"Start Delete the {configInfo.JobCertificate.Alias} Private Key");
            DeleteCertBaseRequest deleteKeyRoot;
            if (inventoryResult.InventoryList[0].PrivateKeyEntry)
                deleteKeyRoot = new DeleteCertBaseRequest
                {
                    DeleteCert = new DeleteCertRequest
                    {
                        CertName = configInfo.JobCertificate.Alias,
                        PrivateKey = configInfo.JobCertificate.Alias
                    }
                };
            else
                deleteKeyRoot = new DeleteCertBaseRequest
                {
                    DeleteCert = new DeleteCertRequest
                    {
                        CertName = configInfo.JobCertificate.Alias
                    }
                };

            apiClient.RemoveCertificate(deleteKeyRoot);
            logger.LogTrace($"Successful Delete of the {configInfo.JobCertificate.Alias} Private Key");
        }

        protected internal virtual void Add(ILogger logger, ManagementJobConfiguration configInfo, ApiClient apiClient)
        {
            logger.LogTrace($"Entering Add Function for {configInfo.JobCertificate.Alias}");
            var privateKeyString = "";
            string certPem;

            if (!string.IsNullOrEmpty(configInfo.JobCertificate.PrivateKeyPassword))
            {
                logger.LogTrace($"Pfx Password exists getting Private Key string for {configInfo.JobCertificate.Alias}");
                var certData = Convert.FromBase64String(configInfo.JobCertificate.Contents);
                var store = new Pkcs12Store(new MemoryStream(certData),
                    configInfo.JobCertificate.PrivateKeyPassword.ToCharArray());

                using (var memoryStream = new MemoryStream())
                {
                    using (TextWriter streamWriter = new StreamWriter(memoryStream))
                    {
                        var pemWriter = new PemWriter(streamWriter);
                        logger.LogTrace($"Getting Public Key for {configInfo.JobCertificate.Alias}");
                        Alias = store.Aliases.Cast<string>().SingleOrDefault(a => store.IsKeyEntry(a));
                        var publicKey = store.GetCertificate(Alias).Certificate.GetPublicKey();
                        logger.LogTrace($"Getting Private Key for {configInfo.JobCertificate.Alias}");
                        var privateKey = store.GetKey(Alias).Key;
                        var keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);
                        logger.LogTrace($"Writing Private Key for {configInfo.JobCertificate.Alias}");
                        pemWriter.WriteObject(keyPair.Private);
                        streamWriter.Flush();
                        privateKeyString = Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim()
                            .Replace("\r", "").Replace("\0", "");
                        memoryStream.Close();
                        streamWriter.Close();
                        logger.LogTrace($"Private Key String Retrieved for {configInfo.JobCertificate.Alias}");
                    }
                }

                // Extract server certificate
                var beginCertificate = "-----BEGIN CERTIFICATE-----\n";
                var endCertificate = "\n-----END CERTIFICATE-----";

                logger.LogTrace($"Start getting Server Certificate for {configInfo.JobCertificate.Alias}");
                certPem = beginCertificate +
                          Pemify(Convert.ToBase64String(store.GetCertificate(Alias).Certificate.GetEncoded())) +
                          endCertificate;
                logger.LogTrace($"Finished getting Server Certificate for {configInfo.JobCertificate.Alias}");
            }
            else
            {
                logger.LogTrace($"No Private Key get Cert Pem {configInfo.JobCertificate.Alias}");
                certPem = CertStart + Pemify(configInfo.JobCertificate.Contents) + CertEnd;
            }

            logger.LogTrace($"Creating Cert API Add Request for {configInfo.JobCertificate.Alias}");
            var sslCertRequest = new SslCertificateRequest
            {
                SslCertificate = new SslCert
                {
                    Action = "import",
                    CertificateType = "pem",
                    File = configInfo.JobCertificate.Alias.Replace(".pem", ".pem"),
                    FileHandle = configInfo.JobCertificate.Alias.Replace(".pem", ".pem")
                }
            };

            logger.LogTrace($"Making API Call to Add Certificate For {configInfo.JobCertificate.Alias}");
            apiClient.AddCertificate(sslCertRequest, certPem);
            logger.LogTrace($"Finished API Call to Add Certificate For {configInfo.JobCertificate.Alias}");

            if (!string.IsNullOrEmpty(configInfo.JobCertificate.PrivateKeyPassword))
            {
                logger.LogTrace($"Creating Key API Add Request for {configInfo.JobCertificate.Alias}");
                var sslKeyRequest = new SslKeyRequest
                {
                    SslKey = new SslCertKey
                    {
                        Action = "import",
                        File = configInfo.JobCertificate.Alias.Replace(".pem", ".pem"),
                        FileHandle = configInfo.JobCertificate.Alias.Replace(".pem", ".pem")
                    }
                };

                logger.LogTrace($"Making Add Key API Call for {configInfo.JobCertificate.Alias}");
                apiClient.AddPrivateKey(sslKeyRequest, privateKeyString);
                logger.LogTrace($"Finished Add Key API Call for {configInfo.JobCertificate.Alias}");
            }

            logger.LogTrace($"Starting Log Off for Add {configInfo.JobCertificate.Alias}");
            logger.LogTrace($"Finished Log Off for Add {configInfo.JobCertificate.Alias}");
        }
    }
}