using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace diagramMaker.windows
{
    public partial class VersionInfo : Window
    {
        private readonly Dictionary<string, string> version;
        private List<Button> versionButtons;
        private int lag;
        private readonly int versioncount;

        public VersionInfo()
        {
            InitializeComponent();
            version = AddInfo();
            versionButtons = new List<Button>();
            lag = 0;
            versioncount = 5;

            info.Content = "Version 0.5(latest)" + ".\n" + version["Version 0.5(latest)"];

            Canvas versionField = new Canvas();
            versionField.Width = 180;
            versionField.Height = 200;
            versionField.Background = new SolidColorBrush(Color.FromArgb(255, 249, 234, 234));
            infoPanel.Children.Add(versionField);
            Canvas.SetLeft(versionField, 46);
            Canvas.SetTop(versionField, 196);
            versionField.MouseWheel += VersionField_MouseWheel;

            int _i = 2;
            Button _btn;
            int _j = 0;
            foreach (var _vers in version)
            {
                _btn = new Button();
                _btn.Width = 176;
                _btn.Height = 30;
                _btn.Content = _vers.Key;
                _btn.Click += Btn_Click;
                _btn.Background = new SolidColorBrush(Color.FromArgb(255, 249, 234, 234));
                versionField.Children.Add(_btn);
                Canvas.SetLeft(_btn, 2);
                Canvas.SetTop(_btn, _i);
                versionButtons.Add(_btn);

                _i += 31;

                if (++_j == versioncount) 
                { 
                    break;
                }
            }
        }

        private void VersionField_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                EventNavigationPanelScrollCount(-1);
            }
            else
            {
                EventNavigationPanelScrollCount(1);
            }
            EventButtonVersionShow();
        }

        public void EventNavigationPanelScrollCount(int digit)
        {
            lag += digit;
            if (lag < 0)
            {
                lag = 0;
            }
            if (lag > version.Count - versioncount)
            {
                lag = version.Count - versioncount;
            }
        }
        public void EventButtonVersionShow()
        {
            int _lag = lag;
            int _btn = 0;
            foreach (var _vers in version)
            { 
                if (_lag>0)
                {
                    _lag--;
                    continue;
                }
                versionButtons[_btn].Content = _vers.Key;
                if (++_btn == versioncount)
                {
                    break;
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            info.Content = ((Button)sender).Content + ".\n" + version[((Button)sender).Content.ToString()];
        }

        private Dictionary<string, string> AddInfo()
        {
            Dictionary<string, string> _tmp = new Dictionary<string, string>();

            _tmp.Add("Version 0.5(latest)", "Added save-Load option. \nAdded Multiline. \nAdded Keys for a control of the pannels. \nDone first refactoring iteration.");
            _tmp.Add("Version 0.4", "Added Top Menu. \nAdded Navigation Panel. \nAdded Line item in Creation Menu. \nSome refactoring and fixing of wrong behavior.");
            _tmp.Add("Version 0.3", "Correction of Item Parameter Menu. \nAdded PaintMaker Item in Creation Menu. \nAdded default Paint Parameter Menu.");
            _tmp.Add("Version 0.2", "Added InfoBlock item in Creation Menu. \nAdded handling with one element on the screen. \nAdded Item Parameter Menu.");
            _tmp.Add("Version 0.1", "Initial version. \nAdded Info Panel. \nAdded Item Creation Menu.");
            return _tmp;
        }
    }
}
