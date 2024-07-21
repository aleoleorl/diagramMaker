using System.Collections.Generic;

namespace diagramMaker.helpers.containers
{
    public class MenuContainer
    {
        public readonly int id;
        public int itemId;

        public bool isVisual;
        public bool isOpen;
        public MenuMakeOptions option;
        public List<MenuContainer> menu;
        public List<int> childrenId;

        public double curPos;

        public MenuContainer(int id)
        {
            this.id = id;
            itemId = -1;
            isVisual = false;
            isOpen = false;
            menu = new List<MenuContainer>();
            childrenId = new List<int>();
            option = new MenuMakeOptions();
            curPos = 0;
        }
    }
}