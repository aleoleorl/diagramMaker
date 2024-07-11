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
                if (((CommonParameter)param[EParameter.Content]).ParentId == -1)
                {
                    AppCanvas.Children.Add(Item[id]);

                }
                else
                {
                    int _id = Data.GetItemByID(((CommonParameter)param[EParameter.Content]).ParentId);
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
                if (((CommonParameter)param[EParameter.Content]).ParentId == -1)
                {
                    AppCanvas.Children.Remove(Item[id]);

                }
                else
                {
                    int _id = Data.GetItemByID(((CommonParameter)param[EParameter.Content]).ParentId);
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
            if (((EventParameter)param[EParameter.Event]) != null)
            {
                if (((EventParameter)param[EParameter.Event]).IsMouseDown)
                {
                    for (int _i = 0; _i < Item.Count; _i++)
                    {
                        Item[_i].MouseDown += Item_MouseDown;
                    }
                }
                if (((EventParameter)param[EParameter.Event]).IsMouseMove)
                {
                    for (int _i = 0; _i < Item.Count; _i++)
                    {
                        Item[_i].MouseMove += Item_MouseMove;
                    }
                }
                if (((EventParameter)param[EParameter.Event]).IsMouseUp)
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
            if (param[EParameter.Shape] != null && 
                ((ShapeParameter)param[EParameter.Shape]).Vertex.Count > 0)
            {
                ((CommonParameter)param[EParameter.Common]).AppX = Data.topLeftX + ((ShapeParameter)param[EParameter.Shape]).Vertex[0].x;
                ((CommonParameter)param[EParameter.Common]).AppY = Data.topLeftY + ((ShapeParameter)param[EParameter.Shape]).Vertex[0].y;
            }
        }

        public void HandlerShapeParam()
        {
            if (param[EParameter.Shape] == null || Data.items == null) 
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
                if (((CommonParameter)Data.items[_i].param[EParameter.Common]).ConnectorId == ((CommonParameter)param[EParameter.Common]).Id)
                {
                    Data.items[_i].MouseMoveNotify -= this.FigureItem_MouseMoveNotify;
                }
            }
            //
            int _j = 0;
            while (_j < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count)
            {
                for (int _i = 0; _i < Data.items.Count; _i++)
                {
                    if (((CommonParameter)Data.items[_i].param[EParameter.Common]).ConnectorId == ((CommonParameter)param[EParameter.Common]).Id)
                    {
                        if (((ShapeParameter)param[EParameter.Shape]).Vertex[_j].id == -1)
                        {
                            bool _isNot = true;
                            for (int _k = 0; _k < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count; _k++)
                            {
                                if (((ShapeParameter)param[EParameter.Shape]).Vertex[_k].id == ((CommonParameter)Data.items[_i].param[EParameter.Common]).Id)
                                {
                                    _isNot = false;
                                    break;
                                }
                            }
                            if (_isNot)
                            {
                                ((ShapeParameter)param[EParameter.Shape]).Vertex[_j].id = ((CommonParameter)Data.items[_i].param[EParameter.Common]).Id;
                            }
                        }
                        if (((ShapeParameter)param[EParameter.Shape]).Vertex[_j].id == ((CommonParameter)Data.items[_i].param[EParameter.Common]).Id)
                        {
                            ((ShapeParameter)param[EParameter.Shape]).Vertex[_j].x = ((ItemParameter)Data.items[_i].param[EParameter.Item]).Left + ((ItemParameter)Data.items[_i].param[EParameter.Item]).Width / 2;
                            ((ShapeParameter)param[EParameter.Shape]).Vertex[_j].y = ((ItemParameter)Data.items[_i].param[EParameter.Item]).Top + ((ItemParameter)Data.items[_i].param[EParameter.Item]).Height / 2;
                            Data.items[_i].MouseMoveNotify += this.FigureItem_MouseMoveNotify;
                            _count++;

                            if (_count > 1)
                            {
                                Line _tmp = new Line();
                                _tmp.Stroke = new SolidColorBrush(Colors.Black);
                                _tmp.StrokeThickness = ((ShapeParameter)param[EParameter.Shape]).StrokeThickness;

                                _tmp.X1 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_count - 2].x;
                                _tmp.Y1 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_count - 2].y;
                                _tmp.X2 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_count - 1].x;
                                _tmp.Y2 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_count - 1].y;
                                Item.Add(_tmp);
                                HandleAddItem(Item.Count - 1);
                            }

                            break;
                        }
                    }
                    if (_i == Data.items.Count - 1 && ((CommonParameter)Data.items[_i].param[EParameter.Common]).ConnectorId != ((CommonParameter)param[EParameter.Common]).Id)
                    {
                        ((ShapeParameter)param[EParameter.Shape]).Vertex.RemoveAt(_j);
                        _j--;
                    }
                }
                _j++;
            }
            if (_count < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count)
            {
                while (Item.Count > _count)
                {
                    HandleRemoveItem(_count);
                }
                while (((ShapeParameter)param[EParameter.Shape]).Vertex.Count > _count)
                {
                    ((ShapeParameter)param[EParameter.Shape]).Vertex.RemoveAt(_count);
                }
            }
            AppCoord();
        }

        public void FigureItem_MouseMoveNotify(int id)
        {
            if (param[EParameter.Shape] == null || Data.items == null)
            {
                return;
            }
            for (int _i = 0; _i < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count; _i++)
            {
                if (((ShapeParameter)param[EParameter.Shape]) != null && ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].id == id)
                {
                    if (_i > 0)
                    {
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x =
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Left +
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Width/2;
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y =
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Top +
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Height/2;

                        Item[_i - 1].X2 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x;
                        Item[_i - 1].Y2 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y;
                    }
                    if (_i < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count - 1)
                    {
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x = 
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Left +
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Width / 2;
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y = 
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Top +
                            ((ItemParameter)Data.items[Data.GetItemByID(id)].param[EParameter.Item]).Height / 2;

                        Item[_i].X1 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x;
                        Item[_i].Y1 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y;

                    }
                }
            }
            AppCoord();
        }

        public void ReVertex()
        {
            if (param[EParameter.Shape] == null || Data.items == null)
            {
                return;
            }
            int _id = -1;
            for (int _i = 0; _i < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count; _i++)
            {
                if (((ShapeParameter)param[EParameter.Shape]) != null)
                {
                    _id = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].id;
                    if (_i > 0)
                    {
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x =
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Left +
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Width / 2;
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y =
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Top +
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Height / 2;

                        Item[_i - 1].X2 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x;
                        Item[_i - 1].Y2 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y;
                    }
                    if (_i < ((ShapeParameter)param[EParameter.Shape]).Vertex.Count - 1)
                    {
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x =
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Left +
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Width / 2;
                        ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y =
                            ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Top +
                           ((ItemParameter)Data.items[Data.GetItemByID(_id)].param[EParameter.Item]).Height / 2;

                        Item[_i].X1 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].x;
                        Item[_i].Y1 = ((ShapeParameter)param[EParameter.Shape]).Vertex[_i].y;

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
            Data.tapped = ((CommonParameter)param[EParameter.Common]).Id;
            Trace.WriteLine("data.tapped:" + Data.tapped);

            e.Handled = true;
        }

        private void Item_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }
    }
}