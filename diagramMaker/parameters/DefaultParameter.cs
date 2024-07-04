using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diagramMaker.parameters
{
    public class DefaultParameter
    {
        public DefaultParameter()
        {

        }

        public virtual void handle01()
        {

        }

        public virtual DefaultParameter Clone()
        {
            return (DefaultParameter)this.MemberwiseClone();
        }
    }
}
