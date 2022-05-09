using System;

namespace Keyfactor.AnyAgent.vThunder.Exceptions
{
    class InvalidInventoryInvokeException:Exception
    {
        public InvalidInventoryInvokeException():base("SubmitInventory.Invoke returned false")
        {

        }
    }
}
