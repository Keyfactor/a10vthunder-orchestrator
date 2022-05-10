using System.Collections.Generic;
using Keyfactor.Orchestrators.Extensions;

namespace Keyfactor.Extensions.Orchestrator.vThunder
{
    public class InventoryResult
    {
        public virtual AnyErrors Errors { get; set; }

        public virtual List<CurrentInventoryItem> InventoryList { get; set; }
    }
}
