using System;

namespace Keyfactor.AnyAgent.vThunder.Exceptions
{
    class UnSupportedOperationException : Exception
    {
        public UnSupportedOperationException() : base("Unsupported Operation, only Add, Remove are supported")
        {

        }
    }
}
