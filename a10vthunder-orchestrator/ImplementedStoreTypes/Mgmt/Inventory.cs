using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Renci.SshNet;

namespace Keyfactor.Extensions.Orchestrator.A10vThunder.ThunderMgmt
{
    public class Inventory : IInventoryJobExtension
    {
        private ILogger _logger;

        private readonly IPAMSecretResolver _resolver;

        public Inventory(IPAMSecretResolver resolver)
        {
            _resolver = resolver;
        }

        public string ExtensionName => String.Empty;

        public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate submitInventory)
        {
            _logger = LogHandler.GetClassLogger<Inventory>();
            _logger.MethodEntry();

            try
            {
                dynamic props = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);

                string host = props.OrchToScpServerIp;
                string username = props.ScpUserName;
                string password = props.ScpPassword;
                string path = config.CertificateStoreDetails.StorePath;
                int port = props.port != null ? (int)props.port : 22;

                _logger.LogInformation($"Connecting to {host} via SSH to list files in {path}");

                List<CurrentInventoryItem> inventory = new List<CurrentInventoryItem>();

                using (var sshClient = new SshClient(host, port, username, password))
                using (var scpClient = new ScpClient(host, port, username, password))
                {
                    sshClient.Connect();
                    scpClient.Connect();

                    // List all files in directory
                    var cmd = sshClient.RunCommand($"ls -1 \"{path}\"");
                    if (cmd.ExitStatus != 0)
                    {
                        throw new Exception($"Failed to list directory: {cmd.Error}");
                    }

                    var files = cmd.Result.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var file in files)
                    {
                        string fullFilePath = $"{path}/{file}";
                        _logger.LogInformation($"Checking file: {fullFilePath}");

                        using (var ms = new MemoryStream())
                        {
                            try
                            {
                                scpClient.Download(fullFilePath, ms);
                                string pemContent = System.Text.Encoding.UTF8.GetString(ms.ToArray());

                                string cert = ExtractPemBlock(pemContent, "CERTIFICATE");

                                if (!string.IsNullOrEmpty(cert))
                                {
                                    string alias = Path.GetFileNameWithoutExtension(file);

                                    var inventoryItem = new CurrentInventoryItem
                                    {
                                        ItemStatus = OrchestratorInventoryItemStatus.Unknown,
                                        Alias = alias,
                                        PrivateKeyEntry = true,
                                        UseChainLevel = false,
                                        Certificates = new[] { cert }
                                    };

                                    inventory.Add(inventoryItem);
                                    _logger.LogInformation($"Added cert to inventory: {alias}");
                                }
                                else
                                {
                                    _logger.LogDebug($"Skipped file (no certificate found): {file}");
                                }
                            }
                            catch (Exception fileEx)
                            {
                                _logger.LogWarning($"Failed to process file '{file}': {fileEx.Message}");
                            }
                        }
                    }

                    sshClient.Disconnect();
                    scpClient.Disconnect();
                }

                bool result = submitInventory.Invoke(inventory);

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
