using System;
using System.Reflection;

using Keyfactor.Extensions.Orchestrator.vThunder.api;
using Keyfactor.Extensions.Orchestrator.vThunder.Exceptions;
using Keyfactor.Logging;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

namespace Keyfactor.Extensions.Orchestrator.vThunder
{
    public class Inventory : IInventoryJobExtension
    {
        public string ExtensionName => "";

        protected internal virtual InventoryResult Result { get; set; }
        protected internal virtual CertManager CertificateManager { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual string Protocol { get; set; }
        protected internal virtual bool AllowInvalidCert { get; set; }
        protected internal virtual bool ReturnValue { get; set; }

        public JobResult ProcessJob(InventoryJobConfiguration config, SubmitInventoryUpdate submitInventory)
        {
            ILogger logger = LogHandler.GetClassLogger<Management>();

            dynamic properties = JsonConvert.DeserializeObject(config.CertificateStoreDetails.Properties.ToString());
            Protocol = properties.protocol == null || string.IsNullOrEmpty(properties.protocol.Value) ? "https" : properties.protocol.Value;
            AllowInvalidCert = properties.allowInvalidCert == null || string.IsNullOrEmpty(properties.allowInvalidCert.Value) ? "https" : bool.Parse(properties.protallowInvalidCertocol.Value);

            using (ApiClient = new ApiClient(config.ServerUsername, config.CertificateStoreDetails.StorePassword,
                $"{Protocol}://{config.CertificateStoreDetails.ClientMachine.Trim()}", AllowInvalidCert))
            {
                ApiClient.Logon();
                try
                {
                    logger.LogTrace("Parse: Certificate Inventory: " + config.CertificateStoreDetails.StorePath);
                    logger.LogTrace($"Certificate Store: {config.CertificateStoreDetails.ClientMachine} {config.CertificateStoreDetails.StorePath}");

                    logger.LogTrace("Entering A10 VThunder DataPower: Certificate Inventory");
                    logger.LogTrace($"Entering processJob for Certificate Store: {config.CertificateStoreDetails.ClientMachine} {config.CertificateStoreDetails.StorePath}");
                    CertificateManager = new CertManager();
                    Result = CertificateManager.GetCerts(ApiClient);
                    ReturnValue = submitInventory.Invoke(Result.InventoryList);

                    if (ReturnValue == false)
                        return AnyErrors.ThrowError(logger, new InvalidInventoryInvokeException(), this.GetType().Name, "Inventory");

                    if (Result.Errors.HasError)
                        return new JobResult() { JobHistoryId = config.JobHistoryId, FailureMessage = $"Inventory had issues retrieving some certificates: {Result.Errors.ErrorMessage}", Result = OrchestratorJobStatusJobResult.Warning };

                    return new JobResult() { JobHistoryId = config.JobHistoryId, Result = OrchestratorJobStatusJobResult.Success };
                }
                catch (Exception e)
                {
                    return AnyErrors.ThrowError(logger, e, this.GetType().Name, "Inventory");
                }
            }
        }
    }
}