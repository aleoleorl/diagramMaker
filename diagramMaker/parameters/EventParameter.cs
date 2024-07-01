using diagramMaker.helpers;
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
        public bool MouseMove;
        public bool mouseUp;
        public string MouseUpInfo;
        public Boolean IsHitTestVisible;
        public bool MouseClick;

        public ECommand Command;
        public int CommandParameter;

        public EventParameter(
            bool moveSensitive = false,
            bool mouseDown = false,
            bool mouseUp = true,
            string mouseUpInfo = "",
            Boolean IsHitTestVisible = true,
            bool mouseClick = false,
            ECommand command = ECommand.None,
            int commandParameter = -1,
            bool mouseMove = false)
        {
            this.moveSensitive = moveSensitive;
            this.mouseDown = mouseDown;
            this.mouseUp = mouseUp;
            this.MouseUpInfo = mouseUpInfo;
            this.IsHitTestVisible = IsHitTestVisible;
            this.MouseClick = mouseClick;
            this.Command = command;
            this.CommandParameter = commandParameter;
            this.MouseMove = mouseMove;
        }
    }
}
