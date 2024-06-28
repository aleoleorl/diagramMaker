using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace diagramMaker.parameters
{
    public class ItemParameter:DefaultParameter
    {
        public double left;
        public double top;
        public double width;
        public double height;
        public Brush? bgColor;
        public Brush? frColor;

        public ItemParameter(double left, double top, double width, double height,
            Brush? bgColor = null, Brush? frColor = null) 
        {   
            this.left = left;
            this.top = top;
            this.width = width;
            this.height = height;
            this.bgColor = bgColor;
            this.frColor = frColor;
        }
    }
}
