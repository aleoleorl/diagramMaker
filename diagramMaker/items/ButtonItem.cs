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
        public Button Item { get; set; }

        public delegate void ItemClickHandler(int id, ECommand command);
        public event ItemClickHandler? ItemClickHandlerNotify;

        public ButtonItem(DataHub data, Canvas? appCanvas, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Button)
        {
            Item = new Button();

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
        protected void HandlerContent()
        {
            if (Content != null)
            {
                if (!string.IsNullOrEmpty(Content.Content))
                {
                    Item.Content = Content.Content;
                }

                if (Content.HorAlign != null)
                {
                    Item.HorizontalContentAlignment = Content.HorAlign ?? HorizontalAlignment.Left;
                }
                if (Content.VerAlign != null)
                {
                    Item.VerticalContentAlignment = Content.VerAlign ?? VerticalAlignment.Top;
                }
            }
        }
        protected void HandlerBParam()
        {
            if (BParam != null)
            {
                if (BParam.IsBorder)
                {
                    Item.BorderThickness = new Thickness(BParam.BorderThickness);
                    Item.BorderBrush = new SolidColorBrush(BParam.Color ?? Colors.Black);
                }
            }
        }
        protected void HandlerEParam()
        {
            if (EParam != null)
            {
                if (EParam.IsMouseClick)
                {
                    Item.Click += Item_Click;
                }
            }
        }       

        protected void HandlerImgParam()
        {
            if (ImgParam == null || string.IsNullOrEmpty(ImgParam?.ImagePath))
            {
                return;
            }
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(ImgParam.ImagePath, UriKind.RelativeOrAbsolute));
            Item.Background = imageBrush;
        }

        public void Item_Click(object sender, RoutedEventArgs e)
        {
            if (EParam != null)
            {
                ItemClickHandlerNotify?.Invoke(EParam.CommandParameter, EParam.Command);
            }
        }
    }
}
