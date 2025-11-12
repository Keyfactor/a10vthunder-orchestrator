// Copyright 2025 Keyfactor
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Keyfactor.Orchestrators.Common.Enums;
using Keyfactor.Orchestrators.Extensions;
using Microsoft.Extensions.Logging;

namespace a10vthunder
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