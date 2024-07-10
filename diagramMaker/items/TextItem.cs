using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                    Item.Text = Content.Content;
                }
                if (Content.HorAlign != null)
                {
                    Item.HorizontalContentAlignment = Content.HorAlign ?? HorizontalAlignment.Left;
                }
                if (Content.VerAlign != null)
                {
                    Item.VerticalContentAlignment = Content.VerAlign ?? VerticalAlignment.Top;
                }
                if (Content.IsTextChanged)
                {
                    Item.TextChanged += Item_TextChanged;
                    Item.KeyDown += Item_KeyDown;
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
                if (EParam.IsMouseDown)
                {
                    Item.MouseDown += Item_MouseDown;
                }
            }
        }

        private void Item_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Content != null)
            {
                if (Content.IsDigitsOnly && !IsTextNumeric(Item.Text))
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
            if (Content != null && Content.BindID != 0 && Data.items != null)
            {
                Data.items[Data.GetItemByID(Content.BindID)].ValueChanger(Content.BindParameter, Item.Text);
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
                    Name = txt;
                    break;
                case EBindParameter.Content:
                    Item.Text = txt;
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
            base.Item_MouseDown(sender, e);
            Trace.WriteLine("Item_MouseDown");
        }
    }
}