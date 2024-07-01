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
using diagramMaker.managers.DefaultPreparation;
using diagramMaker.parameters;

namespace diagramMaker.managers
{
    public class Initiator
    {
        private DataHub data;
        private Canvas? appCanvas;
        private MainWindow mainWindow;
        private MenuMaker Maker;

        public Initiator(DataHub data, MainWindow mainWindow)
        {
            this.data = data;
            this.mainWindow = mainWindow;
            Maker = new MenuMaker();
        }
        public void Prepare()
        {
            setParameters();
            setItems();
            setItemCollections();

            mainWindow.itemMoveHandlerNotify += mainWindow.eventner.eventItemMoveHandler;
            mainWindow.PreviewKeyDown += mainWindow.eventner.MainWindow_PreviewKeyDown;
        }

        public void setItems()
        {
            appCanvas = Maker.Make_AppCanvas(data, mainWindow);
            Maker.Make_InfoLine(data, mainWindow, appCanvas);
            Maker.Make_CreationMenu(data, mainWindow, appCanvas);
            Maker.Make_ParameterMenu(data, mainWindow, appCanvas);
            Maker.Make_PainterMakerMenu(data, mainWindow, appCanvas);
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

            _ip = new ItemParameter(
                left: 160,
                top: 33,
                width: 100,
                height: 100,
                bgColor: null,
                frColor: null);
            data.parameters.Add("itemPaintMaker", _ip);

            _ip = new EventParameter(
                moveSensitive: true,
                mouseUp: true,
                mouseDown: true,
                mouseMove: true
                );
            data.parameters.Add("eventPaintMaker", _ip);

            _ip = new BorderParameter(
                isBorder: true,
                borderThickness: 1,
                color: Colors.Black,
                cornerRadius: 1);
            data.parameters.Add("borderVersion01", _ip);
        }

        public void setItemCollections()
        {
            ItemMaker _im;
            ItemMaker _imChild;

            //infoBlock
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

            //PaintMaker
            _im = new ItemMaker();
            _im.Props.Add("itemPaintMaker", EParameter.Item);
            _im.Props.Add("borderVersion01", EParameter.Border);
            _im.Props.Add("eventPaintMaker", EParameter.Event);
            _im.Item = EItem.Painter;
            data.itemCollection.Add("PaintMaker", _im);
        }        
    }
}
