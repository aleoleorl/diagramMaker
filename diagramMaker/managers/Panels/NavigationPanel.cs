using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace diagramMaker.managers.DefaultPanels
{
    public class NavigationPanel
    {
        private DataHub data;
        private DefaultManager defMan;

        public delegate void NavigationItemHandler(ECommand command, List<int> items);
        public event NavigationItemHandler? NavigationItemNotify;

        public NavigationPanel(DataHub data, DefaultManager defMan)
        {
            this.data = data;
            this.defMan = defMan;
        }

        #region Navigation

        public void NavigationPanelScrollCount_Activation()
        {
            List<int> _tmp = NavigationPanelScrollCount();
            int _cnt = data.menuNavigationPanel_ScrollCount;
            while (_cnt > 0)
            {
                _tmp.RemoveAt(0);
                _cnt--;
            }
            NavigationItemNotify?.Invoke(ECommand.DescribeItem, _tmp);
        }
        public List<int> NavigationPanelScrollCount()
        {
            List<int> _tmp = new List<int>();
            for (int _i = 0; _i < data.items.Count; ++_i)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).ItemAttach == EItemAttach.Custom)
                {
                    if (((CommonParameter)data.items[_i].param[EParameter.Common]).ParentId == -1)
                    {
                        if (((CommonParameter)data.items[_i].param[EParameter.Common]).Connect.IsConnector == false)
                        {
                            _tmp.Add(_i);
                        }
                    }
                }
            }
            return _tmp;
        }

        public void NavigationMoveToItem(int id)
        {
            int _id = -1;
            CommonParameter _comPar = ((CommonParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Common]);
            for (int _i = 0; _i < data.items.Count; ++_i)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).Id ==
                    _comPar.Connect.Users[0])
                {
                    _id = _i;
                    break;
                }
            }
            if (_id != -1)
            {
                data.topLeftX = Math.Floor(((CommonParameter)data.items[_id].param[EParameter.Common]).AppX - data.winWidth / 2);
                data.topLeftY = Math.Floor(((CommonParameter)data.items[_id].param[EParameter.Common]).AppY - data.winHeight / 2);

                //connectedId item go after main item. There are no events now to check viceversa
                for (int _i = data.items.Count - 1; _i >= 0; _i--)
                {
                    if (((CommonParameter)data.items[_i].param[EParameter.Common]).ParentId == -1)
                    {
                        if (data.items[_i].param.ContainsKey(EParameter.Event) && ((EventParameter)data.items[_i].param[EParameter.Event]).IsMoveSensitive)
                        {
                            switch (((CommonParameter)data.items[_i].param[EParameter.Common]).ItemType)
                            {
                                case EItem.Canvas:
                                    Canvas.SetLeft(
                                    ((CanvasItem)data.items[_i]).Item,
                                        ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX - data.topLeftX
                                    );
                                    Canvas.SetTop(
                                    ((CanvasItem)data.items[_i]).Item,
                                        ((CommonParameter)data.items[_i].param[EParameter.Common]).AppY - data.topLeftY
                                        );
                                    if (data.items[_i].param.ContainsKey(EParameter.Item))
                                    {
                                        ((ItemParameter)data.items[_i].param[EParameter.Item]).Left = ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX - data.topLeftX;
                                        ((ItemParameter)data.items[_i].param[EParameter.Item]).Top = ((CommonParameter)data.items[_i].param[EParameter.Common]).AppY - data.topLeftY;
                                    }
                                    break;
                                case EItem.Painter:
                                    Canvas.SetLeft(
                                    ((PainterItem)data.items[_i]).item,
                                    ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX - data.topLeftX);
                                    Canvas.SetTop(
                                    ((PainterItem)data.items[_i]).item,
                                        ((CommonParameter)data.items[_i].param[EParameter.Common]).AppY - data.topLeftY);
                                    if (data.items[_i].param.ContainsKey(EParameter.Item))
                                    {
                                        ((ItemParameter)data.items[_i].param[EParameter.Item]).Left =
                                        ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX -
                                            data.topLeftX;
                                        ((ItemParameter)data.items[_i].param[EParameter.Item]).Top =
                                            ((CommonParameter)data.items[_i].param[EParameter.Common]).AppY -
                                            data.topLeftY;
                                    }
                                    break;
                                case EItem.Figure:
                                    if (data.items[_i].param.ContainsKey(EParameter.Item))
                                    {
                                        ((ItemParameter)data.items[_i].param[EParameter.Item]).Left =
                                            ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX -
                                            data.topLeftX;
                                        ((ItemParameter)data.items[_i].param[EParameter.Item]).Top =
                                            ((CommonParameter)data.items[_i].param[EParameter.Common]).AppY -
                                            data.topLeftY;
                                    }
                                    ((FigureItem)data.items[_i]).ReFigure();
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }

        public void NavigationPanel_AddItem(int ItemArrIndex)
        {
            DefaultItem _itm = data.items[ItemArrIndex];
            if (((CommonParameter)_itm.param[EParameter.Common]).ItemAttach == EItemAttach.Menu)
            {
                return;
            }
            if (((CommonParameter)_itm.param[EParameter.Common]).Connect.IsConnector)
            {
                return;
            }

            MenuMakeOptions _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            MenuContainer _layer = data.panel["itemNavigationPanel"];
            _option.parentId = _layer.menu[0].itemId;
            _option.panelLabel = "> Layer 0";
            _option.x = 3;
            _option.y = _layer.menu[0].option.y + _layer.menu[0].childrenId.Count * 30;
            _option.w = _layer.menu[0].option.w * 0.95;
            _option.h = 28;
            _option.itmStringContent.Add(((CommonParameter)_itm.param[EParameter.Common]).Name);
            _option.itmStringContent.Add(((CommonParameter)_itm.param[EParameter.Common]).Id.ToString());
            int _id = MenuMaker.Make_SubPanelItem_EventNavigationItem(
            data,
            defMan,
                ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item,
                _option,
                -1
                );
            _layer.menu[0].childrenId.Add(_id);

            bool _found = false;
            foreach (var _panel in _layer.menu)
            {
                if (_panel.itemId == _layer.menu[0].itemId)
                {
                    _found = true;
                    _panel.option.h += 30;

                    CanvasItem _subItm = (CanvasItem)data.items[data.GetItemIndexByID(_panel.itemId)];
                    ((ItemParameter)_subItm.param[EParameter.Item]).Height += 30;
                    if (_panel.isOpen)
                    {
                        _subItm.Item.Height += 30;
                        _subItm.Item.Clip = new RectangleGeometry(new Rect(
                            0,
                            0,
                            _subItm.Item.Width,
                            _subItm.Item.Height));
                    }
                }
                if (_found && _panel.itemId != _layer.menu[0].itemId)
                {

                }
            }

        }

        public void NavigationPanel_DeleteItem(int id)
        {
            MenuContainer _layer = data.panel["itemNavigationPanel"];
            int _panelItemId = -1;
            foreach (int _id in _layer.menu[0].childrenId)
            {
                if (data.GetItemIndexByID(_id) != -1)
                {
                    CommonParameter _comPar = (CommonParameter)data.items[data.GetItemIndexByID(_id)].param[EParameter.Common];
                    if (_comPar.Connect.Users[0] == id)
                    {
                        _panelItemId = _id;
                        break;
                    }
                }
            }
            if (_panelItemId == -1)
            {
                return;
            }
            int _childIndex = _layer.menu[0].childrenId.IndexOf(_panelItemId);

            //remove from parent item
            for (int _i = 0; _i < data.items.Count; _i++)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).Id == _layer.menu[0].itemId)
                {
                    Canvas _parent = ((CanvasItem)data.items[_i]).Item;
                    Canvas _child = ((CanvasItem)data.items[data.GetItemIndexByID(_panelItemId)]).Item;
                    _parent.Children.Remove(_child);
                    break;
                }
            }

            //delete from data items
            defMan.eve.EventItemDeleteHandler(_panelItemId, ECommand.DeleteItem);

            //shift menu
            if (_childIndex < _layer.menu[0].childrenId.Count - 1)
            {
                for (int _i = _childIndex + 1; _i < _layer.menu[0].childrenId.Count; _i++)
                {
                    int _id = _layer.menu[0].childrenId[_i];
                    CanvasItem _item = (CanvasItem)data.items[data.GetItemIndexByID(_id)];
                    ((ItemParameter)_item.param[EParameter.Item]).Top -= 30;
                    Canvas.SetTop(_item.Item, ((ItemParameter)_item.param[EParameter.Item]).Top);
                }
            }

            //delete from menu
            _layer.menu[0].childrenId.RemoveAt(_childIndex);
        }

        #endregion
    }
}
