using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;
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
                        int _sizeSubPanelId = -1;
                        foreach (var topPanel in data.panel)
                        {
                            bool _hasPanel = false;
                            double _heightShift = 0;
                            foreach (var panel in topPanel.Value.subPanel)
                            {
                                if (panel.itemId == id)
                                {
                                    _sizeSubPanelId = panel.itemId;
                                    _hasPanel = true;
                                    DefaultItem _itm = data.items[data.GetItemIndexByID(_sizeSubPanelId)];
                                    if (panel.isOpen)
                                    {
                                        ((CanvasItem)_itm).Item.Height = 24;
                                        if (((CanvasItem)_itm).Border != null)
                                        {
                                            ((CanvasItem)_itm).Border.Height = 24;
                                        }
                                        ((CanvasItem)_itm).Item.Clip =
                                            new RectangleGeometry(
                                                new Rect(
                                                    0,
                                                    0,
                                                    ((CanvasItem)_itm).Item.Width,
                                                    24));
                                        _heightShift = 24 - ((ItemParameter)_itm.param[EParameter.Item]).Height;
                                    }
                                    else
                                    {
                                        ((CanvasItem)_itm).Item.Height = ((ItemParameter)_itm.param[EParameter.Item]).Height;
                                        if (((CanvasItem)_itm).Border != null)
                                        {
                                            ((CanvasItem)_itm).Border.Height = ((ItemParameter)_itm.param[EParameter.Item]).Height;
                                        }
                                        ((CanvasItem)_itm).Item.Clip =
                                           new RectangleGeometry(
                                               new Rect(
                                                   0,
                                                   0,
                                                   ((CanvasItem)_itm).Item.Width,
                                                   ((CanvasItem)_itm).Item.Height));
                                        _heightShift = ((ItemParameter)_itm.param[EParameter.Item]).Height - 24;
                                    }
                                    panel.isOpen = !panel.isOpen;
                                }
                                if (_hasPanel && panel.itemId != id)
                                {
                                    ((ItemParameter)data.items[data.GetItemIndexByID(panel.itemId)].param[EParameter.Item]).Top += _heightShift;
                                    Canvas.SetTop(
                                        ((CanvasItem)data.items[data.GetItemIndexByID(panel.itemId)]).Item,
                                        ((ItemParameter)data.items[data.GetItemIndexByID(panel.itemId)].param[EParameter.Item]).Top);
                                }
                            }
                        }
                        if (_sizeSubPanelId != -1)
                        {
                            EventScrollPanelHandler(0, _sizeSubPanelId);
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