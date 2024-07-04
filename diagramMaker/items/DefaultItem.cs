using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using diagramMaker.helpers;
using diagramMaker.parameters;

namespace diagramMaker.items
{
    public class DefaultItem
    {
        public double appX;
        public double appY;
        public int id;
        private static int ID = 1;
        public int parentId;
        public int connectorId;
        public string name;

        protected DataHub data;

        protected Canvas? appCanvas;
        public EItem itemType;
        public EItemAttach itemAttach;

        public ItemParameter? iParam;
        public ContentParameter? content;
        public BorderParameter? bParam;
        public EventParameter? eParam;
        public ImageParameter? imgParam;
        public ShapeParameter? shapeParam;

        public delegate void MouseAppHandler(string item);
        public event MouseAppHandler? MouseAppNotify;
        public delegate void MouseAppIdHandler(int id);
        public event MouseAppIdHandler? MouseAppIdNotify;
        public delegate void MouseMoveHandler(int id);
        public event MouseMoveHandler? MouseMoveNotify;
        public delegate void MouseScrollHandler(int digit);
        public event MouseScrollHandler? MouseScrollNotify;
        public delegate void MouseDoubleClickHandler(int id);
        public event MouseDoubleClickHandler? MouseDoubleClickNotify;

        public DefaultItem(DataHub data, Canvas? appCanvas = null, int parentId = -1, EItem itemType = EItem.Default)
        {
            this.data = data;
            appX = data.topLeftX + data.oldMouseX;
            appY = data.topLeftY + data.oldMouseY;
            this.appCanvas = appCanvas;
            this.parentId = parentId;
            SetId();
            iParam = new ItemParameter(0, 0, 0, 0, null, null);
            name = "item_" + id;
            this.itemType = itemType;
            this.itemAttach = EItemAttach.Menu;
            connectorId = -1;
            shapeParam = new ShapeParameter();
        }

        private void SetId()
        {
            id = ID++;
        }
        public static int GetCurrentFreeId()
        {
            return ID;
        }
        public static void RestartID(int id = 1)
        {
            ID = id;
        }

        public virtual void setParameters(
            ItemParameter? iParam = null, 
            ContentParameter? content = null, 
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParam = null)
        {
            this.iParam = iParam!=null?iParam.Clone():null;
            this.content = content;
            this.bParam = bParam;
            this.eParam = eParam != null ? eParam.Clone() : null;
            this.imgParam = imgParam;
            this.shapeParam = shapeParam;
        }

        public virtual void setParameter(EParameter type, DefaultParameter dParam)
        {
            try
            {
                switch (type)
                {
                    case EParameter.Border:
                        this.bParam = (BorderParameter)dParam.Clone();
                        break;
                    case EParameter.Content:
                        this.content = (ContentParameter)dParam.Clone();
                        break;
                    case EParameter.Event:
                        this.eParam = ((EventParameter)dParam).Clone();
                        break;
                    case EParameter.Image:
                        this.imgParam = (ImageParameter)dParam.Clone();
                        break;
                    case EParameter.Item:
                        this.iParam = ((ItemParameter)dParam).Clone();
                        appX = data.topLeftX + ((ItemParameter)dParam).left;
                        appY = data.topLeftY + ((ItemParameter)dParam).top;
                        break;
                    case EParameter.Shape:
                        this.shapeParam = (ShapeParameter)dParam.Clone();
                        break;
                    default:
                        break;
                }
            } catch (Exception e) 
            {
                Trace.WriteLine("setParameter:"+e);
            }
        }

        public void Default_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (eParam != null)
            {
                MouseAppNotify?.Invoke(item: eParam.mouseUpInfo);
                MouseAppIdNotify?.Invoke(id: id);
            }
            e.Handled = true;
        }

        public void Default_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (data.tapped == id)
            {
                MouseMoveNotify?.Invoke(id: id);
            }
        }

        public virtual void ValueChanger(
            EBindParameter eBindParameter = EBindParameter.None,
            string txt = "")
        {

        }

        public virtual void FinishHandling()
        {

        }

        public virtual void EventOutdataHandler(int id, ECommand command)
        {
        }

        public void Item_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            
            if (e.Delta > 0)
            {
                MouseScrollNotify?.Invoke(-1);
            }
            else
            {
                MouseScrollNotify?.Invoke(1);
            }
        }
        public void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MouseDoubleClickNotify?.Invoke(id);
            e.Handled = true;
        }


        public void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MouseDoubleClickNotify?.Invoke(id);
            e.Handled = true;
        }
    }
}