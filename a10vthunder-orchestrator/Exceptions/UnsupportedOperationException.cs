using System;

namespace a10vthunder_orchestrator.Exceptions
{
    internal class UnSupportedOperationException : Exception
    {
        public UnSupportedOperationException() : base("Unsupported Operation, only Add, Remove are supported")
        {
        }
    }
}