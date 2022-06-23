using System;

namespace a10vthunder_orchestrator.Exceptions
{
    internal class InvalidInventoryInvokeException : Exception
    {
        public InvalidInventoryInvokeException() : base("SubmitInventory.Invoke returned false")
        {
        }
    }
}