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

namespace diagramMaker.managers
{
    public class EventManager
    {
        private DataHub Data;

        public EventManager(DataHub data)
        {
            this.Data = data;
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
        public void eventItemHandler(int _id)
        {
            ((CanvasItem)Data.items[Data.GetItemByID(Data.MenuItemParametersID)]).item.Visibility = Visibility.Visible;
            Data.IsMenuItem = true;
            Data.ChoosenItemID = _id;
            ItemParametersMenuAdd();
        }

        public void ItemParametersMenuAdd()
        {
            ItemParametersMenuDelete();
            //
            Canvas _appCanvas = ((CanvasItem)Data.items[Data.GetItemByID(Data.appCanvasID)]).item;

            //menuCreation > txt label
            Data.items.Add(new LabelItem(Data, _appCanvas, Data.MenuItemParametersID));
            Data.items[Data.items.Count - 1].setParameters(
                iParam: new ItemParameter(0, 3, 144, 30, null, Brushes.Black),
                content: new ContentParameter(
                    content: "Item Menu:",
                    horAlign: HorizontalAlignment.Center,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );

            //ItemParametersMenu > label id
            Data.items.Add(new LabelItem(Data, _appCanvas, Data.MenuItemParametersID));
            Data.items[Data.items.Count - 1].setParameters(
                iParam: new ItemParameter(0, 30, 144, 26, null, Brushes.Black),
                content: new ContentParameter(
                    content: "id:" + Data.ChoosenItemID.ToString(), 
                    horAlign: HorizontalAlignment.Left, 
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > label name:
            Data.items.Add(new LabelItem(Data, _appCanvas, Data.MenuItemParametersID));
            Data.items[Data.items.Count - 1].setParameters(
                iParam: new ItemParameter(0, 50, 46, 26, null, Brushes.Black),
                content: new ContentParameter(content: "name:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > text name:
            Data.items.Add(new TextItem(Data, _appCanvas, Data.MenuItemParametersID));
            Data.items[Data.items.Count - 1].setParameters(
                iParam: new ItemParameter(46, 50, 98, 26, null, Brushes.Black),
                content: new ContentParameter(
                    content: Data.items[Data.GetItemByID(Data.ChoosenItemID)].name, 
                    horAlign: HorizontalAlignment.Left, 
                    verAlign: VerticalAlignment.Center,
                    IsTextChanged: true,
                    BindParameter: EBindParameter.Name,
                    BindID: Data.ChoosenItemID),
                bParam: null,
                eParam: null
                );
            int _i = 0;
            int _coord = 50 + 26;
            while (_i < Data.items.Count)
            {
                DefaultItem _item = Data.items[_i];
                if (_item.parentId == Data.ChoosenItemID)
                {
                    var a = _item.GetType();
                    switch (_item.GetType().Name)
                    {
                        case "LabelItem":
                            //label text:
                            Data.items.Add(new LabelItem(Data, _appCanvas, Data.MenuItemParametersID));
                            Data.items[Data.items.Count - 1].setParameters(
                                iParam: new ItemParameter(0, _coord, 52, 26, null, Brushes.Black),
                                content: new ContentParameter(
                                    content: "content:", 
                                    horAlign: HorizontalAlignment.Left, 
                                    verAlign: VerticalAlignment.Center),
                                bParam: null,
                                eParam: null
                                );
                            // text content:
                            Data.items.Add(new TextItem(Data, _appCanvas, Data.MenuItemParametersID));
                            Data.items[Data.items.Count - 1].setParameters(
                                iParam: new ItemParameter(52, _coord, 92, 26, null, Brushes.Black),
                                content: new ContentParameter(
                                    content: ((LabelItem)Data.items[Data.GetItemByID(_item.id)]).item.Content.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    IsTextChanged: true,
                                    BindParameter: EBindParameter.Content,
                                    BindID: _item.id),
                                bParam: null,
                                eParam: null
                                );
                            _coord += 26;
                            break;
                        default:
                            break;
                    }
                }
                _i++;
            }
        }
        public void ItemParametersMenuDelete()
        {
            ((CanvasItem)Data.items[Data.GetItemByID(Data.MenuItemParametersID)]).item.Children.Clear();
           
            int _i = 0;
            while (_i< Data.items.Count) 
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
    }
}
