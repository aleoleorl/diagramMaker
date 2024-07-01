using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            handlerConnector();
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
            handlerConnector();
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
            imgParam.bitmapImage = new BitmapImage(new Uri(imgParam.imagePath, UriKind.RelativeOrAbsolute));
            item = new Image
            {
                Width = imgParam.bitmapImage.Width,
                Height = imgParam.bitmapImage.Height,
                Source = imgParam.bitmapImage
            };            
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

        protected void handlerConnector()
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
                    if (_id != -1)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Add(item);
                    }
                }
            }
        }
    }
}