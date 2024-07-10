using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace diagramMaker.items
{
    public class DefaultItem
    {
        public double AppX { get; set; }
        public double AppY { get; set; }
        public int Id { get; set; }

        private static int id = 1;
        private static int ID 
        {
            get { return id++; }
            set
            {
                if (DataHub.IsClear) 
                { 
                    id = value; 
                }
            }
        }
        public int ParentId { get; set; }
        public int ConnectorId { get; set; }
        public string Name { get; set; }

        protected DataHub Data { get; set; }

        protected Canvas? AppCanvas { get; set; }
        public EItem ItemType { get; set; }
        public EItemAttach ItemAttach { get; set; }

        public ItemParameter? IParam { get; set; }
        public ContentParameter? Content { get; set; }
        public BorderParameter? BParam { get; set; }
        public EventParameter? EParam { get; set; }
        public ImageParameter? ImgParam { get; set; }
        public ShapeParameter? ShapeParam { get; set; }

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
            this.Data = data;
            AppX = data.topLeftX + data.oldMouseX;
            AppY = data.topLeftY + data.oldMouseY;
            this.AppCanvas = appCanvas;
            this.ParentId = parentId;
            SetId();
            IParam = new ItemParameter(0, 0, 0, 0, null, null);
            Name = "item_" + Id;
            this.ItemType = itemType;
            this.ItemAttach = EItemAttach.Menu;
            ConnectorId = -1;
        }

        private void SetId()
        {
            Id = ID++;
        }
        public static int GetCurrentFreeId()
        {
            return ID;
        }
        public static void RestartID(int id = 1)
        {
            ID = id;
        }

        public virtual void SetParameters(
            ItemParameter? iParam = null, 
            ContentParameter? content = null, 
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParam = null)
        {
            this.IParam = iParam?.Clone();
            this.Content = content;
            this.BParam = bParam;
            this.EParam = eParam?.Clone();
            this.ImgParam = imgParam;
        }

        public virtual void SetParameter(EParameter type, DefaultParameter dParam, int crazyChoice = 0)
        {
            if (crazyChoice == 1)
            {
                return;
            }
            try
            {
                switch (type)
                {
                    case EParameter.Border:
                        this.BParam = (BorderParameter)dParam.Clone();
                        break;
                    case EParameter.Content:
                        this.Content = (ContentParameter)dParam.Clone();
                        break;
                    case EParameter.Event:
                        this.EParam = ((EventParameter)dParam).Clone();
                        break;
                    case EParameter.Image:
                        this.ImgParam = (ImageParameter)dParam.Clone();
                        break;
                    case EParameter.Item:
                        this.IParam = ((ItemParameter)dParam).Clone();
                        AppX = Data.topLeftX + ((ItemParameter)dParam).Left;
                        AppY = Data.topLeftY + ((ItemParameter)dParam).Top;
                        break;
                    case EParameter.Shape:
                        this.ShapeParam = ((ShapeParameter)dParam).Clone();
                        break;
                    default:
                        break;
                }
            } catch (Exception e) 
            {
                Trace.WriteLine("SetParameter:"+e);
            }
        }

        public virtual void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        public virtual void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (EParam != null)
            {
                MouseAppNotify?.Invoke(item: EParam.MouseUpInfo);
                MouseAppIdNotify?.Invoke(id: Id);
            }

            MouseDoubleClickNotify?.Invoke(Id);
            e.Handled = true;
        }

        public void Default_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Data.tapped == Id)
            {
                MouseMoveNotify?.Invoke(id: Id);
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
            MouseDoubleClickNotify?.Invoke(Id);
            e.Handled = true;
        }
    }
}