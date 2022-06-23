using System;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;

namespace a10vthunder_orchestrator
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