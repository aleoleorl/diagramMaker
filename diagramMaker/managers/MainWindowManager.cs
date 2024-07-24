using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;
using System.Windows.Input;
using System.Windows;
using diagramMaker.managers.DefaultPreparation;

namespace diagramMaker.managers
{
    public class MainWindowManager
    {
        public MainWindow mainWindow;
        private DataHub data;
        private DefaultManager defMan;

        public delegate void CommonInfoHandler(string message, ItemParameter iParam);
        public event CommonInfoHandler? CommonInfoNotify;

        public delegate void ResizeHandler(double left, double top, double width, double height);
        public event ResizeHandler? ResizeNotify;

        public delegate void AppCanvasMoveHandler(string? message = null, ItemParameter? iParam = null);
        public event AppCanvasMoveHandler? AppCanvasMoveNotify;

        public delegate void AppCanvasMoveSensitiveHandler(double x, double y);
        public event AppCanvasMoveSensitiveHandler? AppCanvasMoveSensitiveNotify;

        public delegate void ItemMoveHandler(double x, double y);
        public event ItemMoveHandler? ItemMoveNotify;

        public MainWindowManager(
            MainWindow mainWindow, 
            DataHub data,
            DefaultManager defMan)
        {
            this.mainWindow = mainWindow;
            this.data = data;
            this.defMan = defMan;

            mainWindow.MouseMove += MainWindow_MouseMove;
            mainWindow.MouseUp += MainWindow_MouseUp;
            mainWindow.SizeChanged += MainWindow_SizeChanged;

            mainWindow.Width = 1024;
            mainWindow.Height = 768;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            defMan.topMenu.mainMenu.Width = mainWindow.Width;

            data.winWidth = e.NewSize.Width;
            data.winHeight = e.NewSize.Height - SystemParameters.WindowCaptionHeight;
            string _msg = "Win size:" + e.NewSize.Width + "x" + e.NewSize.Height + "px; " +
            "Scene(LeftxTop):" + data.topLeftX + "x" + data.topLeftY;
            CommonInfoNotify?.Invoke(message: _msg,
                iParam: new ItemParameter(0, data.winHeight - 45, data.winWidth, 30, null, null));
            ResizeNotify?.Invoke(0, 0, data.winWidth, data.winHeight);
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            Point _mousePosition = Mouse.GetPosition(Application.Current.MainWindow);

            if (data.tapped >= 0)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (data.tapped == ((CommonParameter)data.items[data.GetItemIndexByID(data.appCanvasID)].param[EParameter.Common]).Id)
                    {
                        data.topLeftX += (_mousePosition.X - data.oldMouseX);
                        data.topLeftY += (_mousePosition.Y - data.oldMouseY);
                        string _msg = "Win size:" + data.winWidth + "x" + data.winHeight + "px; " +
                            "Scene(LeftxTop):" + data.topLeftX + "x" + data.topLeftY;
                        AppCanvasMoveNotify?.Invoke(_msg, null);
                        AppCanvasMoveSensitiveNotify?.Invoke(_mousePosition.X - data.oldMouseX, _mousePosition.Y - data.oldMouseY);
                    }
                    else
                    {
                        data.oldMouseX = _mousePosition.X;
                        data.oldMouseY = _mousePosition.Y;
                        ItemMoveNotify?.Invoke(
                            _mousePosition.X,
                            _mousePosition.Y);
                    }
                }
                else
                {
                    TapControl();
                }
            }
            data.oldMouseX = _mousePosition.X;
            data.oldMouseY = _mousePosition.Y;
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TapControl();
            if (data.isMenuItem)
            {
                ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Hidden;
                data.isMenuItem = false;
                defMan.eve.ItemMenuDelete(data.menuItemParametersID);
                data.items[data.GetItemIndexByID(data.choosenItemID)].FinishHandling();
                data.choosenItemID = -1;
                if (data.isMenuPainter)
                {
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Hidden;
                    data.isMenuPainter = false;
                }
            }
        }

        private void TapControl()
        {
            if (data.tapped != -1)
            {
                data.tapped = -1;
                data.tapXX = 0;
                data.tapYY = 0;
            }
        }
    }
}