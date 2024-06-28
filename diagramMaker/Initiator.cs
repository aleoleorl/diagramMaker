using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;

namespace diagramMaker
{
    public class Initiator
    {
        private DataHub data;
        private Canvas appCanvas;
        private MainWindow mainWindow;        

        public Initiator(DataHub data, MainWindow mainWindow) 
        { 
            this.data = data;
            this.mainWindow = mainWindow;
        }
        public void Prepare()
        {
            setParameters();
            setItems();

            mainWindow.itemMoveHandlerNotify += eventItemMoveHandler;
        }

        public void setItems()
        {
            //appCanvas
            data.items.Add(new CanvasItem(data, null));
            data.items[data.items.Count - 1].setParameters(
                iParam: (ItemParameter)data.parameters["itemCanvasDefault"],
                content: null,
                bParam: null,
                eParam: new EventParameter(moveSensitive:false, mouseDown:true)
                );
            appCanvas = ((CanvasItem)(data.items[data.items.Count - 1])).item;
            mainWindow.Content = appCanvas;
            data.appCanvasID = data.items[data.items.Count - 1].id;
            Trace.WriteLine("data.appCanvasID:" + data.appCanvasID);

            int _tmpId = data.items[data.items.Count - 1].id;

            //info line
            data.items.Add(new LabelItem(data, appCanvas));
            data.items[data.items.Count - 1].setParameters(
                iParam: (ItemParameter)data.parameters["itemLabelInfoLine"],
                content: new ContentParameter(content: "info line", horAlign:HorizontalAlignment.Left, verAlign: VerticalAlignment.Center),
                bParam: new BorderParameter(isBorder:true, borderThickness:1, color:Colors.Gray, cornerRadius:0),
                eParam: new EventParameter(moveSensitive: false)
                );
            data.informer = data.items.Count - 1;
            mainWindow.cihNotify += ((LabelItem)data.items[data.informer]).setContent;
            mainWindow.appCanvasMoveNotify += ((LabelItem)data.items[data.informer]).setContent;
            Canvas.SetZIndex(((LabelItem)data.items[data.informer]).item, 30);
            mainWindow.appCanvasMoveHandlerNotify += eventItemsShifter;

            //item creation menu
            data.items.Add(new CanvasItem(data, appCanvas));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(5, 10, 150, 500, Brushes.LightCyan, null),
                content: null,
                bParam: new BorderParameter(isBorder:true, borderThickness:1, color:Colors.DarkSlateGray, cornerRadius:2),
                eParam: new EventParameter(moveSensitive: false, mouseDown: true)
                );
            data.items[data.items.Count - 1].appX = data.topLeftX + 5;
            data.items[data.items.Count - 1].appY = data.topLeftY + 10;
            data.menuCreation = data.items.Count - 1;
            Canvas.SetZIndex(((CanvasItem)data.items[data.menuCreation]).item, 20);

            //menuCreation > txt label
            data.items.Add(new LabelItem(data, appCanvas, data.items[data.menuCreation].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(0, 3, 144, 30, null, Brushes.Black),
                content: new ContentParameter(content: "Items Menu:", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );

            //menuCreation > item add canvas
            data.items.Add(new CanvasItem(data, appCanvas, data.items[data.menuCreation].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 33, 144, 50, Brushes.Beige, null),
                content: null,
                bParam: new BorderParameter(isBorder: true, borderThickness:1, color:Colors.Black, cornerRadius:2),
                eParam: null
                );
            data.menuCreationCanvas = data.items.Count - 1;

            //menuCreation > item canvas > txt
            data.items.Add(new LabelItem(data, appCanvas, data.items[data.menuCreationCanvas].id));
            data.items[data.items.Count - 1].setParameters(
                iParam: new ItemParameter(3, 3, 138, 44, null, Brushes.Black),
                content: new ContentParameter(content: "Canvas", horAlign: HorizontalAlignment.Center, verAlign: VerticalAlignment.Center),
                bParam: null,
                eParam: null
                );
        }

        public void setParameters()
        {
            ItemParameter _ip;
            //

            _ip = new ItemParameter(
                left: 0,
                top: 0,
                width: data.screenWidth,
                height: data.screenHeight,
                bgColor: Brushes.LightSteelBlue,
                frColor: null);
            data.parameters.Add("itemCanvasDefault", _ip);

            _ip = new ItemParameter(
                left: 0,
                top: data.winHeight - 45,
                width: data.winWidth - 2,
                height: 30,
                bgColor: Brushes.LightCyan,
                frColor: Brushes.Black);
            data.parameters.Add("itemLabelInfoLine", _ip);
        }

        public void eventItemsShifter(double x, double y)
        {
            foreach ( var item in data.items )
            {
                if (item.eParam != null && item.eParam.moveSensitive)
                {
                    item.setParameter(EParameter.Item,
                        new ItemParameter(
                            left: item.iParam != null ? item.iParam.left + x: x, 
                            top: item.iParam != null ? item.iParam.top + y : y,
                            width: item.iParam!=null?item.iParam.width:1,
                            height: item.iParam != null ? item.iParam.height : 1,
                            bgColor: item.iParam != null ? item.iParam.bgColor : Brushes.Black,
                            frColor: item.iParam != null ? item.iParam.frColor : Brushes.White
                        ));
                }
            }
        }

        public void eventItemMoveHandler(double mouseX, double mouseY)
        {
            for (int _i =0; _i< data.items.Count; _i++)
            {
                if (data.items[_i].id == data.tapped)
                {
                    data.items[_i].setParameter(
                        EParameter.Item,
                        new ItemParameter(
                            left: mouseX + data.tapXX ,
                            top: mouseY + data.tapYY,
                            width: data.items[_i].iParam != null ? data.items[_i].iParam.width : 1,
                            height: data.items[_i].iParam != null ? data.items[_i].iParam.height : 1,
                            bgColor: null,
                            frColor: null
                            ));
                    data.items[_i].appX = data.topLeftX + mouseX + data.tapXX;
                    data.items[_i].appY = data.topLeftY + mouseY + data.tapYY;
                    //data.tapXX = 0; 
                    //data.tapYY = 0;
                    break;
                }
            }
        }
    }
}
