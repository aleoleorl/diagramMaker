using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace diagramMaker.items
{
    public class PainterItem : DefaultItem
    {
        public Image Image { get; set; }
        public WriteableBitmap Painter { get; set; }
        public byte[] SourcePixelData { get; set; }
        public bool IsDrawStart { get; set; }
        private int OldX { get; set; }
        private int OldY { get; set; }

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
            Image = new Image
            {
                Source = Painter,
                Width = Painter.PixelWidth,
                Height = Painter.PixelHeight
            };
            IsDrawStart = false;
            OldX = -1;
            OldY = -1;

            item = new Border();
            item.BorderThickness = new Thickness(2);
            item.BorderBrush = Brushes.Black;
            item.Child = Image;

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
                    if (_id != -1 && data.items != null)
                    {
                        ((CanvasItem)data.items[_id]).Item.Children.Add(item);
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
            base.SetParameters(iParam, content, bParam, eParam, imgParam);

            HandlerImgParam();
            HandlerIParam();
            HandlerEParam();
        }
        public override void SetParameter(EParameter type, DefaultParameter dParam, int crazyChoice = 0)
        {
            base.SetParameter(type, dParam);

            try
            {
                switch (type)
                {
                    case EParameter.Image:
                        HandlerImgParam();
                        break;
                    case EParameter.Item:
                        HandlerIParam();
                        break;
                    case EParameter.Event:
                        HandlerEParam();
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

        protected void HandlerImgParam()
        {
            if (ImgParam == null && string.IsNullOrEmpty(ImgParam?.ImagePath))
            {
                return;
            }
        }

        protected void HandlerIParam()
        {
            if (IParam != null)
            {
                item.Width = IParam.Width;
                item.Height = IParam.Height;
                Canvas.SetLeft(item, IParam.Left);
                Canvas.SetTop(item, IParam.Top);
            }
        }

        protected void HandlerEParam()
        {
            if (EParam != null)
            {
                if (EParam.IsMouseDown)
                {
                    item.MouseDown += Item_MouseDown;
                }
                if (EParam.IsMouseUp)
                {
                    item.MouseUp += Item_MouseUp;
                }
                if (EParam.IsMouseMove)
                {
                    item.MouseMove += Item_MouseMove; 
                }
                if (EParam.IsMouseLeave)
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
            if (IParam!= null && mX >= IParam.Width)
            {
                mX = (int)IParam.Width-1;
            }
            if (IParam != null && mY >= IParam.Height)
            {
                mY = (int)IParam.Height-1;
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
            switch (Data.painterTool)
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

        public override void Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            base.Item_MouseUp(sender, e);
            if (IsDrawStart)
            {
                IsDrawStart = false;
                OldX = -1;
                OldY = -1;
                e.Handled = true;
            }
        }

        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (Data.painterTool)
            {
                case EPainterTool.Move:
                Point mousePosition = e.GetPosition(item);
                Data.tapXX = -mousePosition.X;
                Data.tapYY = -mousePosition.Y;
                Data.tapped = Id;

                e.Handled = true;
                    break;
                case EPainterTool.Draw:
                    IsDrawStart = true;
                    Point _mousePosition = e.GetPosition((IInputElement)sender);
                    OldX = (int)_mousePosition.X;
                    OldY = (int)_mousePosition.Y;
                    PaintHandler((int)_mousePosition.X, (int)_mousePosition.Y);
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsDrawStart)
            {
                IsDrawStart = false;
                OldX = -1;
                OldY = -1;
                e.Handled = true;
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
                    Name = txt;
                    break;
                case EBindParameter.Width:
                    if (IParam != null)
                    {
                        IParam.Width = Convert.ToDouble(txt);
                    }
                    item.Width = Convert.ToDouble(txt); //border
                    Image.Width = Convert.ToDouble(txt);
                    Painter = Painter.Resize(Convert.ToInt32(txt), Painter.PixelHeight, WriteableBitmapExtensions.Interpolation.Bilinear);
                    Image.Source = Painter;
                    break;
                case EBindParameter.Height:
                    if (IParam != null)
                    {
                        IParam.Height = Convert.ToDouble(txt);
                    }
                    item.Height = Convert.ToDouble(txt); //border
                    Image.Height = Convert.ToDouble(txt);
                    Painter = Painter.Resize(Painter.PixelWidth, Convert.ToInt32(txt), WriteableBitmapExtensions.Interpolation.Bilinear);
                    Image.Source = Painter;
                    break;
                default:
                    break;
            }
        }
    }
}