using System.Collections.Generic;
using Keyfactor.Platform.Extensions.Agents;

namespace Keyfactor.AnyAgent.vThunder
{
    public class InventoryResult
    {
        public virtual AnyErrors Errors { get; set; }

        public virtual List<AgentCertStoreInventoryItem> InventoryList { get; set; }
    }
}
