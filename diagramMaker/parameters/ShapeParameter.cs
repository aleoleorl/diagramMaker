using diagramMaker.helpers;
using System.Collections.Generic;
using System.Windows.Media;

namespace diagramMaker.parameters
{
    public class ShapeParameter : DefaultParameter
    {
        public EShape Shape { get; set; }
        public List<FigureContainer> Vertex { get; set; }
        public Color? Color { get; set; }
        public double StrokeThickness { get; set; }

        public ShapeParameter(
            EShape shape = EShape.Line,
            List<FigureContainer>? vertex = null,
            Color? color = null,
            double strokeThickness = 10)
        {
            this.Shape = shape;
            this.Vertex =
                    new List<FigureContainer>
                    {
                    new FigureContainer(x:1, y:1, id:-1),
                    new FigureContainer(x:1, y:1, id:-1)
                    };
            if (vertex != null)
            {
                for (int _i=0; _i< vertex.Count; _i++)
                {
                    this.Vertex[_i].x = vertex[_i].x;
                    this.Vertex[_i].y = vertex[_i].y;
                }
            }
            this.Color = color;
            this.StrokeThickness = strokeThickness;
        }

        public override ShapeParameter Clone()
        {
            ShapeParameter _tmp = (ShapeParameter)this.MemberwiseClone();
            _tmp.Vertex = new List<FigureContainer>();
            for (int _i = 0; _i < Vertex.Count; _i++)
            {
                _tmp.Vertex.Add(new FigureContainer(
                    x: Vertex[_i].x,
                    y: Vertex[_i].y,
                    id: Vertex[_i].id));
            }
            return _tmp;
        }
    }
}