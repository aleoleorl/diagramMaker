using System.Windows.Media;

namespace diagramMaker.parameters
{
    public class BorderParameter: DefaultParameter
    {
        public bool IsBorder { get; set; }
        public double BorderThickness { get; set; }
        public Color? Color { get; set; }
        public double CornerRadius { get; set; }

        public BorderParameter(
            bool isBorder = false,
            double borderThickness = 0, 
            Color? color = null,
            double cornerRadius = 0)
        {
            this.IsBorder = isBorder;
            this.BorderThickness = borderThickness;
            this.Color = color;
            this.CornerRadius = cornerRadius;
        }

        public override BorderParameter Clone()
        {
            return (BorderParameter)this.MemberwiseClone();
        }
    }
}
