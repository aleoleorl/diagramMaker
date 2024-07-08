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

                foreach (var _item in data.items)
                {
                    if (_item.itemAttach != helpers.EItemAttach.Custom)
                    {
                        continue;
                    }
                    writer.WriteStartElement("item");
                    writer.WriteAttributeString("id", _item.id.ToString());
                    writer.WriteAttributeString("parentId", _item.parentId.ToString());
                    writer.WriteAttributeString("connectorId", _item.connectorId.ToString());
                    writer.WriteAttributeString("name", _item.name);
                    writer.WriteAttributeString("itemType", _item.itemType.ToString());
                    writer.WriteAttributeString("appX", _item.appX.ToString());
                    writer.WriteAttributeString("appY", _item.appY.ToString());

                    writer.WriteStartElement("itemSpecial");
                    switch (_item.itemType)
                    {
                        case helpers.EItem.Painter:
                            writer.WriteAttributeString("isExist", "1");
                            PngBitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(((PainterItem)_item).painter));
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
                    writer.WriteAttributeString("isExist", _item.iParam == null ? "0" : "1");
                    if (_item.iParam != null)
                    {
                        writer.WriteElementString("left", _item.iParam.left.ToString());
                        writer.WriteElementString("top", _item.iParam.top.ToString());
                        writer.WriteElementString("width", _item.iParam.width.ToString());
                        writer.WriteElementString("height", _item.iParam.height.ToString());
                        writer.WriteElementString("bgColor", _item.iParam.bgColor == null ? "null" : _item.iParam.bgColor.ToString());
                        writer.WriteElementString("frColor", _item.iParam.frColor == null ? "null" : _item.iParam.frColor.ToString());
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("content");
                    writer.WriteAttributeString("isExist", _item.content == null ? "0" : "1");
                    if (_item.content != null)
                    {
                        writer.WriteElementString("content", _item.content.content == null ? "null" : _item.content.content);
                        writer.WriteElementString("horAlign", _item.content.horAlign == null ? "null" : _item.content.horAlign.ToString());
                        writer.WriteElementString("verAlign", _item.content.verAlign == null ? "null" : _item.content.verAlign.ToString());
                        writer.WriteElementString("isTextChanged", _item.content.isTextChanged.ToString());
                        writer.WriteElementString("bindParameter", _item.content.bindParameter.ToString());
                        writer.WriteElementString("bindID", _item.content.bindID.ToString());
                        writer.WriteElementString("isDigitsOnly", _item.content.isDigitsOnly.ToString());
                        writer.WriteElementString("count", _item.content.count.ToString());
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("bParam");
                    writer.WriteAttributeString("isExist", _item.bParam == null ? "0" : "1");
                    if (_item.bParam != null)
                    {
                        writer.WriteElementString("isBorder", _item.bParam.isBorder.ToString());
                        writer.WriteElementString("borderThickness", _item.bParam.borderThickness.ToString());
                        writer.WriteElementString("color", _item.bParam.color == null ? "null" : _item.bParam.color.ToString());
                        writer.WriteElementString("cornerRadius", _item.bParam.cornerRadius.ToString());
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("eParam");
                    writer.WriteAttributeString("isExist", _item.eParam == null ? "0" : "1");
                    if (_item.eParam != null)
                    {
                        writer.WriteElementString("isMoveSensitive", _item.eParam.isMoveSensitive.ToString());
                        writer.WriteElementString("isMouseDown", _item.eParam.isMouseDown.ToString());
                        writer.WriteElementString("isMouseMove", _item.eParam.isMouseMove.ToString());
                        writer.WriteElementString("isMouseUp", _item.eParam.isMouseUp.ToString());
                        writer.WriteElementString("mouseUpInfo", _item.eParam.mouseUpInfo);
                        writer.WriteElementString("isHitTestVisible", _item.eParam.isHitTestVisible.ToString());
                        writer.WriteElementString("isMouseClick", _item.eParam.isMouseClick.ToString());
                        writer.WriteElementString("isMouseLeave", _item.eParam.isMouseLeave.ToString());
                        writer.WriteElementString("isMouseWheel", _item.eParam.isMouseWheel.ToString());
                        writer.WriteElementString("isMouseDoubleClick", _item.eParam.isMouseDoubleClick.ToString());
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("imgParam");
                    writer.WriteAttributeString("isExist", _item.imgParam == null ? "0" : "1");
                    if (_item.imgParam != null)
                    {
                        writer.WriteElementString("imagePath", _item.imgParam.imagePath == null ? "null" : _item.imgParam.imagePath);
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("shapeParam");
                    writer.WriteAttributeString("isExist", _item.shapeParam == null ? "0" : "1");
                    writer.WriteAttributeString("vertex", _item.shapeParam == null ? "0": _item.shapeParam.vertex.Count.ToString());
                    if (_item.shapeParam != null)
                    {
                        writer.WriteElementString("shape", _item.shapeParam.shape.ToString());
                        writer.WriteElementString("color", _item.shapeParam.color == null ? "null" : _item.shapeParam.color.ToString());
                        writer.WriteElementString("strokeThickness", _item.shapeParam.strokeThickness.ToString());
                        
                        foreach (FigureContainer _fc in _item.shapeParam.vertex)
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
                    if (reader.Name == "item" && data.items[^1].itemType == EItem.Painter)
                    {
                        using (var ms = new MemoryStream(_imageData))
                        {
                            var bitmap = BitmapFrame.Create(
                                ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                            ((PainterItem)data.items[^1]).painter = new WriteableBitmap(bitmap);
                        }
                        ((PainterItem)data.items[^1]).image.Source = ((PainterItem)data.items[^1]).painter;
                        ((PainterItem)data.items[^1]).image.Width = ((PainterItem)data.items[^1]).painter.PixelWidth;
                        ((PainterItem)data.items[^1]).image.Height = ((PainterItem)data.items[^1]).painter.PixelHeight;
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
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            case "Canvas":
                                _itm = new CanvasItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            case "Label":
                                _itm = new LabelItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item,
                                    _parentId);
                                V0_5_DefaultParams(_itm, reader, ev);
                                data.items.Add(_itm);
                                break;
                            case "Figure":
                                _itm = new FigureItem(
                                    data,
                                    ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item,
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
                                        _iParam.left = data.items[^1].appX - data.topLeftX;
                                        break;
                                    case "top":
                                        _iParam.top = data.items[^1].appY - data.topLeftY;
                                        break;
                                    case "width":
                                        _iParam.width = int.Parse(_str);
                                        break;
                                    case "height":
                                        _iParam.height = int.Parse(_str);
                                        break;
                                    case "bgColor":
                                        _iParam.bgColor = _str == "null" ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(_str));
                                        break;
                                    case "frColor":
                                        _iParam.frColor = _str == "null" ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(_str));
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
                                        _content.content = _str;
                                        break;
                                    case "horAlign":
                                        _content.horAlign = (HorizontalAlignment)Enum.Parse(typeof(HorizontalAlignment), _str, true);
                                        break;
                                    case "verAlign":
                                        _content.verAlign = (VerticalAlignment)Enum.Parse(typeof(VerticalAlignment), _str, true);
                                        break;
                                    case "isTextChanged":
                                        _content.isTextChanged = bool.Parse(_str);
                                        break;
                                    case "bindParameter":
                                        Enum.TryParse(_str, out _content.bindParameter);
                                        break;
                                    case "bindID":
                                        _content.bindID = int.Parse(_str);
                                        break;
                                    case "isDigitsOnly":
                                        _content.isDigitsOnly = bool.Parse(_str);
                                        break;
                                    case "count":
                                        _content.count = int.Parse(_str);
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
                                        _bParam.isBorder = bool.Parse(_str);
                                        break;
                                    case "borderThickness":
                                        _bParam.borderThickness = double.Parse(_str);
                                        break;
                                    case "color":
                                        _bParam.color = (Color)ColorConverter.ConvertFromString(_str);
                                        break;
                                    case "cornerRadius":
                                        _bParam.cornerRadius = double.Parse(_str);
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
                                        _eParam.isMoveSensitive = bool.Parse(_str);
                                        break;
                                    case "isMouseDown":
                                        _eParam.isMouseDown = bool.Parse(_str);
                                        break;
                                    case "isMouseMove":
                                        _eParam.isMouseMove = bool.Parse(_str);
                                        break;
                                    case "isMouseUp":
                                        _eParam.isMouseUp = bool.Parse(_str);
                                        break;
                                    case "mouseUpInfo":
                                        //_eParam.mouseUpInfo = reader.GetAttribute("isBorder") != null? reader.GetAttribute("isBorder"):null;
                                        break;
                                    case "isHitTestVisible":
                                        _eParam.isHitTestVisible = bool.Parse(_str);
                                        break;
                                    case "isMouseClick":
                                        _eParam.isMouseClick = bool.Parse(_str);
                                        break;
                                    case "isMouseLeave":
                                        _eParam.isMouseLeave = bool.Parse(_str);
                                        break;
                                    case "isMouseWheel":
                                        _eParam.isMouseWheel = bool.Parse(_str);
                                        break;
                                    case "isMouseDoubleClick":
                                        _eParam.isMouseDoubleClick = bool.Parse(_str);
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
                                        _imgParam.imagePath = _str;
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
                            _shapeParam.vertex = new System.Collections.Generic.List<FigureContainer>();
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
                                        Enum.TryParse(_str, out _shapeParam.shape);
                                        break;
                                    case "color":
                                        _shapeParam.color = (Color)ColorConverter.ConvertFromString(_str);
                                        break;
                                    case "strokeThickness":
                                        _shapeParam.strokeThickness = Double.Parse(_str);
                                        break;
                                    case "vertex":
                                        _shapeParam.vertex.Add(new FigureContainer(
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
                if (_item.itemAttach == EItemAttach.Custom &&
                    _item.itemType == EItem.Figure)
                {
                    ((FigureItem)_item).HandlerShapeParam();
                }
            }
        }

        public void V0_5_DefaultParams(DefaultItem itm, XmlReader reader, EventManager ev)
        {
            itm.id = int.Parse(reader.GetAttribute("id"));
            itm.connectorId = int.Parse(reader.GetAttribute("connectorId"));
            itm.parentId = int.Parse(reader.GetAttribute("parentId"));
            itm.name = reader.GetAttribute("name");
            itm.appX = int.Parse(reader.GetAttribute("appX"));
            itm.appY = int.Parse(reader.GetAttribute("appY"));
            itm.itemAttach = EItemAttach.Custom;
            itm.MouseAppIdNotify += ev.EventItemMenuHandler;
        }
    }
}