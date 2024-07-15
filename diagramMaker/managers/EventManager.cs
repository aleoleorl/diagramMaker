using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using diagramMaker.managers.DefaultPreparation;
using diagramMaker.helpers.enumerators;
using diagramMaker.helpers.containers;

namespace diagramMaker.managers
{
    public class EventManager
    {
        private DataHub data;

        public delegate void NavigationItemHandler(ECommand command, List<int> items);
        public event NavigationItemHandler? NavigationItemNotify;
        public delegate void KeyDownHandler(object sender, KeyEventArgs e);
        public event KeyDownHandler? KeyDownNotify;

        public EventManager(DataHub data)
        {
            this.data = data;
        }

        #region Navigation

        public void EventNavigationPanelScrollCount(int digit)
        {
            data.menuNavigationPanel_ScrollCount += digit;
            if (data.menuNavigationPanel_ScrollCount < 0)
            {
                data.menuNavigationPanel_ScrollCount = 0;
            }
            List<int> _tmp = NavigationPanelScrollCount();
            if (data.menuNavigationPanel_ScrollCount > _tmp.Count - data.menuNavigationPanel_SlotNumber)
            {
                data.menuNavigationPanel_ScrollCount = _tmp.Count - data.menuNavigationPanel_SlotNumber;
            }
            NavigationPanelScrollCount_Activation();
        }

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
            for (int _i = 0; _i < data.items.Count; ++_i)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).Name == ((ContentParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Content]).Content)
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
        #endregion

        #region ItemCreation

        public void eventCreateItem(string itemName)
        {
            ItemMakerContainer _im = data.itemCollection[itemName];
            if (_im == null)
            {
                return;
            }
            int _id = CreateItemUnit(_im);
            NavigationPanelScrollCount_Activation();
        }
        
        public int CreateItemUnit(ItemMakerContainer im, int parentId = -1, int conId = -1)
        {
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item;

            switch (im.Item)
            {
                case EItem.Canvas:
                    data.items.Add(new CanvasItem(data, _appCanvas, parentId));
                    break;
                case EItem.Label:
                    data.items.Add(new LabelItem(data, _appCanvas, parentId));
                    break;
                case EItem.Painter:
                    data.items.Add(new PainterItem(data, _appCanvas, parentId));
                    break;
                case EItem.Figure:
                    data.items.Add(new FigureItem(data, _appCanvas, parentId));
                    break;
                default:
                    break;
            }

            int _id = data.items.Count - 1;
            data.items[_id].MouseAppIdNotify += EventItemMenuHandler;
            ((CommonParameter)data.items[_id].param[EParameter.Common]).ItemAttach = EItemAttach.Custom;            

            //currParameters
            foreach (var _prop in im.Props)
            {
                data.items[_id].SetParameter(
                type: _prop.Value,
                    dParam: data.parameters[_prop.Key]);
            }
            //currEvents
            foreach (var _ev in im.Events)
            {
                switch (_ev)
                {
                    case EEvent.AddLine:
                        switch (im.Item)
                        {
                            case EItem.Canvas:
                                ((CanvasItem)data.items[_id]).EEventNotify += EventAddConnector;
                                break;
                            default:
                                break;
                        }
                        break;
                    case EEvent.Connector:                        
                        break;
                    default:
                        break;
                }
            }

            //children
            for (int _i = 0; _i < im.Children.Count; _i++)
            {
                CreateItemUnit(im.Children[_i], ((CommonParameter)data.items[_id].param[EParameter.Common]).Id);
            }

            //group id for connections
            if (conId != -1)
            {
                ((CommonParameter)data.items[_id].param[EParameter.Common]).Connect.GroupID = conId;
            }
            int _conId = conId;
            if (im.Connector.Count > 0 && _conId == -1)
            {
                _conId = Connection.GetID();
                ((CommonParameter)data.items[_id].param[EParameter.Common]).Connect.GroupID = _conId;
            }
            //connected
            for (int _i = 0; _i < im.Connector.Count; _i++)
            {
                CreateItemUnit(im:im.Connector[_i], conId:_conId);
            }

            MakeConnections(_id);

            return _id;
        }

        public void MakeConnections(int id)
        {
            Connection _con = ((CommonParameter)data.items[id].param[EParameter.Common]).Connect;
            if (_con.IsConnector)
            {
                if (_con.support.ContainsKey(EConnectorSupport.FromAncestorToLeft) || _con.support.ContainsKey(EConnectorSupport.FromAncestorToRight))
                {
                    //connector created from the ancestor

                } else
                if (_con.Users.Count == 0)
                {
                    //new connector added by found connector>specialId
                    for (int _i= 0; _i< data.items.Count; _i++)
                    {
                        if (((CommonParameter)data.items[_i].param[EParameter.Common]).Connect.IsUser && ((CommonParameter)data.items[_i].param[EParameter.Common]).Connect.GroupID == _con.GroupID)
                        {
                            _con.Users.Add(((CommonParameter)data.items[_i].param[EParameter.Common]).Id);
                        }
                    }
                }
            }
            if (_con.IsUser)
            {
                data.items[id].PrepareConnections();
            }
        }

        public void EventAddConnector(int id)
        {
            if (!data.btnControl && !data.btnAlt)
            {
                return;
            }

            int _ancestorArrIndex = data.GetItemIndexByID(id);
            CommonParameter _ancCommon = ((CommonParameter)data.items[_ancestorArrIndex].param[EParameter.Common]);
            ItemParameter _ancItem = (ItemParameter)data.items[_ancestorArrIndex].param[EParameter.Item];

            for (int _i = 0; _i < _ancCommon.Connect.Users.Count; _i++)
            {
                eventCreateItem("Connector");
                int _connArrIndex = data.items.Count - 1;

                CommonParameter _conCommon = (CommonParameter)data.items[_connArrIndex].param[EParameter.Common];
                _conCommon.ParentId = -1;
                _conCommon.Connect.GroupID = _ancCommon.Connect.GroupID;
                _conCommon.Connect.support.Add(EConnectorSupport.Ancestor, _ancCommon.Id);

                ItemParameter _conItem = (ItemParameter)data.items[_connArrIndex].param[EParameter.Item];
                _conItem.Left = _ancItem.Left;
                _conItem.Top = _ancItem.Top;
                Canvas.SetLeft(((CanvasItem)data.items[_connArrIndex]).Item, _ancItem.Left);
                Canvas.SetTop(((CanvasItem)data.items[_connArrIndex]).Item, _ancItem.Top);
                data.choosenItemID = _conCommon.Id;
                data.tapped = _conCommon.Id;

                if (data.btnControl)
                {
                    //next
                    _conCommon.Connect.support.Add(EConnectorSupport.FromAncestorToRight, _ancCommon.Id);
                }
                else if (data.btnAlt)
                {
                    //previous
                    _conCommon.Connect.support.Add(EConnectorSupport.FromAncestorToLeft, _ancCommon.Id);
                }

                int _userArrIndex = data.GetItemIndexByID(((CommonParameter)data.items[_ancestorArrIndex].param[EParameter.Common]).Connect.Users[0]);
                data.items[_userArrIndex].PrepareConnections();
            }
        }
        #endregion

        #region MenuHandling

        public void EventItemMenuHandler(int id)
        {
            ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Visible;
            data.isMenuItem = true;
            data.choosenItemID = id;
            ItemParametersMenuAdd();

            if (((CommonParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Common]).ItemType == EItem.Painter)
            {
                ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Visible;
                data.isMenuPainter = true;
                ItemPaintMakerMenuAdd();
            }
        }

        public void ItemPaintMakerMenuAdd()
        {
            ItemMenuDelete(data.menuItemPaintMakerID);
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item;
            MenuMaker.Make_PainterMakerMenu_Content(data, _appCanvas, this);
        }

        public void ItemParametersMenuAdd()
        {
            ItemMenuDelete(data.menuItemParametersID);
            //
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item;
            DefaultItem _item = data.items[data.GetItemIndexByID(data.choosenItemID)];
            MenuMaker.Make_ParameterMenu_Content(data, _appCanvas, _item, this);
        }

        public void ItemMenuDelete(int id)
        {
            ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Children.Clear();

            int _i = 0;
            while (_i < data.items.Count)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).ParentId == data.menuItemParametersID)
                {
                    data.items.RemoveAt(_i);
                    continue;
                }
                _i++;
            }
        }
        #endregion

        #region MoveMouse_ItemShift

        public void EventItemsShifter(double x, double y)
        {
            foreach (var item in data.items)
            {
                if (item.param.ContainsKey(EParameter.Event) && ((EventParameter)item.param[EParameter.Event]).IsMoveSensitive)
                {
                    switch (((CommonParameter)item.param[EParameter.Common]).ItemType)
                    {
                        case EItem.Figure:
                            item.HandlerShapeParam();
                            item.HandlerEParam();
                            break;
                        default:
                            item.SetParameter(EParameter.Item,
                                new ItemParameter(
                                    left: item.param.ContainsKey(EParameter.Item) ? ((ItemParameter)item.param[EParameter.Item]).Left + x : x,
                                    top: item.param.ContainsKey(EParameter.Item) ? ((ItemParameter)item.param[EParameter.Item]).Top + y : y,
                                    width: item.param.ContainsKey(EParameter.Item) ? ((ItemParameter)item.param[EParameter.Item]).Width : 1,
                                    height: item.param.ContainsKey(EParameter.Item) ? ((ItemParameter)item.param[EParameter.Item]).Height : 1,
                                    bgColor: item.param.ContainsKey(EParameter.Item) ? ((ItemParameter)item.param[EParameter.Item]).BgColor : Brushes.Black,
                                    frColor: item.param.ContainsKey(EParameter.Item) ? ((ItemParameter)item.param[EParameter.Item]).FrColor : Brushes.White
                                ));
                            break;
                    }
                }
            }
        }

        public void EventItemMoveHandler(double mouseX, double mouseY)
        {
            for (int _i = 0; _i < data.items.Count; _i++)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).Id == data.tapped)
                {
                    data.items[_i].SetParameter(
                        EParameter.Item,
                    new ItemParameter(
                    left: mouseX + data.tapXX,
                    top: mouseY + data.tapYY,
                    width: data.items[_i].param.ContainsKey(EParameter.Item) ? ((ItemParameter)data.items[_i].param[EParameter.Item]).Width : 1,
                    height: data.items[_i].param.ContainsKey(EParameter.Item) ? ((ItemParameter)data.items[_i].param[EParameter.Item]).Height : 1,
                    bgColor: null,
                    frColor: null
                            ));
                    ((CommonParameter)data.items[_i].param[EParameter.Common]).AppX = data.topLeftX + mouseX + data.tapXX;
                    ((CommonParameter)data.items[_i].param[EParameter.Common]).AppY = data.topLeftY + mouseY + data.tapYY;
                    break;
                }
            }
        }
        #endregion

        #region DeleteItem

        public void EventItemDeleteHandler(int id, ECommand command)
        {
            if (command == ECommand.DeleteItem)
            {
                List<int> _ids = new List<int>();
                _ids.Add(id);
                _ids.AddRange(SearchItemChildren(id));
                switch (((CommonParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Common]).ItemType)
                {
                    case EItem.Canvas:
                        ((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item.Children.Clear();
                        ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item.Children.Remove(((CanvasItem)data.items[data.GetItemIndexByID(id)]).Item);
                        break;
                    case EItem.Painter:
                        ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item.Children.Remove(((PainterItem)data.items[data.GetItemIndexByID(id)]).item);
                        break;
                    case EItem.Figure:
                        data.items[data.GetItemIndexByID(id)].HandleRemoveItem();
                        break;
                    default:
                        break;
                }
                //connections
                CommonParameter _comm = ((CommonParameter)data.items[data.GetItemIndexByID(id)].param[EParameter.Common]);
                _comm.Connect.GroupID = -2;
                //users
                for (int _i=0; _i < _comm.Connect.Users.Count; _i++)
                {
                    int _tmp = _comm.Connect.Users[_i];
                    _comm.Connect.Users.RemoveAt(_i);
                    EventDeleteUser(_tmp);
                }
                //connectors
                for (int _i = 0; _i < _comm.Connect.Connectors.Count; _i++)
                {
                    int _tmp = _comm.Connect.Connectors[_i];
                    EventDeleteConnector(_tmp, id);

                }

                //
                for (int _i = 0; _i < _ids.Count; _i++)
                {
                    data.items.RemoveAt(data.GetItemIndexByID(_ids[_i]));
                }
                //
                if (id == data.choosenItemID)
                {
                    ItemMenuDelete(data.menuItemParametersID);
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Hidden;
                    data.isMenuItem = false;
                    data.choosenItemID = -1;
                }
                if (data.isMenuPainter)
                {
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Hidden;
                    data.isMenuPainter = false;
                    ItemMenuDelete(data.menuItemPaintMakerID);
                }
                EventNavigationPanelScrollCount(0);
            }
        }

        public List<int> SearchItemChildren(int id)
        {
            List<int> _itms = new List<int>();
            for (int _i = 0; _i < data.items.Count; _i++)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).ParentId == id)
                {
                    _itms.Add(((CommonParameter)data.items[_i].param[EParameter.Common]).Id);
                    _itms.AddRange(SearchItemChildren(((CommonParameter)data.items[_i].param[EParameter.Common]).Id));
                }
            }
            return _itms;
        }

        public void EventDeleteUser(int id)
        {
            int _userArrIndex = data.GetItemIndexByID(id);
            data.items[_userArrIndex].HandlerShapeParam();
            data.items[_userArrIndex].HandlerEParam();

            if (((ShapeParameter)data.items[_userArrIndex].param[EParameter.Shape]).Vertex.Count == 0)
            {
                EventItemDeleteHandler(id, ECommand.DeleteItem);
            }
        }
        public void EventDeleteConnector(int id, int friedId)
        {
            int _conArrIndex = data.GetItemIndexByID(id);
            CommonParameter _conCommon = (CommonParameter)data.items[_conArrIndex].param[EParameter.Common];
             _conCommon.Connect.Users.Remove(friedId);
            if (_conCommon.Connect.Users.Count == 0)
            {
                EventItemDeleteHandler(id, ECommand.DeleteItem);
            }
        }
        #endregion

        #region KeyEvents
        public void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    if (data.choosenItemID != -1)
                    {
                        EventItemDeleteHandler(data.choosenItemID, ECommand.DeleteItem);
                    }
                    break;

                default:
                    break;
            }
        }

        public void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                data.btnControl = true;
            }
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                data.btnAlt = true;
            }
            KeyDownNotify?.Invoke(sender, e);
        }
        
        public void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            data.btnControl = false;
            data.btnAlt = false;
        }
        #endregion

        public void EventButtonClickHandler(int id, ECommand command)
        {
            switch (command)
            {
                case ECommand.Data_PainterTool:
                    switch (id)
                    {
                        case 1:
                            data.painterTool = EPainterTool.Move;
                            break;
                        case 2:
                            data.painterTool = EPainterTool.Draw;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}