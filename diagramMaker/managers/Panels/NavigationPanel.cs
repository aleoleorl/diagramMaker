using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Markup;
using System.Diagnostics;
using System.Linq;

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
                                    ((PainterItem)data.items[_i]).Item,
                                    ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX - data.topLeftX);
                                    Canvas.SetTop(
                                    ((PainterItem)data.items[_i]).Item,
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

        private void Navigation_AddItem(
            MenuContainer topLayer,
            int newItemIndex = -1, 
            ENavigType eNavigType = ENavigType.Layer)
        {
            //get the list index of the active navi item
            int _indexOfActive = data.activePersonalId == -1? 
                -1 : 
                data.layerInfoItems.FindIndex(item => item.personalId == data.activePersonalId);

            double _heightShift = 0;
            int _currParentPersonalId = -1;

            MenuMakeOptions _option = new MenuMakeOptions();

            //in depends on type of active item and its existance get
            // the parentId for a new item into MenuMakeOptions _option
            // currParentPersonalId
            if (_indexOfActive == -1 ||
                (data.layerInfoItems[_indexOfActive].type == ENavigType.Item &&
                    data.layerInfoItems[_indexOfActive].parentItemId == -1))
            {
                _option.parentId = topLayer.itemId;
                _currParentPersonalId = -1;
            }
            else
            {
                if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                {
                    _option.parentId = data.layerInfoItems[_indexOfActive].itemId;
                    _currParentPersonalId = data.layerInfoItems[_indexOfActive].personalId;
                }
                else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item &&
                    data.layerInfoItems[_indexOfActive].parentItemId != -1)
                {
                    _option.parentId = data.layerInfoItems[_indexOfActive].parentItemId;
                    _currParentPersonalId = data.layerInfoItems
                    [
                        data.layerInfoItems.FindIndex(item => item.itemId == data.layerInfoItems[_indexOfActive].parentItemId)
                    ].personalId;
                }
            }

            switch (eNavigType)
            {
                case ENavigType.Item:
                    {
                        //item can't be not-existant in items list.
                        if (newItemIndex == -1)
                        {
                            return;
                        }
                        DefaultItem _itm = data.items[newItemIndex];
                        //item shouldn't be the part of any menu
                        if (((CommonParameter)_itm.param[EParameter.Common]).ItemAttach == EItemAttach.Menu)
                        {
                            return;
                        }
                        //item shouldn't be any type of the connector
                        if (((CommonParameter)_itm.param[EParameter.Common]).Connect.IsConnector)
                        {
                            return;
                        }

                        //prepare data for a new item creation
                        _option.itmStringContent.Add(((CommonParameter)_itm.param[EParameter.Common]).Name);
                        _option.itmStringContent.Add(((CommonParameter)_itm.param[EParameter.Common]).Id.ToString());
                        _option.x = 3;
                        //if there are still no items in the navi list
                        if (_indexOfActive == -1)
                        {
                            _option.y = 24;
                        }
                        else
                        {
                            if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                            {
                                _option.y = 24;
                            }
                            if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item)
                            {
                                DefaultItem _prev = data.items[data.GetItemIndexByID(data.layerInfoItems[_indexOfActive].itemId)];
                                _option.y = ((ItemParameter)_prev.param[EParameter.Item]).Top;
                            }
                        }
                        _option.w = 146;
                        _option.h = 28;
                        _heightShift += 30;

                        //create the item in navig menu
                        int _id = MenuMaker.Make_SubPanelItem_EventNavigationItem(
                            data: data,
                            defMan: defMan,
                            appCanvas: ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item,
                            option: _option,
                            counter: -1
                        );
                        //add navigation item controller content
                        MenuMakeOptions _addOption = new MenuMakeOptions();
                        _addOption.x = _option.x;
                        _addOption.y = _option.y;
                        _addOption.w = _option.w;
                        _addOption.h = _option.h;
                        _addOption.parentId = _id;
                        _addOption.itmStringContent.Add(_id.ToString());
                        _addOption.itmStringContent.Add(((CommonParameter)_itm.param[EParameter.Common]).Id.ToString());
                        MenuMaker.Make_SubPanelAddItemContent_EventNavigationItem(
                            data: data,
                            defMan: defMan,
                            appCanvas: ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item,
                            option: _addOption,
                            counter: -1
                        );

                        //add the new item as a child into panel structure
                        if (_indexOfActive == -1)
                        {
                            topLayer.childrenId.Insert(0, _id);
                        }
                        else
                        {
                            List<int> _parentChain = topLayer.GetParentIndexList_SubPanelChain(
                                data.layerInfoItems[_indexOfActive].itemId);
                            MenuContainer _parentMenu = topLayer;
                            for (int _i = 0; _i < _parentChain.Count - 1; _i++)
                            {
                                _parentMenu = _parentMenu.subPanel[_parentChain[_i]];
                            }
                            if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item)
                            {
                                _parentMenu.childrenId.Insert(_parentChain[_parentChain.Count - 1], _id);
                            }
                            else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                            {
                                _parentMenu = _parentMenu.subPanel.Find(
                                    item => item.itemId == _parentMenu.childrenId[_parentChain[^1]]);
                                _parentMenu.childrenId.Insert(0, _id);
                            }
                        }

                        //create and add new layer of navy list
                        LayerInfo _newLayer = new LayerInfo(
                             type: ENavigType.Item,
                             itemId: _id,
                             itemOnStageId: ((CommonParameter)_itm.param[EParameter.Common]).Id,
                             parentItemId: topLayer.itemId == _option.parentId ? -1 : _option.parentId
                             );
                        _newLayer.name = ((CommonParameter)_itm.param[EParameter.Common]).Name;

                        if (_indexOfActive == -1)
                        {
                            data.layerInfoItems.Insert(0, _newLayer);
                            NavigationPanel_ChangeActivePersonalId(data.layerInfoItems[0].itemId);
                        }
                        else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item)
                        {
                            data.layerInfoItems.Insert(_indexOfActive, _newLayer);
                            NavigationPanel_ChangeActivePersonalId(data.layerInfoItems[_indexOfActive].itemId);
                        }
                        else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                        {
                            data.layerInfoItems.Insert(_indexOfActive + 1, _newLayer);
                            NavigationPanel_ChangeActivePersonalId(data.layerInfoItems[_indexOfActive + 1].itemId);
                        }
                        //data.activePersonalId = data.layerInfoItems[_indexOfActive == -1 ? 0 : _indexOfActive].personalId;
                    }
                    break;
                case ENavigType.Layer:
                    {
                        //layer must be not-existant in items list.
                        if (newItemIndex != -1)
                        {
                            return;
                        }

                        if (data.layerInfoItems.Count > 0)
                        {

                            _option.panelLabel = "Layer " + 
                                (LayerInfo.GetPersonalId + 1).ToString();
                        } else
                        {
                            _option.panelLabel = "Layer 1";
                        }
                        _option.x = 2;
                        //if there are still no items in the navi list
                        if (_indexOfActive == -1)
                        {
                            _option.y = 24;
                        }
                        else
                        {
                            if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                            {
                                _option.y = 24;
                            }
                            if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item)
                            {
                                DefaultItem _prev = data.items[data.GetItemIndexByID(data.layerInfoItems[_indexOfActive].itemId)];
                                _option.y = ((ItemParameter)_prev.param[EParameter.Item]).Top;
                            }
                        }
                        _option.w = 146;
                        _option.h = 28;
                        _option.category = EMenuCategory.SubMenu;
                        _heightShift += 30;

                        //create a new sub panel
                        MenuContainer _subMenu1 = MenuMaker.Make_CreationSubPanelCarcas(
                                   data: data,
                                   defMan: defMan,
                                   appCanvas: ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item,
                                   option: _option);
                        ((CanvasItem)data.items[data.GetItemIndexByID(_subMenu1.itemId)]).EEventNotify +=
                            defMan.navPanel.NavigationPanel_ChangeActivePersonalId;

                        //add navigation item controller content
                        MenuMakeOptions _addOption = new MenuMakeOptions();
                        _addOption.x = _option.x;
                        _addOption.y = _option.y;
                        _addOption.w = _option.w;
                        _addOption.h = _option.h;
                        _addOption.parentId = _subMenu1.itemId;
                        _addOption.itmStringContent.Add(_subMenu1.itemId.ToString());
                        _addOption.itmStringContent.Add(_subMenu1.itemId.ToString());
                        MenuMaker.Make_SubPanelAddItemContent_EventNavigationItem(
                            data: data,
                            defMan: defMan,
                            appCanvas: ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item,
                            option: _addOption,
                            counter: -1
                        );

                        //add the new item as a child and as a layer member into panel structure
                        if (_indexOfActive == -1)
                        {
                            topLayer.childrenId.Insert(0, _subMenu1.itemId);
                            topLayer.subPanel.Insert(0, _subMenu1);
                        }
                        else
                        {
                            List<int> _parentChain = topLayer.GetParentIndexList_SubPanelChain(
                                data.layerInfoItems[_indexOfActive].itemId);
                            MenuContainer _parentMenu = topLayer;
                            for (int _i = 0; _i < _parentChain.Count - 1; _i++)
                            {
                                _parentMenu = _parentMenu.subPanel[_parentChain[_i]];
                            }
                            if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item)
                            {
                                _parentMenu.childrenId.Insert(_parentChain[^1], _subMenu1.itemId);
                                
                                for (int _i = _parentChain[^1]; _i < _parentMenu.childrenId.Count; _i++)
                                {
                                    if (_parentMenu.GetSubPanelIndexByItItemId(_parentMenu.childrenId[_i]) != -1)
                                    {
                                        _parentMenu.subPanel.Insert(
                                            _parentMenu.GetSubPanelIndexByItItemId(_parentMenu.childrenId[_i]),
                                            _subMenu1);
                                        break;
                                    }
                                    if (_i == _parentMenu.childrenId.Count - 1)
                                    {
                                        _parentMenu.subPanel.Add(_subMenu1);
                                    }
                                }
                            }
                            else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                            {
                                //_parentMenu = _parentMenu.subPanel[_parentChain[^1]];
                                _parentMenu = _parentMenu.subPanel.Find(item => item.itemId == _parentMenu.childrenId[_parentChain[^1]]);
                                _parentMenu.childrenId.Insert(0, _subMenu1.itemId);
                                _parentMenu.subPanel.Insert(0, _subMenu1);
                            }
                        }

                        LayerInfo _newLayer = new LayerInfo(
                             type: ENavigType.Layer,
                             itemId: _subMenu1.itemId,
                             itemOnStageId: -1,
                             parentItemId: topLayer.itemId == _option.parentId ? -1 : _option.parentId
                             );

                        if (_indexOfActive == -1)
                        {
                            data.layerInfoItems.Insert(0, _newLayer);
                            NavigationPanel_ChangeActivePersonalId(data.layerInfoItems[0].itemId);
                        }
                        else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Item)
                        {
                            data.layerInfoItems.Insert(_indexOfActive, _newLayer);
                            NavigationPanel_ChangeActivePersonalId(data.layerInfoItems[_indexOfActive].itemId);
                        }
                        else if (data.layerInfoItems[_indexOfActive].type == ENavigType.Layer)
                        {
                            data.layerInfoItems.Insert(_indexOfActive + 1, _newLayer);
                            NavigationPanel_ChangeActivePersonalId(data.layerInfoItems[_indexOfActive + 1].itemId);
                        }
                    }
                    break;
                default:
                    break;
            }

            Navigation_PanelGraphicShift(_heightShift, data.activePersonalId);
        }

        public void Navigation_PanelGraphicShift(double heightShift, int activePersonalId)
        {
            //activePersonalId
            int _indexOfActive = data.layerInfoItems.FindIndex(item => item.personalId == activePersonalId);
            int _currParentItemId = _indexOfActive!= -1 ? data.layerInfoItems[_indexOfActive].parentItemId : -1;
            bool _isParent = false;
            int _grandParentId = -1;
            bool _isGrandParent = false;
            while (_indexOfActive < data.layerInfoItems.Count)
            {
                if (_currParentItemId != -1 && !_isGrandParent)
                {
                    _grandParentId = data.layerInfoItems[
                        data.layerInfoItems.FindIndex(item => item.itemId ==_currParentItemId)
                        ].parentItemId;
                    _isGrandParent = true;
                }

                //current parent should be grow up
                if (!_isParent)
                {
                    if (_currParentItemId != -1)
                    {
                        //find a parent in data.layerInfoItems
                        int _tempIndex = data.layerInfoItems.FindIndex(item => item.itemId == _currParentItemId);
                        int _parentId = data.layerInfoItems[_tempIndex].itemId;
                        CanvasItem _parentItem = (CanvasItem)data.items[data.GetItemIndexByID(_parentId)];
                        ((ItemParameter)_parentItem.param[EParameter.Item]).Height += heightShift;
                        
                        MenuContainer _parentMenu = data.panel["itemNavigationPanel"].GetSubPanel(_parentId);
                        if (_parentMenu != null && _parentMenu.isOpen)
                        {
                            _parentItem.Item.Height = ((ItemParameter)_parentItem.param[EParameter.Item]).Height;
                            _parentItem.Item.Clip = new RectangleGeometry(new Rect(
                                0,
                                0,
                                _parentItem.Item.Width,
                                ((ItemParameter)_parentItem.param[EParameter.Item]).Height));

                            if (_parentItem.Border != null)
                            {
                                _parentItem.Border.Height += 24;
                            }
                        }
                    }
                    _isParent = true;
                }

                //if current item is not the new added item
                if (_indexOfActive != -1 && data.layerInfoItems[_indexOfActive].personalId != activePersonalId)
                {
                    if (data.layerInfoItems[_indexOfActive].parentItemId == _currParentItemId &&
                        data.GetItemIndexByID(data.layerInfoItems[_indexOfActive].itemId) != -1)
                    {
                        CanvasItem _childItem = (CanvasItem)data.items[data.GetItemIndexByID(data.layerInfoItems[_indexOfActive].itemId)];
                        ((ItemParameter)_childItem.param[EParameter.Item]).Top += heightShift;
                        Canvas.SetTop(_childItem.Item, ((ItemParameter)_childItem.param[EParameter.Item]).Top);
                    }
                }

                _indexOfActive++;

                //we achive the end of the list and now should check if it the top of sub panel layers which is -1
                if (_indexOfActive == data.layerInfoItems.Count &&
                    _isGrandParent)
                {
                    _isGrandParent = false;
                    _indexOfActive = data.layerInfoItems.FindIndex(item => item.personalId == activePersonalId);
                    _currParentItemId = _grandParentId;
                    _isParent = false;
                }
            }
        }

        public void NavigationPanel_AddItem(int newItemArrIndex, ECommand command = ECommand.None)
        {
            if (command == ECommand.AddItem)
            {
                Navigation_AddItem(data.panel["itemNavigationPanel"], newItemArrIndex, ENavigType.Item);
            } else if (command == ECommand.AddLayer)
            {
                Navigation_AddItem(data.panel["itemNavigationPanel"], -1, ENavigType.Layer);
            }
        }

        public void NavigationPanel_ChangeActivePersonalId(int clickedId) 
        {
            Trace.WriteLine(clickedId+" - NavigationPanel_ChangeActivePersonalId");
            int _changeItem = -1;
            if (data.layerInfoItems.FindIndex(item => item.itemId == clickedId) != -1)
            {
                _changeItem = clickedId;
            }
            if (_changeItem != -1)
            {
                CanvasItem _item;
                LayerInfo _layer;

                if (data.activePersonalId != -1)
                {
                    _layer = data.layerInfoItems.FirstOrDefault(item => item.personalId == data.activePersonalId);
                    if (_layer.personalId == data.activePersonalId)
                    {    
                        _item = (CanvasItem)data.items[data.GetItemIndexByID(_layer.itemId)];
                        if (_layer.type == ENavigType.Item)
                        {
                            _item.Item.Background = new SolidColorBrush(Colors.Beige);
                        }
                        else
                        {
                            _item.Item.Background = new SolidColorBrush(Colors.LightCyan);
                        }
                    }

                }

                _layer = data.layerInfoItems.FirstOrDefault(item => item.itemId == clickedId);
                if (_layer.itemId == clickedId)
                {
                    data.activePersonalId = _layer.personalId;
                }                    
                _item = (CanvasItem)data.items[data.GetItemIndexByID(clickedId)];
                _item.Item.Background = new SolidColorBrush(Colors.Peru);
            }
        }

        public void NavigationPanel_DeleteItem(int id)
        {
            int _index = data.layerInfoItems.FindIndex(item => item.itemOnStageId == id);
            bool isLayer = false;
            if (_index == -1)
            {
                _index = data.layerInfoItems.FindIndex(item => item.itemId == id);
                isLayer = true;
                if (_index == -1)
                {
                    return;
                }
            }
            //find the most closest item or get parent or -1 of it on top level
            int _preDeletedItemPersonalId = -1;
            for (int _i = _index-1; _i >= 0; _i--)
            {
                if (data.layerInfoItems[_i].itemId == data.layerInfoItems[_index].parentItemId)
                {
                    _preDeletedItemPersonalId = data.layerInfoItems[_index].personalId;
                    break;
                }
                if (data.layerInfoItems[_i].parentItemId == data.layerInfoItems[_index].parentItemId)
                {
                    _preDeletedItemPersonalId = data.layerInfoItems[_index].personalId;
                    break;
                }
            }

            MenuContainer _layer = data.panel["itemNavigationPanel"];

            //delete from MenuContainer tree, create a List of _panelItemId
            List<int> _itemsToDelete = _layer.DeleteItem(data.layerInfoItems[_index].itemId);
            if (_itemsToDelete.Count == 0)
            {
                return;
            }

            for (int _panelItemId = 0; _panelItemId < _itemsToDelete.Count; _panelItemId++)
            {
                //remove from parent item
                LayerInfo _itm = data.layerInfoItems.Find(item => item.itemId == _itemsToDelete[_panelItemId]);
                if (_itm.parentItemId != -1)
                {
                    if (data.GetItemIndexByID(_itemsToDelete[_panelItemId]) != -1)
                    {
                        for (int _j = 0; _j < data.items.Count; _j++)
                        {
                            if (((CommonParameter)data.items[_j].param[EParameter.Common]).Id == _itm.parentItemId)
                            {
                                Canvas _parent = ((CanvasItem)data.items[_j]).Item;
                                Canvas _child = ((CanvasItem)data.items[data.GetItemIndexByID(_itemsToDelete[_panelItemId])]).Item;
                                _parent.Children.Remove(_child);
                                break;
                            }
                        }
                    }
                } else
                {
                    if (data.GetItemIndexByID(_itemsToDelete[_panelItemId]) != -1)
                    {
                        Canvas _parent = ((CanvasItem)data.items[data.GetItemIndexByID(data.menuNavigationPanelID)]).Item;
                        Canvas _child = ((CanvasItem)data.items[data.GetItemIndexByID(_itemsToDelete[_panelItemId])]).Item;
                        _parent.Children.Remove(_child);
                    }
                }

                //delete from data items
                if (!isLayer)
                {
                    defMan.eve.EventItemDeleteHandler(_itemsToDelete[_panelItemId], ECommand.DeleteItem);
                }
            }

            //shift menu
            Navigation_PanelGraphicShift(-_itemsToDelete.Count * 30, _preDeletedItemPersonalId);

            //check data.activePersonalId
            for (int _i = 0; _i < _itemsToDelete.Count; _i++)
            {
                if (data.layerInfoItems.FindIndex(item => 
                item.personalId == data.activePersonalId && 
                item.itemId == _itemsToDelete[_i]) != -1)
                {
                    data.activePersonalId = -1;
                }
            }                

            //clear data.layerInfoItems
            for (int _i = 0; _i < _itemsToDelete.Count; _i++)
            {
                data.layerInfoItems.Remove(data.layerInfoItems.Find(item => item.itemId == _itemsToDelete[_i]));
            }
        }

        public void NavigationPanel_VisibleHiddenItem(int itemId, ECommand command = ECommand.None)
        {
            List<int> _itemsIndex = new List<int>();
            int _i = 0;
            int _begin = data.layerInfoItems.FindIndex(item => item.itemId == itemId);
            _itemsIndex.Add(_begin);

            while (_i < _itemsIndex.Count)
            {
                for (int _j = _begin + 1; _j < data.layerInfoItems.Count; _j++)
                {
                    if (data.layerInfoItems[_j].parentItemId == data.layerInfoItems[_itemsIndex[_i]].itemId)
                    {
                        _itemsIndex.Add(_j);
                    }
                }
                _i++;
            }

            _i = 0;
            for (_i = 0; _i < _itemsIndex.Count; _i++)
            {
                if (data.layerInfoItems[_itemsIndex[_i]].itemOnStageId != -1)
                {
                    _begin = data.GetItemIndexByID(data.layerInfoItems[_itemsIndex[_i]].itemOnStageId);
                    DefaultItem _item = data.items[_begin];
                    switch (((CommonParameter)_item.param[EParameter.Common]).ItemType)
                    {
                        case EItem.Canvas:
                            if (command == ECommand.VisibleItem)
                            {
                                ((CanvasItem)_item).Item.Visibility = Visibility.Visible;
                            }
                            else if (command == ECommand.HiddenItem)
                            {
                                ((CanvasItem)_item).Item.Visibility = Visibility.Hidden;
                            }
                            break;
                        case EItem.Figure:
                            {
                                for (int _j = 0; _j< ((FigureItem)_item).Item.Count; _j++)
                                {
                                    if (command == ECommand.VisibleItem)
                                    {
                                        ((FigureItem)_item).Item[_j].Visibility = Visibility.Visible;
                                    }
                                    else if (command == ECommand.HiddenItem)
                                    {
                                        ((FigureItem)_item).Item[_j].Visibility = Visibility.Hidden;
                                    }
                                }
                            }
                            break;
                        case EItem.Painter:
                            if (command == ECommand.VisibleItem)
                            {
                                ((PainterItem)_item).Item.Visibility = Visibility.Visible;
                            }
                            else if (command == ECommand.HiddenItem)
                            {
                                ((PainterItem)_item).Item.Visibility = Visibility.Hidden;
                            }
                            break;
                        default:
                            break;
                    }
                }

                _begin = data.GetItemIndexByID(itemId);
                for (int _j = data.GetItemIndexByID(itemId) + 1; _j < data.items.Count; _j++)
                {
                    if (((CommonParameter)data.items[_j].param[EParameter.Common]).ParentId == data.layerInfoItems[_itemsIndex[_i]].itemId &&
                        ((CommonParameter)data.items[_j].param[EParameter.Common]).Name == "visItem")
                    {
                        if (command == ECommand.HiddenItem)
                        {
                            ((ButtonItem)data.items[_j]).Item.Visibility = Visibility.Visible;
                        }
                        else if (command == ECommand.VisibleItem)
                        {
                            ((ButtonItem)data.items[_j]).Item.Visibility = Visibility.Hidden;
                        }
                    }
                    if (((CommonParameter)data.items[_j].param[EParameter.Common]).ParentId == data.layerInfoItems[_itemsIndex[_i]].itemId &&
                        ((CommonParameter)data.items[_j].param[EParameter.Common]).Name == "hidItem")
                    {
                        if (command == ECommand.HiddenItem)
                        {
                            ((ButtonItem)data.items[_j]).Item.Visibility = Visibility.Hidden;
                        }
                        else if (command == ECommand.VisibleItem)
                        {
                            ((ButtonItem)data.items[_j]).Item.Visibility = Visibility.Visible;
                        }
                    }
                }

            }
        }

        #endregion
    }
}