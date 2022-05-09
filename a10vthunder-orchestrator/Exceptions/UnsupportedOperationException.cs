using System;

namespace Keyfactor.Extensions.Orchestrator.vThunder.Exceptions
{
    class UnSupportedOperationException : Exception
    {
        public UnSupportedOperationException() : base("Unsupported Operation, only Add, Remove are supported")
        {

        }
    }
}
