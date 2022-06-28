using System;

namespace a10vthunder_orchestrator
{
    [AttributeUsage(AttributeTargets.Class)]
    public class JobAttribute : Attribute
    {
        // ReSharper disable once InconsistentNaming
        private string jobClass { get; set; }

        public JobAttribute(string jobClass)
        {
            this.jobClass = jobClass;
        }

        public virtual string JobClass => jobClass;
    }
}
