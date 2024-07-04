using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace diagramMaker.items
{
    internal class ButtonItem : DefaultItem
    {
        public delegate void ItemClickHandler(int id, ECommand command);
        public event ItemClickHandler? itemClickHandlerNotify;

        public Button item;

        public ButtonItem(DataHub data, Canvas? appCanvas, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Button)
        {
            item = new Button();

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
            base.setParameters(iParam, content, bParam, eParam, imgParam);

            handlerIParam();
            handlerContent();
            handlerBParam();
            handlerEParam();
            handlerImgParam();
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
                    case EParameter.Content:
                        handlerContent();
                        break;
                    case EParameter.Image:
                        handlerImgParam();
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
        protected void handlerContent()
        {
            if (content != null)
            {
                if (!string.IsNullOrEmpty(content.content))
                {
                    item.Content = content.content;
                }

                if (content.horAlign != null)
                {
                    item.HorizontalContentAlignment = content.horAlign ?? HorizontalAlignment.Left;
                }
                if (content.verAlign != null)
                {
                    item.VerticalContentAlignment = content.verAlign ?? VerticalAlignment.Top;
                }
            }
        }
        protected void handlerBParam()
        {
            if (bParam != null)
            {
                if (bParam.isBorder)
                {
                    item.BorderThickness = new Thickness(bParam.borderThickness);
                    item.BorderBrush = new SolidColorBrush(bParam.color ?? Colors.Black);
                }
            }
        }
        protected void handlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.isMouseClick)
                {
                    item.Click += Item_Click;
                }
            }
        }       

        protected void handlerImgParam()
        {
            if (imgParam == null && string.IsNullOrEmpty(imgParam?.imagePath))
            {
                return;
            }
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(imgParam.imagePath, UriKind.RelativeOrAbsolute));
            item.Background = imageBrush;
        }

        public void Item_Click(object sender, RoutedEventArgs e)
        {
            if (eParam != null)
            {
                itemClickHandlerNotify?.Invoke(eParam.CommandParameter, eParam.Command);
            }
        }
    }
}
