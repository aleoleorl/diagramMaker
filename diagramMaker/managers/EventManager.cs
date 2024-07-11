using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using diagramMaker.managers.DefaultPreparation;
using System.Diagnostics;

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

        #region navigation
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
                        if (((CommonParameter)data.items[_i].param[EParameter.Common]).ConnectorId == -1)
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
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).Name == ((ContentParameter)data.items[data.GetItemByID(id)].param[EParameter.Content]).Content)
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
                                    ((FigureItem)data.items[_i]).ReVertex();
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

        public void eventCreateItem(string itemName)
        {
            ItemMaker _im = data.itemCollection[itemName];
            if (_im == null)
            {
                return;
            }
            int _id = CreateItemUnit(_im);
            //data.items[_id].MouseAppIdNotify += eventItemMenuHandler;
            //
            NavigationPanelScrollCount_Activation();
        }

        public int CreateItemUnit(ItemMaker im, int id = -1, bool connector = false)
        {
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item;

            switch (im.Item)
            {
                case EItem.Canvas:
                    data.items.Add(new CanvasItem(data, _appCanvas, connector ? -1 : id));
                    break;
                case EItem.Label:
                    data.items.Add(new LabelItem(data, _appCanvas, connector ? -1 : id));
                    break;
                case EItem.Painter:
                    data.items.Add(new PainterItem(data, _appCanvas, connector ? -1 : id));
                    break;
                case EItem.Figure:
                    data.items.Add(new FigureItem(data, _appCanvas, connector ? -1 : id));
                    break;
                default:
                    break;
            }
            int _id = data.items.Count - 1;
            data.items[_id].MouseAppIdNotify += EventItemMenuHandler;
            ((CommonParameter)data.items[_id].param[EParameter.Common]).ItemAttach = EItemAttach.Custom;
            ((CommonParameter)data.items[_id].param[EParameter.Common]).ConnectorId = connector ? id : -1;

            //children
            for (int _i = 0; _i < im.Children.Count; _i++)
            {
                CreateItemUnit(im.Children[_i], ((CommonParameter)data.items[_id].param[EParameter.Common]).Id);
            }
            //connected
            for (int _i = 0; _i < im.Connector.Count; _i++)
            {
                CreateItemUnit(im.Connector[_i], ((CommonParameter)data.items[_id].param[EParameter.Common]).Id, true);
            }
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
                                ((CanvasItem)data.items[_id]).EEventNotify += EventAddMultiLine;
                                break;
                            default:
                                break;
                        }
                        break;
                    case EEvent.Connector:
                        ((CommonParameter)data.items[_id].param[EParameter.Common]).ConnectorId = -2;
                        break;
                    default:
                        break;
                }
            }

            return _id;
        }

        public void EventItemMenuHandler(int id)
        {
            ((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Visible;
            data.isMenuItem = true;
            data.choosenItemID = id;
            ItemParametersMenuAdd();

            if (((CommonParameter)data.items[data.GetItemByID(id)].param[EParameter.Common]).ItemType == EItem.Painter)
            {
                ((CanvasItem)data.items[data.GetItemByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Visible;
                data.isMenuPainter = true;
                ItemPaintMakerMenuAdd();
            }
        }

        public void ItemPaintMakerMenuAdd()
        {
            ItemMenuDelete(data.menuItemPaintMakerID);
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item;
            MenuMaker.Make_PainterMakerMenu_Content(data, _appCanvas, this);
        }

        public void ItemParametersMenuAdd()
        {
            ItemMenuDelete(data.menuItemParametersID);
            //
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item;
            DefaultItem _item = data.items[data.GetItemByID(data.choosenItemID)];
            MenuMaker.Make_ParameterMenu_Content(data, _appCanvas, _item, this);
        }

        public void ItemMenuDelete(int id)
        {
            ((CanvasItem)data.items[data.GetItemByID(id)]).Item.Children.Clear();

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

        public void EventItemsShifter(double x, double y)
        {
            foreach (var item in data.items)
            {
                if (item.param.ContainsKey(EParameter.Event) && ((EventParameter)item.param[EParameter.Event]).IsMoveSensitive)
                {
                    switch (((CommonParameter)item.param[EParameter.Common]).ItemType)
                    {
                        case EItem.Figure:
                            ((FigureItem)item).HandlerShapeParam();
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

        public void EventItemDeleteHandler(int id, ECommand command)
        {
            if (command == ECommand.DeleteItem)
            {
                List<int> _ids = new List<int>();
                _ids.Add(id);
                _ids.AddRange(SearchItemChildren(id));
                switch (((CommonParameter)data.items[data.GetItemByID(id)].param[EParameter.Common]).ItemType)
                {
                    case EItem.Canvas:
                        ((CanvasItem)data.items[data.GetItemByID(id)]).Item.Children.Clear();
                        ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item.Children.Remove(((CanvasItem)data.items[data.GetItemByID(id)]).Item);
                        break;
                    case EItem.Painter:
                        ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item.Children.Remove(((PainterItem)data.items[data.GetItemByID(id)]).item);
                        break;
                    case EItem.Figure:
                        for (int _i = 0; _i < ((FigureItem)data.items[data.GetItemByID(id)]).Item.Count; _i++)
                        {
                            ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item.Children.Remove(((FigureItem)data.items[data.GetItemByID(id)]).Item[_i]);
                        }
                        int _j = 0;
                        while (_j < data.items.Count)
                        {
                            if (((CommonParameter)data.items[_j].param[EParameter.Common]).ConnectorId == id)
                            {
                                _ids.Add(((CommonParameter)data.items[_j].param[EParameter.Common]).Id);
                                switch (((CommonParameter)data.items[_j].param[EParameter.Common]).ItemType)
                                {
                                    case EItem.Canvas:
                                        ((CanvasItem)data.items[_j]).Item.Children.Clear();
                                        ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item.Children.Remove(((CanvasItem)data.items[_j]).Item);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            _j++;
                        }
                        break;
                    default:
                        break;
                }
                if (((CommonParameter)data.items[data.GetItemByID(id)].param[EParameter.Common]).ConnectorId != -1) 
                {
                    int _tmp = ((CommonParameter)data.items[data.GetItemByID(id)].param[EParameter.Common]).ConnectorId;
                    ((CommonParameter)data.items[data.GetItemByID(id)].param[EParameter.Common]).ConnectorId = -1;
                    EventDeleteMultiLine(_tmp);
                }
                //
                for (int _i = 0; _i < _ids.Count; _i++)
                {
                    data.items.RemoveAt(data.GetItemByID(_ids[_i]));
                }
                //
                if (id == data.choosenItemID)
                {
                    ItemMenuDelete(data.menuItemParametersID);
                    ((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Hidden;
                    data.isMenuItem = false;
                    data.choosenItemID = -1;
                }
                if (data.isMenuPainter)
                {
                    ((CanvasItem)data.items[data.GetItemByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Hidden;
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
            KeyDownNotify?.Invoke(sender, e);
        }
        
        public void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            data.btnControl = false;
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

        public void EventAddMultiLine(int id)
        {
            if (!data.btnControl)
            {
                return;
            }
            int _arrConnecterId = data.GetItemByID(id);
            var _shapeArrId = data.GetItemByID(((CommonParameter)data.items[_arrConnecterId].param[EParameter.Common]).ConnectorId);
            int _i = 0;
            bool _found = false;
            while (_i < ((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex.Count)
            {
                if (((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex[_i].id == id)
                {
                    _found = true;
                    break;
                }
                _i++;
            }
            if (_found)
            {
                eventCreateItem("Connector");
                int _connArrId = data.items.Count - 1;
                ((CommonParameter)data.items[_connArrId].param[EParameter.Common]).ParentId = -1;
                ((CommonParameter)data.items[_connArrId].param[EParameter.Common]).ConnectorId = ((CommonParameter)data.items[_shapeArrId].param[EParameter.Common]).Id;
                ((ItemParameter)data.items[_connArrId].param[EParameter.Item]).Left = ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Left;
                ((ItemParameter)data.items[_connArrId].param[EParameter.Item]).Top = ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Top;
                Canvas.SetLeft(((CanvasItem)data.items[_connArrId]).Item, ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Left);
                Canvas.SetTop(((CanvasItem)data.items[_connArrId]).Item, ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Top);

                if (_i == ((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex.Count - 1)
                {
                    ((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex.Add(
                        new FigureContainer(
                            x: ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Left,
                            y: ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Top,
                            id: ((CommonParameter)data.items[_connArrId].param[EParameter.Common]).Id
                            ));
                }
                else
                {
                    ((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex.Insert(
                       _i + 1,
                       new FigureContainer(
                           x: ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Left,
                           y: ((ItemParameter)data.items[_arrConnecterId].param[EParameter.Item]).Top,
                           id: ((CommonParameter)data.items[_connArrId].param[EParameter.Common]).Id
                           ));
                }
                data.choosenItemID = ((CommonParameter)data.items[_connArrId].param[EParameter.Common]).Id;
                data.tapped = ((CommonParameter)data.items[_connArrId].param[EParameter.Common]).Id;
                ((FigureItem)data.items[_shapeArrId]).HandlerShapeParam();
                ((FigureItem)data.items[_shapeArrId]).HandlerEParam();
            }
        }

        public void EventDeleteMultiLine(int id)
        {
            var _shapeArrId = data.GetItemByID(id);
            ((FigureItem)data.items[_shapeArrId]).HandlerShapeParam();
            ((FigureItem)data.items[_shapeArrId]).HandlerEParam();

            if (((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex.Count == 1)
            {
                EventItemDeleteHandler(
                    ((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex[0].id, 
                    ECommand.DeleteItem);
            } else if (((ShapeParameter)data.items[_shapeArrId].param[EParameter.Shape]).Vertex.Count == 0)
            {
                EventItemDeleteHandler(id, ECommand.DeleteItem);
            }
        }
    }
}