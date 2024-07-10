using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using diagramMaker.helpers;
using diagramMaker.parameters;

namespace diagramMaker.items
{

    public class LabelItem : DefaultItem
    {
        public Label Item { get; set; }

        public LabelItem(DataHub data, Canvas? appCanvas = null, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Label)
        {
            Item = new Label();
            Item.Content = "";

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

        public void SetContent(string? message, ItemParameter? iParam = null)
        {
            if (Content != null)
            {
                Content.Content = message;
            }
            Item.Content = message;

            if (iParam != null)
            {
                this.IParam = iParam;
                HandlerIParam();
            }
        }        

        public override void SetParameters(
            ItemParameter? iParam,
            ContentParameter? content = null, 
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParameter = null)
        {
            base.SetParameters(iParam, content, bParam, eParam);

            HandlerIParam();
            HandlerContentParam();
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
                    case EParameter.Content:
                        HandlerContentParam();
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
                if (IParam.FrColor != null)
                {
                    Item.Foreground = IParam.FrColor;
                }
                Item.Width = IParam.Width;
                Item.Height = IParam.Height;
                Canvas.SetLeft(Item, IParam.Left);
                Canvas.SetTop(Item, IParam.Top);
            }
        }

        protected void HandlerContentParam()
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
                Item.IsHitTestVisible = EParam.IsHitTestVisible;
                if (EParam.IsMouseDown)
                {
                    Item.MouseDown += Item_MouseDown;
                }
                if (EParam.IsMouseDoubleClick)
                {
                    Item.MouseDoubleClick += Item_MouseDoubleClick;
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
                case EBindParameter.Content:
                    Item.Content = txt;
                    break;
                case EBindParameter.Width:
                    if (IParam != null)
                    {
                        IParam.Width = Convert.ToDouble(txt);
                    }
                    Item.Width = Convert.ToDouble(txt);
                    break;
                case EBindParameter.Height:
                    if (IParam != null)
                    {
                        IParam.Height = Convert.ToDouble(txt);
                    }
                    Item.Height = Convert.ToDouble(txt);
                    break;
                default:
                    break;
            }
        }

        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Item_MouseDown");
        }

        public override void EventOutdataHandler(int id, ECommand command)
        {
            switch (command)
            {
                case ECommand.Data_PainterTool:
                    switch (id)
                    {
                        case 1:
                            Item.Content = "Move";
                            break;
                        case 2:
                            Item.Content = "Draw";
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void EventContent(ECommand commang, List<int> items)
        {
            if (Content == null || Data.items == null)
            {
                return;
            }
            switch (commang)
            {
                case ECommand.DescribeItem:
                    if (items.Count > Content.Count)
                    {
                        Content.Content = Data.items[items[Content.Count]].Name;

                        string _tmp = Data.items[items[Content.Count]].Name.Replace("_", "__");
                        Item.Content = _tmp;
                        Item.Visibility = Visibility.Visible;
                    } else
                    {
                        Item.Visibility = Visibility.Hidden;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}