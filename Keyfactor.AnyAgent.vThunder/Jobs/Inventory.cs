using System;
using System.Configuration;
using System.Reflection;
using Keyfactor.AnyAgent.vThunder.api;
using Keyfactor.AnyAgent.vThunder.Exceptions;
using Keyfactor.Platform.Extensions.Agents;
using Keyfactor.Platform.Extensions.Agents.Delegates;

namespace Keyfactor.AnyAgent.vThunder.Jobs
{
    [Job(JobTypes.Inventory)]
    public class Inventory : AgentJob
    {

        protected internal virtual InventoryResult Result { get; set; }
        protected internal virtual CertManager CertificateManager { get; set; }
        protected internal virtual ApiClient ApiClient { get; set; }
        protected internal virtual string Protocol { get; set; }
        protected internal virtual Configuration AppConfig { get; set; }
        protected internal virtual bool ReturnValue { get; set; }

        public override AnyJobCompleteInfo processJob(AnyJobConfigInfo config, SubmitInventoryUpdate submitInventory,
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
                    Logger.Trace("Parse: Certificate Inventory: " + config.Store.StorePath);
                    Logger.Trace($"Certificate Store: {config.Store.StoretypeShortName}");

                    Logger.Trace("Entering A10 VThunder DataPower: Certificate Inventory");
                    Logger.Trace($"Entering processJob for Certificate Store: {config.Store.StoretypeShortName}");
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