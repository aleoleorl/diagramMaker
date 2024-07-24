using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media;

namespace diagramMaker.managers.DefaultPanels
{
    public class MenuMaker
    {
        public MenuMaker() { }

        public static Canvas Make_AppCanvas(DataHub data, DefaultManager defMan)
        {
            //appCanvas
            data.items.Add(new CanvasItem(data, null));
            data.items[^1].SetParameter(EParameter.Item, (ItemParameter)data.parameters["itemCanvasDefault"]);
            data.items[^1].SetParameter(EParameter.Event, new EventParameter(moveSensitive: false, mouseDown: true, mouseUp: false));

            Canvas appCanvas = ((CanvasItem)data.items[^1]).Item;
            defMan.windowManager.mainWindow.Content = appCanvas;
            data.appCanvasID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            defMan.windowManager.ResizeNotify += ((CanvasItem)data.items[^1]).CommonResizeHandler;

            return appCanvas;
        }

        public static void Make_InfoLine(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas)
        {
            data.items.Add(new LabelItem(data, appCanvas));
            data.items[^1].SetParameter(EParameter.Item, (ItemParameter)data.parameters["itemLabelInfoLine"]);
            data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: "info line", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center));
            data.items[^1].SetParameter(EParameter.Border, new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Gray, cornerRadius: 0));
            data.items[^1].SetParameter(EParameter.Event, new EventParameter(moveSensitive: false));

            data.informerID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            ((LabelItem)data.items[^1]).Item.SetValue(AutomationProperties.NameProperty, "InfoLine");
            defMan.windowManager.CommonInfoNotify += ((LabelItem)data.items[^1]).SetContent;
            defMan.windowManager.AppCanvasMoveNotify += ((LabelItem)data.items[^1]).SetContent;
            Panel.SetZIndex(((LabelItem)data.items[^1]).Item, 30);
            defMan.windowManager.AppCanvasMoveSensitiveNotify += defMan.eve.EventItemsShifter;
        }

        public static MenuContainer Make_CreationPanelMenu(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option)
        {
            MenuContainer _item = new MenuContainer(id: 1);
            _item.option = option.Clone();

            int _indexArray = MenuCreatePanelCarcas(data, defMan, appCanvas, _item);
            _item.itemId = ((CommonParameter)data.items[_indexArray].param[EParameter.Common]).Id;
            _item.isOpen = true;

            Panel.SetZIndex(((CanvasItem)data.items[_indexArray]).Item, 20);

            return _item;
        }

        public static int MenuCreatePanelCarcas(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuContainer item)
        {
            if (item.option.category == EMenuCategory.TopMenu)
            {
                //top panel
                data.items.Add(new CanvasItem(data, appCanvas));
                data.items[^1].SetParameter(
                    EParameter.Item,
                    new ItemParameter(
                        left: item.option.x,
                        top: item.option.y,
                        width: item.option.w,
                        height: item.option.h,
                        bgColor: Brushes.LightCyan,
                        frColor: null));
                data.items[^1].SetParameter(EParameter.Border, (BorderParameter)data.parameters["borderVersion01"]);
                data.items[^1].SetParameter(
                    EParameter.Event,
                    new EventParameter(
                        moveSensitive: false,
                        mouseDown: true,
                        mouseWheel: true));
                ((CommonParameter)data.items[^1].param[EParameter.Common]).AppX = data.topLeftX + item.option.x;
                ((CommonParameter)data.items[^1].param[EParameter.Common]).AppY = data.topLeftY + item.option.y;
                data.items[^1].MouseScrolWithIdNotify +=
                    defMan.panelEvent.EventScrollPanelHandler;

                int _topItem = data.items.Count - 1;
                int _topItemId = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;

                //top panel > btn min-max
                data.items.Add(new ButtonItem(data, appCanvas, _topItemId));
                data.items[^1].SetParameter(EParameter.Item, new ItemParameter(item.option.w - 44, 2, 20, 20, null, Brushes.Black));
                data.items[^1].SetParameter(EParameter.Event, new EventParameter(mouseClick: true, command: ECommand.Panel_MinMax, commandParameter: _topItemId));
                data.items[^1].SetParameter(EParameter.Image, new ImageParameter("..\\..\\assets\\item07.png"));
                ((ButtonItem)data.items[^1]).Item.BorderBrush = Brushes.Transparent;
                ((ButtonItem)data.items[^1]).ItemClickNotify +=
                    defMan.panelEvent.EventPanelHandler;
                Panel.SetZIndex(((ButtonItem)data.items[^1]).Item, 100003);

                //top panel > btn close
                data.items.Add(new ButtonItem(data, appCanvas, _topItemId));
                data.items[^1].SetParameter(EParameter.Item, new ItemParameter(item.option.w - 22, 2, 20, 20, null, Brushes.Black));
                data.items[^1].SetParameter(EParameter.Event, new EventParameter(mouseClick: true, command: ECommand.Panel_Close, commandParameter: _topItemId));
                data.items[^1].SetParameter(EParameter.Image, new ImageParameter("..\\..\\assets\\item08.png"));
                ((ButtonItem)data.items[^1]).Item.BorderBrush = Brushes.Transparent;
                ((ButtonItem)data.items[^1]).ItemClickNotify +=
                    defMan.panelEvent.EventPanelHandler;
                Panel.SetZIndex(((ButtonItem)data.items[^1]).Item, 100002);

                //top panel > txt label
                data.items.Add(new LabelItem(data, appCanvas, _topItemId));
                data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, -2, 110, 27, null, Brushes.Black));
                data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: item.option.panelLabel, horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center));
                Panel.SetZIndex(((LabelItem)data.items[^1]).Item, 100001);

                //top panel > back
                data.items.Add(new CanvasItem(data, appCanvas, _topItemId));
                data.items[^1].SetParameter(
                    EParameter.Item,
                    new ItemParameter(
                        left: 0,
                        top: 0,
                        width: item.option.w,
                        height: 24,
                        bgColor: Brushes.LightCyan,
                        frColor: null));
                Panel.SetZIndex(((CanvasItem)data.items[^1]).Item, 100000);

                //mask
                ((CanvasItem)data.items[_topItem]).Item.Clip = new RectangleGeometry(
                new Rect(
                    0,
                    0,
                    ((CanvasItem)data.items[_topItem]).Item.Width,
                    ((CanvasItem)data.items[_topItem]).Item.Height));

                //top panel > btn size change bottom
                data.items.Add(new ImageItem(data, appCanvas, _topItemId));
                data.items[^1].SetParameter(
                    EParameter.Item,
                    new ItemParameter(
                        item.option.w / 2 - 130 / 2,
                        item.option.h - 12,
                        130,
                        12,
                        null,
                        null,
                        vert: EChildItemPosition.Bottom));
                data.items[^1].SetParameter(
                    EParameter.Event,
                    new EventParameter(
                        mouseDown: true,
                        mouseMove: true,
                        command: ECommand.Panel_SizeChange_Bottom,
                        commandParameter: _topItemId));
                data.items[^1].SetParameter(EParameter.Image, new ImageParameter(imagePath: "..\\..\\assets\\item12.png"));

                defMan.windowManager.ItemMoveNotify += ((ImageItem)data.items[^1]).EventBindedCommands;
                ((ImageItem)data.items[^1]).BindByIdAndCommandNotify +=
                    defMan.panelEvent.EventPanelHandler;
                Panel.SetZIndex(((ImageItem)data.items[^1]).Item, 700100);

                return _topItem;
            }
            if (item.option.category == EMenuCategory.SubMenu)
            {
                //canvas
                data.items.Add(new CanvasItem(data, appCanvas, item.option.parentId));
                data.items[^1].SetParameter(
                    EParameter.Item,
                    new ItemParameter(
                        left: item.option.x,
                        top: item.option.y,
                        width: item.option.w,
                        height: item.option.h,
                        bgColor: Brushes.LightCyan,
                        frColor: null));

                int _canvasId = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
                int _canvasIndex = data.items.Count - 1;

                //canvas > btn min-max
                data.items.Add(new ButtonItem(data, appCanvas, _canvasId));
                data.items[^1].SetParameter(EParameter.Item, new ItemParameter(1, 3, 24, 20, null, Brushes.Black));
                data.items[^1].SetParameter(EParameter.Event, new EventParameter(mouseClick: true, command: ECommand.SubPanel_MinMax, commandParameter: _canvasId));
                data.items[^1].SetParameter(EParameter.Image, new ImageParameter("..\\..\\assets\\item09.png"));
                ((ButtonItem)data.items[^1]).Item.BorderBrush = Brushes.Transparent;
                ((ButtonItem)data.items[^1]).ItemClickNotify +=
                    defMan.panelEvent.EventPanelHandler;

                //canvas > txt
                data.items.Add(new LabelItem(data, appCanvas, _canvasId));
                data.items[^1].SetParameter(EParameter.Item, new ItemParameter(26, 0, item.option.w - 26, 25, null, Brushes.Black));
                data.items[^1].SetParameter(
                    EParameter.Content,
                    new ContentParameter(
                        content: item.option.panelLabel,
                        horAlign: HorizontalAlignment.Left,
                        verAlign: VerticalAlignment.Center)
                );

                ((CanvasItem)data.items[_canvasIndex]).Item.Clip = new RectangleGeometry(
                new Rect(
                    0,
                    0,
                    ((CanvasItem)data.items[_canvasIndex]).Item.Width,
                    ((CanvasItem)data.items[_canvasIndex]).Item.Height));

                return _canvasIndex;
            }

            return -1;
        }

        public static MenuContainer Make_CreationSubPanelCarcas(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option)
        {
            MenuContainer _item = new MenuContainer(id: 1);
            _item.option = option.Clone();
            _item.isOpen = true;

            int _indexArray = MenuCreatePanelCarcas(data, defMan, appCanvas, _item);
            _item.itemId = ((CommonParameter)data.items[_indexArray].param[EParameter.Common]).Id;

            MenuMakeOptions _itmOption = new MenuMakeOptions();
            _itmOption.x = option.itmPosSize.x;
            _itmOption.w = option.itmPosSize.w;
            _itmOption.h = option.itmPosSize.h;
            _itmOption.parentId = _item.itemId;
            _itmOption.itmStringContent = option.itmStringContent;
            _itmOption.itmEventContent = option.itmEventContent;
            _itmOption.itmImgPath = option.itmImgPath;

            for (int _i = 0; _i < option.itmFunc.Count; _i++)
            {
                _itmOption.y = option.itmPosSize.y + (_itmOption.h + 2) * _i;
                _item.childrenId.Add(
                    option.itmFunc[_i](data, defMan, appCanvas, _itmOption, _i)
                );
            }
            for (int _i = 0; _i < option.addFunc.Count; _i++)
            {
                _item.option.addFunc.Add(option.addFunc[_i]);
            }
            for (int _i = 0; _i < option.delFunc.Count; _i++)
            {
                _item.option.delFunc.Add(option.delFunc[_i]);
            }

            /* Value 28 comes from MenuCreatePanelCarcas() as a top part of the panel space */
            _item.option.h = 28 + (_itmOption.h + 2) * option.itmFunc.Count;

            return _item;
        }

        public static int Make_SubPanelItem_EventCreateItem(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option,
            int counter)
        {
            //item canvas
            data.items.Add(new CanvasItem(data, appCanvas, option.parentId));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(option.x, option.y, option.w, option.h, Brushes.Beige, null));
            data.items[^1].SetParameter(EParameter.Border, new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2));
            data.items[^1].SetParameter(EParameter.Event, new EventParameter(mouseUp: true, mouseUpContent: option.itmEventContent[counter]));
            int _id = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;

            int _menuID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            data.items[^1].MouseAppNotify += defMan.eve.eventCreateItem;

            //item canvas > img
            data.items.Add(new ImageItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(
                left: option.w * 0.02,
                top: option.h * 0.06,
                width: option.w * 0.3,
                height: option.h * (1 - 0.12),
                null,
                Brushes.Black));
            data.items[^1].SetParameter(EParameter.Image, new ImageParameter(option.itmImgPath[counter]));

            //item canvas > txt
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(
                left: option.w * 0.3 + option.w * 0.02,
                top: option.h * 0.06,
                width: option.w * 0.7,
                height: option.h * (1 - 0.12),
                null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: option.itmStringContent[counter],
                    horAlign: HorizontalAlignment.Center,
                    verAlign: VerticalAlignment.Center)
            );
            return _id;
        }

        public static int Make_SubPanelItem_EventNavigationItem(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option,
            int counter)
        {
            int userId = int.Parse(option.itmStringContent[1]);

            //item canvas
            data.items.Add(new CanvasItem(data, appCanvas, option.parentId));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(option.x, option.y, option.w, option.h, Brushes.Beige, null));

            ((CommonParameter)data.items[^1].param[EParameter.Common]).Connect.Users.Add(userId);

            int _id = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;
            int _index = data.items.Count - 1;

            //item txt label tool
            data.items.Add(new LabelItem(data, appCanvas, _id));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, 0, option.w, option.h, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: option.itmStringContent[0],
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center)
            );
            data.items[^1].SetParameter(EParameter.Border, new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Gray, cornerRadius: 0));
            data.items[^1].SetParameter(EParameter.Event, new EventParameter(IsHitTestVisible: true, mouseDoubleClick: true));
            //change content
            data.items[data.GetItemIndexByID(userId)].ItemChangeNotify += data.items[^1].ValueChanger;
            //move to item          
            data.items[^1].MouseDoubleClickParentNotify +=
                 defMan.navPanel.NavigationMoveToItem;

            return _id;
        }

        public static int Make_ParameterMenu_Content(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option,
            int counter)
        {
            //item canvas
            data.items.Add(new CanvasItem(data, appCanvas, option.parentId));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(option.x, option.y, option.w, option.h, Brushes.Beige, null));
            data.items[^1].SetParameter(EParameter.Border, new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2));

            int _menuID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;

            //ItemParametersMenu > label id
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(3, 3, 144, 26, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: "id:",
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center)
            );

            //ItemParametersMenu > label id
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(20, 3, 144, 26, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: data.choosenItemID.ToString(),
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center)
            );
            defMan.eve.ChoosenItemNotify += data.items[^1].ValueChanger;

            return _menuID;
        }

        public static int Make_ParameterMenu_Panel1Content(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option,
            int counter)
        {
            int _menuID = option.parentId;

            if (data.choosenItemID == -1)
            {
                return -1;
            }

            CanvasItem _topItem = (CanvasItem)data.items[data.GetItemIndexByID(option.parentId)];
            _topItem.Item.Height = 270;
            _topItem.Item.Clip = new RectangleGeometry(new Rect(
                0, 0, _topItem.Item.Width, _topItem.Item.Height));
            if (_topItem.Border != null)
            {
                _topItem.Border.Width = _topItem.Item.Width;
                _topItem.Border.Height = _topItem.Item.Height;
            }

            int topParentId = ((CommonParameter)_topItem.param[EParameter.Common]).ParentId;
            CanvasItem _superTopItem = (CanvasItem)data.items[data.GetItemIndexByID(topParentId)];
            _superTopItem.Item.Height = 300;
            _superTopItem.Item.Clip = new RectangleGeometry(new Rect(
                0, 0, _superTopItem.Item.Width, _topItem.Item.Height));
            if (_superTopItem.Border != null)
            {
                _superTopItem.Border.Width = _superTopItem.Item.Width;
                _superTopItem.Border.Height = _superTopItem.Item.Height;
            }

            Make_ParameterMenu_Content(data, defMan, appCanvas, option, counter);

            //menuCreation > btn delete
            data.items.Add(new ButtonItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(_topItem.Item.Width - 35, 5, 30, 30, null, Brushes.Black));
            data.items[^1].SetParameter(EParameter.Border, new BorderParameter(isBorder: true, borderThickness: 2, color: Colors.DarkSlateGray, cornerRadius: 2));
            data.items[^1].SetParameter(EParameter.Event, new EventParameter(mouseClick: true, command: ECommand.DeleteItem, commandParameter: data.choosenItemID));
            data.items[^1].SetParameter(EParameter.Image, new ImageParameter("..\\..\\assets\\item04.png"));
            ((ButtonItem)data.items[^1]).ItemClickNotify
                += defMan.eve.EventItemDeleteHandler;


            DefaultItem _item = data.items[data.GetItemIndexByID(data.choosenItemID)];
            //ItemParametersMenu > label name:
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, 50, 46, 26, null, Brushes.Black));
            data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: "name:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center));

            //ItemParametersMenu > text name:
            data.items.Add(new TextItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(46, 50, 98, 26, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content, new
                ContentParameter(
                    content: ((CommonParameter)_item.param[EParameter.Common]).Name,
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center,
                    isTextChanged: true,
                    bindParameter: EBindParameter.Name,
                    bindID: data.choosenItemID)
            );

            //ItemParametersMenu > label width:
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, 76, 46, 26, null, Brushes.Black));
            data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: "width:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center));

            //ItemParametersMenu > text width:
            data.items.Add(new TextItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(46, 76, 98, 26, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: _item.param[EParameter.Item] == null ?
                        ((CanvasItem)_item).Item.Width.ToString() :
                        ((ItemParameter)_item.param[EParameter.Item]).Width.ToString(),
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center,
                    isTextChanged: true,
                    bindParameter: EBindParameter.Width,
                    bindID: data.choosenItemID,
                    isDigitsOnly: true)
            );

            //ItemParametersMenu > label height:
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, 102, 44, 26, null, Brushes.Black));
            data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: "height:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center));

            //ItemParametersMenu > text height:
            data.items.Add(new TextItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(44, 102, 100, 26, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: _item.param[EParameter.Item] == null ?
                        ((CanvasItem)_item).Item.Height.ToString() :
                        ((ItemParameter)_item.param[EParameter.Item]).Height.ToString(),
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center,
                    isTextChanged: true,
                    bindParameter: EBindParameter.Height,
                    bindID: data.choosenItemID,
                    isDigitsOnly: true)
            );

            int _i = 0;
            int _coord = 102 + 26;
            while (_i < data.items.Count)
            {
                _item = data.items[_i];
                if (((CommonParameter)_item.param[EParameter.Common]).ParentId == data.choosenItemID)
                {
                    //label separator:
                    data.items.Add(new LabelItem(data, appCanvas, _menuID));
                    data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, _coord, _coord, 26, null, Brushes.Black));
                    data.items[^1].SetParameter(EParameter.Content, new ContentParameter(
                        content: "--> child",
                        horAlign: HorizontalAlignment.Left,
                        verAlign: VerticalAlignment.Center)
                    );
                    _coord += 26;
                    //
                    var a = _item.GetType();
                    switch (_item.GetType().Name)
                    {
                        case "LabelItem":
                            //label content:
                            data.items.Add(new LabelItem(data, appCanvas, _menuID));
                            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, _coord, 52, 26, null, Brushes.Black));
                            data.items[^1].SetParameter(
                                EParameter.Content,
                                new ContentParameter(
                                    content: "content:",
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center)
                            );

                            // text content:
                            data.items.Add(new TextItem(data, appCanvas, _menuID));
                            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(52, _coord, 92, 26, null, Brushes.Black));
                            data.items[^1].SetParameter(
                                EParameter.Content,
                                new ContentParameter(
                                    content: ((LabelItem)data.items[data.GetItemIndexByID(((CommonParameter)_item.param[EParameter.Common]).Id)]).Item.Content.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    isTextChanged: true,
                                    bindParameter: EBindParameter.Content,
                                    bindID: ((CommonParameter)_item.param[EParameter.Common]).Id)
                            );
                            _coord += 26;

                            //ItemParametersMenu > label width:
                            data.items.Add(new LabelItem(data, appCanvas, _menuID));
                            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, _coord, 46, 26, null, Brushes.Black));
                            data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: "width:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center));

                            //ItemParametersMenu > text width:
                            data.items.Add(new TextItem(data, appCanvas, _menuID));
                            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(46, _coord, 98, 26, null, Brushes.Black));
                            data.items[^1].SetParameter(
                                EParameter.Content,
                                new ContentParameter(
                                    content: _item.param[EParameter.Item] == null ? ((CanvasItem)_item).Item.Width.ToString() : ((ItemParameter)_item.param[EParameter.Item]).Width.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    isTextChanged: true,
                                    bindParameter: EBindParameter.Width,
                                    bindID: ((CommonParameter)_item.param[EParameter.Common]).Id,
                                    isDigitsOnly: true)
                            );
                            _coord += 26;

                            //ItemParametersMenu > label height:
                            data.items.Add(new LabelItem(data, appCanvas, _menuID));
                            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(0, _coord, 44, 26, null, Brushes.Black));
                            data.items[^1].SetParameter(EParameter.Content, new ContentParameter(content: "height:", horAlign: HorizontalAlignment.Left, verAlign: VerticalAlignment.Center));

                            //ItemParametersMenu > text height:
                            data.items.Add(new TextItem(data, appCanvas, _menuID));
                            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(44, _coord, 100, 26, null, Brushes.Black));
                            data.items[^1].SetParameter(
                                EParameter.Content,
                                new ContentParameter(
                                    content: _item.param[EParameter.Item] == null ? ((CanvasItem)_item).Item.Height.ToString() : ((ItemParameter)_item.param[EParameter.Item]).Height.ToString(),
                                    horAlign: HorizontalAlignment.Left,
                                    verAlign: VerticalAlignment.Center,
                                    isTextChanged: true,
                                    bindParameter: EBindParameter.Height,
                                    bindID: ((CommonParameter)_item.param[EParameter.Common]).Id,
                                    isDigitsOnly: true)
                            );
                            break;
                        default:
                            break;
                    }
                }
                _i++;
            }


            return _menuID;
        }

        public static int ParameterMenu_DeleteContent(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option,
            int counter)
        {
            CanvasItem _item = (CanvasItem)data.items[data.GetItemIndexByID(option.parentId)];
            _item.Item.Children.Clear();
            int _i = 0;
            while (_i < data.items.Count)
            {
                if (((CommonParameter)data.items[_i].param[EParameter.Common]).ParentId == option.parentId)
                {
                    data.items.RemoveAt(_i);
                    continue;
                }
                _i++;
            }
            return -1;
        }

        public static int Make_PainterMakerMenu_Content(
            DataHub data,
            DefaultManager defMan,
            Canvas appCanvas,
            MenuMakeOptions option,
            int counter)
        {
            //item canvas
            data.items.Add(new CanvasItem(data, appCanvas, option.parentId));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(option.x, option.y, option.w, option.h, Brushes.Beige, null));
            data.items[^1].SetParameter(EParameter.Border, new BorderParameter(isBorder: true, borderThickness: 1, color: Colors.Black, cornerRadius: 2));

            int _menuID = ((CommonParameter)data.items[^1].param[EParameter.Common]).Id;

            //menuPainter > txt label tool
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(3, 0, 40, 30, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: "Tool:",
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center)
            );

            //menuPainter > txt label tool type
            data.items.Add(new LabelItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(34, 0, 123, 30, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Content,
                new ContentParameter(
                    content: "Move",
                    horAlign: HorizontalAlignment.Left,
                    verAlign: VerticalAlignment.Center)
            );
            int _looker = data.items.Count - 1;

            //menuPainter > btn move
            data.items.Add(new ButtonItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(5, 30, 42, 42, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Border,
                new BorderParameter(
                    isBorder: true,
                    borderThickness: 0,
                    color: Colors.DarkSlateGray,
                    cornerRadius: 0)
            );
            data.items[^1].SetParameter(
                EParameter.Event,
                new EventParameter(
                    mouseClick: true,
                    command: ECommand.Data_PainterTool,
                    commandParameter: 1)
            );
            data.items[^1].SetParameter(EParameter.Image, new ImageParameter(option.itmImgPath[0]));

            ((ButtonItem)data.items[^1]).ItemClickNotify
                += defMan.eve.EventButtonClickHandler;
            ((ButtonItem)data.items[^1]).ItemClickNotify
                += data.items[_looker].EventOutdataHandler;


            //menuPainter > btn paint
            data.items.Add(new ButtonItem(data, appCanvas, _menuID));
            data.items[^1].SetParameter(EParameter.Item, new ItemParameter(48, 30, 42, 42, null, Brushes.Black));
            data.items[^1].SetParameter(
                EParameter.Border,
                new BorderParameter(
                    isBorder: true,
                    borderThickness: 0,
                    color: Colors.DarkSlateGray,
                    cornerRadius: 0)
            );
            data.items[^1].SetParameter(
                EParameter.Event,
                new EventParameter(
                    mouseClick: true,
                    command: ECommand.Data_PainterTool,
                    commandParameter: 2
                    )
            );
            data.items[^1].SetParameter(EParameter.Image, new ImageParameter(option.itmImgPath[1]));

            ((ButtonItem)data.items[^1]).ItemClickNotify
                += defMan.eve.EventButtonClickHandler;
            ((ButtonItem)data.items[^1]).ItemClickNotify
                += data.items[_looker].EventOutdataHandler;

            return _menuID;
        }
    }
}