using diagramMaker.helpers.enumerators;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace diagramMaker.items
{
    public class TextItem : DefaultItem
    {
        public TextBox Item { get; set; }

        public TextItem(DataHub data, Canvas appCanvas, int parentId = -1) :
            base(data, appCanvas, parentId, EItem.Text)
        {
            Item = new TextBox();
            Item.Text = "";
            HandlerConnector();            
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
                Trace.WriteLine("setParameter:" + e);
            }
        }

        protected override void HandlerConnector()
        {
            if (AppCanvas != null && param.ContainsKey(EParameter.Common))
            {
                if (((CommonParameter)param[EParameter.Common]).ParentId == -1)
                {
                    AppCanvas.Children.Add(Item);
                }
                else
                {
                    int _id = Data.GetItemByID(((CommonParameter)param[EParameter.Common]).ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        ((CanvasItem)Data.items[_id]).Item.Children.Add(Item);
                    }
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

        protected override void HandlerContent()
        {
            if (param.ContainsKey(EParameter.Content))
            {
                if (!string.IsNullOrEmpty(((ContentParameter)param[EParameter.Content]).Content))
                {
                    Item.Text = ((ContentParameter)param[EParameter.Content]).Content;
                }
                if (((ContentParameter)param[EParameter.Content]).HorAlign != null)
                {
                    Item.HorizontalContentAlignment = ((ContentParameter)param[EParameter.Content]).HorAlign ?? HorizontalAlignment.Left;
                }
                if (((ContentParameter)param[EParameter.Content]).VerAlign != null)
                {
                    Item.VerticalContentAlignment = ((ContentParameter)param[EParameter.Content]).VerAlign ?? VerticalAlignment.Top;
                }
                if (((ContentParameter)param[EParameter.Content]).IsTextChanged)
                {
                    Item.TextChanged += Item_TextChanged;
                    Item.KeyDown += Item_KeyDown;
                }
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

        public override void HandlerEParam()
        {
            if (param.ContainsKey(EParameter.Event))
            {
                if (((EventParameter)param[EParameter.Event]).IsMouseDown)
                {
                    Item.MouseDown += Item_MouseDown;
                }
            }
        }

        private void Item_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (param.ContainsKey(EParameter.Content))
            {
                if (((ContentParameter)param[EParameter.Content]).IsDigitsOnly && !IsTextNumeric(Item.Text))
                {
                    int _i = 0;
                    while (_i < Item.Text.Length)
                    {
                        if (!IsTextNumeric(Item.Text.Substring(_i, 1)))
                        {
                            Item.Text = Item.Text.Remove(_i, 1);
                            continue;
                        }
                        _i++;
                    }

                }
                if (Item.Text.Length == 0)
                {
                    Item.Text = "0";
                }
            }
        }

        private void Item_KeyDown(object sender, KeyEventArgs e)
        {
            if (param.ContainsKey(EParameter.Content) && ((ContentParameter)param[EParameter.Content]).BindID != 0 && Data.items != null)
            {
                Data.items[Data.GetItemByID(((ContentParameter)param[EParameter.Content]).BindID)].ValueChanger(((ContentParameter)param[EParameter.Content]).BindParameter, Item.Text);
            }
        }

        private static bool IsTextNumeric(string str)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[^0-9]");
            return !reg.IsMatch(str);
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
                    Item.Text = txt;
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

        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            base.Item_MouseDown(sender, e);
            Trace.WriteLine("TextItem_MouseDown");
        }
    }
}