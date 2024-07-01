using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.parameters;

namespace diagramMaker
{
    public class DataHub
    {
        public List<DefaultItem> items;
        public Dictionary<string, DefaultParameter> parameters;
        public Dictionary<string, ItemMaker> itemCollection;
        
        public int tapped;
        public double tapXX;
        public double tapYY;

        public double winWidth;
        public double winHeight;
        public double screenWidth;
        public double screenHeight;
        public double topLeftX;
        public double topLeftY;

        public int appCanvasID = -1;
        public int informer = -1;
        public int menuCreation = -1;
        public int menuCreationCanvasID = -1;

        public int MenuItemParametersID = -1;
        public bool IsMenuItem = false;
        public int ChoosenItemID = -1;

        public int MenuItemPaintMakerID = -1;
        public bool IsMenuPainter = false;
        public EPainterTool PainterTool = EPainterTool.Move;

        public double oldMouseX;
        public double oldMouseY;

        public bool isInit;

        public DataHub() 
        {
            items = new List<DefaultItem>();
            parameters = new Dictionary<string, DefaultParameter>();
            itemCollection = new Dictionary<string, ItemMaker>();

            tapped = -1;
            screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            topLeftX = 400000;
            topLeftY = 400000;

            oldMouseX = 0;
            oldMouseY = 0;

            isInit = false;
        }  
        
        public int GetItemByID(int id)
        {
            for (var _i=0; _i< items.Count; _i++)
            {
                if (items[_i].id == id)
                {
                    return _i;
                }
            }
            return -1;
        }
    }
}
