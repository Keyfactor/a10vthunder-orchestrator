using System;

namespace Keyfactor.Extensions.Orchestrator.vThunder.Exceptions
{
    class InvalidInventoryInvokeException:Exception
    {
        public InvalidInventoryInvokeException():base("SubmitInventory.Invoke returned false")
        {

        }
    }
}
