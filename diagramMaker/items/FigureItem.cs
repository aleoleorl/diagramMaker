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
        public List<Line> Item { get; set; }

        public FigureItem(DataHub data, Canvas? appCanvas, int parentId = -1, EShape shape = EShape.Line) :
            base(data, appCanvas, parentId, EItem.Figure)
        {
            Item = new List<Line>();
        }

        public void HandleAddItem(int id)
        {
            if (AppCanvas != null)
            {
                if (ParentId == -1)
                {
                    AppCanvas.Children.Add(Item[id]);

                }
                else
                {
                    int _id = Data.GetItemByID(ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        ((CanvasItem)Data.items[_id]).Item.Children.Add(Item[id]);

                    }
                }
            }
        }
        public void HandleRemoveItem(int id)
        {
            if (AppCanvas != null)
            {
                if (ParentId == -1)
                {
                    AppCanvas.Children.Remove(Item[id]);

                }
                else
                {
                    int _id = Data.GetItemByID(ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        ((CanvasItem)Data.items[_id]).Item.Children.Remove(Item[id]);

                    }
                }

                Item.RemoveAt(id);
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
            if (EParam != null)
            {
                if (EParam.IsMouseDown)
                {
                    for (int _i = 0; _i < Item.Count; _i++)
                    {
                        Item[_i].MouseDown += Item_MouseDown;
                    }
                }
                if (EParam.IsMouseMove)
                {
                    for (int _i = 0; _i < Item.Count; _i++)
                    {
                        Item[_i].MouseMove += Item_MouseMove;
                    }
                }
                if (EParam.IsMouseUp)
                {
                    for (int _i = 0; _i < Item.Count; _i++)
                    {
                        Item[_i].MouseUp += Item_MouseUp;
                    }
                }

            }
        }

        protected void HandlerIParam()
        {
        }

        public void AppCoord()
        {
            if (ShapeParam != null && ShapeParam.Vertex.Count > 0)
            {
                AppX = Data.topLeftX + ShapeParam.Vertex[0].x;
                AppY = Data.topLeftY + ShapeParam.Vertex[0].y;
            }
        }

        public void HandlerShapeParam()
        {
            if (ShapeParam == null || Data.items == null) 
            {
                return;
            }
            while (Item.Count > 0)
            {
                HandleRemoveItem(0);
            }
            //
            int _count = 0;
            //
            for (int _i = 0; _i < Data.items.Count; _i++)
            {
                if (Data.items[_i].ConnectorId == Id)
                {
                    Data.items[_i].MouseMoveNotify -= this.FigureItem_MouseMoveNotify;
                }
            }
            //
            int _j = 0;
            while (_j < ShapeParam.Vertex.Count)
            {
                for (int _i = 0; _i < Data.items.Count; _i++)
                {
                    if (Data.items[_i].ConnectorId == Id)
                    {
                        if (ShapeParam.Vertex[_j].id == -1)
                        {
                            bool _isNot = true;
                            for (int _k = 0; _k < ShapeParam.Vertex.Count; _k++)
                            {
                                if (ShapeParam.Vertex[_k].id == Data.items[_i].Id)
                                {
                                    _isNot = false;
                                    break;
                                }
                            }
                            if (_isNot)
                            {
                                ShapeParam.Vertex[_j].id = Data.items[_i].Id;
                            }
                        }
                        if (ShapeParam.Vertex[_j].id == Data.items[_i].Id)
                        {
                            ShapeParam.Vertex[_j].x = Data.items[_i].IParam.Left + Data.items[_i].IParam.Width / 2;
                            ShapeParam.Vertex[_j].y = Data.items[_i].IParam.Top + Data.items[_i].IParam.Height / 2;
                            Data.items[_i].MouseMoveNotify += this.FigureItem_MouseMoveNotify;
                            _count++;

                            if (_count > 1)
                            {
                                Line _tmp = new Line();
                                _tmp.Stroke = new SolidColorBrush(Colors.Black);
                                _tmp.StrokeThickness = ShapeParam.StrokeThickness;

                                _tmp.X1 = ShapeParam.Vertex[_count - 2].x;
                                _tmp.Y1 = ShapeParam.Vertex[_count - 2].y;
                                _tmp.X2 = ShapeParam.Vertex[_count - 1].x;
                                _tmp.Y2 = ShapeParam.Vertex[_count - 1].y;
                                Item.Add(_tmp);
                                HandleAddItem(Item.Count - 1);
                            }

                            break;
                        }
                    }
                    if (_i == Data.items.Count - 1 && Data.items[_i].ConnectorId != Id)
                    {
                        ShapeParam.Vertex.RemoveAt(_j);
                        _j--;
                    }
                }
                _j++;
            }
            if (_count < ShapeParam.Vertex.Count)
            {
                while (Item.Count > _count)
                {
                    HandleRemoveItem(_count);
                }
                while (ShapeParam.Vertex.Count > _count)
                {
                    ShapeParam.Vertex.RemoveAt(_count);
                }
            }
            AppCoord();
        }

        public void FigureItem_MouseMoveNotify(int id)
        {
            if (ShapeParam == null || Data.items == null)
            {
                return;
            }
            for (int _i = 0; _i < ShapeParam.Vertex.Count; _i++)
            {
                if (ShapeParam != null && ShapeParam.Vertex[_i].id == id)
                {
                    if (_i > 0)
                    {
                        ShapeParam.Vertex[_i].x = Data.items[Data.GetItemByID(id)].IParam.Left +
                            Data.items[Data.GetItemByID(id)].IParam.Width/2;
                        ShapeParam.Vertex[_i].y = Data.items[Data.GetItemByID(id)].IParam.Top +
                            Data.items[Data.GetItemByID(id)].IParam.Height/2;

                        Item[_i - 1].X2 = ShapeParam.Vertex[_i].x;
                        Item[_i - 1].Y2 = ShapeParam.Vertex[_i].y;
                    }
                    if (_i < ShapeParam.Vertex.Count - 1)
                    {
                        ShapeParam.Vertex[_i].x = Data.items[Data.GetItemByID(id)].IParam.Left +
                            Data.items[Data.GetItemByID(id)].IParam.Width / 2;
                        ShapeParam.Vertex[_i].y = Data.items[Data.GetItemByID(id)].IParam.Top +
                            Data.items[Data.GetItemByID(id)].IParam.Height / 2;

                        Item[_i].X1 = ShapeParam.Vertex[_i].x;
                        Item[_i].Y1 = ShapeParam.Vertex[_i].y;

                    }
                }
            }
            AppCoord();
        }

        public void ReVertex()
        {
            if (ShapeParam == null || Data.items == null)
            {
                return;
            }
            int _id = -1;
            for (int _i = 0; _i < ShapeParam.Vertex.Count; _i++)
            {
                if (ShapeParam != null)
                {
                    _id = ShapeParam.Vertex[_i].id;
                    if (_i > 0)
                    {
                        ShapeParam.Vertex[_i].x = Data.items[Data.GetItemByID(_id)].IParam.Left +
                            Data.items[Data.GetItemByID(_id)].IParam.Width / 2;
                        ShapeParam.Vertex[_i].y = Data.items[Data.GetItemByID(_id)].IParam.Top +
                            Data.items[Data.GetItemByID(_id)].IParam.Height / 2;

                        Item[_i - 1].X2 = ShapeParam.Vertex[_i].x;
                        Item[_i - 1].Y2 = ShapeParam.Vertex[_i].y;
                    }
                    if (_i < ShapeParam.Vertex.Count - 1)
                    {
                        ShapeParam.Vertex[_i].x = Data.items[Data.GetItemByID(_id)].IParam.Left +
                            Data.items[Data.GetItemByID(_id)].IParam.Width / 2;
                        ShapeParam.Vertex[_i].y = Data.items[Data.GetItemByID(_id)].IParam.Top +
                            Data.items[Data.GetItemByID(_id)].IParam.Height / 2;

                        Item[_i].X1 = ShapeParam.Vertex[_i].x;
                        Item[_i].Y1 = ShapeParam.Vertex[_i].y;

                    }
                }
            }
            AppCoord();
        }

        public override void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePosition = e.GetPosition(Item[0]);
            Data.tapXX = -mousePosition.X;
            Data.tapYY = -mousePosition.Y;
            Data.tapped = Id;
            Trace.WriteLine("data.tapped:" + Data.tapped);

            e.Handled = true;
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}