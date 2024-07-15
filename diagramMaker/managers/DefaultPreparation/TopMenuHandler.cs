using diagramMaker.helpers.enumerators;
using diagramMaker.items;
using diagramMaker.windows;
using Microsoft.Win32;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace diagramMaker.managers.DefaultPreparation
{
    public class TopMenuHandler
    {
        private DataHub data;
        private MainWindow mainWindow;

        public Menu mainMenu;
        public MenuItem menuItem_File;
        public MenuItem menuItem_View;
        public MenuItem menuItem_Info;
        public MenuItem subItem_New;
        public MenuItem subItem_Save;
        public MenuItem subItem_SaveAs;
        public MenuItem subItem_Load;
        public MenuItem subItem_ConstructorMenu;
        public MenuItem subItem_NavigationPanelMenu;
        public MenuItem subItem_InfoLine;
        public MenuItem subItem_CurrentVersion;

        public delegate void MenuHandler(ETopMenuActionType action, bool isIt);
        public event MenuHandler? MenuHandlerNotify;

        private VersionInfo winVersionInfo;

        public TopMenuHandler(DataHub data, MainWindow mainWindow) 
        {
            this.data = data;
            this.mainWindow = mainWindow;

            winVersionInfo = new VersionInfo();
            winVersionInfo.Left = mainWindow.Left;
            winVersionInfo.Top = mainWindow.Top;

            string _exePath = Assembly.GetExecutingAssembly().Location;
            string _directoryPath = Path.GetDirectoryName(_exePath);
            data.saveName = _directoryPath +"\\" + "diaFile.dmf";

            mainWindow.eventner.KeyDownNotify += KeyDown;
        }

        public void SetMainMenu()
        {
            mainMenu = new Menu();
            mainMenu.Width = mainWindow.Width;
            Panel.SetZIndex(mainMenu, 100);

            menuItem_File = new MenuItem { Header = "File" };
            mainMenu.Items.Add(menuItem_File);
            menuItem_View = new MenuItem { Header = "View" };
            mainMenu.Items.Add(menuItem_View);
            menuItem_Info = new MenuItem { Header = "Info" };
            mainMenu.Items.Add(menuItem_Info);

            subItem_New = new MenuItem { Header = "New" };
            subItem_New.Click += NewMenuItem_Click;
            menuItem_File.Items.Add(subItem_New);
            menuItem_File.Items.Add(new Separator());
            subItem_Save = new MenuItem { Header = "Save" };
            subItem_Save.Click += SaveMenuItem_Click;
            menuItem_File.Items.Add(subItem_Save);
            subItem_SaveAs = new MenuItem { Header = "Save As..." };
            subItem_SaveAs.Click += SaveAsMenuItem_Click;
            menuItem_File.Items.Add(subItem_SaveAs);
            subItem_Load = new MenuItem { Header = "Load" };
            subItem_Load.Click += LoadMenuItem_Click;
            menuItem_File.Items.Add(subItem_Load);
                        
            subItem_ConstructorMenu = new MenuItem { Header = "Constructor menu", IsCheckable = true, IsChecked = true, InputGestureText="Ctrl+C"};
            menuItem_View.Items.Add(subItem_ConstructorMenu);
            subItem_ConstructorMenu.Click += ConstructorMenuItem_Click;
            subItem_NavigationPanelMenu = new MenuItem { Header = "Navigation Panel", IsCheckable = true, InputGestureText = "Ctrl+N" };
            menuItem_View.Items.Add(subItem_NavigationPanelMenu);
            subItem_NavigationPanelMenu.Click += NavigationPanel_Click;
            subItem_InfoLine = new MenuItem { Header = "Info Line", IsCheckable = true, IsChecked = true, InputGestureText = "Ctrl+I" };
            menuItem_View.Items.Add(subItem_InfoLine);
            subItem_InfoLine.Click += InfoLine_Click;

            subItem_CurrentVersion = new MenuItem { Header = "Version Info" };
            menuItem_Info.Items.Add(subItem_CurrentVersion);
            menuItem_Info.Click += CurrentVersion_Click;
            ((CanvasItem)data.items[data.GetItemIndexByID(data.appCanvasID)]).Item.Children.Add(mainMenu);
        }

        private void NavigationPanel_Click(object sender, RoutedEventArgs e)
        {
            if (subItem_NavigationPanelMenu.IsChecked)
            {
                ((CanvasItem)data.items[data.GetItemIndexByID(data.menuNavigationPanelID)]).Item.Visibility = Visibility.Visible;
            }
            else
            {
                ((CanvasItem)data.items[data.GetItemIndexByID(data.menuNavigationPanelID)]).Item.Visibility = Visibility.Hidden;
            }
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog
            {
                Filter = "Diagram Maker files (*.dmf)|*.dmf",
                InitialDirectory = @"C:\Users\YourUsername\Documents"
            };

            if (openDialog.ShowDialog() == true)
            {
                data.loadName = openDialog.FileName;
                MenuHandlerNotify?.Invoke(action: ETopMenuActionType.Load, isIt: true);
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuHandlerNotify?.Invoke(action: ETopMenuActionType.Save, isIt: true);
        }
        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Diagram Maker files (*.dmf)|*.dmf",
                InitialDirectory = @"C:\Users\YourUsername\Documents"
            };

            if (saveDialog.ShowDialog() == true)
            {
                data.saveName = saveDialog.FileName;
                MenuHandlerNotify?.Invoke(action: ETopMenuActionType.Save, isIt: true);
            }
        }

        private void CurrentVersion_Click(object sender, RoutedEventArgs e)
        {
            winVersionInfo.Show();
        }

        private void InfoLine_Click(object sender, RoutedEventArgs e)
        {
            if (subItem_InfoLine.IsChecked)
            {
                ((LabelItem)data.items[data.GetItemIndexByID(data.informerID)]).Item.Visibility = Visibility.Visible;
            }
            else
            {
                ((LabelItem)data.items[data.GetItemIndexByID(data.informerID)]).Item.Visibility = Visibility.Hidden;
            }
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuHandlerNotify?.Invoke(action:ETopMenuActionType.New, isIt: true);
        }

        private void ConstructorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (subItem_ConstructorMenu.IsChecked)
            {
                ((CanvasItem)data.items[data.GetItemIndexByID(data.menuCreationCanvasID)]).Item.Visibility = Visibility.Visible;
            }
            else
            {
                ((CanvasItem)data.items[data.GetItemIndexByID(data.menuCreationCanvasID)]).Item.Visibility = Visibility.Hidden;
            }
        }

        public void KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.C))
            {
                if (subItem_ConstructorMenu.IsChecked)
                {
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuCreationCanvasID)]).Item.Visibility = Visibility.Hidden;
                }
                else
                {
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuCreationCanvasID)]).Item.Visibility = Visibility.Visible;
                }
                subItem_ConstructorMenu.IsChecked = !subItem_ConstructorMenu.IsChecked;
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.I))
            {
                if (subItem_InfoLine.IsChecked)
                {
                    ((LabelItem)data.items[data.GetItemIndexByID(data.informerID)]).Item.Visibility = Visibility.Hidden;
                }
                else
                {
                    ((LabelItem)data.items[data.GetItemIndexByID(data.informerID)]).Item.Visibility = Visibility.Visible;
                }
                subItem_InfoLine.IsChecked = !subItem_InfoLine.IsChecked;
            }
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.N))
            {
                if (subItem_NavigationPanelMenu.IsChecked)
                {
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuNavigationPanelID)]).Item.Visibility = Visibility.Hidden;
                }
                else
                {
                    ((CanvasItem)data.items[data.GetItemIndexByID(data.menuNavigationPanelID)]).Item.Visibility = Visibility.Visible;
                }
                subItem_NavigationPanelMenu.IsChecked = !subItem_NavigationPanelMenu.IsChecked;
            }            
        }
    }
}