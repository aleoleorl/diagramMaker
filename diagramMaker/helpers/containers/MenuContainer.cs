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

        public List<int> DeleteItem(int itemId)
        {
            List<int> _ret = new List<int>();
            if (childrenId.IndexOf(itemId) != -1)
            {
                _ret.Add(itemId);
                int _itemIndex = subPanel.FindIndex(item => item.itemId == itemId);
                if (_itemIndex != -1)
                {
                    _ret.AddRange(GetAllChildrenById(subPanel[_itemIndex]));
                    subPanel.RemoveAt(_itemIndex);
                }

                childrenId.Remove(itemId);
                return _ret;
            }
            else
            {
                for (int _i = 0; _i < subPanel.Count; _i++)
                {
                    _ret = subPanel[_i].DeleteItem(itemId);
                    if (_ret.Count != 0)
                    {
                        break;
                    }
                }
            }
            return _ret;
        }

        public List<int> GetAllChildrenById(MenuContainer menu)
        {
            List<int> _ret = new List<int>();
            _ret.AddRange(menu.childrenId);
            for (int _i = 0; _i < menu.subPanel.Count; _i++)
            {
                _ret.AddRange(menu.subPanel[_i].GetAllChildrenById(menu.subPanel[_i]));
            }

            return _ret;
        }
    }
}