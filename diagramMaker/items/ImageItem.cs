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
            if (param[EParameter.Image] == null || string.IsNullOrEmpty(((ImageParameter)param[EParameter.Image]).ImagePath))
            {
                return;
            }
            ((ImageParameter)param[EParameter.Image]).BitmapImage = new BitmapImage(
                new Uri(((ImageParameter)param[EParameter.Image]).ImagePath, UriKind.RelativeOrAbsolute));
            Item = new Image
            {
                Width = ((ImageParameter)param[EParameter.Image]).BitmapImage.Width,
                Height = ((ImageParameter)param[EParameter.Image]).BitmapImage.Height,
                Source = ((ImageParameter)param[EParameter.Image]).BitmapImage
            };            
        }

        protected void HandlerIParam()
        {
            if (param[EParameter.Item] != null)
            {
                Item.Width = ((ItemParameter)param[EParameter.Item]).Width;
                Item.Height = ((ItemParameter)param[EParameter.Item]).Height;
                Canvas.SetLeft(Item, ((ItemParameter)param[EParameter.Item]).Left);
                Canvas.SetTop(Item, ((ItemParameter)param[EParameter.Item]).Top);
            }
        }

        protected void HandlerConnector()
        {
            if (AppCanvas != null)
            {
                if (((CommonParameter)param[EParameter.Common]).ParentId == -1)
                {
                    AppCanvas.Children.Add(Item);
                }
                else
                {
                    int _id = Data.GetItemByID(((CommonParameter)param[EParameter.Common]).ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        ((CanvasItem)Data.items[_id]).Item.Children.Add(Item);
                    }
                }
            }
        }
    }
}