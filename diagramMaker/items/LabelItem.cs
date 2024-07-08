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
        public Label item;

        public LabelItem(DataHub data, Canvas? appCanvas = null, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Label)
        {
            item = new Label();
            item.Content = "";

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

        public void SetContent(string? message, ItemParameter? iParam = null)
        {
            if (content != null)
            {
                content.content = message;
            }
            item.Content = message;

            if (iParam != null)
            {
                this.iParam = iParam;
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
                item.IsHitTestVisible = eParam.isHitTestVisible;
                if (eParam.isMouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
                if (eParam.isMouseDoubleClick)
                {
                    item.MouseDoubleClick += Item_MouseDoubleClick;
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
                    name = txt;
                    break;
                case EBindParameter.Content:
                    item.Content = txt;
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
                            item.Content = "Move";
                            break;
                        case 2:
                            item.Content = "Draw";
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
            if (content == null || data.items == null)
            {
                return;
            }
            switch (commang)
            {
                case ECommand.DescribeItem:
                    if (items.Count > content.count)
                    {
                        content.content = data.items[items[content.count]].name;

                        string _tmp = data.items[items[content.count]].name.Replace("_", "__");
                        item.Content = _tmp;
                        item.Visibility = Visibility.Visible;
                    } else
                    {
                        item.Visibility = Visibility.Hidden;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}