using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace diagramMaker.parameters
{
    public class ImageParameter:DefaultParameter
    {
        public string? imagePath;
        public BitmapImage bitmapImage;

        public ImageParameter()
        {
            
        }

        public ImageParameter(string imagePath)
        {
            this.imagePath = imagePath;
        }
    }
}
