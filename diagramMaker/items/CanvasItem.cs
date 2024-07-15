using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using diagramMaker.parameters;
using diagramMaker.helpers.enumerators;

namespace diagramMaker.items
{
    public class CanvasItem : DefaultItem
    {
        public Canvas Item { get; set; }
        public Border? Border { get; set; }

        public delegate void EEventHandler(int id);
        public event EEventHandler? EEventNotify;

        public CanvasItem(DataHub data, Canvas? appCanvas, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Canvas)
        {
            Item = new Canvas();
            Item.Background = Brushes.White;

            if (appCanvas != null)
            {
                if (parentId == -1)
                {
                    appCanvas.Children.Add(Item);
                }
                else
                {
                    int _id = data.GetItemIndexByID(parentId);
                    if (_id != -1 && data.items != null)
                    {
                        ((CanvasItem)data.items[_id]).Item.Children.Add(Item);
                    }
                }
            }
        }

        public override void SetParameter(EParameter type, DefaultParameter dParam, int crazyChoice = 0)
        {
            base.SetParameter(type, dParam);

            try
            {
                switch (type)
                {
                    case EParameter.Border:
                        HandlerBParam();
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
       
        protected override void HandlerBParam()
        {
            BorderParameter? _bParam = param.ContainsKey(EParameter.Border) ?
                (BorderParameter)param[EParameter.Border] :
                null;
            ItemParameter? _iParam = param.ContainsKey(EParameter.Item) ?
                (ItemParameter)param[EParameter.Item] :
                null;
            if (_bParam != null)
            {
                if (_bParam.IsBorder && Border == null)
                {
                    Border = new Border();
                    Border.Background = new SolidColorBrush(Colors.Transparent);
                    Item.Children.Add(Border);
                }
                if (Border != null)
                {
                    Border = null;
                    Border = new Border();
                    Item.Children.Add(Border);
                    Border.BorderThickness = new Thickness(_bParam.BorderThickness);
                    Border.BorderBrush = new SolidColorBrush(_bParam.Color ?? Colors.Black);
                    Border.CornerRadius = new CornerRadius(_bParam.CornerRadius);
                    if (_iParam != null)
                    {
                        Border.Width = _iParam.Width;
                        Border.Height = _iParam.Height;
                    }
                    Border.Background = new SolidColorBrush(Colors.Transparent);
                }
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
                    Item.MouseDown += Item_MouseDown;
                }
                if (_eParam.IsMouseUp)
                {
                    Item.MouseUp += Item_MouseUp;
                }
                if (_eParam.IsMouseMove)
                {
                    Item.MouseMove += Default_MouseMove;
                }
                if (_eParam.IsMouseWheel)
                {
                    Item.MouseWheel += Item_MouseWheel;
                }
            }
        }

        protected override void HandlerIParam()
        {
            ItemParameter? _iParam = param[EParameter.Item] != null ?
                (ItemParameter)param[EParameter.Item] :
                null;
            if (_iParam != null)
            {
                if (_iParam.BgColor != null)
                {
                    Item.Background = _iParam.BgColor;
                }
                Canvas.SetLeft(Item, _iParam.Left);
                Canvas.SetTop(Item, _iParam.Top);
                Item.Width = _iParam.Width;
                Item.Height = _iParam.Height;
            }
        }

        public override void ValueChanger(
            EBindParameter eBindParameter = EBindParameter.None,
            string txt = "")
        {
            switch (eBindParameter)
            {
                case EBindParameter.Name:
                    if (param.ContainsKey(EParameter.Common))
                    {
                        ((CommonParameter)param[EParameter.Common]).Name = txt;
                    }
                    break;
                case EBindParameter.Width:                    
                    if (param.ContainsKey(EParameter.Item))
                    {
                        ((ItemParameter)param[EParameter.Item]).Width = Convert.ToDouble(txt);                        
                    }
                    Item.Width = Convert.ToDouble(txt);
                    if (Border != null && 
                        param.ContainsKey(EParameter.Border) && 
                        ((BorderParameter)param[EParameter.Border]).IsBorder)
                    {
                        Border.Width = Convert.ToDouble(txt);
                    }
                    break;
                case EBindParameter.Height:                    
                    if (param.ContainsKey(EParameter.Item))
                    {
                        ((ItemParameter)param[EParameter.Item]).Height = Convert.ToDouble(txt);
                    }
                    Item.Height = Convert.ToDouble(txt);
                    if (Border != null &&
                        param.ContainsKey(EParameter.Border) &&
                        ((BorderParameter)param[EParameter.Border]).IsBorder)
                    {
                        Border.Height = Convert.ToDouble(txt);
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {            
            Point mousePosition = e.GetPosition(Item);
            Data.tapXX = -mousePosition.X;
            Data.tapYY = -mousePosition.Y;
            Data.tapped = ((CommonParameter)param[EParameter.Common]).Id;
            Trace.WriteLine("data.tapped:" + Data.tapped);

            EEventNotify?.Invoke(((CommonParameter)param[EParameter.Common]).Id);

            e.Handled = true;
        }

        public void CommonResizeHandler(double left, double top, double width, double height)
        {
            Canvas.SetLeft(Item, left);
            Canvas.SetTop(Item, top);
            Item.Width = width;
            Item.Height = height;
            if (param[EParameter.Item] != null)
            {
                ((ItemParameter)param[EParameter.Item]).Left = left;
                ((ItemParameter)param[EParameter.Item]).Top = top;
                ((ItemParameter)param[EParameter.Item]).Width = width;
                ((ItemParameter)param[EParameter.Item]).Height = height;
            }
        }
    }
}