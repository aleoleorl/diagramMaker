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
using System.Windows.Markup;
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
        private ExternalManager ExtManager;
       

        public Initiator(DataHub data, MainWindow mainWindow)
        {
            this.data = data;
            this.mainWindow = mainWindow;
            Maker = new MenuMaker();
            ExtManager = new ExternalManager(data);
        }
        public void Prepare()
        {
            DefaultPreparation();            
        }

        private void TopMenu_MenuHandlerNotify(ETopMenuActionType action, bool isIt)
        {
            switch (action)
            {
                case ETopMenuActionType.New:
                    ClearApp();
                    DefaultPreparation();
                    break;
                case ETopMenuActionType.Save:
                    ExtManager.Save();
                    break;
                case ETopMenuActionType.Load:
                    ExtManager.Load();
                    break;
                default:
                    break;
            }
        }

        private void ClearApp()
        {
            mainWindow.Width = 1024;
            mainWindow.Height = 768;
            mainWindow.ItemMoveNotify -= mainWindow.eventner.eventItemMoveHandler;
            mainWindow.PreviewKeyDown -= mainWindow.eventner.MainWindow_PreviewKeyDown;
            mainWindow.topMenu.MenuHandlerNotify -= TopMenu_MenuHandlerNotify;
            ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item.Children.Clear();
            data.ClearData();
            DefaultItem.RestartID();
        }


        public void DefaultPreparation()
        {
            setParameters();
            setItems();
            setItemCollections();
            //
            mainWindow.ItemMoveNotify += mainWindow.eventner.eventItemMoveHandler;
            mainWindow.PreviewKeyDown += mainWindow.eventner.MainWindow_PreviewKeyDown;
            //
            mainWindow.topMenu.SetMainMenu();
            mainWindow.topMenu.MenuHandlerNotify += TopMenu_MenuHandlerNotify;
        }
        
        public void setItems()
        {
            appCanvas = Maker.Make_AppCanvas(data, mainWindow);
            Maker.Make_InfoLine(data, mainWindow, appCanvas);
            Maker.Make_CreationMenu(data, mainWindow, appCanvas);
            Maker.Make_ParameterMenu(data, mainWindow, appCanvas);
            Maker.Make_PainterMakerMenu(data, mainWindow, appCanvas);
            Maker.Make_NavigationPanelMenu(data, mainWindow, appCanvas);
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
                mouseMove: true,
                mouseLeave: true
                );
            data.parameters.Add("eventPaintMaker", _ip);

            _ip = new BorderParameter(
                isBorder: true,
                borderThickness: 1,
                color: Colors.Black,
                cornerRadius: 1);
            data.parameters.Add("borderVersion01", _ip);

            _ip = new ItemParameter(
                left: 160,
                top: 50,
                width: 1,
                height: 1,
                bgColor: null,
                frColor: Brushes.Black);
            data.parameters.Add("itemLineContent", _ip);

            List<FigureContainer> _vertex = new List<FigureContainer>();
            _vertex.Add(new FigureContainer(x:160, y:50, id:-1));
            _vertex.Add(new FigureContainer(x: 190, y: 50, id: -1));
            _ip = new ShapeParameter(
                shape: EShape.Line,
                vertex: _vertex,
                color: Colors.Black,
                strokeThickness: 2);
            data.parameters.Add("figureLineContent", _ip);
            _ip = new EventParameter(
                moveSensitive: true,
                mouseDown: true,
                mouseMove: true
                );
            data.parameters.Add("eventLineContent", _ip);


            _ip = new ItemParameter(
                left: 160,
                top: 50,
                width: 5,
                height: 5,
                bgColor: Brushes.Black,
                frColor: null);
            data.parameters.Add("itemLineConnect01Content", _ip);

            _ip = new ItemParameter(
                left: 190,
                top: 50,
                width: 5,
                height: 5,
                bgColor: Brushes.Black,
                frColor: null);
            data.parameters.Add("itemLineConnect02Content", _ip);

            _ip = new EventParameter(
                moveSensitive: true,
                mouseUp: true,
                mouseDown: true,
                mouseMove: true
                );
            data.parameters.Add("eventLineConnect", _ip);

        }

        public void setItemCollections()
        {
            ItemMaker _im;
            ItemMaker _imChild;
            ItemMaker _imConnect;

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

            //line
            _im = new ItemMaker();
            _im.Props.Add("itemLineContent", EParameter.Item);
            _im.Props.Add("figureLineContent", EParameter.Shape);
            _im.Props.Add("eventLineContent", EParameter.Event);
            _im.Item = EItem.Figure;
            data.itemCollection.Add("Line", _im);

            _imConnect = new ItemMaker();
            _imConnect.Props.Add("itemLineConnect01Content", EParameter.Item);
            _imConnect.Props.Add("eventLineConnect", EParameter.Event);
            _imConnect.Item = EItem.Canvas;
            _im.Connector.Add(_imConnect);

            _imConnect = new ItemMaker();
            _imConnect.Props.Add("itemLineConnect02Content", EParameter.Item);
            _imConnect.Props.Add("eventLineConnect", EParameter.Event);
            _imConnect.Item = EItem.Canvas;
            _im.Connector.Add(_imConnect);
        }        
    }
}
