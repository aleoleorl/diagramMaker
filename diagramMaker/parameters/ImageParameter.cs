using System.Windows.Media.Imaging;

namespace diagramMaker.parameters
{
    public class ImageParameter:DefaultParameter
    {
        public string? imagePath;
        public BitmapImage? bitmapImage;

        public ImageParameter()
        {
            
        }

        public ImageParameter(string imagePath)
        {
            this.imagePath = imagePath;
        }

        public override ImageParameter Clone()
        {
            return (ImageParameter)this.MemberwiseClone();
        }
    }
}