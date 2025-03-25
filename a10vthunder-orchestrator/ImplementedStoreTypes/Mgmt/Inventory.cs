using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renci.SshNet;

namespace linux_scp_orchestrator.ImplementedStoreTypes
{
    public class Inventory : IInventoryJobExtension
    {
        private readonly ILogger<Inventory> _logger;

        public Inventory(ILogger<Inventory> logger)
        {
            _logger = logger;
        }

        public string ExtensionName => "ThunderMgmt";

        public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate submitInventory)
        {
            _logger.MethodEntry();

            try
            {
                dynamic props = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);

                string host = props.host;
                string username = props.username;
                string password = props.password;
                string path = props.path;
                int port = props.port != null ? (int)props.port : 22;

                _logger.LogInformation($"Connecting to {host} via SCP to retrieve PEM from {path}");

                string pemContent;

                using (var client = new ScpClient(host, port, username, password))
                {
                    client.Connect();
                    using (var ms = new MemoryStream())
                    {
                        client.Download(path, ms);
                        pemContent = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                    }
                    client.Disconnect();
                }

                string cert = ExtractPemBlock(pemContent, "CERTIFICATE");
                string key = ExtractPemBlock(pemContent, "PRIVATE KEY");

                if (string.IsNullOrEmpty(cert) || string.IsNullOrEmpty(key))
                {
                    throw new Exception("Failed to extract certificate or key from PEM.");
                }

                var inventoryItem = new CurrentInventoryItem
                {
                    ItemStatus = OrchestratorInventoryItemStatus.Unknown,
                    Alias = Path.GetFileName(path),
                    PrivateKeyEntry = true,
                    UseChainLevel = false,
                    Certificates = new[] { cert }
                };

                bool result = submitInventory.Invoke(new List<CurrentInventoryItem> { inventoryItem });

                return new JobResult
                {
                    JobHistoryId = config.JobHistoryId,
                    Result = result ? OrchestratorJobStatusJobResult.Success : OrchestratorJobStatusJobResult.Failure,
                    FailureMessage = result ? null : "Inventory submission failed."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during SCP Inventory job: {ex.Message}");
                return new JobResult
                {
                    Result = OrchestratorJobStatusJobResult.Failure,
                    JobHistoryId = config.JobHistoryId,
                    FailureMessage = $"Inventory Error: {ex.Message}"
                };
            }
            finally
            {
                _logger.MethodExit();
            }
        }

        private string ExtractPemBlock(string pem, string blockType)
        {
            var match = Regex.Match(pem,
                $"-----BEGIN {blockType}-----(.*?)-----END {blockType}-----",
                RegexOptions.Singleline);

            return match.Success ? match.Value : null;
        }
    }
}
