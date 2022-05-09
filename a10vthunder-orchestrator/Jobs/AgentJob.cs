using System;
using System.Linq;
using CSS.Common.Logging;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;

namespace Keyfactor.Extensions.Orchestrator.vThunder.Jobs
{
    public abstract class AgentJob : LoggingClientBase, IAgentJobExtension
    {
        public string ExtensionName => "";

        protected JobResult Success(string message = null)
        {
            return new JobResult
            {
                Result = OrchestratorJobStatusJobResult.Success,
            };
        }

        protected JobResult Warning(string message = null)
        {
            return new JobResult
            {
                Result = OrchestratorJobStatusJobResult.Warning,
                FailureMessage = message ?? $"{this.GetType().Name} {WindowsUserAnyAgentConstants.StoreTypeName} Complete With Warnings"
            };
        }

        protected JobResult ThrowError(Exception exception, string jobSection)
        {
            var message = FlattenException(exception);
            ILogger logger = LogHandler.GetClassLogger<Management>();

            logger.LogError($"Error performing {jobSection} in {this.GetType().Name} {WindowsUserAnyAgentConstants.StoreTypeName} - {message}");
            return new JobResult
            {
                Result = OrchestratorJobStatusJobResult.Failure,
                FailureMessage = message
            };
        }

        private string FlattenException(Exception ex)
        {
            var returnMessage = ex.Message;
            if (ex.InnerException != null)
                returnMessage += " - " + FlattenException(ex.InnerException);

            return returnMessage;
        }
    }
}