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
        public bool mouseUp;
        public string MouseUpInfo;
        public Boolean IsHitTestVisible;

        public EventParameter(
            bool moveSensitive = false,
            bool mouseDown = false,
            bool mouseUp = true,
            string mouseUpInfo = "",
            Boolean IsHitTestVisible = true)
        {
            this.moveSensitive = moveSensitive;
            this.mouseDown = mouseDown;
            this.mouseUp = mouseUp;
            this.MouseUpInfo = mouseUpInfo;
            this.IsHitTestVisible = IsHitTestVisible;
        }
    }
}
