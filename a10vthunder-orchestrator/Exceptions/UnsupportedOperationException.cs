using System;

namespace a10vthunder.Exceptions
{
    internal class UnSupportedOperationException : Exception
    {
        public UnSupportedOperationException() : base("Unsupported Operation, only Add, Remove are supported")
        {
        }
    }
}