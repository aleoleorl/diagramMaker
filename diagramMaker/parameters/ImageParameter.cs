using System.Windows.Media.Imaging;

namespace diagramMaker.parameters
{
    public class ImageParameter:DefaultParameter
    {
        public string? ImagePath { get; set; }
        public BitmapImage? BitmapImage { get; set; }

        public ImageParameter()
        {
            
        }

        public ImageParameter(string imagePath)
        {
            this.ImagePath = imagePath;
        }

        public override ImageParameter Clone()
        {
            return (ImageParameter)this.MemberwiseClone();
        }
    }
}