using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
//using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using diagramMaker.helpers;
using diagramMaker.parameters;
using static System.Net.Mime.MediaTypeNames;

namespace diagramMaker.items
{

    public class LabelItem : DefaultItem
    {
        public Label item;

        public LabelItem(DataHub data, Canvas? appCanvas = null, int parentId = -1) : base(data, appCanvas, parentId)
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
                    if (_id != -1)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Add(item);
                    }
                }
            }
        }

        public void setContent(string? message, ItemParameter? iParam = null)
        {
            if (content != null)
            {
                Trace.WriteLine("message:" + message);
                content.content = message;
            }
            item.Content = message;

            if (iParam != null)
            {
                this.iParam = iParam;
                handlerIParam();
            }
        }        

        public override void setParameters(
            ItemParameter? iParam,
            ContentParameter? content = null, 
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null)
        {
            base.setParameters(iParam, content, bParam, eParam);

            handlerIParam();
            handlerContentParam();
            handlerBParam();
            handlerEParam();
        }
        public override void setParameter(EParameter type, DefaultParameter dParam)
        {
            base.setParameter(type, dParam);
            try
            {
                switch (type)
                {
                    case EParameter.Border:
                        handlerBParam();
                        break;
                    case EParameter.Content:
                        handlerContentParam();
                        break;
                    case EParameter.Event:
                        handlerEParam();
                        break;
                    case EParameter.Item:
                        handlerIParam();
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

        protected void handlerIParam()
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
        protected void handlerContentParam()
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
        protected void handlerBParam()
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
        protected void handlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.mouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
                item.IsHitTestVisible = eParam.IsHitTestVisible;
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
                default:
                    break;
            }
        }

        public void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Item_MouseDown");
        }
    }
}
