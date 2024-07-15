using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace diagramMaker.helpers.enumerators
{
    public enum EConnectorSupport
    {
        None,
        NewConnector,
        Ancestor,
        FromAncestorToLeft,
        FromAncestorToRight
    }
}
