using diagramMaker.helpers.enumerators;
using diagramMaker.helpers.containers;
using diagramMaker.items;
using diagramMaker.parameters;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;

namespace diagramMaker.managers
{
    public class EventManager
    {
        private DataHub data;
        private DefaultManager defMan;

        public delegate void KeyDownHandler(object sender, KeyEventArgs e);
        public event KeyDownHandler? KeyDownNotify;
        public delegate void ChoosenItemHandler(EBindParameter eBindParameter, string txt);
        public event ChoosenItemHandler? ChoosenItemNotify;

        public EventManager(DataHub data, DefaultManager defMan)
        {
            this.data = data;
            this.defMan = defMan;
        }        

        #region ItemCreation

        public void eventCreateItem(string itemName)
        {
            ItemMakerContainer _im = data.itemCollection[itemName];
            if (_im == null)
            {
                return;
            }
            int _index = CreateItemUnit(_im);
            defMan.navPanel.NavigationPanel_AddItem(_index);
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

            int _index = data.items.Count - 1;
            data.items[_index].MouseAppIdNotify += EventItemMenuHandler;
            ((CommonParameter)data.items[_index].param[EParameter.Common]).ItemAttach = EItemAttach.Custom;            

            //currParameters
            foreach (var _prop in im.Props)
            {
                data.items[_index].SetParameter(
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
                                ((CanvasItem)data.items[_index]).EEventNotify += EventAddConnector;
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
                CreateItemUnit(im.Children[_i], ((CommonParameter)data.items[_index].param[EParameter.Common]).Id);
            }

            //group id for connections
            if (conId != -1)
            {
                ((CommonParameter)data.items[_index].param[EParameter.Common]).Connect.GroupID = conId;
            }
            int _conId = conId;
            if (im.Connector.Count > 0 && _conId == -1)
            {
                _conId = Connection.GetID();
                ((CommonParameter)data.items[_index].param[EParameter.Common]).Connect.GroupID = _conId;
            }
            //connected
            for (int _i = 0; _i < im.Connector.Count; _i++)
            {
                CreateItemUnit(im:im.Connector[_i], conId:_conId);
            }

            MakeConnections(_index);

            return _index;
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

        #region PanelsHandling

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
            }
        }

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

        public void ItemParametersMenuAdd()
        {
            ChoosenItemNotify?.Invoke(EBindParameter.Content, data.choosenItemID.ToString());

            ItemMenuDelete(data.menuItemParametersID);

            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item;
            MenuMakeOptions _option = new MenuMakeOptions();
            var _panel = data.panel["itemParametersPanel"];
            _option.parentId = _panel.menu[0].childrenId[0];

            for (int _i=0; _i<_panel.menu[0].option.addFunc.Count; _i++)
            {
                _panel.menu[0].option.addFunc[_i](data, defMan, _appCanvas, _option, _i);
            }
        }

        public void ItemMenuDelete(int id)
        {
            foreach (var panel in data.panel)
            {
                if (panel.Value.itemId == id)
                {
                    Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item;
                    MenuMakeOptions _option = new MenuMakeOptions();
                    _option.parentId = panel.Value.menu[0].childrenId[0];

                    for (int _i = 0; _i < panel.Value.menu[0].option.addFunc.Count; _i++)
                    {
                        panel.Value.menu[0].option.delFunc[_i](data, defMan, _appCanvas, _option, _i);
                    }
                }
            }
        }

        public void ParameterMenuAdd(int id)
        {

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
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).ParentId == -1 &&
                    ((CommonParameter)data.items[_i].param[EParameter.Common]).Id == data.tapped)
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
                if (_comm.Connect.IsConnector)
                {
                    for (int _i = 0; _i < _comm.Connect.Users.Count; _i++)
                    {
                        int _tmp = _comm.Connect.Users[_i];
                        _comm.Connect.Users.RemoveAt(_i);
                        EventDeleteUser(_tmp);
                    }
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
                defMan.navPanel.NavigationPanel_DeleteItem(id);
                //EventNavigationPanelScrollCount(0);
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

    }
}