using Newtonsoft.Json;

namespace a10vthunder_orchestrator.Api.Models
{
    public class ActivePartition
    {
        public string curr_part_name { get; set; }
    }

    public class SetPartitionRequest
    {
        [JsonProperty("active-partition")]
        public ActivePartition activepartition { get; set; }
    }
}
