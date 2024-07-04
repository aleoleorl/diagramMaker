using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using diagramMaker.helpers;

namespace diagramMaker.parameters
{
    public class ShapeParameter : DefaultParameter
    {
        public EShape shape;
        public List<FigureContainer> vertex;
        private List<FigureContainer> tmp = 
            new List<FigureContainer> 
            { 
                new FigureContainer(x:1, y:1, id:1), 
                new FigureContainer(x:1, y:1, id:1) 
            };
        public Color? color;
        public double strokeThickness;

        public ShapeParameter(
            EShape shape = EShape.Line, 
            List<FigureContainer> vertex = null,
            Color? color = null,
            double strokeThickness = 1) 
        { 
            this.shape = shape;
            this.vertex = vertex!=null? vertex: tmp;
            this.color = color;
            this.strokeThickness = strokeThickness;
        }

        public override ShapeParameter Clone()
        {
            return (ShapeParameter)this.MemberwiseClone();
        }
    }
}
