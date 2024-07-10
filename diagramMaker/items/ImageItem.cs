using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace diagramMaker.items
{
    public class ImageItem : DefaultItem
    {
        public Image Item { get; set; }

        public ImageItem(DataHub data, Canvas? appCanvas = null, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Image)
        {
            Item = new Image();

            HandlerConnector();
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
            HandlerConnector();
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
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("SetParameter:" + e);
            }
        }

        protected void HandlerImgParam()
        {
            if (ImgParam == null || string.IsNullOrEmpty(ImgParam?.ImagePath))
            {
                return;
            }
            ImgParam.BitmapImage = new BitmapImage(new Uri(ImgParam.ImagePath, UriKind.RelativeOrAbsolute));
            Item = new Image
            {
                Width = ImgParam.BitmapImage.Width,
                Height = ImgParam.BitmapImage.Height,
                Source = ImgParam.BitmapImage
            };            
        }

        protected void HandlerIParam()
        {
            if (IParam != null)
            {
                Item.Width = IParam.Width;
                Item.Height = IParam.Height;
                Canvas.SetLeft(Item, IParam.Left);
                Canvas.SetTop(Item, IParam.Top);
            }
        }

        protected void HandlerConnector()
        {
            if (AppCanvas != null)
            {
                if (ParentId == -1)
                {
                    AppCanvas.Children.Add(Item);
                }
                else
                {
                    int _id = Data.GetItemByID(ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        ((CanvasItem)Data.items[_id]).Item.Children.Add(Item);
                    }
                }
            }
        }
    }
}