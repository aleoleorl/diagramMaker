using diagramMaker.helpers.enumerators;
using diagramMaker.managers;
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
        public List<Func<DataHub, DefaultManager, Canvas, MenuMakeOptions, int, int>> itmFunc;
        public List<Func<DataHub, DefaultManager, Canvas, MenuMakeOptions, int, int>> addFunc;
        public List<Func<DataHub, DefaultManager, Canvas, MenuMakeOptions, int, int>> delFunc;

        public MenuMakeOptions()
        {
            category = EMenuCategory.None;
            parentId = -1;
            panelLabel = "";
            x = -1;
            y = -1;
            w = -1;
            h = -1;
            itmStringContent = new List<string>();
            itmEventContent = new List<string>();
            itmImgPath = new List<string>();
            itmFunc = new List<Func<DataHub, DefaultManager, Canvas, MenuMakeOptions, int, int>>();
            addFunc = new List<Func<DataHub, DefaultManager, Canvas, MenuMakeOptions, int, int>>();
            delFunc = new List<Func<DataHub, DefaultManager, Canvas, MenuMakeOptions, int, int>>();
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
            return _tmp;
        }
    }
}