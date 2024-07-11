using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.managers;
using diagramMaker.managers.DefaultPreparation;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace diagramMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private DataHub data;
        public diagramMaker.managers.EventManager eventner;
        public TopMenuHandler topMenu;
        private Initiator initiator;

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
        
        public MainWindow()
        {
            Trace.WriteLine("MainWindow");
            InitializeComponent();
            data = new DataHub();            

            this.MouseMove += MainWindow_MouseMove;
            this.MouseUp += MainWindow_MouseUp;
            this.SizeChanged += MainWindow_SizeChanged;
                       
            this.Width = 1024;
            this.Height = 768;

            eventner = new managers.EventManager(data);
            topMenu = new TopMenuHandler(data, this);
            initiator = new Initiator(data, this);
            initiator.Prepare();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            topMenu.mainMenu.Width = this.Width;

            data.winWidth = e.NewSize.Width;
            data.winHeight = e.NewSize.Height - SystemParameters.WindowCaptionHeight;
                        
            string _msg = "Win size:" + e.NewSize.Width + "x" + e.NewSize.Height + "px; "+
                "Scene(LeftxTop):" + data.topLeftX + "x" + data.topLeftY;
            CommonInfoNotify?.Invoke(message: _msg,
                iParam:new ItemParameter(0, data.winHeight - 45, data.winWidth, 30, null, null));
            ResizeNotify?.Invoke(0,0, data.winWidth, data.winHeight);
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            Point _mousePosition = Mouse.GetPosition(Application.Current.MainWindow);
            
            if (data.tapped >= 0)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (data.tapped == ((CommonParameter)data.items[data.GetItemByID(data.appCanvasID)].param[EParameter.Common]).Id)
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
                ((CanvasItem)data.items[data.GetItemByID(data.menuItemParametersID)]).Item.Visibility = Visibility.Hidden;
                data.isMenuItem = false;
                eventner.ItemMenuDelete(data.menuItemParametersID);
                data.items[data.GetItemByID(data.choosenItemID)].FinishHandling();
                data.choosenItemID = -1;

                if (data.isMenuPainter)
                {
                    ((CanvasItem)data.items[data.GetItemByID(data.menuItemPaintMakerID)]).Item.Visibility = Visibility.Hidden;
                    data.isMenuPainter = false;
                    eventner.ItemMenuDelete(data.menuItemPaintMakerID);
                    
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