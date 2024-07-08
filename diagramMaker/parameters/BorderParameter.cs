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

        public override BorderParameter Clone()
        {
            return (BorderParameter)this.MemberwiseClone();
        }
    }
}
