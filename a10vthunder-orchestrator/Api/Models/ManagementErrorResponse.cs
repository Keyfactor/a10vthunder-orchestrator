using Newtonsoft.Json;

namespace a10vthunder.Api.Models
{
    public class Err
    {
        public int code { get; set; }
        public string msg { get; set; }
        public string location { get; set; }
    }

    public class ErrorResponse
    {
        [JsonProperty("http-status")]
        public int httpstatus { get; set; }
        public string status { get; set; }
        public Err err { get; set; }
    }

    public class ManagementErrorResponse
    {
        public Response response { get; set; }
    }

}
