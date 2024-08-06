using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;
using System.Collections.Generic;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace diagramMaker.managers.DefaultPanels
{
    public class PanelEvents
    {
        private readonly DataHub data;

        public PanelEvents(DataHub data)
        {
            this.data = data;
        }

        public void EventPanelHandler(int id, ECommand command)
        {
            switch (command)
            {
                case ECommand.Panel_MinMax:
                    foreach (var panel in data.panel)
                    {
                        if (panel.Value.itemId == id)
                        {
                            //search the parent of the clicked button id
                            int _parentId = ((CommonParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Common]).ParentId;
                            int _parentArrIndex = data.GetItemIndexByID(_parentId);

                            if (panel.Value.isOpen)
                            {
                                ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Height = 24;
                                ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Border.Height = 24;
                                ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Clip =
                                    new RectangleGeometry(
                                        new Rect(
                                            0,
                                            0,
                                            ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Width,
                                            24));
                            }
                            else
                            {
                                ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Height =
                                    ((ItemParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Item]).Height;
                                ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Border.Height =
                                    ((ItemParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Item]).Height;
                                ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Clip =
                                   new RectangleGeometry(
                                       new Rect(
                                           0,
                                           0,
                                           ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Width,
                                           ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Height));
                            }
                            panel.Value.isOpen = !panel.Value.isOpen;
                            break;
                        }
                    }
                    break;
                case ECommand.Panel_Close:
                    data.items[data.GetItemIndexByID(id)].doVisual(false);
                    foreach (var panel in data.panel)
                    {
                        if (panel.Value.itemId == id)
                        {
                            panel.Value.isVisual = false;
                            break;
                        }
                    }
                    break;
                case ECommand.SubPanel_MinMax:
                    {
                        MenuContainer _topPanel = null;
                        foreach (var _panel in data.panel)
                        {
                            if (_panel.Value.GetSubPanel(id) != null)
                            {
                                _topPanel = _panel.Value;
                                break;
                            }
                        }
                        if (_topPanel == null)
                        {
                            return;
                        }

                        bool _hasPanel = false;
                        double _heightShift = 0;
                        int _id = id;
                        CanvasItem _itm = null;
                        bool _isOpen = false;

                        List<int> _panelListId = _topPanel.GetParentIndexList_SubPanelChain(_id);
                        //go from the last inner layer to the top one
                        for (int _i = _panelListId.Count - 1; _i>=0; _i--)
                        {
                            //get parent layer of items
                            int _j = 0;
                            MenuContainer _parentPanel = _topPanel;
                            while (_j < _i)
                            {
                                _parentPanel = _parentPanel.subPanel[_panelListId[_j]];
                                _j++;
                            }
                            //goto throw All children from the first one
                            for (_j = _panelListId[_i]; _j < _parentPanel.childrenId.Count; _j++)
                            {
                                if (_parentPanel.childrenId[_j] == _id)
                                {
                                    if (!_hasPanel)
                                    {
                                        _itm = (CanvasItem)data.items[data.GetItemIndexByID(_id)];
                                        int _subIndex = _parentPanel.subPanel.FindIndex(item => item.itemId == _parentPanel.childrenId[_panelListId[_i]]);
                                        if (_parentPanel.subPanel[_subIndex].isOpen)
                                        {
                                            _itm.Item.Height = 24;
                                            if (_itm.Border != null)
                                            {
                                                _itm.Border.Height = 24;
                                            }
                                            _itm.Item.Clip = new RectangleGeometry(new Rect(0, 0, _itm.Item.Width, 24));
                                            _heightShift = 24 - ((ItemParameter)_itm.param[EParameter.Item]).Height;
                                        }
                                        else
                                        {
                                            _itm.Item.Height = ((ItemParameter)_itm.param[EParameter.Item]).Height;
                                            if (_itm.Border != null)
                                            {
                                                _itm.Border.Height = ((ItemParameter)_itm.param[EParameter.Item]).Height;
                                            }
                                            _itm.Item.Clip = new RectangleGeometry(new Rect(0, 0, _itm.Item.Width, _itm.Item.Height));
                                            _heightShift = ((ItemParameter)_itm.param[EParameter.Item]).Height - 24;
                                        }
                                        _parentPanel.subPanel[_subIndex].isOpen = !_parentPanel.subPanel[_subIndex].isOpen;
                                        _isOpen = _parentPanel.subPanel[_subIndex].isOpen;
                                        _hasPanel = true;
                                    } 
                                    
                                    if (_parentPanel != _topPanel)
                                    {
                                        CanvasItem _parentItm = (CanvasItem)data.items[data.GetItemIndexByID(_parentPanel.itemId)];
                                        if (_isOpen)
                                        {
                                            _parentItm.Item.Height += (((ItemParameter)_itm.param[EParameter.Item]).Height - 24);
                                            if (_parentItm.Border != null)
                                            {
                                                _parentItm.Border.Height += _parentItm.Item.Height;
                                            }
                                            _parentItm.Item.Clip = new RectangleGeometry(new Rect(
                                                0,
                                                0,
                                                _parentItm.Item.Width,
                                                _parentItm.Item.Height));
                                        } else
                                        {
                                            _parentItm.Item.Height -= (((ItemParameter)_itm.param[EParameter.Item]).Height - 24);
                                            if (_parentItm.Border != null)
                                            {
                                                _parentItm.Border.Height -= _parentItm.Item.Height;
                                            }
                                            _parentItm.Item.Clip = new RectangleGeometry(new Rect(
                                                0,
                                                0,
                                                _parentItm.Item.Width,
                                                _parentItm.Item.Height));
                                        }
                                    }
                                }

                                if (_hasPanel && _parentPanel.childrenId[_j] != _id)
                                {
                                    ((ItemParameter)data.items[data.GetItemIndexByID(_parentPanel.childrenId[_j])].param[EParameter.Item]).Top += _heightShift;
                                    Canvas.SetTop(
                                        ((CanvasItem)data.items[data.GetItemIndexByID(_parentPanel.childrenId[_j])]).Item,
                                        ((ItemParameter)data.items[data.GetItemIndexByID(_parentPanel.childrenId[_j])].param[EParameter.Item]).Top);
                                }
                                _id = _parentPanel.itemId;
                            }
                        }
                    }
                    break;
                case ECommand.Panel_SizeChange_Bottom:
                    {
                        CanvasItem _item = (CanvasItem)data.items[data.GetItemIndexByID(id)];
                        if (data.oldMouseY - ((ItemParameter)_item.param[EParameter.Item]).Top > 30)
                        {
                            _item.Item.Height = data.oldMouseY - ((ItemParameter)_item.param[EParameter.Item]).Top;
                            if (_item.Border != null)
                            {
                                _item.Border.Height = _item.Item.Height;
                            }
                        ((ItemParameter)_item.param[EParameter.Item]).Height = _item.Item.Height;
                        }
                        _item.Item.Clip = new RectangleGeometry(new Rect(0, 0, _item.Item.Width, _item.Item.Height));
                    }
                    break;
                case ECommand.Panel_SizeChange_BottomLeft:
                    {
                        CanvasItem _item = (CanvasItem)data.items[data.GetItemIndexByID(id)];
                        if (_item.Item.Width + (((ItemParameter)_item.param[EParameter.Item]).Left - data.oldMouseX) > 24)
                        {
                            Canvas.SetLeft(_item.Item, data.oldMouseX);
                            _item.Item.Width += ((ItemParameter)_item.param[EParameter.Item]).Left - data.oldMouseX;
                            if (_item.Border != null)
                            {
                                _item.Border.Width = _item.Item.Width;
                            }
                            ((ItemParameter)_item.param[EParameter.Item]).Left = data.oldMouseX;
                            ((ItemParameter)_item.param[EParameter.Item]).Width = _item.Item.Width;
                        }
                        if (data.oldMouseY - ((ItemParameter)_item.param[EParameter.Item]).Top > 24)
                        {
                            _item.Item.Height = data.oldMouseY - ((ItemParameter)_item.param[EParameter.Item]).Top;
                            if (_item.Border != null)
                            {
                                _item.Border.Height = _item.Item.Height;
                            }
                        ((ItemParameter)_item.param[EParameter.Item]).Height = _item.Item.Height;
                        }
                        _item.Item.Clip = new RectangleGeometry(new Rect(0, 0, _item.Item.Width, _item.Item.Height));
                    }
                    break;
                case ECommand.Panel_SizeChange_BottomRight:
                    {
                        CanvasItem _item = (CanvasItem)data.items[data.GetItemIndexByID(id)];

                        if (data.oldMouseX - ((ItemParameter)_item.param[EParameter.Item]).Left > 24)
                        {
                            _item.Item.Width = data.oldMouseX - ((ItemParameter)_item.param[EParameter.Item]).Left;
                            if (_item.Border != null)
                            {
                                _item.Border.Width = _item.Item.Width;
                            }
                            ((ItemParameter)_item.param[EParameter.Item]).Width = _item.Item.Width;
                        }

                        if (data.oldMouseY - ((ItemParameter)_item.param[EParameter.Item]).Top > 24)
                        {
                            _item.Item.Height = data.oldMouseY - ((ItemParameter)_item.param[EParameter.Item]).Top;
                            if (_item.Border != null)
                            {
                                _item.Border.Height = _item.Item.Height;
                            }

                        ((ItemParameter)_item.param[EParameter.Item]).Height = _item.Item.Height;
                        }

                        _item.Item.Clip = new RectangleGeometry(new Rect(0, 0, _item.Item.Width, _item.Item.Height));
                    }
                    break;
                default:
                    break;

            }
        }

        public void EventScrollPanelHandler(int digit, int id)
        {
            MenuContainer _topPanel = null;
            foreach (var _panel in data.panel)
            {
                if (_panel.Value.itemId == id)
                {
                    _topPanel = _panel.Value;
                    break;
                }
            }

            if (_topPanel == null)
            {
                return;
            }

            double _commonHeight = 0;
            foreach (int _child in _topPanel.childrenId)
            {
                CanvasItem _itm = (CanvasItem)data.items[data.GetItemIndexByID(_child)];
                _commonHeight += _itm.Item.Height;
            }

            if (digit > 0) //content goes up
            {
                if (_topPanel.curPos + _commonHeight > 20)
                {
                    _topPanel.curPos -= 10;
                }
            }
            else if (digit < 0)//content goes down
            {
                if (_topPanel.curPos <= -10)
                {
                    _topPanel.curPos += 10;
                }
            }

            double _dist = _topPanel.curPos + 20;
            foreach (int _child in _topPanel.childrenId)
            {
                CanvasItem _itm = (CanvasItem)data.items[data.GetItemIndexByID(_child)];
                Canvas.SetTop(_itm.Item, _dist);
                ((ItemParameter)_itm.param[EParameter.Item]).Top = _dist;
                _dist += _itm.Item.Height;
            }
        }
    }
}