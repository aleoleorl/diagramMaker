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
        public Image item;

        public ImageItem(DataHub data, Canvas? appCanvas = null, int parentId = -1) : 
            base(data, appCanvas, parentId, EItem.Image)
        {
            item = new Image();

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
            if (imgParam == null || string.IsNullOrEmpty(imgParam?.imagePath))
            {
                return;
            }
            imgParam.bitmapImage = new BitmapImage(new Uri(imgParam.imagePath, UriKind.RelativeOrAbsolute));
            item = new Image
            {
                Width = imgParam.bitmapImage.Width,
                Height = imgParam.bitmapImage.Height,
                Source = imgParam.bitmapImage
            };            
        }

        protected void HandlerIParam()
        {
            if (iParam != null)
            {
                item.Width = iParam.width;
                item.Height = iParam.height;
                Canvas.SetLeft(item, iParam.left);
                Canvas.SetTop(item, iParam.top);
            }
        }

        protected void HandlerConnector()
        {
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
                        ((CanvasItem)data.items[_id]).item.Children.Add(item);
                    }
                }
            }
        }
    }
}