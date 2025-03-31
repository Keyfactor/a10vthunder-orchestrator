using System;
using a10vthunder.Api;
using a10vthunder.Api.Models;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Extensions.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace a10vthunder.ImplementedStoreTypes.ThunderSsl
{
    public class Inventory : IInventoryJobExtension
    {
        private ILogger _logger;

        private readonly IPAMSecretResolver _resolver;

        public Inventory(IPAMSecretResolver resolver)
        {
            _resolver = resolver;
        }

        protected internal virtual InventoryResult Result { get; set; }
        protected internal virtual CertManager CertificateManager { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual string Protocol { get; set; }
        protected internal virtual bool AllowInvalidCert { get; set; }
        protected internal virtual bool ReturnValue { get; set; }
        private string ServerPassword { get; set; }
        private string ServerUserName { get; set; }
        public string ExtensionName => "ThunderSsl";
        public string ResolvePamField(string name, string value)
        {
            _logger.LogTrace($"Attempting to resolved PAM eligible field {name}");
            return _resolver.Resolve(value);
        }

        public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate submitInventory)
        {
            _logger = LogHandler.GetClassLogger<Inventory>();
            _logger.MethodEntry();
            
            ServerPassword = ResolvePamField("ServerPassword", config.ServerPassword);
            ServerUserName = ResolvePamField("ServerUserName", config.ServerUsername);

            dynamic properties = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties);
            Protocol = properties != null &&
                       (properties.protocol == null || string.IsNullOrEmpty(properties.protocol.Value))
                ? "https"
                : properties?.protocol.Value;
            AllowInvalidCert =
                properties?.allowInvalidCert == null || string.IsNullOrEmpty(properties.allowInvalidCert.Value)
                    ? false
                    : bool.Parse(properties.allowInvalidCert.Value);

            using (ApiClient = new ApiClient(ServerUserName, ServerPassword,
                $"{Protocol}://{config.CertificateStoreDetails.ClientMachine.Trim()}", AllowInvalidCert))
            {
                ApiClient.Logon();
                ActivePartition activePartition = new ActivePartition
                {
                    curr_part_name = config.CertificateStoreDetails.StorePath
                };
                SetPartitionRequest partRequest = new SetPartitionRequest
                {
                    activepartition = activePartition
                };
                ApiClient.SetPartition(partRequest);
                try
                {
                    _logger.LogTrace("Parse: Certificate Inventory: " + config.CertificateStoreDetails.StorePath);
                    _logger.LogTrace(
                        $"Certificate Store: {config.CertificateStoreDetails.ClientMachine} {config.CertificateStoreDetails.StorePath}");

                    _logger.LogTrace("Entering A10 VThunder DataPower: Certificate Inventory");
                    _logger.LogTrace(
                        $"Entering processJob for Certificate Store: {config.CertificateStoreDetails.ClientMachine} {config.CertificateStoreDetails.StorePath}");
                    CertificateManager = new CertManager();
                    Result = CertificateManager.GetCerts(ApiClient);
                    _logger.LogTrace($"Got {Result.InventoryList.Count} Certs from Inventory");

                    ReturnValue = submitInventory.Invoke(Result.InventoryList);
                    _logger.LogTrace("Invoked Inventory");
                    _logger.MethodExit();

                    if (ReturnValue == false)
                        return new JobResult
                        {
                            Result = OrchestratorJobStatusJobResult.Failure,
                            JobHistoryId = config.JobHistoryId,
                            FailureMessage = "Error Invoking Inventory"
                        };

                    if (Result.Errors.HasError)
                        return new JobResult
                        {
                            JobHistoryId = config.JobHistoryId,
                            FailureMessage =
                                $"Inventory had issues retrieving some certificates: {Result.Errors.ErrorMessage}",
                            Result = OrchestratorJobStatusJobResult.Warning
                        };

                    return new JobResult
                    { JobHistoryId = config.JobHistoryId, Result = OrchestratorJobStatusJobResult.Success };
                }
                catch (Exception e)
                {
                    return new JobResult
                    {
                        Result = OrchestratorJobStatusJobResult.Failure,
                        JobHistoryId = config.JobHistoryId,
                        FailureMessage = $"Inventory Error Unknown {LogHandler.FlattenException(e)}"
                    };
                }
            }
        }
    }
}