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

namespace diagramMaker.items
{
    public class CanvasItem : DefaultItem
    {
        public Canvas item;
        public Border? border;

        public CanvasItem(DataHub data, Canvas? appCanvas, int parentId = -1) : base(data, appCanvas, parentId)
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
            EventParameter? eParam = null)
        {
            base.setParameters(iParam, content, bParam, eParam);

            handlerIParam();
            handlerBParam();
            handlerEParam();
        }
        public override void setParameter(EParameter type, DefaultParameter dParam)
        {
            base.setParameter(type, dParam);

            handlerIParam();
            handlerBParam();

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
                    border.BorderThickness = new Thickness(bParam.borderThickness);
                    border.BorderBrush = new SolidColorBrush(bParam.color ?? System.Windows.Media.Colors.Black);
                    border.CornerRadius = new CornerRadius(bParam.cornerRadius);
                    border.Width = iParam.width;
                    border.Height = iParam.height;
                }
            }
        }
        protected void handlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.mouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
            }
        }

        public void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(item);
            data.tapXX = -mousePosition.X;
            data.tapYY = -mousePosition.Y;
            data.tapped = id;

            e.Handled = true;
        }
    }
}
