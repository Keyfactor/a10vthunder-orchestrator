using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class Response
    {
        [JsonProperty("http-status")]
        public int httpstatus { get; set; }
        public string status { get; set; }
        public string msg { get; set; }
    }

    public class ManagementSuccessResponse
    {
        public Response response { get; set; }
    }
}
