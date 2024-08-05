using diagramMaker.helpers.enumerators;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace diagramMaker.items
{
    public class ImageItem : DefaultItem
    {
        public Image Item { get; set; }

        public delegate void ItemClickHandler(int id, ECommand command);
        public event ItemClickHandler? ItemClickHandlerNotify;
        public delegate void BindByIdAndCommandHandler(int id, ECommand command);
        public event BindByIdAndCommandHandler? BindByIdAndCommandNotify;

        public ImageItem(DataHub data, Canvas? appCanvas = null, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Image)
        {
            Item = new Image();

            HandlerConnector();
        }

        public override void SetParameter(EParameter type, DefaultParameter dParam, int crazyChoice = 0)
        {
            base.SetParameter(type, dParam);

            try
            {
                switch (type)
                {
                    case EParameter.Image:
                        HandlerImgParam();
                        HandlerConnector();
                        HandlerEParam();
                        break;
                    case EParameter.Event:
                        HandlerEParam();
                        break;
                    case EParameter.Item:
                        HandlerIParam();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("SetParameter:" + e);
            }
        }
        protected override void HandlerConnector()
        {
            try
            {
                if (AppCanvas != null)
                {
                    if (((CommonParameter)param[EParameter.Common]).ParentId == -1)
                    {
                        AppCanvas.Children.Add(Item);
                    }
                    else
                    {
                        int _id = Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId);
                        if (_id != -1 && Data.items != null)
                        {
                            ((CanvasItem)Data.items[_id]).Item.Children.Add(Item);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("HandlerConnector:" + e);
            }
        }

        public override void HandlerEParam()
        {
            EventParameter? _eParam = param.ContainsKey(EParameter.Event) ?
               (EventParameter)param[EParameter.Event] :
               null;
            if (_eParam != null)
            {
                if (_eParam.IsMouseDown)
                {
                    Item.IsHitTestVisible = true;
                    Item.MouseDown += Item_ImageMouseDown;
                }
                if (_eParam.IsMouseUp)
                {
                    Item.MouseUp += Item_MouseUp;
                }
                if (_eParam.IsMouseMove)
                {
                    Item.MouseMove += Item_MouseMove;
                }
                if (_eParam.IsMouseWheel)
                {
                    Item.MouseWheel += Item_MouseWheel;
                }
            }
        }

        protected override void HandlerImgParam()
        {
            if (!param.ContainsKey(EParameter.Image) || 
                string.IsNullOrEmpty(((ImageParameter)param[EParameter.Image]).ImagePath))
            {
                return;
            }
            ((ImageParameter)param[EParameter.Image]).BitmapImage = new BitmapImage(
                new Uri(((ImageParameter)param[EParameter.Image]).ImagePath, 
                UriKind.RelativeOrAbsolute));
            Item = new Image
            {
                Width = ((ImageParameter)param[EParameter.Image]).BitmapImage.Width,
                Height = ((ImageParameter)param[EParameter.Image]).BitmapImage.Height,
                Source = ((ImageParameter)param[EParameter.Image]).BitmapImage
            };
            HandlerIParam();
        }

        protected override void HandlerIParam()
        {
            if (param.ContainsKey(EParameter.Item))
            {
                Item.Width = ((ItemParameter)param[EParameter.Item]).Width;
                Item.Height = ((ItemParameter)param[EParameter.Item]).Height;
                Canvas.SetLeft(Item, ((ItemParameter)param[EParameter.Item]).Left);
                Canvas.SetTop(Item, ((ItemParameter)param[EParameter.Item]).Top);
            }
        }

        public void EventBindedCommands(double x, double y)
        {
            if (Data.tapped == ((CommonParameter)param[EParameter.Common]).Id)
            {
                Trace.WriteLine("image move data.tapped:" + Data.tapped);
                BindByIdAndCommandNotify?.Invoke(
                    ((EventParameter)param[EParameter.Event]).CommandParameter,
                    ((EventParameter)param[EParameter.Event]).Command
                );
            }

            switch (((ItemParameter)param[EParameter.Item]).Vertical)
            {
                case EChildItemPosition.Bottom: 
                    {
                        if (((CommonParameter)param[EParameter.Common]).ParentId != -1)
                        {
                            DefaultItem _itm = Data.items[Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId)];
                            if (((ItemParameter)param[EParameter.Item]).Top !=
                                ((ItemParameter)_itm.param[EParameter.Item]).Height - ((ItemParameter)param[EParameter.Item]).Height)
                            {
                                ((ItemParameter)param[EParameter.Item]).Top = 
                                    ((ItemParameter)_itm.param[EParameter.Item]).Height - 
                                    ((ItemParameter)param[EParameter.Item]).Height;
                                Canvas.SetTop(Item, ((ItemParameter)param[EParameter.Item]).Top);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            switch (((ItemParameter)param[EParameter.Item]).Horizontal)
            {
                case EChildItemPosition.Left:
                    {
                        if (((CommonParameter)param[EParameter.Common]).ParentId != -1)
                        {
                            double _left = Canvas.GetLeft(Item);
                            if (_left != 0)
                            {
                                ((ItemParameter)param[EParameter.Item]).Left = 0;
                                Canvas.SetLeft(Item, ((ItemParameter)param[EParameter.Item]).Left);
                            }
                        }
                    }
                    break;
                case EChildItemPosition.Right:
                    {
                        if (((CommonParameter)param[EParameter.Common]).ParentId != -1)
                        {
                            DefaultItem _itm = Data.items[Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId)];
                            double _left = Canvas.GetLeft(Item);
                            if (_left !=
                                ((CanvasItem)_itm).Item.Width - ((ItemParameter)param[EParameter.Item]).Width)
                            {
                                ((ItemParameter)param[EParameter.Item]).Left =
                                    ((CanvasItem)_itm).Item.Width -
                                    ((ItemParameter)param[EParameter.Item]).Width;
                                Canvas.SetLeft(Item, ((ItemParameter)param[EParameter.Item]).Left);
                                
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        public override void ValueChanger(
            EBindParameter eBindParameter = EBindParameter.None,
            string txt = "")
        {
            switch (eBindParameter)
            {
                case EBindParameter.Z:
                    Panel.SetZIndex(Item, int.Parse(txt));
                    break;
                default:
                    break;
            }

            base.ValueChanger(eBindParameter, txt);
        }

        public void Item_ImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(Item);
            Data.tapXX = -mousePosition.X;
            Data.tapYY = -mousePosition.Y;
            Data.tapped = ((CommonParameter)param[EParameter.Common]).Id;
            e.Handled = true;
        }

        public override void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public void Item_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EventBindedCommands(-1, -1);
        }
    }
}