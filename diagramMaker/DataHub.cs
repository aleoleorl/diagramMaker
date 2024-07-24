﻿using System.Collections.Generic;
using diagramMaker.helpers.containers;
using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.parameters;

namespace diagramMaker
{
    public class DataHub
    {
        public List<DefaultItem> items;
        public Dictionary<string, DefaultParameter> parameters;
        public Dictionary<string, ItemMakerContainer> itemCollection;

        public int tapped;
        public double tapXX;
        public double tapYY;

        public double winWidth;
        public double winHeight;
        public double screenWidth;
        public double screenHeight;
        public double topLeftX;
        public double topLeftY;

        public int appCanvasID;
        public int informerID;
        public int menuCreation;
        public int menuCreationCanvasID;

        public int menuItemParametersID;
        public bool isMenuItem;
        public int choosenItemID;

        public int menuItemPaintMakerID;
        public bool isMenuPainter;
        public EPainterTool painterTool;

        public int menuNavigationPanelID;
        public int menuNavigationPanel_ScrollCount;
        public int menuNavigationPanel_SlotNumber;

        public double oldMouseX;
        public double oldMouseY;

        public bool isInit;

        public string? saveName;
        public string? loadName;

        public bool btnControl;
        public bool btnAlt;

        private static bool isClear = true;
        public static bool IsClear
        {
            get 
            { 
                if (isClear) 
                {
                    isClear = false;
                    return true;
                } else 
                { return isClear;
                } 
            }
        }

        public List<Connection> addConnector;

        public Dictionary <string, MenuContainer> panel;

        public int activeLayer;
        public int countOfLayers;
        public List<LayerInfo> layerInfoItems;
        public List<LayerInfo> layerInfoDefPanels;

        public DataHub()
        {
            ClearData();
        }
        
        public void ClearData()
        {
            isClear = true;

            items = new List<DefaultItem>();
            parameters = new Dictionary<string, DefaultParameter>();
            itemCollection = new Dictionary<string, ItemMakerContainer>();

            tapped = -1;
            tapXX = 0;
            tapYY = 0;

            screenWidth = System.Windows.SystemParameters.PrimaryScreenHeight;
            screenHeight = System.Windows.SystemParameters.PrimaryScreenWidth;

            winWidth = 1024;
            winHeight = 768;

            topLeftX = 400000;
            topLeftY = 400000;

            oldMouseX = 0;
            oldMouseY = 0;
            isInit = false;

            appCanvasID = -1;
            informerID = -1;
            menuCreation = -1;
            menuCreationCanvasID = -1;

            menuItemParametersID = -1;
            isMenuItem = false;
            choosenItemID = -1;

            menuNavigationPanelID = 1;
            menuNavigationPanel_ScrollCount = 0;
            menuNavigationPanel_SlotNumber = 15;

            menuItemPaintMakerID = -1;
            isMenuPainter = false;
            painterTool = EPainterTool.Move;

            saveName = "diaFile.dmf";

            btnControl = false;
            btnAlt = false;

            addConnector = new List<Connection>();

            panel = new Dictionary<string, MenuContainer>();

            activeLayer = 1;
            countOfLayers = 1;
            layerInfoItems = new List<LayerInfo>();
            layerInfoDefPanels = new List<LayerInfo>();
        }

        public int GetItemIndexByID(int id)
        {
            if (items==null)
            {
                return -1;
            }
            for (var _i = 0; _i < items.Count; _i++)
            {
                if (((CommonParameter)items[_i].param[EParameter.Common]).Id == id)
                {
                    return _i;
                }
            }
            return -1;
        }
    }
}