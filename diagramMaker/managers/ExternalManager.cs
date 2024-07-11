using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;

namespace diagramMaker.managers
{
    public class ExternalManager
    {
        public DataHub data;

        public ExternalManager(DataHub data) 
        {
            this.data = data;
        }

        public void Save()
        {
            if (File.Exists(data.saveName))
            {
                File.SetAttributes(data.saveName, FileAttributes.Normal);
            }
            //XmlSerializer _xs;
            //string _xml;
            //foreach (var _item in data.items)
            //{
            //    if (_item.itemAttach == helpers.EItemAttach.Menu)
            //    {
            //        continue;
            //    }
            //    _xs = new XmlSerializer(_item.GetType());
            //    using (StringWriter _writer = new StringWriter())
            //    {
            //        _xs.Serialize(_writer, _item);
            //        _xml = _writer.ToString();
            //    }
            //}

            using (FileStream fs = new FileStream(data.saveName, FileMode.Create))
            using (XmlWriter writer = XmlWriter.Create(fs))
            {
                writer.WriteStartElement("diaMaker");

                writer.WriteStartElement("commonInfo");
                writer.WriteAttributeString("version", "0.5");
                writer.WriteAttributeString("author", "AleOleOrl");
                writer.WriteEndElement();

                writer.WriteStartElement("workspaceInfo");
                writer.WriteAttributeString("id", DefaultItem.GetCurrentFreeId().ToString());
                writer.WriteEndElement();

                if (data.items != null)
                {
                    foreach (var _item in data.items)
                    {
                        if (((CommonParameter)_item.param[EParameter.Common]).ItemAttach != helpers.EItemAttach.Custom)
                        {
                            continue;
                        }
                        writer.WriteStartElement("item");
                        writer.WriteAttributeString("id", ((CommonParameter)_item.param[EParameter.Common]).Id.ToString());
                        writer.WriteAttributeString("parentId", ((CommonParameter)_item.param[EParameter.Common]).ParentId.ToString());
                        writer.WriteAttributeString("connectorId", ((CommonParameter)_item.param[EParameter.Common]).ConnectorId.ToString());
                        writer.WriteAttributeString("name", ((CommonParameter)_item.param[EParameter.Common]).Name);
                        writer.WriteAttributeString("itemType", ((CommonParameter)_item.param[EParameter.Common]).ItemType.ToString());
                        writer.WriteAttributeString("appX", ((CommonParameter)_item.param[EParameter.Common]).AppX.ToString());
                        writer.WriteAttributeString("appY", ((CommonParameter)_item.param[EParameter.Common]).AppY.ToString());

                        writer.WriteStartElement("itemSpecial");
                        switch (((CommonParameter)_item.param[EParameter.Common]).ItemType)
                        {
                            case helpers.EItem.Painter:
                                writer.WriteAttributeString("isExist", "1");
                                PngBitmapEncoder encoder = new PngBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(((PainterItem)_item).Painter));
                                using (MemoryStream _ms = new MemoryStream())
                                {
                                    encoder.Save(_ms);
                                    byte[] _imageBytes = _ms.ToArray();
                                    string _base64Image = Convert.ToBase64String(_imageBytes);
                                    writer.WriteStartElement("imageData");
                                    writer.WriteValue(_base64Image);
                                    writer.WriteEndElement();
                                }
                                break;
                            default:
                                writer.WriteAttributeString("isExist", "0");
                                break;
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("iParam");
                        writer.WriteAttributeString("isExist", _item.param[EParameter.Item] == null ? "0" : "1");
                        if (_item.param[EParameter.Item] != null)
                        {
                            writer.WriteElementString("left", ((ItemParameter)_item.param[EParameter.Item]).Left.ToString());
                            writer.WriteElementString("top", ((ItemParameter)_item.param[EParameter.Item]).Top.ToString());
                            writer.WriteElementString("width", ((ItemParameter)_item.param[EParameter.Item]).Width.ToString());
                            writer.WriteElementString("height", ((ItemParameter)_item.param[EParameter.Item]).Height.ToString());
                            writer.WriteElementString("bgColor", ((ItemParameter)_item.param[EParameter.Item]).BgColor == null ? "null" : ((ItemParameter)_item.param[EParameter.Item]).BgColor.ToString());
                            writer.WriteElementString("frColor", ((ItemParameter)_item.param[EParameter.Item]).FrColor == null ? "null" : ((ItemParameter)_item.param[EParameter.Item]).FrColor.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("content");
                        writer.WriteAttributeString("isExist", _item.param[EParameter.Content] == null ? "0" : "1");
                        if (_item.param[EParameter.Content] != null)
                        {
                            writer.WriteElementString("content", ((ContentParameter)_item.param[EParameter.Content]).Content == null ? "null" : ((ContentParameter)_item.param[EParameter.Content]).Content);
                            writer.WriteElementString("horAlign", ((ContentParameter)_item.param[EParameter.Content]).HorAlign == null ? "null" : ((ContentParameter)_item.param[EParameter.Content]).HorAlign.ToString());
                            writer.WriteElementString("verAlign", ((ContentParameter)_item.param[EParameter.Content]).VerAlign == null ? "null" : ((ContentParameter)_item.param[EParameter.Content]).VerAlign.ToString());
                            writer.WriteElementString("isTextChanged", ((ContentParameter)_item.param[EParameter.Content]).IsTextChanged.ToString());
                            writer.WriteElementString("bindParameter", ((ContentParameter)_item.param[EParameter.Content]).BindParameter.ToString());
                            writer.WriteElementString("bindID", ((ContentParameter)_item.param[EParameter.Content]).BindID.ToString());
                            writer.WriteElementString("isDigitsOnly", ((ContentParameter)_item.param[EParameter.Content]).IsDigitsOnly.ToString());
                            writer.WriteElementString("count", ((ContentParameter)_item.param[EParameter.Content]).Count.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("bParam");
                        writer.WriteAttributeString("isExist", _item.param[EParameter.Border] == null ? "0" : "1");
                        if (_item.param[EParameter.Border] != null)
                        {
                            writer.WriteElementString("isBorder", ((BorderParameter)_item.param[EParameter.Border]).IsBorder.ToString());
                            writer.WriteElementString("borderThickness", ((BorderParameter)_item.param[EParameter.Border]).BorderThickness.ToString());
                            writer.WriteElementString("color", ((BorderParameter)_item.param[EParameter.Border]) == null ? "null" : ((BorderParameter)_item.param[EParameter.Border]).Color.ToString());
                            writer.WriteElementString("cornerRadius", ((BorderParameter)_item.param[EParameter.Border]).CornerRadius.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("eParam");
                        writer.WriteAttributeString("isExist", _item.param[EParameter.Event] == null ? "0" : "1");
                        if (_item.param[EParameter.Event] != null)
                        {
                            writer.WriteElementString("isMoveSensitive", ((EventParameter)_item.param[EParameter.Event]).IsMoveSensitive.ToString());
                            writer.WriteElementString("isMouseDown", ((EventParameter)_item.param[EParameter.Event]).IsMouseDown.ToString());
                            writer.WriteElementString("isMouseMove", ((EventParameter)_item.param[EParameter.Event]).IsMouseMove.ToString());
                            writer.WriteElementString("isMouseUp", ((EventParameter)_item.param[EParameter.Event]).IsMouseUp.ToString());
                            writer.WriteElementString("mouseUpInfo", ((EventParameter)_item.param[EParameter.Event]).MouseUpInfo);
                            writer.WriteElementString("isHitTestVisible", ((EventParameter)_item.param[EParameter.Event]).IsHitTestVisible.ToString());
                            writer.WriteElementString("isMouseClick", ((EventParameter)_item.param[EParameter.Event]).IsMouseClick.ToString());
                            writer.WriteElementString("isMouseLeave", ((EventParameter)_item.param[EParameter.Event]).IsMouseLeave.ToString());
                            writer.WriteElementString("isMouseWheel", ((EventParameter)_item.param[EParameter.Event]).IsMouseWheel.ToString());
                            writer.WriteElementString("isMouseDoubleClick", ((EventParameter)_item.param[EParameter.Event]).IsMouseDoubleClick.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("imgParam");
                        writer.WriteAttributeString("isExist", _item.param[EParameter.Image] == null ? "0" : "1");
                        if (_item.param[EParameter.Image] != null)
                        {
                            writer.WriteElementString("imagePath", ((ImageParameter)_item.param[EParameter.Image]).ImagePath == null ? "null" : ((ImageParameter)_item.param[EParameter.Image]).ImagePath);
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("shapeParam");
                        writer.WriteAttributeString("isExist", _item.param[EParameter.Shape] == null ? "0" : "1");
                        writer.WriteAttributeString("vertex", _item.param[EParameter.Shape] == null ? "0" : ((ShapeParameter)_item.param[EParameter.Shape]).Vertex.Count.ToString());
                        if (_item.param[EParameter.Shape] != null)
                        {
                            writer.WriteElementString("shape", ((ShapeParameter)_item.param[EParameter.Shape]).Shape.ToString());
                            writer.WriteElementString("color", ((ShapeParameter)_item.param[EParameter.Shape]).Color == null ? "null" : ((ShapeParameter)_item.param[EParameter.Shape]).Color.ToString());
                            writer.WriteElementString("strokeThickness", ((ShapeParameter)_item.param[EParameter.Shape]).StrokeThickness.ToString());

                            foreach (FigureContainer _fc in ((ShapeParameter)_item.param[EParameter.Shape]).Vertex)
                            {
                                writer.WriteStartElement("vertex");
                                writer.WriteAttributeString("x", _fc.x.ToString());
                                writer.WriteAttributeString("y", _fc.y.ToString());
                                writer.WriteAttributeString("id", _fc.id.ToString());
                                writer.WriteEndElement();
                            }
                        }
                        writer.WriteEndElement();
                        //
                        writer.WriteEndElement();
                    }
                }
                writer.WriteEndElement();
                writer.Flush();
            }
        }

        public void Load(EventManager ev)
        {
            if (!File.Exists(data.loadName))
            {
                return;
            }
            using (XmlReader reader = XmlReader.Create(data.loadName))
            {
                while (reader.Read())
                {
                    Trace.WriteLine(">" + reader);
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "commonInfo")
                    {
                        string _version = reader.GetAttribute("version");
                        if (_version == "0.5")
                        {
                            Load_V0_5(reader, ev);
                            break;
                        }
                    }
                }
            }
        }

        public void Load_V0_5(XmlReader reader, EventManager ev)
        {
            DefaultItem _itm;
            int _ID = 0;
            string _str;
            byte[] _imageData = {0};
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == "item" && data.items!= null && ((CommonParameter)data.items[^1].param[EParameter.Common]).ItemType == EItem.Painter)
                    {
                        using (var ms = new MemoryStream(_imageData))
                        {
                            var bitmap = BitmapFrame.Create(
                                ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                            ((PainterItem)data.items[^1]).Painter = new WriteableBitmap(bitmap);
                        }
                        ((PainterItem)data.items[^1]).Image.Source = ((PainterItem)data.items[^1]).Painter;
                        ((PainterItem)data.items[^1]).Image.Width = ((PainterItem)data.items[^1]).Painter.PixelWidth;
                        ((PainterItem)data.items[^1]).Image.Height = ((PainterItem)data.items[^1]).Painter.PixelHeight;
                    }
                    continue;
                }
                switch (reader.Name)
                {
                    case "workspaceInfo":
                        _str = reader.GetAttribute("id");
                        Trace.WriteLine("id:" + _str);
                        _ID = int.Parse(reader.GetAttribute("id"));
                        break;
                    case "item":
                        _str = reader.GetAttribute("parentId");
                        Trace.WriteLine("id:" + _str);
                        int _parentId = int.Parse(reader.GetAttribute("parentId"));

                        switch (reader.GetAttribute("itemType"))
                        {
                            case "Painter":
                                _itm = new PainterItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            case "Canvas":
                                _itm = new CanvasItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            case "Label":
                                _itm = new LabelItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            case "Figure":
                                _itm = new FigureItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).Item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "itemSpecial":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 1;
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType != XmlNodeType.Element)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                if (reader.Name == "imageData")
                                {
                                    _imageData = Convert.FromBase64String(_str);
                                }
                            }
                        }
                        break;
                    case "iParam":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 6;
                            ItemParameter _iParam = new ItemParameter(0, 0, 0, 0);
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType != XmlNodeType.Element)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                switch (reader.Name)
                                {
                                    case "left":
                                        _iParam.Left = ((CommonParameter)data.items[^1].param[EParameter.Common]).AppX - data.topLeftX;
                                        break;
                                    case "top":
                                        _iParam.Top = ((CommonParameter)data.items[^1].param[EParameter.Common]).AppY - data.topLeftY;
                                        break;
                                    case "width":
                                        _iParam.Width = int.Parse(_str);
                                        break;
                                    case "height":
                                        _iParam.Height = int.Parse(_str);
                                        break;
                                    case "bgColor":
                                        _iParam.BgColor = _str == "null" ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(_str));
                                        break;
                                    case "frColor":
                                        _iParam.FrColor = _str == "null" ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(_str));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            data.items[^1].SetParameter(EParameter.Item, _iParam);
                        }
                        break;
                    case "content":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 8;
                            ContentParameter _content = new ContentParameter();
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                switch (reader.Name)
                                {
                                    case "content":
                                        _content.Content = _str;
                                        break;
                                    case "horAlign":
                                        _content.HorAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), _str, true);
                                        break;
                                    case "verAlign":
                                        _content.VerAlign = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), _str, true);
                                        break;
                                    case "isTextChanged":
                                        _content.IsTextChanged = bool.Parse(_str);
                                        break;
                                    case "bindParameter":
                                        object _bindParameter;
                                        _ = Enum.TryParse(typeof(EBindParameter), _str, out _bindParameter);
                                        _content.BindParameter = _bindParameter!=null?(EBindParameter)_bindParameter: EBindParameter.None;
                                        break;
                                    case "bindID":
                                        _content.BindID = int.Parse(_str);
                                        break;
                                    case "isDigitsOnly":
                                        _content.IsDigitsOnly = bool.Parse(_str);
                                        break;
                                    case "count":
                                        _content.Count = int.Parse(_str);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            data.items[^1].SetParameter(EParameter.Content, _content);
                        }
                        break;
                    case "bParam":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 4;
                            BorderParameter _bParam = new BorderParameter();
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                switch (reader.Name)
                                {
                                    case "isBorder":
                                        _bParam.IsBorder = bool.Parse(_str);
                                        break;
                                    case "borderThickness":
                                        _bParam.BorderThickness = double.Parse(_str);
                                        break;
                                    case "color":
                                        _bParam.Color = (Color)ColorConverter.ConvertFromString(_str);
                                        break;
                                    case "cornerRadius":
                                        _bParam.CornerRadius = double.Parse(_str);
                                        break;                                   
                                    default:
                                        break;
                                }
                            }
                            data.items[^1].SetParameter(EParameter.Border, _bParam);
                        }
                        break;
                    case "eParam":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 10;
                            EventParameter _eParam = new EventParameter();
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                switch (reader.Name)
                                {
                                    case "isMoveSensitive":
                                        _eParam.IsMoveSensitive = bool.Parse(_str);
                                        break;
                                    case "isMouseDown":
                                        _eParam.IsMouseDown = bool.Parse(_str);
                                        break;
                                    case "isMouseMove":
                                        _eParam.IsMouseMove = bool.Parse(_str);
                                        break;
                                    case "isMouseUp":
                                        _eParam.IsMouseUp = bool.Parse(_str);
                                        break;
                                    case "mouseUpInfo":
                                        //_eParam.mouseUpInfo = reader.GetAttribute("isBorder") != null? reader.GetAttribute("isBorder"):null;
                                        break;
                                    case "isHitTestVisible":
                                        _eParam.IsHitTestVisible = bool.Parse(_str);
                                        break;
                                    case "isMouseClick":
                                        _eParam.IsMouseClick = bool.Parse(_str);
                                        break;
                                    case "isMouseLeave":
                                        _eParam.IsMouseLeave = bool.Parse(_str);
                                        break;
                                    case "isMouseWheel":
                                        _eParam.IsMouseWheel = bool.Parse(_str);
                                        break;
                                    case "isMouseDoubleClick":
                                        _eParam.IsMouseDoubleClick = bool.Parse(_str);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            data.items[^1].SetParameter(EParameter.Event, _eParam);
                        }
                        break;
                    case "imgParam":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 1;
                            ImageParameter _imgParam = new ImageParameter();
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                switch (reader.Name)
                                {
                                    case "imagePath":
                                        _imgParam.ImagePath = _str;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            data.items[^1].SetParameter(EParameter.Image, _imgParam);
                        }
                        break;
                    case "shapeParam":
                        if (reader.GetAttribute("isExist") == "1")
                        {
                            int _countAttr = 3 + int.Parse(reader.GetAttribute("vertex"));
                            ShapeParameter _shapeParam = new ShapeParameter();
                            _shapeParam.Vertex = new System.Collections.Generic.List<FigureContainer>();
                            while (_countAttr > 0)
                            {
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    continue;
                                }

                                _str = reader.ReadString();
                                _countAttr--;
                                switch (reader.Name)
                                {
                                    case "shape":
                                        object _obj;
                                        _ = Enum.TryParse(typeof(EShape), _str, out _obj);
                                        _shapeParam.Shape = _obj!= null? (EShape)_obj: EShape.Line;
                                        break;
                                    case "color":
                                        _shapeParam.Color = (Color)ColorConverter.ConvertFromString(_str);
                                        break;
                                    case "strokeThickness":
                                        _shapeParam.StrokeThickness = Double.Parse(_str);
                                        break;
                                    case "vertex":
                                        _shapeParam.Vertex.Add(new FigureContainer(
                                            x: Double.Parse(reader.GetAttribute("x")),
                                            y: Double.Parse(reader.GetAttribute("y")),
                                            id: int.Parse(reader.GetAttribute("id"))
                                            ));
                                        break;
                                    default:
                                        break;
                                }
                            }
                            data.items[^1].SetParameter(EParameter.Shape, _shapeParam, -1);
                        }
                        break;
                    default:
                        break;
                }
            }

            if (_ID != 0)
            {
                DefaultItem.RestartID(_ID);
            }

            foreach(var _item in data.items)
            {
                if (((CommonParameter)_item.param[EParameter.Common]).ItemAttach == EItemAttach.Custom &&
                    ((CommonParameter)_item.param[EParameter.Common]).ItemType == EItem.Figure)
                {
                    ((FigureItem)_item).HandlerShapeParam();
                }
            }
        }

        public void V0_5_DefaultParams(DefaultItem itm, XmlReader reader, EventManager ev)
        {
            ((CommonParameter)itm.param[EParameter.Common]).Id = int.Parse(reader.GetAttribute("id"));
            ((CommonParameter)itm.param[EParameter.Common]).ConnectorId = int.Parse(reader.GetAttribute("connectorId"));
            ((CommonParameter)itm.param[EParameter.Common]).ParentId = int.Parse(reader.GetAttribute("parentId"));
            ((CommonParameter)itm.param[EParameter.Common]).Name = reader.GetAttribute("name");
            ((CommonParameter)itm.param[EParameter.Common]).AppX = int.Parse(reader.GetAttribute("appX"));
            ((CommonParameter)itm.param[EParameter.Common]).AppY = int.Parse(reader.GetAttribute("appY"));
            ((CommonParameter)itm.param[EParameter.Common]).ItemAttach = EItemAttach.Custom;
            itm.MouseAppIdNotify += ev.EventItemMenuHandler;
        }
    }
}