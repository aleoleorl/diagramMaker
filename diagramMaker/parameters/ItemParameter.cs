using diagramMaker.helpers.enumerators;
using System.Windows.Media;

namespace diagramMaker.parameters
{
    public class ItemParameter:DefaultParameter
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public Brush? BgColor { get; set; }
        public Brush? FrColor { get; set; }

        public EChildItemPosition Vertical {  get; set; }
        public EChildItemPosition Horizontal { get; set; }

        public double shiftLeft;
        public double shiftTop;
        public double shiftRight;
        public double shiftBottom;


        public ItemParameter(double left, 
            double top, 
            double width, 
            double height,
            Brush? bgColor = null, 
            Brush? frColor = null,
            EChildItemPosition vert = EChildItemPosition.None,
            EChildItemPosition hor = EChildItemPosition.None) 
        {   
            this.Left = left;
            this.Top = top;
            this.Width = width;
            this.Height = height;
            this.BgColor = bgColor;
            this.FrColor = frColor;
            this.Vertical = vert;
            this.Horizontal = hor;

            shiftLeft = 0;
            shiftTop = 0;
            shiftRight = 0;
            shiftBottom = 0;
        }

        public ItemParameter(ItemParameter param)
        {
            this.Left = param.Left;
            this.Top = param.Top;
            this.Width = param.Width;
            this.Height = param.Height;
            this.BgColor = param.BgColor;
            this.FrColor = param.FrColor;
        }

        public void GetData(ItemParameter param)
        {
            this.Left = param.Left;
            this.Top = param.Top;
            this.Width = param.Width;
            this.Height = param.Height;
            this.BgColor = param.BgColor;
            this.FrColor = param.FrColor;
        }

        public override ItemParameter Clone()
        {
            return (ItemParameter)this.MemberwiseClone();
        }

        public void addShift(double parentWidth, double parentHeight)
        {
            shiftLeft = Left;
            shiftTop = Top;
            shiftRight = parentWidth - Left - Width;
            shiftBottom = parentHeight - Top - Height;
        }
    }
}