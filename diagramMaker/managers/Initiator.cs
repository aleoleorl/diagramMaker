using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.managers.DefaultPreparation;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace diagramMaker.managers
{
    public class Initiator
    {
        private DataHub data;
        private Canvas? appCanvas;
        private MainWindow mainWindow;
        private ExternalManager extManager;
       
        public Initiator(DataHub data, MainWindow mainWindow)
        {
            this.data = data;
            this.mainWindow = mainWindow;
            extManager = new ExternalManager(data);
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
                    extManager.Save();
                    break;
                case ETopMenuActionType.Load:
                    ClearApp();
                    DefaultPreparation();
                    extManager.Load(mainWindow.eventner);
                    mainWindow.eventner.NavigationPanelScrollCount_Activation();
                    break;
                default:
                    break;
            }
        }

        private void ClearApp()
        {
            mainWindow.Width = 1024;
            mainWindow.Height = 768;
            mainWindow.ItemMoveNotify -= mainWindow.eventner.EventItemMoveHandler;
            mainWindow.PreviewKeyDown -= mainWindow.eventner.MainWindow_PreviewKeyDown;
            mainWindow.KeyDown -= mainWindow.eventner.MainWindow_KeyDown;
            mainWindow.topMenu.MenuHandlerNotify -= TopMenu_MenuHandlerNotify;
            ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item.Children.Clear();
            data.ClearData();
            DefaultItem.RestartID();
        }

        public void DefaultPreparation()
        {
            SetParameterCollections();
            SetDefaultItemPanels();
            SetItemCollections();
            //
            mainWindow.ItemMoveNotify += mainWindow.eventner.EventItemMoveHandler;
            mainWindow.PreviewKeyDown += mainWindow.eventner.MainWindow_PreviewKeyDown;
            mainWindow.KeyDown += mainWindow.eventner.MainWindow_KeyDown;
            mainWindow.KeyUp += mainWindow.eventner.MainWindow_KeyUp;
            //
            mainWindow.topMenu.SetMainMenu();
            mainWindow.topMenu.MenuHandlerNotify += TopMenu_MenuHandlerNotify;
        }

        public void SetDefaultItemPanels()
        {
            appCanvas = MenuMaker.Make_AppCanvas(data, mainWindow);
            appCanvas.SetValue(AutomationProperties.NameProperty, "appCanvas");

            MenuMaker.Make_InfoLine(data, mainWindow, appCanvas);

            MenuMakeOptions _option;
            MenuContainer _menu;

            //itemCreationPanel
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.TopMenu;
            _option.panelLabel = "Items Menu";
            _option.x = 5;
            _option.y = 20;
            _option.w = 150;
            _option.h = 450;            
            _menu = MenuMaker.Make_CreationPanelMenu(data, mainWindow, appCanvas, _option);
            data.menuCreationCanvasID = _menu.itemId;
            data.menuCreation = data.GetItemIndexByID(data.menuCreationCanvasID);

            data.panel.Add("itemCreationPanel", _menu);

            //itemSubPanels

            //infoBlock, PaintMaker
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            _option.parentId = _menu.itemId;
            _option.panelLabel = "> Info items";
            _option.x = 2;
            _option.y = 24;
            _option.w = 146;
            _option.h = 130;
            _option.itmPosSize = new Vector4<double>(x: 1, y: 28, w: 144, h: 50);
            _option.itmStringContent.Add("info block");
            _option.itmStringContent.Add("image maker");
            _option.itmEventContent.Add("InfoBlock");
            _option.itmEventContent.Add("PaintMaker");
            _option.itmImgPath.Add("..\\..\\assets\\item01.png");
            _option.itmImgPath.Add("..\\..\\assets\\item02.png");
            _option.itmFunc.Add(MenuMaker.Make_SubPanelItem_EventCreateItem);
            _option.itmFunc.Add(MenuMaker.Make_SubPanelItem_EventCreateItem);
            _menu.menu.Add(MenuMaker.Make_CreationSubPanelCarcas(data, mainWindow, appCanvas, _option));

            //multiline
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            _option.parentId = _menu.itemId;
            _option.panelLabel = "> Lines";
            _option.x = 2;
            _option.y = 155;
            _option.w = 146;
            _option.h = 80;
            _option.itmPosSize = new Vector4<double>(x: 1, y: 28, w: 144, h: 50);
            _option.itmStringContent.Add("multiline");
            _option.itmEventContent.Add("Line");
            _option.itmImgPath.Add("..\\..\\assets\\item03.png");
            _option.itmFunc.Add(MenuMaker.Make_SubPanelItem_EventCreateItem);
            _menu.menu.Add(MenuMaker.Make_CreationSubPanelCarcas(data, mainWindow, appCanvas, _option));

            //multiline 2(test)
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            _option.parentId = _menu.itemId;
            _option.panelLabel = "> Lines dublicate";
            _option.x = 2;
            _option.y = 235;
            _option.w = 146;
            _option.h = 80;
            _option.itmPosSize = new Vector4<double>(x: 1, y: 28, w: 144, h: 50);
            _option.itmStringContent.Add("multiline");
            _option.itmEventContent.Add("Line");
            _option.itmImgPath.Add("..\\..\\assets\\item03.png");
            _option.itmFunc.Add(MenuMaker.Make_SubPanelItem_EventCreateItem);
            _menu.menu.Add(MenuMaker.Make_CreationSubPanelCarcas(data, mainWindow, appCanvas, _option));


            //MenuMaker.Make_ParameterMenu(data, mainWindow, appCanvas);
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.TopMenu;
            _option.panelLabel = "Parameters Menu";
            _option.x = 5;
            _option.y = 20;
            _option.w = 150;
            _option.h = 450;
            _menu = MenuMaker.Make_CreationPanelMenu(data, mainWindow, appCanvas, _option);
            data.menuItemParametersID = _menu.itemId;
            data.panel.Add("itemParametersPanel", _menu);
            ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Hidden;

            //itemSubPanels
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            _option.parentId = _menu.itemId;
            _option.panelLabel = "Tools:";
            _option.x = 2;
            _option.y = 24;
            _option.w = 146;
            _option.h = 120;
            _option.itmFunc.Add(MenuMaker.Make_ParameterMenu_Content);
            _option.addFunc.Add(MenuMaker.Make_ParameterMenu_Panel1Content);
            _option.delFunc.Add(MenuMaker.ParameterMenu_DeleteContent);
            _option.itmPosSize = new Vector4<double>(x: 1, y: 26, w: 144, h: 40);
            _menu.menu.Add(MenuMaker.Make_CreationSubPanelCarcas(data, mainWindow, appCanvas, _option));

            //MenuMaker.Make_PainterMakerMenu(data, mainWindow, appCanvas);
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.TopMenu;
            _option.panelLabel = "Paint Maker Menu";
            _option.x = data.winWidth - 170;
            _option.y = 24;
            _option.w = 150;
            _option.h = 450;
            _menu = MenuMaker.Make_CreationPanelMenu(data, mainWindow, appCanvas, _option);
            data.menuItemPaintMakerID = _menu.itemId;
            data.panel.Add("itemPainterMakerPanel", _menu);
            ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Hidden;    

            //itemSubPanels
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            _option.parentId = _menu.itemId;
            _option.panelLabel = "Tools:";
            _option.x = 2;
            _option.y = 24;
            _option.w = 146;
            _option.h = 120;
            _option.itmImgPath.Add("..\\..\\assets\\item05.png");
            _option.itmImgPath.Add("..\\..\\assets\\item06.png");
            _option.itmFunc.Add(MenuMaker.Make_PainterMakerMenu_Content);
            _option.itmPosSize = new Vector4<double>(x: 1, y: 26, w: 144, h: 80);
            _menu.menu.Add(MenuMaker.Make_CreationSubPanelCarcas(data, mainWindow, appCanvas, _option));


            //itemNavigationPanel
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.TopMenu;
            _option.panelLabel = "Navigation Panel";
            _option.x = data.winWidth - 170;
            _option.y = 20;
            _option.w = 150;
            _option.h = 450;
            _menu = MenuMaker.Make_CreationPanelMenu(data, mainWindow, appCanvas, _option);
            data.menuNavigationPanelID = _menu.itemId;
            data.panel.Add("itemNavigationPanel", _menu);
            ((CanvasItem)data.items[data.GetItemIndexByID(data.menuNavigationPanelID)]).Item.Visibility = Visibility.Hidden;

            //itemSubPanels

            //layer 0
            _option = new MenuMakeOptions();
            _option.category = EMenuCategory.SubMenu;
            _option.parentId = _menu.itemId;
            _option.panelLabel = "> Layer 0";
            _option.x = 2;
            _option.y = 24;
            _option.w = 146;
            _option.h = 50;
            _option.itmPosSize = new Vector4<double>(x: 1, y: 28, w: 144, h: 40);
            _menu.menu.Add(MenuMaker.Make_CreationSubPanelCarcas(data, mainWindow, appCanvas, _option));
            data.activeLayer = _menu.menu[0].itemId;
        }

        public void SetParameterCollections()
        {
            if (data.parameters == null)
            {
                data.parameters = new Dictionary<string, DefaultParameter> ();
            }

            DefaultParameter _ip;
            Connection _con;

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

            //line
            _con = new Connection();
            _con.IsUser = true;
            _ip = new CommonParameter(
                connect: _con
                );
            data.parameters.Add("itemLineCommon", _ip);

            _ip = new ItemParameter(
                left: 160,
                top: 50,
                width: 1,
                height: 1,
                bgColor: null,
                frColor: Brushes.Black);
            data.parameters.Add("itemLineContent", _ip);

            List<FigureContainer> _vertex = new List<FigureContainer>();
            _ip = new ShapeParameter(
                shape: EShape.Line,
                vertex: _vertex,
                color: Colors.Black,
                strokeThickness: 2);
            data.parameters.Add("figureLineContent", _ip);

            _ip = new EventParameter(
                moveSensitive: true,
                mouseUp: true,
                mouseDown: true,
                mouseMove: true
                );
            data.parameters.Add("eventLineContent", _ip);

            //connectors
            _con = new Connection();
            _con.IsConnector = true;
            _con.support.Add(EConnectorSupport.NewConnector, -1);
            _ip = new CommonParameter(
                connect: _con
                );
            data.parameters.Add("itemCommonConnector", _ip);

            _con = new Connection();
            _con.IsConnector = true;
            _ip = new CommonParameter(
                connect: _con
                );
            data.parameters.Add("itemCommonSpecialConnector", _ip);

            _ip = new ItemParameter(
                left: 160,
                top: 50,
                width: 8,
                height: 8,
                bgColor: Brushes.White,
                frColor: null);
            data.parameters.Add("itemLineConnect01Content", _ip);

            _ip = new ItemParameter(
                left: 190,
                top: 50,
                width: 8,
                height: 8,
                bgColor: Brushes.White,
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

        public void SetItemCollections()
        {
            ItemMakerContainer _im;
            ItemMakerContainer _imChild;
            ItemMakerContainer _imConnect;
            if (data.itemCollection == null)
            {
                data.itemCollection = new Dictionary<string, ItemMakerContainer> ();
            }

            //infoBlock
            _im = new ItemMakerContainer();
            _im.Props.Add("itemInfoBlock", EParameter.Item);
            _im.Props.Add("borderInfoBlock", EParameter.Border);
            _im.Props.Add("eventInfoBlock", EParameter.Event);
            _im.Item = EItem.Canvas;
            data.itemCollection.Add("InfoBlock", _im);

            _imChild = new ItemMakerContainer();
            _imChild.Props.Add("itemTextInfoBlock", EParameter.Item);
            _imChild.Props.Add("contentTextInfoBlock", EParameter.Content);
            _imChild.Props.Add("eventNoHit", EParameter.Event);
            _imChild.Item = EItem.Label;
            _im.Children.Add(_imChild);

            //PaintMaker
            _im = new ItemMakerContainer();
            _im.Props.Add("itemPaintMaker", EParameter.Item);
            _im.Props.Add("borderVersion01", EParameter.Border);
            _im.Props.Add("eventPaintMaker", EParameter.Event);
            _im.Item = EItem.Painter;
            data.itemCollection.Add("PaintMaker", _im);

            //line
            _im = new ItemMakerContainer();
            _im.Props.Add("itemLineCommon", EParameter.Common);
            _im.Props.Add("itemLineContent", EParameter.Item);
            _im.Props.Add("figureLineContent", EParameter.Shape);
            _im.Props.Add("eventLineContent", EParameter.Event);
            _im.Item = EItem.Figure;
            data.itemCollection.Add("Line", _im);

            _imConnect = new ItemMakerContainer();
            _imConnect.Props.Add("itemCommonConnector", EParameter.Common);
            _imConnect.Props.Add("itemLineConnect01Content", EParameter.Item);
            _imConnect.Props.Add("eventLineConnect", EParameter.Event);
            _imConnect.Item = EItem.Canvas;
            _imConnect.Events.Add(EEvent.AddLine);
            _im.Connector.Add(_imConnect);

            _imConnect = new ItemMakerContainer();
            _imConnect.Props.Add("itemCommonConnector", EParameter.Common);
            _imConnect.Props.Add("itemLineConnect02Content", EParameter.Item);
            _imConnect.Props.Add("eventLineConnect", EParameter.Event);
            _imConnect.Item = EItem.Canvas;
            _imConnect.Events.Add(EEvent.AddLine);
            _im.Connector.Add(_imConnect);

            //connector
            _im = new ItemMakerContainer();
            _im.Props.Add("itemCommonSpecialConnector", EParameter.Common);
            _im.Props.Add("itemLineConnect02Content", EParameter.Item);
            _im.Props.Add("eventLineConnect", EParameter.Event);
            _im.Item = EItem.Canvas;
            _im.Events.Add(EEvent.AddLine);
            _im.Events.Add(EEvent.Connector);
            data.itemCollection.Add("Connector", _im);
        }        
    }
}