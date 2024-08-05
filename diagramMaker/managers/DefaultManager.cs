using diagramMaker.managers.DefaultPanels;
using diagramMaker.managers.DefaultPreparation;

namespace diagramMaker.managers
{
    public class DefaultManager
    {
        public diagramMaker.managers.EventManager eve;
        public MenuMaker menuMaker;
        public NavigationPanel navPanel;
        public PanelEvents panelEvent;
        public TopMenuHandler topMenu;
        public MainWindowManager windowManager;
        public LayerControl layerControl;

        public MainWindow mainWindow;
        private DataHub data;

        public DefaultManager(MainWindow mainWindow, DataHub data)
        {
            this.mainWindow = mainWindow;
            this.data = data;

            eve = new managers.EventManager(this.data, this);
            menuMaker = new MenuMaker();
            navPanel = new NavigationPanel(this.data, this);
            panelEvent = new PanelEvents(this.data);
            windowManager = new MainWindowManager(this.mainWindow, this.data, this);
            topMenu = new TopMenuHandler(this.data, this);
            layerControl = new LayerControl(this.data);
        }
    }
}