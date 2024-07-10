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
                        if (_item.ItemAttach != helpers.EItemAttach.Custom)
                        {
                            continue;
                        }
                        writer.WriteStartElement("item");
                        writer.WriteAttributeString("id", _item.Id.ToString());
                        writer.WriteAttributeString("parentId", _item.ParentId.ToString());
                        writer.WriteAttributeString("connectorId", _item.ConnectorId.ToString());
                        writer.WriteAttributeString("name", _item.Name);
                        writer.WriteAttributeString("itemType", _item.ItemType.ToString());
                        writer.WriteAttributeString("appX", _item.AppX.ToString());
                        writer.WriteAttributeString("appY", _item.AppY.ToString());

                        writer.WriteStartElement("itemSpecial");
                        switch (_item.ItemType)
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
                        writer.WriteAttributeString("isExist", _item.IParam == null ? "0" : "1");
                        if (_item.IParam != null)
                        {
                            writer.WriteElementString("left", _item.IParam.Left.ToString());
                            writer.WriteElementString("top", _item.IParam.Top.ToString());
                            writer.WriteElementString("width", _item.IParam.Width.ToString());
                            writer.WriteElementString("height", _item.IParam.Height.ToString());
                            writer.WriteElementString("bgColor", _item.IParam.BgColor == null ? "null" : _item.IParam.BgColor.ToString());
                            writer.WriteElementString("frColor", _item.IParam.FrColor == null ? "null" : _item.IParam.FrColor.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("content");
                        writer.WriteAttributeString("isExist", _item.Content == null ? "0" : "1");
                        if (_item.Content != null)
                        {
                            writer.WriteElementString("content", _item.Content.Content == null ? "null" : _item.Content.Content);
                            writer.WriteElementString("horAlign", _item.Content.HorAlign == null ? "null" : _item.Content.HorAlign.ToString());
                            writer.WriteElementString("verAlign", _item.Content.VerAlign == null ? "null" : _item.Content.VerAlign.ToString());
                            writer.WriteElementString("isTextChanged", _item.Content.IsTextChanged.ToString());
                            writer.WriteElementString("bindParameter", _item.Content.BindParameter.ToString());
                            writer.WriteElementString("bindID", _item.Content.BindID.ToString());
                            writer.WriteElementString("isDigitsOnly", _item.Content.IsDigitsOnly.ToString());
                            writer.WriteElementString("count", _item.Content.Count.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("bParam");
                        writer.WriteAttributeString("isExist", _item.BParam == null ? "0" : "1");
                        if (_item.BParam != null)
                        {
                            writer.WriteElementString("isBorder", _item.BParam.IsBorder.ToString());
                            writer.WriteElementString("borderThickness", _item.BParam.BorderThickness.ToString());
                            writer.WriteElementString("color", _item.BParam.Color == null ? "null" : _item.BParam.Color.ToString());
                            writer.WriteElementString("cornerRadius", _item.BParam.CornerRadius.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("eParam");
                        writer.WriteAttributeString("isExist", _item.EParam == null ? "0" : "1");
                        if (_item.EParam != null)
                        {
                            writer.WriteElementString("isMoveSensitive", _item.EParam.IsMoveSensitive.ToString());
                            writer.WriteElementString("isMouseDown", _item.EParam.IsMouseDown.ToString());
                            writer.WriteElementString("isMouseMove", _item.EParam.IsMouseMove.ToString());
                            writer.WriteElementString("isMouseUp", _item.EParam.IsMouseUp.ToString());
                            writer.WriteElementString("mouseUpInfo", _item.EParam.MouseUpInfo);
                            writer.WriteElementString("isHitTestVisible", _item.EParam.IsHitTestVisible.ToString());
                            writer.WriteElementString("isMouseClick", _item.EParam.IsMouseClick.ToString());
                            writer.WriteElementString("isMouseLeave", _item.EParam.IsMouseLeave.ToString());
                            writer.WriteElementString("isMouseWheel", _item.EParam.IsMouseWheel.ToString());
                            writer.WriteElementString("isMouseDoubleClick", _item.EParam.IsMouseDoubleClick.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("imgParam");
                        writer.WriteAttributeString("isExist", _item.ImgParam == null ? "0" : "1");
                        if (_item.ImgParam != null)
                        {
                            writer.WriteElementString("imagePath", _item.ImgParam.ImagePath == null ? "null" : _item.ImgParam.ImagePath);
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("shapeParam");
                        writer.WriteAttributeString("isExist", _item.ShapeParam == null ? "0" : "1");
                        writer.WriteAttributeString("vertex", _item.ShapeParam == null ? "0" : _item.ShapeParam.Vertex.Count.ToString());
                        if (_item.ShapeParam != null)
                        {
                            writer.WriteElementString("shape", _item.ShapeParam.Shape.ToString());
                            writer.WriteElementString("color", _item.ShapeParam.Color == null ? "null" : _item.ShapeParam.Color.ToString());
                            writer.WriteElementString("strokeThickness", _item.ShapeParam.StrokeThickness.ToString());

                            foreach (FigureContainer _fc in _item.ShapeParam.Vertex)
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
                    if (reader.Name == "item" && data.items!= null && data.items[^1].ItemType == EItem.Painter)
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
                                        _iParam.Left = data.items[^1].AppX - data.topLeftX;
                                        break;
                                    case "top":
                                        _iParam.Top = data.items[^1].AppY - data.topLeftY;
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
                if (_item.ItemAttach == EItemAttach.Custom &&
                    _item.ItemType == EItem.Figure)
                {
                    ((FigureItem)_item).HandlerShapeParam();
                }
            }
        }

        public void V0_5_DefaultParams(DefaultItem itm, XmlReader reader, EventManager ev)
        {
            itm.Id = int.Parse(reader.GetAttribute("id"));
            itm.ConnectorId = int.Parse(reader.GetAttribute("connectorId"));
            itm.ParentId = int.Parse(reader.GetAttribute("parentId"));
            itm.Name = reader.GetAttribute("name");
            itm.AppX = int.Parse(reader.GetAttribute("appX"));
            itm.AppY = int.Parse(reader.GetAttribute("appY"));
            itm.ItemAttach = EItemAttach.Custom;
            itm.MouseAppIdNotify += ev.EventItemMenuHandler;
        }
    }
}