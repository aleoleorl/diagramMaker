using diagramMaker.helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace diagramMaker.windows
{
    public partial class VersionInfo : Window
    {
        private Dictionary<string, string> version;

        public VersionInfo()
        {
            InitializeComponent();
            version = AddInfo();

            info.Content = "Version 0.4(latest)" + ".\n" + version["Version 0.4(latest)"];

            Canvas versionField = new Canvas();
            versionField.Width = 180;
            versionField.Height = 200;
            versionField.Background = new SolidColorBrush(Color.FromArgb(255, 249, 234, 234));
            infoPanel.Children.Add(versionField);
            Canvas.SetLeft(versionField, 46);
            Canvas.SetTop(versionField, 196);

            int _i = 2;
            foreach (var _vers in version)
            {
                Button _btn= new Button();
                _btn.Width = 176;
                _btn.Height = 30;
                _btn.Content = _vers.Key;
                _btn.Click += btn_Click;
                _btn.Background = new SolidColorBrush(Color.FromArgb(255, 249, 234, 234));
                versionField.Children.Add(_btn);
                Canvas.SetLeft(_btn, 2);
                Canvas.SetTop(_btn, _i);
                _i += 31;
            }
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            info.Content = ((Button)sender).Content + ".\n" + version[((Button)sender).Content.ToString()];
        }

        private Dictionary<string, string> AddInfo()
        {
            Dictionary<string, string> _tmp = new Dictionary<string, string>();

            _tmp.Add("Version 0.4(latest)", "Added Top Menu. \nAdded Navigation Panel. \nAdded Line item in Creation Menu. \nSome refactoring and fixing of wrong behavior.");
            _tmp.Add("Version 0.3", "Correction of Item Parameter Menu. \nAdded PaintMaker Item in Creation Menu. \nAdded default Paint Parameter Menu.");
            _tmp.Add("Version 0.2", "Added InfoBlock item in Creation Menu. \nAdded handling with one element on the screen. \nAdded Item Parameter Menu.");
            _tmp.Add("Version 0.1", "Initial version. \nAdded Info Panel. \nAdded Item Creation Menu.");
            return _tmp;
        }
    }
}
