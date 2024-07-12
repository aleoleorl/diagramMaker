using diagramMaker.helpers.enumerators;
using System;

namespace diagramMaker.parameters
{
    public class EventParameter: DefaultParameter
    {
        public bool IsMoveSensitive { get; set; }
        public bool IsMouseDown { get; set; }
        public bool IsMouseMove { get; set; }
        public bool IsMouseUp { get; set; }
        public string MouseUpInfo { get; set; }
        public bool IsHitTestVisible { get; set; }
        public bool IsMouseClick { get; set; }
        public bool IsMouseLeave { get; set; }
        public bool IsMouseWheel { get; set; }
        public bool IsMouseDoubleClick { get; set; }

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
            this.IsMoveSensitive = moveSensitive;
            this.IsMouseDown = mouseDown;
            this.IsMouseUp = mouseUp;
            this.MouseUpInfo = mouseUpInfo;
            this.IsHitTestVisible = IsHitTestVisible;
            this.IsMouseClick = mouseClick;
            this.Command = command;
            this.CommandParameter = commandParameter;
            this.IsMouseMove = mouseMove;
            this.IsMouseLeave = mouseLeave;
            this.IsMouseWheel = mouseWheel;
            this.IsMouseDoubleClick = mouseDoubleClick;
        }

        public override EventParameter Clone()
        {
            return (EventParameter)this.MemberwiseClone();
        }
    }
}