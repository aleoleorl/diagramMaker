using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace diagramMaker.items
{
    internal class ButtonItem : DefaultItem
    {
        public delegate void ItemClickHandler(int id, ECommand command);
        public event ItemClickHandler? ItemClickHandlerNotify;

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
                    if (_id != -1 && data.items != null)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Add(item);
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
            base.SetParameters(iParam, content, bParam, eParam, imgParam);

            HandlerIParam();
            HandlerContent();
            HandlerBParam();
            HandlerEParam();
            HandlerImgParam();
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
                    case EParameter.Content:
                        HandlerContent();
                        break;
                    case EParameter.Image:
                        HandlerImgParam();
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

        protected void HandlerIParam()
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
        protected void HandlerContent()
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
        protected void HandlerBParam()
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
        protected void HandlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.isMouseClick)
                {
                    item.Click += Item_Click;
                }
            }
        }       

        protected void HandlerImgParam()
        {
            if (imgParam == null || string.IsNullOrEmpty(imgParam?.imagePath))
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
                ItemClickHandlerNotify?.Invoke(eParam.CommandParameter, eParam.Command);
            }
        }
    }
}
