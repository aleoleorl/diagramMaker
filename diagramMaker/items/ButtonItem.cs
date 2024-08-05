﻿using diagramMaker.helpers.enumerators;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace diagramMaker.items
{
    internal class ButtonItem : DefaultItem
    {
        public Button Item { get; set; }

        public delegate void ItemClickHandler(int id, ECommand command);
        public event ItemClickHandler? ItemClickNotify;


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

        
        protected override void HandlerBParam()
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
        protected override void HandlerContent()
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

        public override void HandlerEParam()
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

        protected override void HandlerImgParam()
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

        protected override void HandlerIParam()
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

        public void EventBindedCommands(double x, double y)
        {
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
                case EChildItemPosition.BottomWithShift:
                    {
                        if (((CommonParameter)param[EParameter.Common]).ParentId != -1)
                        {
                            DefaultItem _itm = Data.items[Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId)];
                            if (((ItemParameter)param[EParameter.Item]).Top !=
                                ((ItemParameter)_itm.param[EParameter.Item]).Height - 
                                ((ItemParameter)param[EParameter.Item]).Height -
                                ((ItemParameter)param[EParameter.Item]).shiftBottom)
                            {
                                ((ItemParameter)param[EParameter.Item]).Top =
                                    ((ItemParameter)_itm.param[EParameter.Item]).Height -
                                    ((ItemParameter)param[EParameter.Item]).Height -
                                ((ItemParameter)param[EParameter.Item]).shiftBottom;
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
                case EChildItemPosition.RightWithShift:
                    {
                        if (((CommonParameter)param[EParameter.Common]).ParentId != -1)
                        {
                            DefaultItem _itm = Data.items[Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId)];
                            double _left = Canvas.GetLeft(Item);
                            if (_left !=
                                ((CanvasItem)_itm).Item.Width - 
                                ((ItemParameter)param[EParameter.Item]).Width -
                                ((ItemParameter)param[EParameter.Item]).shiftRight)
                            {
                                ((ItemParameter)param[EParameter.Item]).Left =
                                    ((CanvasItem)_itm).Item.Width -
                                    ((ItemParameter)param[EParameter.Item]).Width -
                                    ((ItemParameter)param[EParameter.Item]).shiftRight;
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

        public void Item_Click(object sender, RoutedEventArgs e)
        {
            EventParameter? _eParam = param.ContainsKey(EParameter.Event) ?
                (EventParameter)param[EParameter.Event] :
                null;
            int _send = 
                _eParam!=null && _eParam.CommandParameter != -1?
                _eParam.CommandParameter:
                ((CommonParameter)param[EParameter.Common]).Id;            
            if (_eParam != null)
            {
                ItemClickNotify?.Invoke(_send, _eParam.Command);
            }
        }
    }
}
