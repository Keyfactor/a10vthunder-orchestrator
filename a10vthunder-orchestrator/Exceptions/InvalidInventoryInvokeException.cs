using System;

namespace a10vthunder.Exceptions
{
    internal class InvalidInventoryInvokeException : Exception
    {
        public InvalidInventoryInvokeException() : base("SubmitInventory.Invoke returned false")
        {
        }
    }
}