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
            ItemParameter? _iParam = param.ContainsKey(EParameter.Item) ? 
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
        protected void HandlerContent()
        {
            ContentParameter? _content = param.ContainsKey(EParameter.Content) ?
                (ContentParameter)param[EParameter.Content] :
                null;
            if (_content != null)
            {
                if (!string.IsNullOrEmpty(_content.Content))
                {
                    Item.Content = _content.Content;
                }

                if (_content.HorAlign != null)
                {
                    Item.HorizontalContentAlignment = _content.HorAlign ?? HorizontalAlignment.Left;
                }
                if (_content.VerAlign != null)
                {
                    Item.VerticalContentAlignment = _content.VerAlign ?? VerticalAlignment.Top;
                }
            }
        }
        protected void HandlerBParam()
        {
            BorderParameter? _bParam = param.ContainsKey(EParameter.Border) ?
                (BorderParameter)param[EParameter.Border] :
                null;
            if (_bParam != null)
            {
                if (_bParam.IsBorder)
                {
                    Item.BorderThickness = new Thickness(_bParam.BorderThickness);
                    Item.BorderBrush = new SolidColorBrush(_bParam.Color ?? Colors.Black);
                }
            }
        }
        protected void HandlerEParam()
        {
            EventParameter? _eParam = param.ContainsKey(EParameter.Event) ?
                (EventParameter)param[EParameter.Event] :
                null;
            if (_eParam != null)
            {
                if (_eParam.IsMouseClick)
                {
                    Item.Click += Item_Click;
                }
            }
        }       

        protected void HandlerImgParam()
        {
            ImageParameter? _imgParam = param.ContainsKey(EParameter.Image) ?
                (ImageParameter)param[EParameter.Image] :
                null;
            if (_imgParam == null || string.IsNullOrEmpty(_imgParam?.ImagePath))
            {
                return;
            }
            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri(_imgParam.ImagePath, UriKind.RelativeOrAbsolute));
            Item.Background = imageBrush;
        }

        public void Item_Click(object sender, RoutedEventArgs e)
        {
            EventParameter? _eParam = param.ContainsKey(EParameter.Event) ?
                (EventParameter)param[EParameter.Event] :
                null;
            if (_eParam != null)
            {
                ItemClickHandlerNotify?.Invoke(_eParam.CommandParameter, _eParam.Command);
            }
        }
    }
}
