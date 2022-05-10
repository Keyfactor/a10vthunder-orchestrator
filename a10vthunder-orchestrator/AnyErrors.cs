using System;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Keyfactor.Orchestrators.Extensions;
using Keyfactor.Orchestrators.Common.Enums;

namespace Keyfactor.Extensions.Orchestrator.vThunder
{
    public class AnyErrors
    {
        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }

        public static JobResult ThrowError(ILogger logger, Exception exception, string className, string jobSection)
        {
            var message = FlattenException(exception);

            logger.LogError($"Error performing {jobSection} in {className} {WindowsUserAnyAgentConstants.StoreTypeName} - {message}");
            return new JobResult
            {
                Result = OrchestratorJobStatusJobResult.Failure,
                FailureMessage = message
            };
        }

        private static string FlattenException(Exception ex)
        {
            var returnMessage = ex.Message;
            if (ex.InnerException != null)
                returnMessage += " - " + FlattenException(ex.InnerException);

            return returnMessage;
        }
    }
}