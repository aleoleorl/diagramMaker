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

        public ItemParameter(ItemParameter param)
        {
            this.left = param.left;
            this.top = param.top;
            this.width = param.width;
            this.height = param.height;
            this.bgColor = param.bgColor;
            this.frColor = param.frColor;
        }

        public void GetData(ItemParameter param)
        {
            this.left = param.left;
            this.top = param.top;
            this.width = param.width;
            this.height = param.height;
            this.bgColor = param.bgColor;
            this.frColor = param.frColor;
        }

        public override ItemParameter Clone()
        {
            return (ItemParameter)this.MemberwiseClone();
        }
    }
}