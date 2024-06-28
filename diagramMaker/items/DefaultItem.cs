using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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
        protected DataHub data;

        protected Canvas? appCanvas;

        public ItemParameter? iParam;
        public ContentParameter? content;
        public BorderParameter? bParam;
        public EventParameter? eParam;

        public DefaultItem(DataHub data, Canvas? appCanvas = null, int parentId = -1)
        {
            this.data = data;
            appX = data.topLeftX + data.oldMouseX;
            appY = data.topLeftY + data.oldMouseY;
            this.appCanvas = appCanvas;
            this.parentId = parentId;
            SetId();
            iParam = new ItemParameter(0, 0, 0, 0, null, null);

            //this.MouseUp += MainWindow_MouseUp;
        }

        private void SetId()
        {
            id = ID++;
        }

        public virtual void setParameters(
            ItemParameter? iParam = null, 
            ContentParameter? content = null, 
            BorderParameter? bParam = null,
            EventParameter? eParam = null)
        {
            this.iParam = iParam;
            this.content = content;
            this.bParam = bParam;
            this.eParam = eParam;
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

        //private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        //{
        //    Trace.WriteLine("MainWindow_MouseUp");
        //    tapped = false;
        //}

        //public void MyCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    Trace.WriteLine("MyCanvas_MouseDown");
        //    Point mousePosition = e.GetPosition(myCanvas);
        //    double mouseX = mousePosition.X;
        //    double canvasX = Canvas.GetLeft(myCanvas);
        //    xx = Math.Abs(mouseX - canvasX);
        //    double mouseY = mousePosition.Y;
        //    double canvasY = Canvas.GetLeft(myCanvas);
        //    yy = Math.Abs(mouseY - canvasY);
        //    tapped = true;
        //}
    }
}
