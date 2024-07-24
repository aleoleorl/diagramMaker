using diagramMaker.managers;
using System.Windows;

namespace diagramMaker
{
    public partial class MainWindow : Window
    {        
        private DataHub data;
        private DefaultManager defMan;
        private Initiator initiator;        
        
        public MainWindow()
        {
            InitializeComponent();

            data = new DataHub();
            defMan = new DefaultManager(this, data);
            initiator = new Initiator(data, defMan);
            initiator.Prepare();
        }        
    }
}