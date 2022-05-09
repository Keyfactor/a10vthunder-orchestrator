using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keyfactor.AnyAgent.vThunder
{
    public class AnyErrors
    {
        public virtual bool HasError { get; set; }

        public virtual string ErrorMessage { get; set; }
    }
}