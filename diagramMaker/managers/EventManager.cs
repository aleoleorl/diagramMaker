using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Windows.Input;
using diagramMaker.managers.DefaultPreparation;
using System.Diagnostics.Metrics;
using System.Security.Policy;
using System.Diagnostics;

namespace diagramMaker.managers
{
    public class EventManager
    {
        private DataHub data;
        private MenuMaker maker;

        public delegate void NavigationItemHandler(ECommand command, List<int> items);
        public event NavigationItemHandler? NavigationItemNotify;

        

        public EventManager(DataHub data)
        {
            this.data = data;
            maker = new MenuMaker();
        }

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
            while (_cnt>0)
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
                if (data.items[_i].itemAttach == EItemAttach.Custom)
                {
                    if (data.items[_i].parentId == -1)
                    {
                        if (data.items[_i].connectorId == -1)
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
            Trace.WriteLine("!id:"+id);
            Trace.WriteLine("!content:" + ((LabelItem)data.items[data.GetItemByID(id)]).content.content);
            int _id = -1;
            for (int _i=0; _i<data.items.Count; ++_i)
            {
                if (data.items[_i].name == ((LabelItem)data.items[data.GetItemByID(id)]).content.content)
                {
                    _id = _i; 
                    break;
                }
            }
            if (_id != -1)
            {
                data.topLeftX = Math.Floor(data.items[_id].appX - data.winWidth / 2);
                data.topLeftY = Math.Floor(data.items[_id].appY - data.winHeight / 2);

                //connectedId item go after main item. There are no events now to check viceversa
                for (int _i = data.items.Count - 1; _i >=0 ; _i--)
                {
                    if (data.items[_i].parentId == -1)
                    {
                        if (data.items[_i].eParam != null && data.items[_i].eParam.isMoveSensitive)
                        {
                            switch(data.items[_i].itemType)
                            {
                                case EItem.Canvas:
                                    Canvas.SetLeft(((CanvasItem)data.items[_i]).item, data.items[_i].appX - data.topLeftX);
                                    Canvas.SetTop(((CanvasItem)data.items[_i]).item, data.items[_i].appY - data.topLeftY);
                                    if (data.items[_i].iParam != null)
                                    {
                                        data.items[_i].iParam.left = data.items[_i].appX - data.topLeftX;
                                        data.items[_i].iParam.top = data.items[_i].appY - data.topLeftY;
                                    }
                                    break;
                                case EItem.Painter:
                                    Canvas.SetLeft(((PainterItem)data.items[_i]).item, data.items[_i].appX - data.topLeftX);
                                    Canvas.SetTop(((PainterItem)data.items[_i]).item, data.items[_i].appY - data.topLeftY);
                                    if (data.items[_i].iParam != null)
                                    {
                                        data.items[_i].iParam.left = data.items[_i].appX - data.topLeftX;
                                        data.items[_i].iParam.top = data.items[_i].appY - data.topLeftY;
                                    }
                                    break;
                                case EItem.Figure:
                                    if (data.items[_i].iParam != null)
                                    {
                                        data.items[_i].iParam.left = data.items[_i].appX - data.topLeftX;
                                        data.items[_i].iParam.top = data.items[_i].appY - data.topLeftY;
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

        public void eventCreateItem(string itemName)
        {
            ItemMaker _im = data.itemCollection[itemName];
            if (_im == null)
            {
                return;
            }
            int _id = CreateItemUnit(_im);
            data.items[_id].MouseAppIdNotify += eventItemMenuHandler;
            //
            NavigationPanelScrollCount_Activation();
        }

        public int CreateItemUnit(ItemMaker im, int id = -1, bool connector = false)
        {
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item;

            switch (im.Item)
            {
                case EItem.Canvas:
                    data.items.Add(new CanvasItem(data, _appCanvas, connector? -1: id));
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
            data.items[_id].itemAttach = EItemAttach.Custom;
            data.items[_id].connectorId = connector ? id : -1;            
            //children
            for (int _i = 0; _i < im.Children.Count; _i++)
            {
                CreateItemUnit(im.Children[_i], data.items[_id].id);
            }
            //connected
            for (int _i = 0; _i < im.Connector.Count; _i++)
            {
                CreateItemUnit(im.Connector[_i], data.items[_id].id, true);
            }
            //currParameters
            foreach (var prop in im.Props)
            {
                data.items[_id].setParameter(
                type: prop.Value,
                    dParam: data.parameters[prop.Key]);
            }

            return _id;
        }
        public void eventItemMenuHandler(int id)
        {
            ((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).item.Visibility = Visibility.Visible;
            Trace.WriteLine("Left:"+((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).iParam.left);
            Trace.WriteLine("Top:" + ((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).iParam.top);
            data.isMenuItem = true;
            data.choosenItemID = id;
            ItemParametersMenuAdd();

            if (data.items[data.GetItemByID(id)].itemType == EItem.Painter)
            {
                ((CanvasItem)data.items[data.GetItemByID(data.menuItemPaintMakerID)]).item.Visibility = Visibility.Visible;
                data.isMenuPainter = true;
                ItemPaintMakerMenuAdd();
            }
        }

        public void ItemPaintMakerMenuAdd()
        {
            ItemMenuDelete(data.menuItemPaintMakerID);
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item;
            maker.Make_PainterMakerMenu_Content(data, _appCanvas, this);
        }

        public void ItemParametersMenuAdd()
        {
            ItemMenuDelete(data.menuItemParametersID);
            //
            Canvas _appCanvas = ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item;
            DefaultItem _item = data.items[data.GetItemByID(data.choosenItemID)];
            maker.Make_ParameterMenu_Content(data, _appCanvas, _item, this);
        }

        public void ItemMenuDelete(int id)
        {
            ((CanvasItem)data.items[data.GetItemByID(id)]).item.Children.Clear();

            int _i = 0;
            while (_i < data.items.Count)
            {
                if (data.items[_i].parentId == data.menuItemParametersID)
                {
                    data.items.RemoveAt(_i);
                    continue;
                }
                _i++;
            }
        }

        public void eventItemsShifter(double x, double y)
        {
            foreach (var item in data.items)
            {
                if (item.eParam != null && item.eParam.isMoveSensitive)
                {
                    switch (item.itemType)
                    {
                        case EItem.Figure:
                            ((FigureItem)item).handlerShapeParam();
                            break;
                        default:
                    item.setParameter(EParameter.Item,
                        new ItemParameter(
                            left: item.iParam != null ? item.iParam.left + x : x,
                            top: item.iParam != null ? item.iParam.top + y : y,
                            width: item.iParam != null ? item.iParam.width : 1,
                            height: item.iParam != null ? item.iParam.height : 1,
                            bgColor: item.iParam != null ? item.iParam.bgColor : Brushes.Black,
                            frColor: item.iParam != null ? item.iParam.frColor : Brushes.White
                        ));
                            break;
                }
                }
            }
        }

        public void eventItemMoveHandler(double mouseX, double mouseY)
        {
            for (int _i = 0; _i < data.items.Count; _i++)
            {
                if (data.items[_i].id == data.tapped)
                {
                    data.items[_i].setParameter(
                        EParameter.Item,
                    new ItemParameter(
                    left: mouseX + data.tapXX,
                    top: mouseY + data.tapYY,
                    width: data.items[_i].iParam != null ? data.items[_i].iParam.width : 1,
                            height: data.items[_i].iParam != null ? data.items[_i].iParam.height : 1,
                    bgColor: null,
                    frColor: null
                            ));
                    data.items[_i].appX = data.topLeftX + mouseX + data.tapXX;
                    data.items[_i].appY = data.topLeftY + mouseY + data.tapYY;
                    break;
                }
            }
        }

        public void eventItemDeleteHandler(int id, ECommand command)
        {
            if (command == ECommand.DeleteItem)
            {
                List<int> _ids = new List<int>();
                _ids.Add(id);
                _ids.AddRange(SearchItemChildren(id));
                switch (data.items[data.GetItemByID(id)].itemType)
                {
                    case EItem.Canvas:
                        ((CanvasItem)data.items[data.GetItemByID(id)]).item.Children.Clear();
                        ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item.Children.Remove(((CanvasItem)data.items[data.GetItemByID(id)]).item);
                        break;
                    case EItem.Painter:
                        ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item.Children.Remove(((PainterItem)data.items[data.GetItemByID(id)]).item);
                        break;
                    case EItem.Figure:
                        for (int _i=0; _i< ((FigureItem)data.items[data.GetItemByID(id)]).item.Count; _i++)
                        {
                            ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item.Children.Remove(((FigureItem)data.items[data.GetItemByID(id)]).item[_i]);
                        }
                        int _j = 0;
                        while (_j < data.items.Count)
                        {
                            if (data.items[_j].connectorId == id)
                            {
                                _ids.Add(data.items[_j].id);
                                switch (data.items[_j].itemType)
                                {
                                    case EItem.Canvas:
                                        ((CanvasItem)data.items[_j]).item.Children.Clear();
                                        ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item.Children.Remove(((CanvasItem)data.items[_j]).item);
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
                //
                for (int _i = 0; _i < _ids.Count; _i++)
                {
                    data.items.RemoveAt(data.GetItemByID(_ids[_i]));
                }
                //
                if (id == data.choosenItemID)
                {
                    ItemMenuDelete(data.menuItemParametersID);
                    ((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).item.Visibility = Visibility.Hidden;
                    data.isMenuItem = false;
                    data.choosenItemID = -1;
                }
                if (data.isMenuPainter)
                {
                    ((CanvasItem)data.items[data.GetItemByID(data.menuItemPaintMakerID)]).item.Visibility = Visibility.Hidden;
                    data.isMenuPainter = false;
                    ItemMenuDelete(data.menuItemPaintMakerID);
                }
                EventNavigationPanelScrollCount(0);
            }
        }

        public List<int> SearchItemChildren(int id)
        {
            List<int> _itms = new List<int>();
            for (int _i=0; _i<data.items.Count; _i++)
            {
                if (data.items[_i].parentId == id)
                {
                    _itms.Add(data.items[_i].id);
                    _itms.AddRange(SearchItemChildren(data.items[_i].id));
                }
            }
            return _itms;
        }

        public void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Delete:
                    if (data.choosenItemID != -1)
                    {
                        eventItemDeleteHandler(data.choosenItemID, ECommand.DeleteItem);
                    }
                    break;
                default:
                    break;
            }
        }

        public void eventButtonClickHandler(int id, ECommand command)
        {
            switch(command)
            {
                case ECommand.Data_PainterTool:
                    switch(id)
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
