using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using diagramMaker.items;
using System.Reflection;
using diagramMaker.parameters;
using diagramMaker.helpers;
using System.Windows.Interop;

namespace diagramMaker.items
{
    public class CanvasItem : DefaultItem
    {     
        public Canvas item;
        public Border? border;

        public CanvasItem(DataHub data, Canvas? appCanvas, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Canvas)
        {
            item = new Canvas();
            item.Background = Brushes.White;

            if (appCanvas != null)
            {
                if (parentId == -1)
                {
                    appCanvas.Children.Add(item);
                }
                else
                {
                    int _id = data.GetItemByID(parentId);
                    if (_id != -1)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Add(item);
                    }
                }
            }
        }

        public override void setParameters(
            ItemParameter? iParam = null,
            ContentParameter? content = null,
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParameter = null)
        {
            base.setParameters(iParam, content, bParam, eParam);

            handlerIParam();
            handlerBParam();
            handlerEParam();
        }
        public override void setParameter(EParameter type, DefaultParameter dParam)
        {
            base.setParameter(type, dParam);

            try
            {
                switch (type)
                {
                    case EParameter.Border:
                        handlerBParam();
                        break;
                    case EParameter.Event:
                        handlerEParam();
                        break;
                    case EParameter.Item:
                        handlerIParam();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("setParameter:" + e);
            }
        }

        protected void handlerIParam()
        {
            if (iParam != null)
            {
                if (iParam.bgColor != null)
                {
                    item.Background = iParam.bgColor;
                }
                Canvas.SetLeft(item, iParam.left);
                Canvas.SetTop(item, iParam.top);
                item.Width = iParam.width;
                item.Height = iParam.height;
            }
        }
        protected void handlerBParam()
        {
            if (bParam != null)
            {
                if (bParam.isBorder && border == null)
                {
                    border = new Border();
                    border.Background = new SolidColorBrush(Colors.Transparent);
                    item.Children.Add(border);
                }
                if (border != null & bParam != null)
                {
                    border = null;
                    border = new Border();
                    item.Children.Add(border);
                    border.BorderThickness = new Thickness(bParam.borderThickness);
                    border.BorderBrush = new SolidColorBrush(bParam.color ?? System.Windows.Media.Colors.Black);
                    border.CornerRadius = new CornerRadius(bParam.cornerRadius);
                    border.Width = iParam.width;
                    border.Height = iParam.height;
                    border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        protected void handlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.isMouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
                if (eParam.isMouseUp)
                {
                    item.MouseUp += Default_MouseUp;
                }
                if (eParam.isMouseMove)
                {
                    item.MouseMove += Default_MouseMove;
                }
                if (eParam.isMouseWheel)
                {
                    item.MouseWheel += Item_MouseWheel;
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
                    name = txt;
                    break;
                case EBindParameter.Width:
                    if (iParam != null)
                    {
                        iParam.width = Convert.ToDouble(txt);                        
                    }
                    item.Width = Convert.ToDouble(txt);
                    if (bParam != null && bParam.isBorder)
                    {
                        border.Width = Convert.ToDouble(txt);
                    }
                    break;
                case EBindParameter.Height:
                    if (iParam != null)
                    {
                        iParam.height = Convert.ToDouble(txt);
                    }
                    item.Height = Convert.ToDouble(txt);
                    if (bParam != null && bParam.isBorder)
                    {
                        border.Height = Convert.ToDouble(txt);
                    }
                    break;
                default:
                    break;
            }
        }

        public void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(item);
            data.tapXX = -mousePosition.X;
            data.tapYY = -mousePosition.Y;
            data.tapped = id;
            Trace.WriteLine("data.tapped:" + data.tapped);

            e.Handled = true;
        }

        public void CommonResizeHandler(double left, double top, double width, double height)
        {
            Canvas.SetLeft(item, left);
            Canvas.SetTop(item, top);
            item.Width = width;
            item.Height = height;
            if (iParam != null)
            {
                iParam.left = left;
                iParam.top = top;
                iParam.width = width; 
                iParam.height = height;
            }
        }
    }
}
