using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;

namespace diagramMaker.managers
{
    public class Initiator
    {
        private DataHub data;
        private Canvas? appCanvas;
        private MainWindow mainWindow;

        public Initiator(DataHub data, MainWindow mainWindow)
        {
            this.data = data;
            this.mainWindow = mainWindow;
        }
        public void Prepare()
        {
            setParameters();
            setItems();
            setItemCollections();

            mainWindow.itemMoveHandlerNotify += mainWindow.eventner.eventItemMoveHandler;
        }

        public void setItems()
        {
            //appCanvas
            data.items.Add(new CanvasItem(data, null));
            data.items[data.items.Count - 1].setParameters(
                iParam: (ItemParameter)data.parameters["itemCanvasDefault"],
                content: null,
                bParam: null,
                eParam: new EventParameter(moveSensitive: false, mouseDown: true, mouseUp: false)
                );
            appCanvas = ((CanvasItem)data.items[data.items.Count - 1]).item;
            mainWindow.Content = appCanvas;
            data.appCanvasID = data.items[data.items.Count - 1].id;
            Trace.WriteLine("data.appCanvasID:" + data.appCanvasID);

            int _tmpId = data.items[data.items.Count - 1].id;

            //info line
            data.items.Add(new LabelItem(data, appCanvas));
            data.items[data.items.Count - 1].setParameters(
                iParam: (ItemParameter)data.parameters["itemLabelInfoLine"],
                content: new ContentParameter(content: "info line", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Gray, cornerRadius: 0),
                eParam: new EventParameter(moveSensitive: false)
                );
            data.informer = data.items.Count - 1;
            mainWindow.cihNotify += ((LabelItem)data.items[data.informer]).setContent;
            mainWindow.appCanvasMoveNotify += ((LabelItem)data.items[data.informer]).setContent;
            Panel.SetZIndex(((LabelItem)data.items[data.informer]).item, 30);
            mainWindow.appCanvasMoveHandlerNotify += mainWindow.eventner.eventItemsShifter;

            //item creation menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(5, 10, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.DarkSlateGray, cornerRadius: 2),
                eParam: new EventParameter(moveSensitive: false, mouseDown: true)
                );
            data.items[data.items.Count - 1].appX = data.topLeftX + 5;
            data.items[data.items.Count - 1].appY = data.topLeftY + 10;
            data.menuCreation = data.items.Count - 1;
            Panel.SetZIndex(((CanvasItem)data.items[data.menuCreation]).item, 20);

            //menuCreation > txt label
            data.items.Add(new LabelItem(data, appCanvas, data.items[data.menuCreation].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(0, 3, 144, 30, null, Brushes.Black),
                content: new ContentParameter(content: "Items Menu:", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //
            //menuCreation > item add info block
            data.items.Add(new CanvasItem(data, appCanvas, data.items[data.menuCreation].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 33, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2),
                eParam: new EventParameter(mouseUp: true, mouseUpInfo: "InfoBlock")
                );
            data.menuCreationCanvasID = data.items[data.items.Count - 1].id;
            data.items[data.items.Count - 1].MouseAppHandlerNotify += mainWindow.eventner.eventCreateItem;

            //menuCreation > item info block > img
            data.items.Add(new ImageItem(data, appCanvas, data.menuCreationCanvasID));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 3, 44, 44, null, Brushes.Black),
                content: null,
                bParam: null,
                eParam: null,
                imgParam: new ImageParameter("..\\..\\assets\\item01.png")
                );

            //menuCreation > item info block > txt
            data.items.Add(new LabelItem(data, appCanvas, data.menuCreationCanvasID));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(50, 3, 99, 44, null, Brushes.Black),
                content: new ContentParameter(content: "info block", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //
            //menuCreation > item add paint canvas
            data.items.Add(new CanvasItem(data, appCanvas, data.items[data.menuCreation].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 84, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2),
                eParam: null
                );
            data.menuCreationCanvasID = data.items[data.items.Count - 1].id;

            //menuCreation > item paint canvas > img
            data.items.Add(new ImageItem(data, appCanvas, data.menuCreationCanvasID));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 3, 44, 44, null, Brushes.Black),
                content: null,
                bParam: null,
                eParam: null,
                imgParam: new ImageParameter("C:\\alar_work\\diagramMaker\\diagramMaker\\assets\\item02.png")
                );

            //menuCreation > item paint canvas > txt
            data.items.Add(new LabelItem(data, appCanvas, data.menuCreationCanvasID));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(50, 3, 99, 44, null, Brushes.Black),
                content: new ContentParameter(content: "image maker", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );

            //
            //menuCreation > item add line
            data.items.Add(new CanvasItem(data, appCanvas, data.items[data.menuCreation].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 135, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2),
                eParam: null
                );
            data.menuCreationCanvasID = data.items[data.items.Count - 1].id;

            //menuCreation > item line > img
            data.items.Add(new ImageItem(data, appCanvas, data.menuCreationCanvasID));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 3, 44, 44, null, Brushes.Black),
                content: null,
                bParam: null,
                eParam: null,
                imgParam: new ImageParameter("C:\\alar_work\\diagramMaker\\diagramMaker\\assets\\item03.png")
                );

            //menuCreation > item line > txt
            data.items.Add(new LabelItem(data, appCanvas, data.menuCreationCanvasID));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(50, 3, 99, 44, null, Brushes.Black),
                content: new ContentParameter(content: "line", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //
            //
            //
            //item item menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(5, 10, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.DarkSlateGray, cornerRadius: 2),
                eParam: new EventParameter(moveSensitive: false, mouseDown: true)
                );
            data.items[data.items.Count - 1].appX = data.topLeftX + 5;
            data.items[data.items.Count - 1].appY = data.topLeftY + 10;
            data.menuCreation = data.items.Count - 1;
            Panel.SetZIndex(((CanvasItem)data.items[data.menuCreation]).item, 21);
            data.MenuItemParametersID = data.items[data.items.Count - 1].id;
            ((CanvasItem)data.items[data.items.Count - 1]).item.Visibility = Visibility.Hidden;

            
        }

        public void setParameters()
        {
            DefaultParameter _ip;
            //

            _ip = new ItemParameter(
                left: 0,
                top: 0,
                width: data.screenWidth,
                height: data.screenHeight,
                bgColor: Brushes.LightSteelBlue,
                frColor: null);
            data.parameters.Add("itemCanvasDefault", _ip);

            _ip = new ItemParameter(
                left: 0,
                top: data.winHeight - 45,
                width: data.winWidth - 2,
                height: 30,
                bgColor: Brushes.LightCyan,
                frColor: Brushes.Black);
            data.parameters.Add("itemLabelInfoLine", _ip);

            _ip = new ItemParameter(
                left: 160,
                top: 33,
                width: 120,
                height: 40,
                bgColor: Brushes.LightGray,
                frColor: null);
            data.parameters.Add("itemInfoBlock", _ip);

            _ip = new BorderParameter(
                isBorder: true,
                borderThickness: 3,
                color: Colors.Black,
                cornerRadius: 2);
            data.parameters.Add("borderInfoBlock", _ip);

            _ip = new EventParameter(
                moveSensitive: true,
                mouseUp: true,
                mouseDown: true
                );
            data.parameters.Add("eventInfoBlock", _ip);

            _ip = new ItemParameter(
                left: 0,
                top: 0,
                width: 120,
                height: 40,
                bgColor: null,
                frColor: Brushes.Black);
            data.parameters.Add("itemTextInfoBlock", _ip);

            _ip = new ContentParameter(
                content: "Item",
                horAlign: HorizontalAlignment.Center,
                verAlign: VerticalAlignment.Center);
            data.parameters.Add("contentTextInfoBlock", _ip);

            _ip = new EventParameter(
                IsHitTestVisible: false
                );
            data.parameters.Add("eventNoHit", _ip);
        }

        public void setItemCollections()
        {
            ItemMaker _im;
            ItemMaker _imChild;
            _im = new ItemMaker();
            _im.Props.Add("itemInfoBlock", EParameter.Item);
            _im.Props.Add("borderInfoBlock", EParameter.Border);
            _im.Props.Add("eventInfoBlock", EParameter.Event);
            _im.Item = EItem.Canvas;
            data.itemCollection.Add("InfoBlock", _im);

            _imChild = new ItemMaker();
            _imChild.Props.Add("itemTextInfoBlock", EParameter.Item);
            _imChild.Props.Add("contentTextInfoBlock", EParameter.Content);
            _imChild.Props.Add("eventNoHit", EParameter.Event);
            _imChild.Item = EItem.Label;
            _im.Children.Add(_imChild);
        }

        
    }
}
