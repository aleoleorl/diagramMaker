using diagramMaker.helpers;
using System.Collections.Generic;
using System.Windows.Media;

namespace diagramMaker.parameters
{
    public class ShapeParameter : DefaultParameter
    {
        public EShape shape;
        public List<FigureContainer> vertex;
        public Color? color;
        public double strokeThickness;

        public ShapeParameter(
            EShape shape = EShape.Line,
            List<FigureContainer>? vertex = null,
            Color? color = null,
            double strokeThickness = 10)
        {
            this.shape = shape;
            this.vertex =
                    new List<FigureContainer>
                    {
                    new FigureContainer(x:1, y:1, id:-1),
                    new FigureContainer(x:1, y:1, id:-1)
                    };
            if (vertex != null)
            {
                for (int _i=0; _i< vertex.Count; _i++)
                {
                    this.vertex[_i].x = vertex[_i].x;
                    this.vertex[_i].y = vertex[_i].y;
                }
            }
            this.color = color;
            this.strokeThickness = strokeThickness;
        }

        public override ShapeParameter Clone()
        {
            ShapeParameter _tmp = (ShapeParameter)this.MemberwiseClone();
            _tmp.vertex = new List<FigureContainer>();
            for (int _i = 0; _i < vertex.Count; _i++)
            {
                _tmp.vertex.Add(new FigureContainer(
                    x: vertex[_i].x,
                    y: vertex[_i].y,
                    id: vertex[_i].id));
            }
            return _tmp;
        }
    }
}