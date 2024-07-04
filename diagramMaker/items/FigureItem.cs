using diagramMaker.helpers;
using diagramMaker.parameters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace diagramMaker.items
{
    public class FigureItem : DefaultItem
    {
        public List<Line> item;

        public FigureItem(DataHub data, Canvas? appCanvas, int parentId = -1, EShape shape = EShape.Line) :
            base(data, appCanvas, parentId, EItem.Figure)
        {
            item = new List<Line>();
        }

        public void HandleAddItem(int id)
        {
            if (appCanvas != null)
            {
                if (parentId == -1)
                {
                    appCanvas.Children.Add(item[id]);

                }
                else
                {
                    int _id = data.GetItemByID(parentId);
                    if (_id != -1)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Add(item[id]);

                    }
                }
            }
        }
        public void HandleRemoveItem(int id)
        {
            if (appCanvas != null)
            {
                if (parentId == -1)
                {
                    appCanvas.Children.Remove(item[id]);

                }
                else
                {
                    int _id = data.GetItemByID(parentId);
                    if (_id != -1)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Remove(item[id]);

                    }
                }

                item.RemoveAt(id);
            }
        }

        public override void setParameters(
            ItemParameter? iParam = null,
            ContentParameter? content = null,
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParam = null)
        {
            base.setParameters(iParam, content, bParam, eParam);

            handlerIParam();
            handlerShapeParam();
            handlerEParam();
        }
        public override void setParameter(EParameter type, DefaultParameter dParam)
        {
            base.setParameter(type, dParam);

            try
            {
                switch (type)
                {
                    case EParameter.Shape:
                        handlerShapeParam();
                        break;
                    case EParameter.Event:
                        handlerEParam();
                        break;
                    case EParameter.Item:
                        handlerIParam();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("setParameter:" + e);
            }
        }

        protected void handlerEParam()
        {
            if (eParam != null)
            {
                if (eParam.isMouseDown)
                {
                    for (int _i = 0; _i < item.Count; _i++)
                    {
                        item[_i].MouseDown += Item_MouseDown;
                    }
                }
                if (eParam.isMouseMove)
                {
                    for (int _i = 0; _i < item.Count; _i++)
                    {
                        item[_i].MouseMove += Item_MouseMove;
                    }
                }
                if (eParam.isMouseUp)
                {
                    for (int _i = 0; _i < item.Count; _i++)
                    {
                        item[_i].MouseUp += Default_MouseUp;
                    }
                }

            }
        }

        protected void handlerIParam()
        {
        }

        public void handlerShapeParam()
        {
            if (shapeParam == null) 
            {
                return;
            }
            while (item.Count > 0)
            {
                HandleRemoveItem(0);
            }
            //
            int _count = 1;
            shapeParam.vertex = new List<FigureContainer>();
            for (int _i = 0; _i < data.items.Count; _i++)
            {
                if (data.items[_i].connectorId == id)
                {
                    if (shapeParam.vertex.Count < _count)
                    {
                        shapeParam.vertex.Add(
                            new FigureContainer(
                                x: data.items[_i].iParam.left + data.items[_i].iParam.width/2,
                                y: data.items[_i].iParam.top + data.items[_i].iParam.height/2,
                                id: data.items[_i].id));
                        data.items[_i].MouseMoveNotify += this.FigureItem_MouseMoveNotify;
                        if (_count > 1)
                        {
                            Line _tmp = new Line();
                            _tmp.Stroke = new SolidColorBrush(Colors.Black);
                            _tmp.StrokeThickness = shapeParam.strokeThickness;

                            _tmp.X1 = shapeParam.vertex[_count - 2].x;
                            _tmp.Y1 = shapeParam.vertex[_count - 2].y;
                            _tmp.X2 = shapeParam.vertex[_count - 1].x;
                            _tmp.Y2 = shapeParam.vertex[_count - 1].y;
                            item.Add(_tmp);
                            HandleAddItem(item.Count - 1);
                        }
                        _count++;
                    }
                }

            }
        }

        public void FigureItem_MouseMoveNotify(int id)
        {
            if (shapeParam == null)
            {
                return;
            }
            for (int _i = 0; _i < shapeParam.vertex.Count; _i++)
            {
                if (shapeParam != null && shapeParam.vertex[_i].id == id)
                {
                    if (_i > 0)
                    {
                        shapeParam.vertex[_i].x = data.items[data.GetItemByID(id)].iParam.left +
                            data.items[data.GetItemByID(id)].iParam.width/2;
                        shapeParam.vertex[_i].y = data.items[data.GetItemByID(id)].iParam.top +
                            data.items[data.GetItemByID(id)].iParam.height/2;

                        item[_i - 1].X2 = shapeParam.vertex[_i].x;
                        item[_i - 1].Y2 = shapeParam.vertex[_i].y;
                    }
                    if (_i < shapeParam.vertex.Count - 1)
                    {
                        shapeParam.vertex[_i].x = data.items[data.GetItemByID(id)].iParam.left +
                            data.items[data.GetItemByID(id)].iParam.width / 2;
                        shapeParam.vertex[_i].y = data.items[data.GetItemByID(id)].iParam.top +
                            data.items[data.GetItemByID(id)].iParam.height / 2;

                        item[_i].X1 = shapeParam.vertex[_i].x;
                        item[_i].Y1 = shapeParam.vertex[_i].y;

                    }
                }
            }
        }

        public void ReVertex()
        {
            int _id = -1;
            for (int _i = 0; _i < shapeParam.vertex.Count; _i++)
            {
                if (shapeParam != null)
                {
                    _id = shapeParam.vertex[_i].id;
                    if (_i > 0)
                    {
                        shapeParam.vertex[_i].x = data.items[data.GetItemByID(_id)].iParam.left +
                            data.items[data.GetItemByID(_id)].iParam.width / 2;
                        shapeParam.vertex[_i].y = data.items[data.GetItemByID(_id)].iParam.top +
                            data.items[data.GetItemByID(_id)].iParam.height / 2;

                        item[_i - 1].X2 = shapeParam.vertex[_i].x;
                        item[_i - 1].Y2 = shapeParam.vertex[_i].y;
                    }
                    if (_i < shapeParam.vertex.Count - 1)
                    {
                        shapeParam.vertex[_i].x = data.items[data.GetItemByID(_id)].iParam.left +
                            data.items[data.GetItemByID(_id)].iParam.width / 2;
                        shapeParam.vertex[_i].y = data.items[data.GetItemByID(_id)].iParam.top +
                            data.items[data.GetItemByID(_id)].iParam.height / 2;

                        item[_i].X1 = shapeParam.vertex[_i].x;
                        item[_i].Y1 = shapeParam.vertex[_i].y;

                    }
                }
            }
        }

        public void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(item[0]);
            data.tapXX = -mousePosition.X;
            data.tapYY = -mousePosition.Y;
            data.tapped = id;
            Trace.WriteLine("data.tapped:" + data.tapped);

            e.Handled = true;
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}
