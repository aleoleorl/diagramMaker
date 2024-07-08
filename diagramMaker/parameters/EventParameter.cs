using diagramMaker.helpers;
using System;

namespace diagramMaker.parameters
{
    public class EventParameter: DefaultParameter
    {
        public bool isMoveSensitive;
        public bool isMouseDown;
        public bool isMouseMove;
        public bool isMouseUp;
        public string mouseUpInfo;
        public Boolean isHitTestVisible;
        public bool isMouseClick;
        public bool isMouseLeave;
        public bool isMouseWheel;
        public bool isMouseDoubleClick;

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
            bool mouseMove = false,
            bool mouseLeave = false,
            bool mouseWheel = false,
            bool mouseDoubleClick = false)
        {
            this.isMoveSensitive = moveSensitive;
            this.isMouseDown = mouseDown;
            this.isMouseUp = mouseUp;
            this.mouseUpInfo = mouseUpInfo;
            this.isHitTestVisible = IsHitTestVisible;
            this.isMouseClick = mouseClick;
            this.Command = command;
            this.CommandParameter = commandParameter;
            this.isMouseMove = mouseMove;
            this.isMouseLeave = mouseLeave;
            this.isMouseWheel = mouseWheel;
            this.isMouseDoubleClick = mouseDoubleClick;
        }

        public override EventParameter Clone()
        {
            return (EventParameter)this.MemberwiseClone();
        }
    }
}