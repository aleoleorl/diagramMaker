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
                    if (_id != -1 && data.items != null)
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
                    if (_id != -1 && data.items != null)
                    {
                        ((CanvasItem)data.items[_id]).item.Children.Remove(item[id]);

                    }
                }

                item.RemoveAt(id);
            }
        }

        public override void SetParameters(
            ItemParameter? iParam = null,
            ContentParameter? content = null,
            BorderParameter? bParam = null,
            EventParameter? eParam = null,
            ImageParameter? imgParam = null,
            ShapeParameter? shapeParam = null)
        {
            base.SetParameters(iParam, content, bParam, eParam);

            HandlerIParam();
            HandlerShapeParam();
            HandlerEParam();
        }
        public override void SetParameter(EParameter type, DefaultParameter dParam, int crazyChoice = 0)
        {
            base.SetParameter(type, dParam);
            if (crazyChoice == -1)
            {
                return;
            }
            try
            {
                switch (type)
                {
                    case EParameter.Shape:
                        HandlerShapeParam();
                        break;
                    case EParameter.Event:
                        HandlerEParam();
                        break;
                    case EParameter.Item:
                        HandlerIParam();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("SetParameter:" + e);
            }
        }

        public void HandlerEParam()
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
                        item[_i].MouseUp += Item_MouseUp;
                    }
                }

            }
        }

        protected void HandlerIParam()
        {
        }

        public void AppCoord()
        {
            if (shapeParam != null && shapeParam.vertex.Count > 0)
            {
                appX = data.topLeftX + shapeParam.vertex[0].x;
                appY = data.topLeftY + shapeParam.vertex[0].y;
            }
        }

        public void HandlerShapeParam()
        {
            if (shapeParam == null || data.items == null) 
            {
                return;
            }
            while (item.Count > 0)
            {
                HandleRemoveItem(0);
            }
            //
            int _count = 0;
            //
            for (int _i = 0; _i < data.items.Count; _i++)
            {
                if (data.items[_i].connectorId == id)
                {
                    data.items[_i].MouseMoveNotify -= this.FigureItem_MouseMoveNotify;
                }
            }
            //
            int _j = 0;
            while (_j < shapeParam.vertex.Count)
            {
                for (int _i = 0; _i < data.items.Count; _i++)
                {
                    if (data.items[_i].connectorId == id)
                    {
                        if (shapeParam.vertex[_j].id == -1)
                        {
                            bool _isNot = true;
                            for (int _k = 0; _k < shapeParam.vertex.Count; _k++)
                            {
                                if (shapeParam.vertex[_k].id == data.items[_i].id)
                                {
                                    _isNot = false;
                                    break;
                                }
                            }
                            if (_isNot)
                            {
                                shapeParam.vertex[_j].id = data.items[_i].id;
                            }
                        }
                        if (shapeParam.vertex[_j].id == data.items[_i].id)
                        {
                            shapeParam.vertex[_j].x = data.items[_i].iParam.left + data.items[_i].iParam.width / 2;
                            shapeParam.vertex[_j].y = data.items[_i].iParam.top + data.items[_i].iParam.height / 2;
                            data.items[_i].MouseMoveNotify += this.FigureItem_MouseMoveNotify;
                            _count++;

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

                            break;
                        }
                    }
                    if (_i == data.items.Count - 1 && data.items[_i].connectorId != id)
                    {
                        shapeParam.vertex.RemoveAt(_j);
                        _j--;
                    }
                }
                _j++;
            }
            if (_count < shapeParam.vertex.Count)
            {
                while (item.Count > _count)
                {
                    HandleRemoveItem(_count);
                }
                while (shapeParam.vertex.Count > _count)
                {
                    shapeParam.vertex.RemoveAt(_count);
                }
            }
            AppCoord();
        }

        public void FigureItem_MouseMoveNotify(int id)
        {
            if (shapeParam == null || data.items == null)
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
            AppCoord();
        }

        public void ReVertex()
        {
            if (shapeParam == null || data.items == null)
            {
                return;
            }
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
            AppCoord();
        }

        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
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