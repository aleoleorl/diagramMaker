using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace diagramMaker.helpers.containers
{
    public class MenuContainer
    {
        public int itemId;
        public string name;

        public bool isVisual;
        public bool isOpen;

        public MenuMakeOptions option;

        public List<MenuContainer> subPanel;
        public List<int> childrenId;

        public double curPos;

        public MenuContainer()
        {
            itemId = -1;
            name = "";
            isVisual = false;
            isOpen = false;
            subPanel = new List<MenuContainer>();
            childrenId = new List<int>();
            option = new MenuMakeOptions();
            curPos = 0;
        }

        public int GetSubPanelIndexByItItemId(int itemId)
        {
            return subPanel.FindIndex(item => item.itemId == itemId);
        }

        public List<int> GetParentIndexList_SubPanelChain(int itemId)
        {
            List<int> _rtn = new List<int>();
            if (childrenId.IndexOf(itemId) != -1)
            {
                _rtn.Add(childrenId.IndexOf(itemId));
            }
            else
            {
                for (int _i = 0; _i < subPanel.Count; _i++)
                {
                    List<int> _temp = new List<int>();
                    _temp.AddRange(subPanel[_i].GetParentIndexList_SubPanelChain(itemId));
                    if (_temp.Count > 0)
                    {
                        _rtn.Add(_i);
                        _rtn.AddRange(_temp);
                        break;
                    }
                }
            }
            return _rtn;
        }

        public MenuContainer GetSubPanel(int itemId)
        {
            if (this.itemId == itemId)
            {
                return this;
            }
            for (int _i=0; _i< subPanel.Count; ++_i)
            {
                MenuContainer _temp = subPanel[_i].GetSubPanel(itemId);
                if (_temp != null)
                {
                    return _temp;
                }
            }
            return null;
        }
    }
}