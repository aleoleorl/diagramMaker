using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using diagramMaker.helpers;
using diagramMaker.parameters;

namespace diagramMaker.items
{
    public class PainterItem : DefaultItem
    {
        public Image image;
        public WriteableBitmap painter;        
        public byte[] sourcePixelData;
        public bool isDrawStart;
        private int oldX;
        private int oldY;

        public Border item;

        public PainterItem(DataHub data, Canvas? appCanvas, int parentId = -1) :
            base(data, appCanvas, parentId, EItem.Painter)
        {
            painter = new WriteableBitmap(
                pixelWidth: 100,
                pixelHeight: 100,
                dpiX: 96,
                dpiY: 96,
                pixelFormat: PixelFormats.Bgra32,
                palette: null);
            sourcePixelData = new byte[] { 0, 0, 255, 255 };
            image = new Image
            {
                Source = painter,
                Width = painter.PixelWidth,
                Height = painter.PixelHeight
            };
            isDrawStart = false;
            oldX = -1;
            oldY = -1;

            item = new Border();
            item.BorderThickness = new Thickness(2); // Set your desired thickness
            item.BorderBrush = Brushes.Black;
            item.Child = image;


            Canvas.SetLeft(item, 100);
            Canvas.SetTop(item, 100);


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

        public override void setParameters(
           ItemParameter? iParam,
           ContentParameter? content = null,
           BorderParameter? bParam = null,
           EventParameter? eParam = null,
           ImageParameter? imgParam = null,
           ShapeParameter? shapeParameter = null)
        {
            base.setParameters(iParam, content, bParam, eParam, imgParam);

            handlerImgParam();
            handlerIParam();
            handlerEParam();
           // handlerConnector();
        }
        public override void setParameter(EParameter type, DefaultParameter dParam)
        {
            base.setParameter(type, dParam);

            try
            {
                switch (type)
                {
                    case EParameter.Image:
                        handlerImgParam();
                        break;
                    case EParameter.Item:
                        handlerIParam();
                        break;
                    case EParameter.Event:
                        handlerEParam();
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

        protected void handlerImgParam()
        {
            if (imgParam == null && string.IsNullOrEmpty(imgParam?.imagePath))
            {
                return;
            }
        }

        protected void handlerIParam()
        {
            if (iParam != null)
            {
                item.Width = iParam.width;
                item.Height = iParam.height;
                Canvas.SetLeft(item, iParam.left);
                Canvas.SetTop(item, iParam.top);
            }
        }

        protected void handlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.isMouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
                if (eParam.isMouseUp)
                {
                    item.MouseUp += Item_MouseUp;
                }
                if (eParam.isMouseMove)
                {
                    item.MouseMove += Item_MouseMove; 
                }
                if (eParam.isMouseLeave)
                {
                    item.MouseLeave += Item_MouseLeave;
                }
            }
        }

        private void PaintHandler(int mX, int mY)
        {   
            if (mX<0)
            {
                mX=0;
            }
            if (mY < 0)
            {
                mY = 0;
            }
            if (iParam!= null && mX >= iParam.width)
            {
                mX = (int)iParam.width-1;
            }
            if (iParam != null && mY >= iParam.height)
            {
                mY = (int)iParam.height-1;
            }

            Int32Rect _rect = new Int32Rect(mX, mY, 1, 1);
            painter.WritePixels(_rect, sourcePixelData, 4, 0);

            if (mX != oldX || mY!=oldY) 
            {
                int _step = 0;
                int _count = 0;

                int minX = mX < oldX ? mX : oldX;
                int maxX = mX < oldX ? oldX : mX;
                int minY = mY < oldY ? mY : oldY;
                int maxY = mY < oldY ? oldY : mY;

                if (Math.Abs(mX - oldX) >= Math.Abs(mY - oldY))
                {
                    int _period = Math.Abs(mX-oldX) / ((mY - oldY)!=0?Math.Abs(mY-oldY):1);                    

                    for (int _i = 1; _i < maxX-minX; _i++)
                    {
                        _rect = new Int32Rect(minX+_i, minY+_step, 1, 1);
                        painter.WritePixels(_rect, sourcePixelData, 4, 0);
                        _count++;
                        if (_count >= _period)
                        {
                            _count = 0;
                            _step++;
                        }
                    }
                }
                else
                {
                    int _period = Math.Abs(mY - oldY) / ((mX - oldX)!=0?Math.Abs(mX - oldX):1);

                    for (int _i = 1; _i < maxY-minY; _i++)
                    { 
                        _rect = new Int32Rect(minX + _step, minY + _i, 1, 1);
                        painter.WritePixels(_rect, sourcePixelData, 4, 0);
                        _count++;
                        if (_count >= _period)
                        {
                            _count = 0;
                            _step++;
                        }
                    }
                }
            }

            oldX = mX;
            oldY = mY;
        }

        private void Item_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //e.Handled = true;
            switch (data.painterTool)
            {
                case EPainterTool.Move:
                    
                    break;
                case EPainterTool.Draw:
                    if (isDrawStart)
                    {
                        Point _mousePosition = e.GetPosition((IInputElement)sender);

                        PaintHandler((int)_mousePosition.X, (int)_mousePosition.Y);
                        e.Handled = true;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Item_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Default_MouseUp(sender, e);
            if (isDrawStart)
            {
                isDrawStart = false;
                oldX = -1;
                oldY = -1;
                e.Handled = true;
            }
            //e.Handled = true;
        }

        private void Item_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (data.painterTool)
            {
                case EPainterTool.Move:
                Point mousePosition = e.GetPosition(item);
                data.tapXX = -mousePosition.X;
                data.tapYY = -mousePosition.Y;
                data.tapped = id;

                e.Handled = true;
                    break;
                case EPainterTool.Draw:
                    isDrawStart = true;
                    Point _mousePosition = e.GetPosition((IInputElement)sender);
                    oldX = (int)_mousePosition.X;
                    oldY = (int)_mousePosition.Y;
                    PaintHandler((int)_mousePosition.X, (int)_mousePosition.Y);
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isDrawStart)
            {
                isDrawStart = false;
                oldX = -1;
                oldY = -1;
                e.Handled = true;
            }
        }

        public override void FinishHandling()
        {

            if (isDrawStart)
            {
                isDrawStart = false;
                oldX = -1;
                oldY = -1;
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
                case EBindParameter.Width:
                    if (iParam != null)
                    {
                        iParam.width = Convert.ToDouble(txt);
                    }
                    item.Width = Convert.ToDouble(txt); //border
                    image.Width = Convert.ToDouble(txt);
                    painter = painter.Resize(Convert.ToInt32(txt), painter.PixelHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
                    image.Source = painter;
                    break;
                case EBindParameter.Height:
                    if (iParam != null)
                    {
                        iParam.height = Convert.ToDouble(txt);
                    }
                    item.Height = Convert.ToDouble(txt); //border
                    image.Height = Convert.ToDouble(txt);
                    painter = painter.Resize(painter.PixelWidth, Convert.ToInt32(txt), WriteableBitmapExtensions.Interpolation.Bilinear);
                    image.Source = painter;
                    break;
                default:
                    break;
            }
        }
    }
}
