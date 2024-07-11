using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Windows.Controls;
using System.Windows.Input;

namespace diagramMaker.items
{
    public class DefaultItem
    {
        protected DataHub Data { get; set; }

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
        protected Canvas? AppCanvas { get; set; }
        public Dictionary<EParameter, DefaultParameter> param { get; set; }
        //public CommonParameter Common { get; set; }
        //public ItemParameter? IParam { get; set; }
        //public ContentParameter? Content { get; set; }
        //public BorderParameter? BParam { get; set; }
        //public EventParameter? EParam { get; set; }
        //public ImageParameter? ImgParam { get; set; }
        //public ShapeParameter? ShapeParam { get; set; }

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
            param = new Dictionary<EParameter, DefaultParameter>();

            param.Add(EParameter.Common, new CommonParameter());            
            CommonParameter _common = (CommonParameter)param[EParameter.Common];
            _common.AppX = data.topLeftX + data.oldMouseX;
            _common.AppY = data.topLeftY + data.oldMouseY;
            this.AppCanvas = appCanvas;
            _common.ParentId = parentId;
            SetId();
            _common.Name = "item_" + _common.Id;
            _common.ItemType = itemType;
            _common.ItemAttach = EItemAttach.Menu;
            _common.ConnectorId = -1;

            param.Add(EParameter.Item, new ItemParameter(0, 0, 0, 0, null, null));
        }

        private void SetId()
        {
            ((CommonParameter)param[EParameter.Common]).Id = ID;
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
            if (iParam != null)
            {
                if (param.ContainsKey(EParameter.Item))
                {
                    param[EParameter.Item] = iParam.Clone();
                } else
                {
                    param.Add(EParameter.Item, iParam.Clone());
                }
            }
            if (content != null)
            {
                if (param.ContainsKey(EParameter.Content))
                {
                    param[EParameter.Content] = content.Clone();
                }
                else
                {
                    param.Add(EParameter.Content, content.Clone());
                }
            }
            if (bParam != null)
            {
                if (param.ContainsKey(EParameter.Border))
                {
                    param[EParameter.Border] = bParam.Clone();
                }
                else
                {
                    param.Add(EParameter.Border, bParam.Clone());
                }
            }
            if (eParam != null)
            {
                if (param.ContainsKey(EParameter.Event))
                {
                    param[EParameter.Event] = eParam.Clone();
                }
                else
                {
                    param.Add(EParameter.Event, eParam.Clone());
                }
            }
            if (imgParam != null)
            {
                if (param.ContainsKey(EParameter.Image))
                {
                    param[EParameter.Image] = imgParam.Clone();
                }
                else
                {
                    param.Add(EParameter.Image, imgParam.Clone());
                }
            }
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
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Border))
                            {
                                param[EParameter.Border] = (BorderParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Border, (BorderParameter)dParam.Clone());
                            }
                        }
                        break;
                    case EParameter.Common:
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Common))
                            {
                                param[EParameter.Common] = (CommonParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Common, (CommonParameter)dParam.Clone());
                            }
                        }
                        break;
                    case EParameter.Content:
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Content))
                            {
                                param[EParameter.Content] = (ContentParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Content, (ContentParameter)dParam.Clone());
                            }
                        }
                        break;
                    case EParameter.Event:
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Event))
                            {
                                param[EParameter.Event] = (EventParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Event, (EventParameter)dParam.Clone());
                            }
                        }
                        break;
                    case EParameter.Image:
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Image))
                            {
                                param[EParameter.Image] = (ImageParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Image, (ImageParameter)dParam.Clone());
                            }
                        }
                        break;
                    case EParameter.Item:
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Item))
                            {
                                param[EParameter.Item] = (ItemParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Item, (ItemParameter)dParam.Clone());
                            }

                            if (param.ContainsKey(EParameter.Item))
                            {
                                ((CommonParameter)param[EParameter.Item]).AppX = Data.topLeftX + ((ItemParameter)dParam).Left;
                                ((CommonParameter)param[EParameter.Item]).AppY = Data.topLeftY + ((ItemParameter)dParam).Top;
                            }
                        }
                        break;
                    case EParameter.Shape:
                        if (dParam != null)
                        {
                            if (param.ContainsKey(EParameter.Shape))
                            {
                                param[EParameter.Shape] = (ShapeParameter)dParam.Clone();
                            }
                            else
                            {
                                param.Add(EParameter.Shape, (ShapeParameter)dParam.Clone());
                            }
                        }
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
            if (param.ContainsKey(EParameter.Event))
            {
                MouseAppNotify?.Invoke(item: ((EventParameter)param[EParameter.Event]).MouseUpInfo);
                MouseAppIdNotify?.Invoke(id: ((CommonParameter)param[EParameter.Common]).Id);
            }

            MouseDoubleClickNotify?.Invoke(((CommonParameter)param[EParameter.Common]).Id);
            e.Handled = true;
        }

        public void Default_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Data.tapped == ((CommonParameter)param[EParameter.Common]).Id)
            {
                MouseMoveNotify?.Invoke(id: ((CommonParameter)param[EParameter.Common]).Id);
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
            MouseDoubleClickNotify?.Invoke(((CommonParameter)param[EParameter.Common]).Id);
            e.Handled = true;
        }
    }
}