using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.managers;
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
        private Initiator initiator;

        public delegate void CommonInfoHandler(string message, ItemParameter iParam);
        public event CommonInfoHandler? cihNotify;

        public delegate void AppCanvasMoveHandler(string? message = null, ItemParameter? iParam = null);
        public event AppCanvasMoveHandler? appCanvasMoveNotify;
        public delegate void AppCanvasMoveSensitiveHandler(double x, double y);
        public event AppCanvasMoveSensitiveHandler? appCanvasMoveHandlerNotify;

        public delegate void ItemMoveHandler(double x, double y);
        public event ItemMoveHandler? itemMoveHandlerNotify;

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
            data.winWidth = 1024;
            data.winHeight = 768;

            eventner = new managers.EventManager(data);
            initiator = new Initiator(data, this);
            initiator.Prepare();
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            data.winWidth = e.NewSize.Width;
            data.winHeight = e.NewSize.Height - SystemParameters.WindowCaptionHeight;
                        
            string _msg = "Win size:" + e.NewSize.Width + "x" + e.NewSize.Height + "px; "+
                "Scene(LeftxTop):" + data.topLeftX + "x" + data.topLeftY;
            cihNotify?.Invoke(message: _msg,
                iParam:new ItemParameter(0, data.winHeight - 45, data.winWidth, 30, null, null));
        }

        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(Application.Current.MainWindow);

            if (data.tapped >= 0)
            {
                if (data.tapped == data.items[data.GetItemByID(data.appCanvasID)].id)
                {
                    data.topLeftX += (mousePosition.X - data.oldMouseX);
                    data.topLeftY += (mousePosition.Y - data.oldMouseY);
                    Trace.WriteLine(data.topLeftX + ":" + data.topLeftY);
                    string _msg = "Win size:" + data.winWidth + "x" + data.winHeight + "px; " +
                        "Scene(LeftxTop):" + data.topLeftX + "x" + data.topLeftY;
                    appCanvasMoveNotify?.Invoke(_msg, null);
                    appCanvasMoveHandlerNotify?.Invoke(mousePosition.X - data.oldMouseX, mousePosition.Y - data.oldMouseY);
                } else
                {
                    Trace.WriteLine("mousePosition.X:" + mousePosition.X + "; mousePosition.Y:" + mousePosition.Y);
                    itemMoveHandlerNotify?.Invoke(
                        mousePosition.X, 
                        mousePosition.Y);
                }
            }
            data.oldMouseX = mousePosition.X;
            data.oldMouseY = mousePosition.Y;
        }

        private void MainWindow_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (data.tapped != -1)
            {
                data.tapped = -1;
                data.tapXX = 0;
                data.tapYY = 0;
            }
            if (data.IsMenuItem)
            { 
                ((CanvasItem)data.items[data.GetItemByID(data.MenuItemParametersID)]).item.Visibility = Visibility.Hidden;
                data.IsMenuItem = false;
                eventner.ItemMenuDelete(data.MenuItemParametersID);
                data.items[data.GetItemByID(data.ChoosenItemID)].FinishHandling();
                data.ChoosenItemID = -1;

                if (data.IsMenuPainter)
                {
                    ((CanvasItem)data.items[data.GetItemByID(data.MenuItemPaintMakerID)]).item.Visibility = Visibility.Hidden;
                    data.IsMenuPainter = false;
                    eventner.ItemMenuDelete(data.MenuItemPaintMakerID);
                    
                }
            }
        }
    }
}
