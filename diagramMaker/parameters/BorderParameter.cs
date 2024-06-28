using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace diagramMaker.parameters
{
    public class BorderParameter: DefaultParameter
    {
        public bool isBorder;
        public double borderThickness;
        public Color? color;
        public double cornerRadius;

        public BorderParameter(
            bool isBorder = false,
            double borderThickness = 0, 
            Color? color = null,
            double cornerRadius = 0)
        {
            this.isBorder = isBorder;
            this.borderThickness = borderThickness;
            this.color = color;
            this.cornerRadius = cornerRadius;
        }
    }
}
