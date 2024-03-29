﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using a10vthunder_orchestrator.Api;
using a10vthunder_orchestrator.Api.Models;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;

namespace a10vthunder_orchestrator.Jobs
{
    public class Management : IManagementJobExtension
    {
        protected internal static Func<string, string> Pemify = ss =>
            ss.Length <= 64 ? ss : ss.Substring(0, 64) + "\n" + Pemify(ss.Substring(64));

        private readonly ILogger<Management> _logger;

        public Management(ILogger<Management> logger)
        {
            _logger = logger;
        }

        protected internal virtual string Protocol { get; set; }
        protected internal virtual bool AllowInvalidCert { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual CertManager CertManager { get; set; }
        protected internal virtual InventoryResult InventoryResult { get; set; }
        protected internal virtual bool ExistingCert { get; set; }
        protected internal virtual string CertStart { get; set; } = "-----BEGIN CERTIFICATE-----\n";
        protected internal virtual string CertEnd { get; set; } = "\n-----END CERTIFICATE-----";
        protected internal virtual string Alias { get; set; }
        public string ExtensionName => "VThunderU";

        public JobResult ProcessJob(ManagementJobConfiguration config)
        {
            _logger.MethodEntry();
            _logger.LogTrace($"config settings: {JsonConvert.SerializeObject(config)}");
            dynamic properties = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);
            _logger.LogTrace($"properties: {JsonConvert.SerializeObject(properties)}");
            Protocol = properties?.protocol == null || string.IsNullOrEmpty(properties.protocol.Value)
                ? "https"
                : properties.protocol.Value;
            AllowInvalidCert =
                properties?.allowInvalidCert == null || string.IsNullOrEmpty(properties.allowInvalidCert.Value)
                    ? false
                    : bool.Parse(properties.allowInvalidCert.Value);

            CertManager = new CertManager();
            _logger.LogTrace($"Ending Management Constructor Protocol is {Protocol}");

            using (ApiClient = new ApiClient(config.ServerUsername, config.ServerPassword,
                $"{Protocol}://{config.CertificateStoreDetails.ClientMachine.Trim()}", AllowInvalidCert))
            {
                _logger.LogTrace("Entering APIClient Using clause");
                if (string.IsNullOrEmpty(config.JobCertificate.Alias))
                    return new JobResult
                    {
                        Result = OrchestratorJobStatusJobResult.Failure,
                        JobHistoryId = config.JobHistoryId,
                        FailureMessage = "Management Missing Alias/Overwrite, Operation Cannot Be Completed"
                    };

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
                                if (config.Overwrite)
                                {
                                    _logger.LogTrace($"Starting Replace Job for {config.JobCertificate.Alias}");
                                    Replace(config, InventoryResult, ApiClient);
                                    _logger.LogTrace($"Finishing Replace Job for {config.JobCertificate.Alias}");
                                }
                                else
                                {
                                    return new JobResult
                                    {
                                        Result = OrchestratorJobStatusJobResult.Failure,
                                        JobHistoryId = config.JobHistoryId,
                                        FailureMessage = "You must use the overwrite flag to replace an existing certificate."
                                    };
                                }
                            }
                            
                            if(!ExistingCert)
                            {
                                _logger.LogTrace($"Starting Add Job for {config.JobCertificate.Alias}");
                                Add(config, ApiClient);
                                _logger.LogTrace($"Finishing Add Job for {config.JobCertificate.Alias}");
                            }
                        }
                        catch (Exception e)
                        {
                            return new JobResult
                            {
                                Result = OrchestratorJobStatusJobResult.Failure,
                                JobHistoryId = config.JobHistoryId,
                                FailureMessage = $"Error Adding Certificate {LogHandler.FlattenException(e)}"
                            };
                        }

                        break;
                    case CertStoreOperationType.Remove:
                        try
                        {
                            _logger.LogTrace($"Starting Remove Job for {config.JobCertificate.Alias}");
                            Remove(config, InventoryResult, ApiClient);
                            _logger.LogTrace($"Finishing Remove Job for {config.JobCertificate.Alias}");
                        }
                        catch (Exception e)
                        {
                            return new JobResult
                            {
                                Result = OrchestratorJobStatusJobResult.Failure,
                                JobHistoryId = config.JobHistoryId,
                                FailureMessage = $"Error Removing Certificate {LogHandler.FlattenException(e)}"
                            };
                        }

                        break;
                    default:
                        return new JobResult
                        {
                            Result = OrchestratorJobStatusJobResult.Failure,
                            JobHistoryId = config.JobHistoryId,
                            FailureMessage = "Unsupported Operation, only Add, Remove and Replace are supported"
                        };
                }

                _logger.LogTrace($"Finishing Process Job for {config.JobCertificate.Alias}");
                return new JobResult
                    {JobHistoryId = config.JobHistoryId, Result = OrchestratorJobStatusJobResult.Success};
            }
        }

        protected internal virtual void Replace(ManagementJobConfiguration config, InventoryResult inventoryResult,
            ApiClient apiClient)
        {
            try
            {
                Remove(config, inventoryResult, apiClient);
                Add(config, apiClient);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Management.Replace: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        protected internal virtual void Remove(ManagementJobConfiguration configInfo, InventoryResult inventoryResult,
            ApiClient apiClient)
        {
            try
            {
                _logger.LogTrace($"Start Delete the {configInfo.JobCertificate.Alias} Private Key");
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
                _logger.LogTrace($"Successful Delete of the {configInfo.JobCertificate.Alias} Private Key");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Management.Remove: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }

        protected internal virtual void Add(ManagementJobConfiguration configInfo, ApiClient apiClient)
        {
            try
            {
                _logger.LogTrace($"Entering Add Function for {configInfo.JobCertificate.Alias}");
                var privateKeyString = "";
                string certPem;

                if (!string.IsNullOrEmpty(configInfo.JobCertificate.PrivateKeyPassword))
                {
                    _logger.LogTrace(
                        $"Pfx Password exists getting Private Key string for {configInfo.JobCertificate.Alias}");
                    var certData = Convert.FromBase64String(configInfo.JobCertificate.Contents);
                    var store = new Pkcs12Store(new MemoryStream(certData),
                        configInfo.JobCertificate.PrivateKeyPassword.ToCharArray());

                    using (var memoryStream = new MemoryStream())
                    {
                        using (TextWriter streamWriter = new StreamWriter(memoryStream))
                        {
                            var pemWriter = new PemWriter(streamWriter);
                            _logger.LogTrace($"Getting Public Key for {configInfo.JobCertificate.Alias}");
                            Alias = store.Aliases.Cast<string>().SingleOrDefault(a => store.IsKeyEntry(a));
                            var publicKey = store.GetCertificate(Alias).Certificate.GetPublicKey();
                            _logger.LogTrace($"Getting Private Key for {configInfo.JobCertificate.Alias}");
                            var privateKey = store.GetKey(Alias).Key;
                            var keyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);
                            _logger.LogTrace($"Writing Private Key for {configInfo.JobCertificate.Alias}");
                            pemWriter.WriteObject(keyPair.Private);
                            streamWriter.Flush();
                            privateKeyString = Encoding.ASCII.GetString(memoryStream.GetBuffer()).Trim()
                                .Replace("\r", "").Replace("\0", "");
                            memoryStream.Close();
                            streamWriter.Close();
                            _logger.LogTrace($"Private Key String Retrieved for {configInfo.JobCertificate.Alias}");
                        }
                    }

                    // Extract server certificate
                    var beginCertificate = "-----BEGIN CERTIFICATE-----\n";
                    var endCertificate = "\n-----END CERTIFICATE-----";

                    _logger.LogTrace($"Start getting Server Certificate for {configInfo.JobCertificate.Alias}");
                    certPem = beginCertificate +
                              Pemify(Convert.ToBase64String(store.GetCertificate(Alias).Certificate.GetEncoded())) +
                              endCertificate;
                    _logger.LogTrace($"Finished getting Server Certificate for {configInfo.JobCertificate.Alias}");
                }
                else
                {
                    _logger.LogTrace($"No Private Key get Cert Pem {configInfo.JobCertificate.Alias}");
                    certPem = CertStart + Pemify(configInfo.JobCertificate.Contents) + CertEnd;
                }

                _logger.LogTrace($"Creating Cert API Add Request for {configInfo.JobCertificate.Alias}");
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

                _logger.LogTrace($"Making API Call to Add Certificate For {configInfo.JobCertificate.Alias}");
                apiClient.AddCertificate(sslCertRequest, certPem);
                _logger.LogTrace($"Finished API Call to Add Certificate For {configInfo.JobCertificate.Alias}");

                if (!string.IsNullOrEmpty(configInfo.JobCertificate.PrivateKeyPassword))
                {
                    _logger.LogTrace($"Creating Key API Add Request for {configInfo.JobCertificate.Alias}");
                    var sslKeyRequest = new SslKeyRequest
                    {
                        SslKey = new SslCertKey
                        {
                            Action = "import",
                            File = configInfo.JobCertificate.Alias.Replace(".pem", ".pem"),
                            FileHandle = configInfo.JobCertificate.Alias.Replace(".pem", ".pem")
                        }
                    };

                    _logger.LogTrace($"Making Add Key API Call for {configInfo.JobCertificate.Alias}");
                    apiClient.AddPrivateKey(sslKeyRequest, privateKeyString);
                    _logger.LogTrace($"Finished Add Key API Call for {configInfo.JobCertificate.Alias}");
                }

                _logger.LogTrace($"Starting Log Off for Add {configInfo.JobCertificate.Alias}");
                _logger.LogTrace($"Finished Log Off for Add {configInfo.JobCertificate.Alias}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Management.Add: {LogHandler.FlattenException(ex)}");
                throw;
            }
        }
    }
}