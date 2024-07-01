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
        public WriteableBitmap Painter;        
        public byte[] SourcePixelData;
        public bool IsDrawStart;
        public int OldX;
        public int OldY;
        public Rect ImageBounds;

        public Border item;

        public PainterItem(DataHub data, Canvas? appCanvas, int parentId = -1) :
            base(data, appCanvas, parentId, EItem.Painter)
        {
            Painter = new WriteableBitmap(
                pixelWidth: 100,
                pixelHeight: 100,
                dpiX: 96,
                dpiY: 96,
                pixelFormat: PixelFormats.Bgra32,
                palette: null);
            SourcePixelData = new byte[] { 0, 0, 255, 255 };
            image = new Image
            {
                Source = Painter,
                Width = Painter.PixelWidth,
                Height = Painter.PixelHeight
            };
            IsDrawStart = false;
            OldX = -1;
            OldY = -1;

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
           ImageParameter? imgParam = null)
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
                if (eParam.mouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
                if (eParam.mouseUp)
                {
                    item.MouseUp += Item_MouseUp;
                }
                if (eParam.MouseMove)
                {
                    item.MouseMove += Item_MouseMove; 
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
            Painter.WritePixels(_rect, SourcePixelData, 4, 0);

            if (mX != OldX || mY!=OldY) 
            {
                int _step = 0;
                int _count = 0;

                int minX = mX < OldX ? mX : OldX;
                int maxX = mX < OldX ? OldX : mX;
                int minY = mY < OldY ? mY : OldY;
                int maxY = mY < OldY ? OldY : mY;

                if (Math.Abs(mX - OldX) >= Math.Abs(mY - OldY))
                {
                    int _period = Math.Abs(mX-OldX) / ((mY - OldY)!=0?Math.Abs(mY-OldY):1);                    

                    for (int _i = 1; _i < maxX-minX; _i++)
                    {
                        _rect = new Int32Rect(minX+_i, minY+_step, 1, 1);
                        Painter.WritePixels(_rect, SourcePixelData, 4, 0);
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
                    int _period = Math.Abs(mY - OldY) / ((mX - OldX)!=0?Math.Abs(mX - OldX):1);

                    for (int _i = 1; _i < maxY-minY; _i++)
                    { 
                        _rect = new Int32Rect(minX + _step, minY + _i, 1, 1);
                        Painter.WritePixels(_rect, SourcePixelData, 4, 0);
                        _count++;
                        if (_count >= _period)
                        {
                            _count = 0;
                            _step++;
                        }
                    }
                }
            }

            OldX = mX;
            OldY = mY;
        }

        private void Item_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //e.Handled = true;
            switch (data.PainterTool)
            {
                case EPainterTool.Move:
                    
                    break;
                case EPainterTool.Draw:
                    if (IsDrawStart)
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
            if (IsDrawStart)
            {
                IsDrawStart = false;
                OldX = -1;
                OldY = -1;
                e.Handled = true;
            }
            //e.Handled = true;
        }

        private void Item_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            switch (data.PainterTool)
            {
                case EPainterTool.Move:
                Point mousePosition = e.GetPosition(item);
                data.tapXX = -mousePosition.X;
                data.tapYY = -mousePosition.Y;
                data.tapped = id;

                e.Handled = true;
                    break;
                case EPainterTool.Draw:
                    IsDrawStart = true;
                    Point _mousePosition = e.GetPosition((IInputElement)sender);
                    OldX = (int)_mousePosition.X;
                    OldY = (int)_mousePosition.Y;
                    ImageBounds = new Rect(0, 0, image.ActualWidth, image.ActualHeight);
                    PaintHandler((int)_mousePosition.X, (int)_mousePosition.Y);
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        public override void FinishHandling()
        {

            if (IsDrawStart)
            {
                IsDrawStart = false;
                OldX = -1;
                OldY = -1;
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
                    Painter = Painter.Resize(Convert.ToInt32(txt), Painter.PixelHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
                    image.Source = Painter;
                    break;
                case EBindParameter.Height:
                    if (iParam != null)
                    {
                        iParam.height = Convert.ToDouble(txt);
                    }
                    item.Height = Convert.ToDouble(txt); //border
                    image.Height = Convert.ToDouble(txt);
                    Painter = Painter.Resize(Painter.PixelWidth, Convert.ToInt32(txt), WriteableBitmapExtensions.Interpolation.Bilinear);
                    image.Source = Painter;
                    break;
                default:
                    break;
            }
        }
    }
}
