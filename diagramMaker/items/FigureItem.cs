using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
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

        public void HandleAddItem(int id)
        {
            if (AppCanvas != null)
            {
                if (((CommonParameter)param[EParameter.Common]).ParentId == -1)
                {
                    AppCanvas.Children.Add(Item[id]);

                }
                else
                {
                    int _id = Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        ((CanvasItem)Data.items[_id]).Item.Children.Add(Item[id]);

                    }
                }
            }
        }

        public override void HandleRemoveItem(int id = -1)
        {
            if (AppCanvas != null)
            {
                if (((CommonParameter)param[EParameter.Common]).ParentId == -1)
                {
                    if (id != -1)
                    {
                        AppCanvas.Children.Remove(Item[id]);
                    } else
                    {
                        while (Item.Count > 0)
                        {
                            AppCanvas.Children.Remove(Item[0]);
                            Item.RemoveAt(0);
                        }
                    }

                }
                else
                {
                    int _id = Data.GetItemIndexByID(((CommonParameter)param[EParameter.Common]).ParentId);
                    if (_id != -1 && Data.items != null)
                    {
                        if (id != -1)
                        {
                            ((CanvasItem)Data.items[_id]).Item.Children.Remove(Item[id]);
                        } else
                        {
                            while (Item.Count > 0)
                            {
                                ((CanvasItem)Data.items[_id]).Item.Children.Remove(Item[0]);
                                Item.RemoveAt(0);
                            }
                        }

                    }
                }

                if (id != -1)
                {
                    Item.RemoveAt(id);
                }
            }
        }

        public override void HandlerEParam()
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

        protected override void HandlerIParam()
        {
        }

        public override void HandlerShapeParam()
        {
            if (param[EParameter.Shape] == null)
            {
                return;
            }
            PrepareConnections();
        }

        public override void PrepareConnections()
        {
            ShapeParameter _shape = (ShapeParameter)param[EParameter.Shape];
            CommonParameter _common = (CommonParameter)param[EParameter.Common];

            //Check the equalance of vertex+connectors with items
            /* 
             * The order of connectors could by random. 
             * The order of vertex should be strict
             */
            List<int> _currConnectors = new List<int>();
            for (int _i = 0; _i < Data.items.Count; ++_i)
            {
                CommonParameter _extComm = ((CommonParameter)Data.items[_i].param[EParameter.Common]);
                //the item is not a connector
                if (!_extComm.Connect.IsConnector)
                {
                    continue;
                }
                //the connector just knows its group
                if (_extComm.Connect.GroupID == _common.Connect.GroupID &&
                    !_extComm.Connect.Users.Contains(_common.Id)
                    )
                {
                    _extComm.Connect.Users.Add(_common.Id);
                }
                //the connector knows this item Id
                if (_extComm.Connect.Users.Contains(_common.Id))
                {
                    //but this item doesn't know about the connector
                    if (!_common.Connect.Connectors.Contains(_extComm.Id))
                    {
                        //fully new connector add to the end
                        if (_extComm.Connect.support.ContainsKey(EConnectorSupport.NewConnector))
                        {
                            _extComm.Connect.support.Remove(EConnectorSupport.NewConnector);
                            _shape.Vertex.Add(
                                new FigureContainer(
                                    x: ((ItemParameter)Data.items[_i].param[EParameter.Item]).Left,
                                    y: ((ItemParameter)Data.items[_i].param[EParameter.Item]).Top,
                                    id: _extComm.Id
                                    )
                            );
                            Data.items[_i].MouseMoveNotify += this.ConnectorItem_MouseMoveNotify;
                            _common.Connect.Connectors.Add(_extComm.Id);
                        }
                        //connector that cloned from the one already existed added to left or to rigth from it
                        if (_extComm.Connect.support.ContainsKey(EConnectorSupport.FromAncestorToRight) ||
                            _extComm.Connect.support.ContainsKey(EConnectorSupport.FromAncestorToLeft))
                        {
                            Data.items[_i].MouseMoveNotify += this.ConnectorItem_MouseMoveNotify;
                            int _ancId = _extComm.Connect.support[EConnectorSupport.Ancestor];
                            int _ancVertexIndex = -1;
                            
                            for (int _j=0; _j < _shape.Vertex.Count; _j++)
                            {
                                if (_shape.Vertex[_j].id == _ancId)
                                {
                                    _ancVertexIndex = _j;
                                    break;
                                }
                            }
                            if (_ancVertexIndex != -1)
                            {
                                if (_extComm.Connect.support.ContainsKey(EConnectorSupport.FromAncestorToRight))
                                {
                                    _extComm.Connect.support.Remove(EConnectorSupport.FromAncestorToRight);
                                    _shape.Vertex.Insert(
                                        _ancVertexIndex + 1,
                                        new FigureContainer(
                                            x: ((ItemParameter)Data.items[_i].param[EParameter.Item]).Left,
                                            y: ((ItemParameter)Data.items[_i].param[EParameter.Item]).Top,
                                            id: _extComm.Id
                                            )
                                    );
                                }
                                if (_extComm.Connect.support.ContainsKey(EConnectorSupport.FromAncestorToLeft))
                                {
                                    _extComm.Connect.support.Remove(EConnectorSupport.FromAncestorToLeft);
                                    _shape.Vertex.Insert(
                                        _ancVertexIndex,
                                        new FigureContainer(
                                            x: ((ItemParameter)Data.items[_i].param[EParameter.Item]).Left,
                                            y: ((ItemParameter)Data.items[_i].param[EParameter.Item]).Top,
                                            id: _extComm.Id
                                            )
                                    );
                                }
                                _extComm.Connect.support.Remove(EConnectorSupport.Ancestor);
                            }
                            else
                            {
                                new Exception("Wrong index in the shape item");
                            }
                            _common.Connect.Connectors.Add(_extComm.Id);
                        }
                    }
                    
                    _currConnectors.Add(_extComm.Id);
                }
            }
            //check deleted connectors and vertex
            int _con = 0;
            while (_con < _common.Connect.Connectors.Count)
            {
                if (!_currConnectors.Contains(_common.Connect.Connectors[_con]))
                {
                    int _vertex = -1;
                    for (int _j=0; _j< _shape.Vertex.Count; _j++)
                    {
                        if (_shape.Vertex[_j].id == _common.Connect.Connectors[_con])
                        {
                            _vertex = _j;
                            break;
                        }
                    }
                    if (_vertex != -1)
                    {
                        _shape.Vertex.RemoveAt(_vertex);
                    }
                    _common.Connect.Connectors.RemoveAt(_con);
                    continue;
                }
                _con++;
            }
            //check the count of Lines
            if (_shape.Shape == EShape.Line)
            {
                while (Item.Count < _shape.Vertex.Count-1)
                {
                    Line _item = new Line();
                    _item.Stroke = new SolidColorBrush(Colors.Black);
                    _item.StrokeThickness = _shape.StrokeThickness;
                    Item.Add(_item);
                    HandleAddItem(Item.Count - 1);
                }
                while (_shape.Vertex.Count>0 && Item.Count >= _shape.Vertex.Count)
                {
                    HandleRemoveItem(Item.Count - 1);
                }
            }
            // correct line view
            ReFigure();
        }

        public void ReFigure()
        {
            ShapeParameter _shape = (ShapeParameter)param[EParameter.Shape];
            int _connId;
            ItemParameter _connItem;
            for (int _i=0; _i< _shape.Vertex.Count; _i++)
            {
                _connId = _shape.Vertex[_i].id;
                _connItem = ((ItemParameter)Data.items[Data.GetItemIndexByID(_connId)].param[EParameter.Item]);
                if (_i > 0)
                {
                    _shape.Vertex[_i].x = _connItem.Left + _connItem.Width / 2;
                    _shape.Vertex[_i].y = _connItem.Top + _connItem.Height / 2;

                    Item[_i - 1].X2 = _shape.Vertex[_i].x;
                    Item[_i - 1].Y2 = _shape.Vertex[_i].y;
                }
                if (_i < _shape.Vertex.Count - 1)
                {
                    _shape.Vertex[_i].x = _connItem.Left + _connItem.Width / 2;
                    _shape.Vertex[_i].y = _connItem.Top + _connItem.Height / 2;

                    Item[_i].X1 = _shape.Vertex[_i].x;
                    Item[_i].Y1 = _shape.Vertex[_i].y;
                }
            }
            AppCoordCorrection();
        }

        public void AppCoordCorrection()
        {
            if (param[EParameter.Shape] != null &&
                ((ShapeParameter)param[EParameter.Shape]).Vertex.Count > 0)
            {
                ((CommonParameter)param[EParameter.Common]).AppX = Data.topLeftX + ((ShapeParameter)param[EParameter.Shape]).Vertex[0].x;
                ((CommonParameter)param[EParameter.Common]).AppY = Data.topLeftY + ((ShapeParameter)param[EParameter.Shape]).Vertex[0].y;
            }
        }

        public void ConnectorItem_MouseMoveNotify(int id)
        {
            if (!param.ContainsKey(EParameter.Shape))
            {
                return;
            }
            ReFigure();
            AppCoordCorrection();
        }

        public override void ValueChanger(
            EBindParameter eBindParameter = EBindParameter.None,
            string txt = "")
        {
            switch (eBindParameter)
            {
                case EBindParameter.Name:
                    ((CommonParameter)param[EParameter.Common]).Name = txt;
                    break;
                default:
                    break;
            }

            base.ValueChanger(eBindParameter, txt);
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