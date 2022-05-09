using System;
using System.Configuration;
using System.Reflection;
using Keyfactor.Extensions.Orchestrator.vThunder.api;
using Keyfactor.Extensions.Orchestrator.vThunder.Exceptions;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;

namespace Keyfactor.Extensions.Orchestrator.vThunder
{
    public class Inventory : IInventoryJobExtension
    {
        public string ExtensionName => "";

        protected internal virtual InventoryResult Result { get; set; }
        protected internal virtual CertManager CertificateManager { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual string Protocol { get; set; }
        protected internal virtual Configuration AppConfig { get; set; }
        protected internal virtual bool ReturnValue { get; set; }

        public override JobResult processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory,
            SubmitEnrollmentRequest submitEnrollmentRequest, SubmitDiscoveryResults sdr)
        {
            AppConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            Protocol = AppConfig.AppSettings.Settings["Protocol"].Value;

            using (ApiClient = new ApiClient(config.Server.Username, config.Server.Password,
                $"{Protocol}://{config.Store.ClientMachine.Trim()}"))
            {
                ApiClient.Logon();
                try
                {
                    logger.LogTrace("Parse: Certificate Inventory: " + config.Store.StorePath);
                    logger.LogTrace($"Certificate Store: {config.Store.StoretypeShortName}");

                    logger.LogTrace("Entering A10 VThunder DataPower: Certificate Inventory");
                    logger.LogTrace($"Entering processJob for Certificate Store: {config.Store.StoretypeShortName}");
                    CertificateManager = new CertManager();
                    Result = CertificateManager.GetCerts(ApiClient);
                    ReturnValue = submitInventory.Invoke(Result.InventoryList);

                    if (ReturnValue == false)
                        return ThrowError(new InvalidInventoryInvokeException(), "Inventory");

                    if (Result.Errors.HasError)
                        return Warning(
                            $"Inventory had issues retrieving some certificates: {Result.Errors.ErrorMessage}");

                    return Success();
                }
                catch (Exception e)
                {
                    return ThrowError(e, "Inventory");
                }
            }
        }
    }
}