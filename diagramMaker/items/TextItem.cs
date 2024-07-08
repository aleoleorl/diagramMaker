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
        public TextBox item;

        public TextItem(DataHub data, Canvas appCanvas, int parentId = -1) :
            base(data, appCanvas, parentId, EItem.Text)
        {
            item = new TextBox();
            item.Text = "";

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
            if (iParam != null)
            {
                if (iParam.bgColor != null)
                {
                    item.Background = iParam.bgColor;
                }
                if (iParam.frColor != null)
                {
                    item.Foreground = iParam.frColor;
                }
                item.Width = iParam.width;
                item.Height = iParam.height;
                Canvas.SetLeft(item, iParam.left);
                Canvas.SetTop(item, iParam.top);
            }
        }

        protected void HandlerContentParam()
        {
            if (content != null)
            {
                if (!string.IsNullOrEmpty(content.content))
                {
                    item.Text = content.content;
                }
                if (content.horAlign != null)
                {
                    item.HorizontalContentAlignment = content.horAlign ?? HorizontalAlignment.Left;
                }
                if (content.verAlign != null)
                {
                    item.VerticalContentAlignment = content.verAlign ?? VerticalAlignment.Top;
                }
                if (content.isTextChanged)
                {
                    item.TextChanged += Item_TextChanged;
                    item.KeyDown += Item_KeyDown;
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
                if (eParam.isMouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
            }
        }

        private void Item_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (content != null)
            {
                if (content.isDigitsOnly && !IsTextNumeric(item.Text))
                {
                    int _i = 0;
                    while (_i < item.Text.Length)
                    {
                        if (!IsTextNumeric(item.Text.Substring(_i, 1)))
                        {
                            item.Text = item.Text.Remove(_i, 1);
                            continue;
                        }
                        _i++;
                    }

                }
                if (item.Text.Length == 0)
                {
                    item.Text = "0";
                }
            }
        }

        private void Item_KeyDown(object sender, KeyEventArgs e)
        {
            if (content != null && content.bindID != 0 && data.items != null)
            {
                data.items[data.GetItemByID(content.bindID)].ValueChanger(content.bindParameter, item.Text);
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
                    name = txt;
                    break;
                case EBindParameter.Content:
                    item.Text = txt;
                    break;
                case EBindParameter.Width:
                    if (iParam != null)
                    {
                        iParam.width = Convert.ToDouble(txt);
                    }
                    item.Width = Convert.ToDouble(txt);
                    break;
                case EBindParameter.Height:
                    if (iParam != null)
                    {
                        iParam.height = Convert.ToDouble(txt);
                    }
                    item.Height = Convert.ToDouble(txt);
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