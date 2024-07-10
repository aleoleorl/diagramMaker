using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using diagramMaker.parameters;
using diagramMaker.helpers;

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
                    int _id = data.GetItemByID(parentId);
                    if (_id != -1 && data.items != null)
                    {
                        ((CanvasItem)data.items[_id]).Item.Children.Add(Item);
                    }
                }
            }
        }

        public override void SetParameters(
            ItemParameter? iParam = null,
            ContentParameter? content = null,
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParameter = null)
        {
            base.SetParameters(iParam, content, bParam, eParam);

            HandlerIParam();
            HandlerBParam();
            HandlerEParam();
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

        protected void HandlerIParam()
        {
            if (IParam != null)
            {
                if (IParam.BgColor != null)
                {
                    Item.Background = IParam.BgColor;
                }
                Canvas.SetLeft(Item, IParam.Left);
                Canvas.SetTop(Item, IParam.Top);
                Item.Width = IParam.Width;
                Item.Height = IParam.Height;
            }
        }
        protected void HandlerBParam()
        {
            if (BParam != null)
            {
                if (BParam.IsBorder && Border == null)
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
                    Border.BorderThickness = new Thickness(BParam.BorderThickness);
                    Border.BorderBrush = new SolidColorBrush(BParam.Color ?? Colors.Black);
                    Border.CornerRadius = new CornerRadius(BParam.CornerRadius);
                    if (IParam != null)
                    {
                        Border.Width = IParam.Width;
                        Border.Height = IParam.Height;
                    }
                    Border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        protected void HandlerEParam()
        {
            if (EParam != null)
            {
                if (EParam.IsMouseDown)
                {
                    Item.MouseDown += Item_MouseDown;
                }
                if (EParam.IsMouseUp)
                {
                    Item.MouseUp += Item_MouseUp;
                }
                if (EParam.IsMouseMove)
                {
                    Item.MouseMove += Default_MouseMove;
                }
                if (EParam.IsMouseWheel)
                {
                    Item.MouseWheel += Item_MouseWheel;
                }
            }
        }

        public override void ValueChanger(
            EBindParameter eBindParameter = EBindParameter.None,
            string txt = "")
        {
            switch (eBindParameter)
            {
                case EBindParameter.Name:
                    Name = txt;
                    break;
                case EBindParameter.Width:
                    if (IParam != null)
                    {
                        IParam.Width = Convert.ToDouble(txt);                        
                    }
                    Item.Width = Convert.ToDouble(txt);
                    if (Border != null && BParam!=null && BParam.IsBorder)
                    {
                        Border.Width = Convert.ToDouble(txt);
                    }
                    break;
                case EBindParameter.Height:
                    if (IParam != null)
                    {
                        IParam.Height = Convert.ToDouble(txt);
                    }
                    Item.Height = Convert.ToDouble(txt);
                    if (Border != null && BParam != null && BParam.IsBorder)
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
            Data.tapped = Id;
            Trace.WriteLine("data.tapped:" + Data.tapped);

            EEventNotify?.Invoke(Id);

            e.Handled = true;
        }

        public void CommonResizeHandler(double left, double top, double width, double height)
        {
            Canvas.SetLeft(Item, left);
            Canvas.SetTop(Item, top);
            Item.Width = width;
            Item.Height = height;
            if (IParam != null)
            {
                IParam.Left = left;
                IParam.Top = top;
                IParam.Width = width; 
                IParam.Height = height;
            }
        }
    }
}