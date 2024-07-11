using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace diagramMaker.managers.DefaultPreparation
{
    public class MenuMaker
    {
        public MenuMaker() { }

        public static Canvas Make_AppCanvas(DataHub data, MainWindow mainWindow)
        {
            //appCanvas
            data.items.Add(new CanvasItem(data, null));
            data.items[^1].SetParameters(
                iParam: (ItemParameter)data.parameters["itemCanvasDefault"],
                content: null,
                bParam: null,
            eParam: new EventParameter(moveSensitive: false, mouseDown: true, mouseUp: false)
                );
            Canvas appCanvas = ((CanvasItem)data.items[^1]).Item;
            mainWindow.Content = appCanvas;
            data.appCanvasID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            mainWindow.ResizeNotify += ((CanvasItem)data.items[^1]).CommonResizeHandler;

            return appCanvas;
        }

        public static void Make_InfoLine(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            data.items.Add(new LabelItem(data, appCanvas));
            data.items[^1].SetParameters(
                iParam: (ItemParameter)data.parameters["itemLabelInfoLine"],
                content: new ContentParameter(content: "info line", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Gray, cornerRadius: 0),
                eParam: new EventParameter(moveSensitive: false)
                );
            data.informerID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;

            ((LabelItem)data.items[^1]).Item.SetValue(AutomationProperties.NameProperty, "InfoLine");
            mainWindow.CommonInfoNotify += ((LabelItem)data.items[^1]).SetContent;
            mainWindow.AppCanvasMoveNotify += ((LabelItem)data.items[^1]).SetContent;
            Panel.SetZIndex(((LabelItem)data.items[^1]).Item, 30);
            mainWindow.AppCanvasMoveSensitiveNotify += mainWindow.eventner.EventItemsShifter;
        }

        public static void Make_CreationMenu(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            //item creation menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(5, 20, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: (BorderParameter)data.parameters["borderVersion01"],
                eParam: new EventParameter(moveSensitive: false, mouseDown: true)
                );
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppX = data.topLeftX + 5;
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppY = data.topLeftY + 10;
            data.menuCreation = data.items.Count - 1;
            data.menuCreationCanvasID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            Panel.SetZIndex(((CanvasItem)data.items[data.menuCreation]).Item, 20);

            //menuCreation > txt label
            data.items.Add(new LabelItem(data, appCanvas, ((CommonParameter)data.items[data.menuCreation].param[EParameter.Common]).Id));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 3, 144, 30, null, Brushes.Black),
                content: new ContentParameter(content: "Items Menu:", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //
            Make_CreationMenu_InfoBlock(data, mainWindow, appCanvas);
            Make_CreationMenu_PaintMaker(data, mainWindow, appCanvas);
            Make_CreationMenu_Line(data, mainWindow, appCanvas);
        }

        public static void Make_CreationMenu_InfoBlock(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            //menuCreation > item add info block
            data.items.Add(new CanvasItem(data, appCanvas, ((CommonParameter)data.items[data.menuCreation].param[EParameter.Common]).Id));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 33, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2),
                eParam: new EventParameter(mouseUp: true, mouseUpInfo: "InfoBlock")
                );
            int menuInfoBlockID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            data.items[^1].MouseAppNotify += mainWindow.eventner.eventCreateItem;

            //menuCreation > item info block > img
            data.items.Add(new ImageItem(data, appCanvas, menuInfoBlockID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 3, 44, 44, null, Brushes.Black),
                content: null,
                bParam: null,
                eParam: null,
                imgParam: new ImageParameter("..\\..\\assets\\item01.png")
                );

            //menuCreation > item info block > txt
            data.items.Add(new LabelItem(data, appCanvas, menuInfoBlockID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(50, 3, 99, 44, null, Brushes.Black),
                content: new ContentParameter(
                    content: "info block", 
                    horAlign: HorizontalAlignment.Center, 
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );            
        }

        public static void Make_CreationMenu_PaintMaker(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            //menuCreation > item add paint canvas
            data.items.Add(new CanvasItem(data, appCanvas, ((CommonParameter)data.items[data.menuCreation].param[EParameter.Common]).Id));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 84, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2),
                eParam: new EventParameter(mouseUp: true, mouseUpInfo: "PaintMaker")
                );
            int menuPaintID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            data.items[^1].MouseAppNotify += mainWindow.eventner.eventCreateItem;

            //menuCreation > item paint canvas > img
            data.items.Add(new ImageItem(data, appCanvas, menuPaintID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 3, 44, 44, null, Brushes.Black),
                content: null,
                bParam: null,
                eParam: null,
                imgParam: new ImageParameter("C:\\alar_work\\diagramMaker\\diagramMaker\\assets\\item02.png")
                );

            //menuCreation > item paint canvas > txt
            data.items.Add(new LabelItem(data, appCanvas, menuPaintID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(50, 3, 99, 44, null, Brushes.Black),
                content: new ContentParameter(content: "image maker", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
        }

        public static void Make_CreationMenu_Line(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            //menuCreation > item add line
            data.items.Add(new CanvasItem(data, appCanvas, ((CommonParameter)data.items[data.menuCreation].param[EParameter.Common]).Id));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 135, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2),
                eParam: new EventParameter(mouseUp: true, mouseUpInfo: "Line")
                );
            int menuLineID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            data.items[^1].MouseAppNotify += mainWindow.eventner.eventCreateItem;

            //menuCreation > item line > img
            data.items.Add(new ImageItem(data, appCanvas, menuLineID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 3, 44, 44, null, Brushes.Black),
                content: null,
                bParam: null,
                eParam: null,
                imgParam: new ImageParameter("C:\\alar_work\\diagramMaker\\diagramMaker\\assets\\item03.png")
                );

            //menuCreation > item line > txt
            data.items.Add(new LabelItem(data, appCanvas, menuLineID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(50, 3, 99, 44, null, Brushes.Black),
                content: new ContentParameter(content: "multiline", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
        }

        public static void Make_ParameterMenu(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            //item item parameters menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(5, 20, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: (BorderParameter)data.parameters["borderVersion01"],
                eParam: new EventParameter(moveSensitive: false, mouseDown: true)
                );
            data.items[^1].SetParameter(EParameter.Border, data.parameters["borderVersion01"]);
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppX = data.topLeftX + 5;
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppY = data.topLeftY + 10;
            data.menuCreation = data.items.Count - 1;
            Panel.SetZIndex(((CanvasItem)data.items[data.menuCreation]).Item, 21);
            data.menuItemParametersID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            ((CanvasItem)data.items[^1]).Item.Visibility = Visibility.Hidden;
        }
        public static void Make_PainterMakerMenu(DataHub data, MainWindow mainWindow, Canvas appCanvas)
        {
            //item painter maker menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(data.winWidth - 160, 20, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.DarkSlateGray, cornerRadius: 2),
                eParam: new EventParameter(moveSensitive: false, mouseDown: true)
                );
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppX = data.topLeftX + 5;
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppY = data.topLeftY + 10;
            Panel.SetZIndex(((CanvasItem)data.items[^1]).Item, 21);
            data.menuItemPaintMakerID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            ((CanvasItem)data.items[^1]).Item.Visibility = Visibility.Hidden;
        }

        public static void Make_ParameterMenu_Content(
            DataHub data,
            Canvas appCanvas,
            DefaultItem item,
            EventManager eve)
        {
            //menuCreation > txt label
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 3, 144, 30, null, Brushes.Black),
                content: new ContentParameter(
                    content: "Item Menu:",
                    horAlign: HorizontalAlignment.Center,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //menuCreation > btn delete
            data.items.Add(new ButtonItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 0, 30, 30, null, Brushes.Black),
                content: null,
                bParam: new BorderParameter(
                    isBorder: true,
                    borderThickness: 2,
                    color: Colors.DarkSlateGray,
                    cornerRadius: 2),
                eParam: new EventParameter(
                    mouseClick: true,
                    command: ECommand.DeleteItem,
                    commandParameter: data.choosenItemID
                    ),
                imgParam: new ImageParameter("..\\..\\assets\\item04.png")
                );
            ((ButtonItem)data.items[^1]).ItemClickHandlerNotify
                += eve.EventItemDeleteHandler;

            //ItemParametersMenu > label id
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 30, 144, 26, null, Brushes.Black),
                content: new ContentParameter(
                    content: "id:" + data.choosenItemID.ToString(),
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > label name:
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 50, 46, 26, null, Brushes.Black),
                content: new ContentParameter(content: "name:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > text name:
            data.items.Add(new TextItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(46, 50, 98, 26, null, Brushes.Black),
                content: new ContentParameter(
                    content: ((CommonParameter)item.param[EParameter.Common]).Name,
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center,
                    isTextChanged: true,
                    bindParameter: EBindParameter.Name,
                    bindID: data.choosenItemID),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > label width:
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 76, 46, 26, null, Brushes.Black),
                content: new ContentParameter(content: "width:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > text width:
            data.items.Add(new TextItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(46, 76, 98, 26, null, Brushes.Black),
                content: new ContentParameter(
                    content: item.param[EParameter.Item] == null ? 
                        ((CanvasItem)item).Item.Width.ToString() : 
                        ((ItemParameter)item.param[EParameter.Item]).Width.ToString(),
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center,
                    isTextChanged: true,
                    bindParameter: EBindParameter.Width,
                    bindID: data.choosenItemID,
                    isDigitsOnly: true),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > label height:
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 102, 44, 26, null, Brushes.Black),
                content: new ContentParameter(content: "height:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            //ItemParametersMenu > text height:
            data.items.Add(new TextItem(data, appCanvas, data.menuItemParametersID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(44, 102, 100, 26, null, Brushes.Black),
                content: new ContentParameter(
                    content: item.param[EParameter.Item] == null ? 
                        ((CanvasItem)item).Item.Height.ToString() : 
                        ((ItemParameter)item.param[EParameter.Item]).Height.ToString(),
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center,
                    isTextChanged: true,
                    bindParameter: EBindParameter.Height,
                    bindID: data.choosenItemID,
                    isDigitsOnly: true),
                bParam: null,
                eParam: null
                );
            int _i = 0;
            int _coord = 102 + 26;
            while (_i < data.items.Count)
            {
                item = data.items[_i];
                if (((CommonParameter)item.param[EParameter.Common]).ParentId == data.choosenItemID)
                {
                    //label separator:
                    data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
                    data.items[^1].SetParameters(
                    iParam: new ItemParameter(0, _coord, _coord, 26, null, Brushes.Black),
                    content: new ContentParameter(
                        content: "--> child",
                        horAlign: HorizontalAlignment.Left,
                        verAlign: VerticalAlignment.Center),
                    bParam: null,
                    eParam: null
                    );
                    _coord += 26;
                    //
                    var a = item.GetType();
                    switch (item.GetType().Name)
                    {
                        case "LabelItem":
                            //label content:
                            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
                            data.items[^1].SetParameters(
                                iParam: new ItemParameter(0, _coord, 52, 26, null, Brushes.Black),
                                content: new ContentParameter(
                                    content: "content:",
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center),
                                bParam: null,
                                eParam: null
                                );
                            // text content:
                            data.items.Add(new TextItem(data, appCanvas, data.menuItemParametersID));
                            data.items[^1].SetParameters(
                                iParam: new ItemParameter(52, _coord, 92, 26, null, Brushes.Black),
                                content: new ContentParameter(
                                    content: ((LabelItem)data.items[data.GetItemByID(((CommonParameter)item.param[EParameter.Common]).Id)]).Item.Content.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    isTextChanged: true,
                                    bindParameter: EBindParameter.Content,
                                    bindID: ((CommonParameter)item.param[EParameter.Common]).Id),
                                bParam: null,
                                eParam: null
                                );
                            _coord += 26;
                            //ItemParametersMenu > label width:
                            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
                            data.items[^1].SetParameters(
                                iParam: new ItemParameter(0, _coord, 46, 26, null, Brushes.Black),
                                content: new ContentParameter(content: "width:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                                bParam: null,
                                eParam: null
                                );
                            //ItemParametersMenu > text width:
                            data.items.Add(new TextItem(data, appCanvas, data.menuItemParametersID));
                            data.items[^1].SetParameters(
                                iParam: new ItemParameter(46, _coord, 98, 26, null, Brushes.Black),
                                content: new ContentParameter(
                                    content: item.param[EParameter.Item] == null ? ((CanvasItem)item).Item.Width.ToString() : ((ItemParameter)item.param[EParameter.Item]).Width.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    isTextChanged: true,
                                    bindParameter: EBindParameter.Width,
                                    bindID: ((CommonParameter)item.param[EParameter.Common]).Id,
                                    isDigitsOnly: true),
                                bParam: null,
                                eParam: null
                                );
                            _coord += 26;
                            //ItemParametersMenu > label height:
                            data.items.Add(new LabelItem(data, appCanvas, data.menuItemParametersID));
                            data.items[^1].SetParameters(
                                iParam: new ItemParameter(0, _coord, 44, 26, null, Brushes.Black),
                                content: new ContentParameter(content: "height:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                                bParam: null,
                                eParam: null
                                );
                            //ItemParametersMenu > text height:
                            data.items.Add(new TextItem(data, appCanvas, data.menuItemParametersID));
                            data.items[^1].SetParameters(
                                iParam: new ItemParameter(44, _coord, 100, 26, null, Brushes.Black),
                                content: new ContentParameter(
                                    content: item.param[EParameter.Item] == null ? ((CanvasItem)item).Item.Height.ToString() : ((ItemParameter)item.param[EParameter.Item]).Height.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    isTextChanged: true,
                                    bindParameter: EBindParameter.Height,
                                    bindID: ((CommonParameter)item.param[EParameter.Common]).Id,
                                    isDigitsOnly: true),
                                bParam: null,
                                eParam: null
                                );
                            break;
                        default:
                            break;
                    }
                }
                _i++;
            }
        }

        public static void Make_PainterMakerMenu_Content(
            DataHub data,
            Canvas appCanvas,
            EventManager eve)
        {
            //menuPainter > txt label
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemPaintMakerID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(0, 3, 144, 30, null, Brushes.Black),
                content: new ContentParameter(
                    content: "Paint Maker Menu:",
                    horAlign: HorizontalAlignment.Center,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );

            //menuPainter > txt label tool
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemPaintMakerID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 30, 40, 30, null, Brushes.Black),
                content: new ContentParameter(
                    content: "Tool:",
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );

            //menuPainter > txt label tool type
            data.items.Add(new LabelItem(data, appCanvas, data.menuItemPaintMakerID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(34, 30, 123, 30, null, Brushes.Black),
                content: new ContentParameter(
                    content: "Move",
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
            int _looker = data.items.Count - 1;

            //menuPainter > btn move
            data.items.Add(new ButtonItem(data, appCanvas, data.menuItemPaintMakerID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(5, 65, 42, 42, null, Brushes.Black),
                content: null,
                bParam: new BorderParameter(
                    isBorder: true,
                    borderThickness: 0,
                    color: Colors.DarkSlateGray,
                    cornerRadius: 0),
                eParam: new EventParameter(
                    mouseClick: true,
                    command: ECommand.Data_PainterTool,
                    commandParameter: 1
                    ),
                imgParam: new ImageParameter("..\\..\\assets\\item05.png")
                );
            ((ButtonItem)data.items[^1]).ItemClickHandlerNotify
                += eve.EventButtonClickHandler;
            ((ButtonItem)data.items[^1]).ItemClickHandlerNotify
                += data.items[_looker].EventOutdataHandler;


            //menuPainter > btn paint
            data.items.Add(new ButtonItem(data, appCanvas, data.menuItemPaintMakerID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(48, 65, 42, 42, null, Brushes.Black),
                content: null,
                bParam: new BorderParameter(
                    isBorder: true,
                    borderThickness: 0,
                    color: Colors.DarkSlateGray,
                    cornerRadius: 0),
                eParam: new EventParameter(
                    mouseClick: true,
                    command: ECommand.Data_PainterTool,
                    commandParameter: 2
                    ),
                imgParam: new ImageParameter("..\\..\\assets\\item06.png")
                );
            ((ButtonItem)data.items[^1]).ItemClickHandlerNotify
                += eve.EventButtonClickHandler;
            ((ButtonItem)data.items[^1]).ItemClickHandlerNotify
                += data.items[_looker].EventOutdataHandler;
        }

        public static void Make_NavigationPanelMenu(DataHub data, MainWindow mainWindow, Canvas appCanvas) 
        {
            //item navigation panel menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(data.winWidth - 170, 20, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.DarkSlateGray, cornerRadius: 2),
                eParam: new EventParameter(
                    moveSensitive: false, 
                    mouseDown: true,
                    mouseWheel:true)
                );
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppX = data.topLeftX + 5;
            ((CommonParameter)data.items[^1].param[EParameter.Common]).AppY = data.topLeftY + 10;
            Panel.SetZIndex(((CanvasItem)data.items[^1]).Item, 22);
            data.menuNavigationPanelID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            ((CanvasItem)data.items[^1]).MouseScrollNotify +=
                mainWindow.eventner.EventNavigationPanelScrollCount;
            ((CanvasItem)data.items[^1]).Item.Visibility = Visibility.Hidden;

            //navigation panel > txt label tool
            data.items.Add(new LabelItem(data, appCanvas, data.menuNavigationPanelID));
            data.items[^1].SetParameters(
                iParam: new ItemParameter(3, 3, 144, 28, null, Brushes.Black),
                content: new ContentParameter(
                    content: "Navigation Panel",
                    horAlign: HorizontalAlignment.Center,
                    verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );

            int _tmp = 0;
            for (int _i = 0; _i < data.menuNavigationPanel_SlotNumber; _i++)
            {
                //navigation panel > txt label tool
                data.items.Add(new LabelItem(data, appCanvas, data.menuNavigationPanelID));
                data.items[^1].SetParameters(
                    iParam: new ItemParameter(3, 33+_i*30, 144, 28, null, Brushes.Black),
                    content: new ContentParameter(
                        content: "",
                        horAlign: HorizontalAlignment.Left,
                        verAlign: VerticalAlignment.Center,
                        count: _tmp++),
                    bParam: new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Gray, cornerRadius: 0),
                    eParam: new EventParameter(
                        IsHitTestVisible: true,
                        mouseDoubleClick: true)
                    );
                mainWindow.eventner.NavigationItemNotify +=
                    ((LabelItem)data.items[^1]).EventContent;
                ((LabelItem)data.items[^1]).Item.Visibility = Visibility.Hidden;
                data.items[^1].MouseDoubleClickNotify +=
                     mainWindow.eventner.NavigationMoveToItem;
            }
        }
    }
}