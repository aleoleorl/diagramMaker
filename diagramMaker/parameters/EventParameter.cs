using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace diagramMaker.parameters
{
    public class EventParameter: DefaultParameter
    {
        public bool moveSensitive;
        public bool mouseDown;

        public EventParameter(
            bool moveSensitive = false,
            bool mouseDown = false)
        {
            this.moveSensitive = moveSensitive;
            this.mouseDown = mouseDown;
        }
    }
}
