namespace a10vthunder.Api.Models
{
    public class ManagementCertRestartRequest
    {
        public Secure secure { get; set; }
    }

    public class Secure
    {
        public int restart { get; set; }
    }
}
