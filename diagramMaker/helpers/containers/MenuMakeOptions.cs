using diagramMaker.helpers.enumerators;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace diagramMaker.helpers.containers
{
    public class MenuMakeOptions
    {
        public EMenuCategory category;
        public int parentId;
        public string panelLabel;

        public double x;
        public double y;
        public double w;
        public double h;

        //items
        public List<string> itmStringContent;
        public List<string> itmEventContent;
        public List<string> itmImgPath;
        public Vector4<double> itmPosSize;
        public List<Func<DataHub, MainWindow, Canvas, MenuMakeOptions, int, int>> itmFunc;
        public List<Func<DataHub, MainWindow, Canvas, MenuMakeOptions, int, int>> addFunc;
        public List<Func<DataHub, MainWindow, Canvas, MenuMakeOptions, int, int>> delFunc;

        public MenuMakeOptions()
        {
            category = new EMenuCategory();
            parentId = -1;
            panelLabel = "";
            x = -1;
            y = -1;
            w = -1;
            h = -1;
            itmStringContent = new List<string>();
            itmEventContent = new List<string>();
            itmImgPath = new List<string>();
            itmFunc = new List<Func<DataHub, MainWindow, Canvas, MenuMakeOptions, int, int>>();
            addFunc = new List<Func<DataHub, MainWindow, Canvas, MenuMakeOptions, int, int>>();
            delFunc = new List<Func<DataHub, MainWindow, Canvas, MenuMakeOptions, int, int>>();
            itmPosSize = new Vector4<double>(x: 0, y: 0, w: 0, h: 0);
        }

        public MenuMakeOptions Clone()
        {
            MenuMakeOptions _tmp = new MenuMakeOptions();
            _tmp.category = category;
            _tmp.parentId = parentId;
            _tmp.panelLabel = panelLabel;
            _tmp.x = x;
            _tmp.y = y;
            _tmp.w = w;
            _tmp.h = h;
            _tmp.itmStringContent.AddRange(itmStringContent);
            _tmp.itmEventContent.AddRange(itmEventContent);
            _tmp.itmImgPath.AddRange(itmImgPath);
            _tmp.itmFunc.AddRange(itmFunc);
            _tmp.itmPosSize.x = itmPosSize.x;
            _tmp.itmPosSize.y = itmPosSize.y;
            _tmp.itmPosSize.w = itmPosSize.w;
            _tmp.itmPosSize.h = itmPosSize.h;
            return _tmp;
        }
    }
}