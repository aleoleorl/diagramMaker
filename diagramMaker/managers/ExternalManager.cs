using diagramMaker.items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
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
                //warning
                File.SetAttributes(data.saveName, FileAttributes.Normal);
            }
            

            using (FileStream fs = new FileStream(data.saveName, FileMode.Create))
            using (XmlWriter writer = XmlWriter.Create(fs))
            {
                writer.WriteStartElement("diaMaker");

                writer.WriteStartElement("commonInfo");
                writer.WriteAttributeString("version", "0.4");
                writer.WriteAttributeString("author", "AleOleOrl");
                writer.WriteEndElement();

                writer.WriteStartElement("workspaceInfo");
                writer.WriteAttributeString("id", DefaultItem.GetCurrentFreeId().ToString());
                writer.WriteEndElement();

                for (int _i=0; _i<data.items.Count; _i++)
                {
                    if (data.items[_i].itemAttach == helpers.EItemAttach.Custom)
                    {
                        writer.WriteStartElement("item");
                        writer.WriteAttributeString("id", data.items[_i].id.ToString());
                        writer.WriteAttributeString("parentId", data.items[_i].parentId.ToString());
                        writer.WriteAttributeString("name", data.items[_i].name);
                        writer.WriteAttributeString("itemType", data.items[_i].itemType.ToString());
                        writer.WriteAttributeString("appX", data.items[_i].appX.ToString());
                        writer.WriteAttributeString("appY", data.items[_i].appY.ToString());

                        writer.WriteStartElement("itemSpecial");
                        switch (data.items[_i].itemType)
                        {
                            case helpers.EItem.Button:                                
                                break;
                            case helpers.EItem.Canvas: 
                                break;
                            case helpers.EItem.Figure: 
                                break;
                            case helpers.EItem.Image: 
                                break;
                            case helpers.EItem.Label: 
                                break;
                            case helpers.EItem.Painter:
                                writer.WriteAttributeString("sourcePixelData", ((PainterItem)data.items[_i]).sourcePixelData.ToString());
                                //writer.WriteAttributeString("painterBackBuffer", ((PainterItem)data.items[_i]).painter.BackBuffer.ToString());
                                //writer.WriteAttributeString("painterStride", ((PainterItem)data.items[_i]).painter.BackBufferStride.ToString());
                                
                                break;
                            case helpers.EItem.Text: 
                                break;
                            default:
                                break;
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("iParam");
                        writer.WriteAttributeString("isExist", data.items[_i].iParam == null?"0":"1");
                        if (data.items[_i].iParam != null)
                        {
                            writer.WriteElementString("left", data.items[_i].iParam.left.ToString());
                            writer.WriteElementString("top", data.items[_i].iParam.top.ToString());
                            writer.WriteElementString("width", data.items[_i].iParam.width.ToString());
                            writer.WriteElementString("height", data.items[_i].iParam.height.ToString());
                            writer.WriteElementString("bgColor", data.items[_i].iParam.bgColor == null? "null": data.items[_i].iParam.bgColor.ToString());
                            writer.WriteElementString("frColor", data.items[_i].iParam.frColor == null? "null": data.items[_i].iParam.frColor.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("content");
                        writer.WriteAttributeString("isExist", data.items[_i].content == null ? "0" : "1");
                        if (data.items[_i].content != null)
                        {
                            writer.WriteElementString("content", data.items[_i].content.content == null? "null": data.items[_i].content.content);
                            writer.WriteElementString("horAlign", data.items[_i].content.horAlign == null ? "null" : data.items[_i].content.horAlign.ToString());
                            writer.WriteElementString("verAlign", data.items[_i].content.verAlign == null ? "null" : data.items[_i].content.verAlign.ToString());
                            writer.WriteElementString("isTextChanged", data.items[_i].content.isTextChanged.ToString());
                            writer.WriteElementString("bindParameter", data.items[_i].content.bindParameter.ToString());
                            writer.WriteElementString("bindID", data.items[_i].content.bindID.ToString());
                            writer.WriteElementString("isDigitsOnly", data.items[_i].content.isDigitsOnly.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("bParam");
                        writer.WriteAttributeString("isExist", data.items[_i].bParam == null ? "0" : "1");
                        if (data.items[_i].bParam != null)
                        {
                            writer.WriteElementString("isBorder", data.items[_i].bParam.isBorder.ToString());
                            writer.WriteElementString("borderThickness", data.items[_i].bParam.borderThickness.ToString());
                            writer.WriteElementString("color", data.items[_i].bParam.color == null ? "null" : data.items[_i].bParam.color.ToString());
                            writer.WriteElementString("cornerRadius", data.items[_i].bParam.cornerRadius.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("eParam");
                        writer.WriteAttributeString("isExist", data.items[_i].eParam == null ? "0" : "1");
                        if (data.items[_i].eParam != null)
                        {
                            writer.WriteElementString("isMoveSensitive", data.items[_i].eParam.isMoveSensitive.ToString());
                            writer.WriteElementString("isMouseDown", data.items[_i].eParam.isMouseDown.ToString());
                            writer.WriteElementString("isMouseMove", data.items[_i].eParam.isMouseMove.ToString());
                            writer.WriteElementString("isMouseUp", data.items[_i].eParam.isMouseUp.ToString());
                            writer.WriteElementString("mouseUpInfo", data.items[_i].eParam.mouseUpInfo);
                            writer.WriteElementString("isHitTestVisible", data.items[_i].eParam.isHitTestVisible.ToString());
                            writer.WriteElementString("isMouseClick", data.items[_i].eParam.isMouseClick.ToString());
                        }
                        writer.WriteEndElement();

                        writer.WriteStartElement("imgParam");
                        writer.WriteAttributeString("isExist", data.items[_i].imgParam == null ? "0" : "1");
                        if (data.items[_i].imgParam != null)
                        {
                            writer.WriteElementString("imagePath", data.items[_i].imgParam.imagePath == null ? "null" : data.items[_i].imgParam.imagePath); 
                        }
                        writer.WriteEndElement();

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
                writer.Flush();
            }
        }

        public void Load()
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
                    if (reader.AttributeCount > 0)
                    {
                        //reader.GetAttribute("asd")
                    }
                }
            }
        }
    }
}