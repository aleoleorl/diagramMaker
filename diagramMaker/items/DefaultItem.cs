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
        public string name;

        protected DataHub data;

        protected Canvas? appCanvas;
        public EItem itemType;

        public ItemParameter? iParam;
        public ContentParameter? content;
        public BorderParameter? bParam;
        public EventParameter? eParam;
        public ImageParameter? imgParam;

        public delegate void MouseAppHandler(string item);
        public event MouseAppHandler? MouseAppHandlerNotify;
        public delegate void MouseAppIdHandler(int id);
        public event MouseAppIdHandler? MouseAppIdHandlerNotify;

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
        }

        private void SetId()
        {
            id = ID++;
        }

        public virtual void setParameters(
            ItemParameter? iParam = null, 
            ContentParameter? content = null, 
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null)
        {
            this.iParam = iParam;
            this.content = content;
            this.bParam = bParam;
            this.eParam = eParam;
            this.imgParam = imgParam;
        }

        public virtual void setParameter(EParameter type, DefaultParameter dParam)
        {
            try
            {
                switch (type)
                {
                    case EParameter.Border:
                        this.bParam = (BorderParameter)dParam;
                        break;
                    case EParameter.Content:
                        this.content = (ContentParameter)dParam;
                        break;
                    case EParameter.Event:
                        this.eParam = (EventParameter)dParam;
                        break;
                    case EParameter.Image:
                        this.imgParam = (ImageParameter)dParam;
                        break;
                    case EParameter.Item:
                        this.iParam = (ItemParameter)dParam;
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
                MouseAppHandlerNotify?.Invoke(item: eParam.MouseUpInfo);
                MouseAppIdHandlerNotify?.Invoke(id: id);
            }
            if (data.tapped != -1)
            {
                data.tapped = -1;
                data.tapXX = 0;
                data.tapYY = 0;
            }
            e.Handled = true;
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
    }
}