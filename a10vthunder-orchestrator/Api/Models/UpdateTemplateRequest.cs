namespace a10vthunder_orchestrator.Api.Models
{
    public class UpdateTemplateCertificate
    {
        public string cert { get; set; }
        public string key { get; set; }
    }

    public class UpdateTemplateRequest
    {
        public UpdateTemplateCertificate certificate { get; set; }
    }
}
