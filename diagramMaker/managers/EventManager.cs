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

namespace diagramMaker.managers
{
    public class EventManager
    {
        private DataHub Data;
        private MenuMaker Maker;

        public EventManager(DataHub data)
        {
            this.Data = data;
            Maker = new MenuMaker();
        }

        public void eventCreateItem(string itemName)
        {
            ItemMaker _im = Data.itemCollection[itemName];
            if (_im == null)
            {
                return;
            }
            int _id = CreateItemUnit(_im);
            Data.items[_id].MouseAppIdHandlerNotify += eventItemHandler;
        }

        public int CreateItemUnit(ItemMaker im, int id = -1)
        {
            Canvas _appCanvas = ((CanvasItem)Data.items[Data.GetItemByID(Data.appCanvasID)]).item;

            switch (im.Item)
            {
                case EItem.Canvas:
                    Data.items.Add(new CanvasItem(Data, _appCanvas, id));
                    break;
                case EItem.Label:
                    Data.items.Add(new LabelItem(Data, _appCanvas, id));
                    break;
                case EItem.Painter:
                    Data.items.Add(new PainterItem(Data, _appCanvas, id));
                    break;
                default:
                    break;
            }
            int _id = Data.items.Count - 1;
            foreach (var prop in im.Props)
            {
                Data.items[_id].setParameter(
                type: prop.Value,
                    dParam: Data.parameters[prop.Key]);
            }
            for (int _i = 0; _i < im.Children.Count; _i++)
            {
                CreateItemUnit(im.Children[_i], Data.items[_id].id);
            }
            return _id;
        }
        public void eventItemHandler(int id)
        {
            ((CanvasItem)Data.items[Data.GetItemByID(Data.MenuItemParametersID)]).item.Visibility = Visibility.Visible;
            Data.IsMenuItem = true;
            Data.ChoosenItemID = id;
            ItemParametersMenuAdd();

            if (Data.items[Data.GetItemByID(id)].itemType == EItem.Painter)
            {
                ((CanvasItem)Data.items[Data.GetItemByID(Data.MenuItemPaintMakerID)]).item.Visibility = Visibility.Visible;
                Data.IsMenuPainter = true;
                ItemPaintMakerMenuAdd();
            }
        }

        public void ItemPaintMakerMenuAdd()
        {
            ItemMenuDelete(Data.MenuItemPaintMakerID);
            Canvas _appCanvas = ((CanvasItem)Data.items[Data.GetItemByID(Data.appCanvasID)]).item;
            Maker.Make_PainterMakerMenu_Content(Data, _appCanvas, this);
        }

        public void ItemParametersMenuAdd()
        {
            ItemMenuDelete(Data.MenuItemParametersID);
            //
            Canvas _appCanvas = ((CanvasItem)Data.items[Data.GetItemByID(Data.appCanvasID)]).item;
            DefaultItem _item = Data.items[Data.GetItemByID(Data.ChoosenItemID)];
            Maker.Make_ParameterMenu_Content(Data, _appCanvas, _item, this);
        }

        public void ItemMenuDelete(int id)
        {
            ((CanvasItem)Data.items[Data.GetItemByID(id)]).item.Children.Clear();

            int _i = 0;
            while (_i < Data.items.Count)
            {
                if (Data.items[_i].parentId == Data.MenuItemParametersID)
                {
                    Data.items.RemoveAt(_i);
                    continue;
                }
                _i++;
            }
        }

        public void eventItemsShifter(double x, double y)
        {
            foreach (var item in Data.items)
            {
                if (item.eParam != null && item.eParam.moveSensitive)
                {
                    item.setParameter(EParameter.Item,
                        new ItemParameter(
                            left: item.iParam != null ? item.iParam.left + x : x,
                            top: item.iParam != null ? item.iParam.top + y : y,
                            width: item.iParam != null ? item.iParam.width : 1,
                            height: item.iParam != null ? item.iParam.height : 1,
                            bgColor: item.iParam != null ? item.iParam.bgColor : Brushes.Black,
                            frColor: item.iParam != null ? item.iParam.frColor : Brushes.White
                        ));
                }
            }
        }

        public void eventItemMoveHandler(double mouseX, double mouseY)
        {
            for (int _i = 0; _i < Data.items.Count; _i++)
            {
                if (Data.items[_i].id == Data.tapped)
                {
                    Data.items[_i].setParameter(
                        EParameter.Item,
                    new ItemParameter(
                    left: mouseX + Data.tapXX,
                    top: mouseY + Data.tapYY,
                    width: Data.items[_i].iParam != null ? Data.items[_i].iParam.width : 1,
                            height: Data.items[_i].iParam != null ? Data.items[_i].iParam.height : 1,
                    bgColor: null,
                    frColor: null
                            ));
                    Data.items[_i].appX = Data.topLeftX + mouseX + Data.tapXX;
                    Data.items[_i].appY = Data.topLeftY + mouseY + Data.tapYY;
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
                switch (Data.items[Data.GetItemByID(id)].itemType)
                {
                    case EItem.Canvas:
                        ((CanvasItem)Data.items[Data.GetItemByID(id)]).item.Children.Clear();
                        ((CanvasItem)Data.items[Data.GetItemByID(Data.appCanvasID)]).item.Children.Remove(((CanvasItem)Data.items[Data.GetItemByID(id)]).item);
                        break;
                    case EItem.Painter:
                        ((CanvasItem)Data.items[Data.GetItemByID(Data.appCanvasID)]).item.Children.Remove(((PainterItem)Data.items[Data.GetItemByID(id)]).item);
                        break;
                    default:
                        break;
                }
                //
                for (int _i = 0; _i < _ids.Count; _i++)
                {
                    Data.items.RemoveAt(Data.GetItemByID(_ids[_i]));
                }
                //
                if (id == Data.ChoosenItemID)
                {
                    ItemMenuDelete(Data.MenuItemParametersID);
                    ((CanvasItem)Data.items[Data.GetItemByID(Data.MenuItemParametersID)]).item.Visibility = Visibility.Hidden;
                    Data.IsMenuItem = false;
                    Data.ChoosenItemID = -1;
                }
                if (Data.IsMenuPainter)
                {
                    ((CanvasItem)Data.items[Data.GetItemByID(Data.MenuItemPaintMakerID)]).item.Visibility = Visibility.Hidden;
                    Data.IsMenuPainter = false;
                    ItemMenuDelete(Data.MenuItemPaintMakerID);
                }
            }
        }

        public List<int> SearchItemChildren(int id)
        {
            List<int> _itms = new List<int>();
            for (int _i=0; _i<Data.items.Count; _i++)
            {
                if (Data.items[_i].parentId == id)
                {
                    _itms.Add(Data.items[_i].id);
                    _itms.AddRange(SearchItemChildren(Data.items[_i].id));
                }
            }
            return _itms;
        }

        public void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Delete:
                    if (Data.ChoosenItemID != -1)
                    {
                        eventItemDeleteHandler(Data.ChoosenItemID, ECommand.DeleteItem);
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
                            Data.PainterTool = EPainterTool.Move;
                            break;
                        case 2:
                            Data.PainterTool = EPainterTool.Draw;
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
