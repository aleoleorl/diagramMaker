using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using diagramMaker.helpers.enumerators;
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
                    case EParameter.Content:
                        HandlerContent();
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
            if (param.ContainsKey(EParameter.Border))
            {
                if (((BorderParameter)param[EParameter.Border]).IsBorder)
                {
                    Item.BorderThickness = new Thickness(((BorderParameter)param[EParameter.Border]).BorderThickness);
                    Item.BorderBrush = new SolidColorBrush(((BorderParameter)param[EParameter.Border]).Color ?? Colors.Black);
                }
            }
        }

        protected override void HandlerContent()
        {
            if (param.ContainsKey(EParameter.Content))
            {
                if (!string.IsNullOrEmpty(((ContentParameter)param[EParameter.Content]).Content))
                {
                    Item.Content = ((ContentParameter)param[EParameter.Content]).Content;
                }
                if (((ContentParameter)param[EParameter.Content]).HorAlign != null)
                {
                    Item.HorizontalContentAlignment = ((ContentParameter)param[EParameter.Content]).HorAlign ?? HorizontalAlignment.Left;
                }
                if (((ContentParameter)param[EParameter.Content]).VerAlign != null)
                {
                    Item.VerticalContentAlignment = ((ContentParameter)param[EParameter.Content]).VerAlign ?? VerticalAlignment.Top;
                }
            }
        }

        public override void HandlerEParam()
        {
            if (param.ContainsKey(EParameter.Event))
            {
                Item.IsHitTestVisible = ((EventParameter)param[EParameter.Event]).IsHitTestVisible;
                if (((EventParameter)param[EParameter.Event]).IsMouseDown)
                {
                    Item.MouseDown += Item_MouseDown;
                }
                if (((EventParameter)param[EParameter.Event]).IsMouseDoubleClick)
                {
                    Item.MouseDoubleClick += Item_MouseDoubleClick;
                }
            }
        }

        protected override void HandlerIParam()
        {
            if (param.ContainsKey(EParameter.Item))
            {
                if (((ItemParameter)param[EParameter.Item]).BgColor != null)
                {
                    Item.Background = ((ItemParameter)param[EParameter.Item]).BgColor;
                }
                if (((ItemParameter)param[EParameter.Item]).FrColor != null)
                {
                    Item.Foreground = ((ItemParameter)param[EParameter.Item]).FrColor;
                }
                Item.Width = ((ItemParameter)param[EParameter.Item]).Width;
                Item.Height = ((ItemParameter)param[EParameter.Item]).Height;
                Canvas.SetLeft(Item, ((ItemParameter)param[EParameter.Item]).Left);
                Canvas.SetTop(Item, ((ItemParameter)param[EParameter.Item]).Top);
            }
        }

        public override void ValueChanger(
            EBindParameter eBindParameter = EBindParameter.None,
            string txt = "")
        {
            switch (eBindParameter)
            {
                case EBindParameter.Name:
                    ((CommonParameter)param[EParameter.Common]).Name = txt;
                    break;
                case EBindParameter.Content:
                    Item.Content = txt;
                    break;
                case EBindParameter.Width:
                    if (param.ContainsKey(EParameter.Item))
                    {
                        ((ItemParameter)param[EParameter.Item]).Width = Convert.ToDouble(txt);
                    }
                    Item.Width = Convert.ToDouble(txt);
                    break;
                case EBindParameter.Height:
                    if (param.ContainsKey(EParameter.Item))
                    {
                        ((ItemParameter)param[EParameter.Item]).Height = Convert.ToDouble(txt);
                    }
                    Item.Height = Convert.ToDouble(txt);
                    break;
                default:
                    break;
            }
        }

        public void SetContent(string? message, ItemParameter? iParam = null)
        {
            if (param.ContainsKey(EParameter.Content))
            {
                ((ContentParameter)param[EParameter.Content]).Content = message;
            }
            Item.Content = message;

            if (iParam != null)
            {
                param[EParameter.Item] = iParam.Clone();
                HandlerIParam();
            }
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
            if (!param.ContainsKey(EParameter.Content) || Data.items == null)
            {
                return;
            }
            switch (commang)
            {
                case ECommand.DescribeItem:
                    if (items.Count > ((ContentParameter)param[EParameter.Content]).Count)
                    {
                        int _item = items[((ContentParameter)param[EParameter.Content]).Count];

                        ((ContentParameter)param[EParameter.Content]).Content =
                            ((CommonParameter)Data.items[_item].param[EParameter.Common]).Name;

                        string _tmp = ((CommonParameter)Data.items[_item].param[EParameter.Common]).Name.Replace("_", "__");
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
        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Label Item_MouseDown");
        }
    }
}