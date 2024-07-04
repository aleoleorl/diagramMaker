using diagramMaker.helpers;
using diagramMaker.items;
using diagramMaker.windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace diagramMaker.managers.DefaultPreparation
{
    public class TopMenuHandler
    {
        private DataHub data;
        private MainWindow mainWindow;

        public Menu mainMenu;
        public MenuItem MenuItem_File;
        public MenuItem MenuItem_View;
        public MenuItem MenuItem_Info;
        public MenuItem SubItem_New;
        public MenuItem SubItem_Save;
        public MenuItem SubItem_SaveAs;
        public MenuItem SubItem_Load;
        public MenuItem SubItem_ConstructorMenu;
        public MenuItem SubItem_NavigationPanelMenu;
        public MenuItem SubItem_InfoLine;
        public MenuItem SubItem_CurrentVersion;

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
        }

        public void SetMainMenu()
        {
            mainMenu = new Menu();
            mainMenu.Width = mainWindow.Width;
            Panel.SetZIndex(mainMenu, 100);

            MenuItem_File = new MenuItem { Header = "File" };
            mainMenu.Items.Add(MenuItem_File);
            MenuItem_View = new MenuItem { Header = "View" };
            mainMenu.Items.Add(MenuItem_View);
            MenuItem_Info = new MenuItem { Header = "Info" };
            mainMenu.Items.Add(MenuItem_Info);

            SubItem_New = new MenuItem { Header = "New" };
            SubItem_New.Click += NewMenuItem_Click;
            MenuItem_File.Items.Add(SubItem_New);
            MenuItem_File.Items.Add(new Separator());
            SubItem_Save = new MenuItem { Header = "Save" };
            SubItem_Save.Click += SaveMenuItem_Click;
            MenuItem_File.Items.Add(SubItem_Save);
            SubItem_SaveAs = new MenuItem { Header = "Save As..." };
            SubItem_SaveAs.Click += SaveAsMenuItem_Click;
            MenuItem_File.Items.Add(SubItem_SaveAs);
            SubItem_Load = new MenuItem { Header = "Load" };
            SubItem_Load.Click += LoadMenuItem_Click;
            MenuItem_File.Items.Add(SubItem_Load);
                        
            SubItem_ConstructorMenu = new MenuItem { Header = "Constructor menu", IsCheckable = true, IsChecked = true };
            MenuItem_View.Items.Add(SubItem_ConstructorMenu);
            SubItem_ConstructorMenu.Click += ConstructorMenuItem_Click;
            SubItem_NavigationPanelMenu = new MenuItem { Header = "Navigation Panel", IsCheckable = true };
            MenuItem_View.Items.Add(SubItem_NavigationPanelMenu);
            SubItem_NavigationPanelMenu.Click += NavigationPanel_Click;
            SubItem_InfoLine = new MenuItem { Header = "Info Line", IsCheckable = true, IsChecked = true };
            MenuItem_View.Items.Add(SubItem_InfoLine);
            SubItem_InfoLine.Click += InfoLine_Click;

            SubItem_CurrentVersion = new MenuItem { Header = "Version Info" };
            MenuItem_Info.Items.Add(SubItem_CurrentVersion);
            MenuItem_Info.Click += CurrentVersion_Click;
            ((CanvasItem)data.items[data.GetItemByID(data.appCanvasID)]).item.Children.Add(mainMenu);
        }

        private void NavigationPanel_Click(object sender, RoutedEventArgs e)
        {
            if (SubItem_NavigationPanelMenu.IsChecked)
            {
                ((CanvasItem)data.items[data.GetItemByID(data.menuNavigationPanelID)]).item.Visibility = Visibility.Visible;
            }
            else
            {
                ((CanvasItem)data.items[data.GetItemByID(data.menuNavigationPanelID)]).item.Visibility = Visibility.Hidden;
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
            if (SubItem_InfoLine.IsChecked)
            {
                ((LabelItem)data.items[data.GetItemByID(data.informerID)]).item.Visibility = Visibility.Visible;
            }
            else
            {
                ((LabelItem)data.items[data.GetItemByID(data.informerID)]).item.Visibility = Visibility.Hidden;
            }
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuHandlerNotify?.Invoke(action:ETopMenuActionType.New, isIt: true);
        }

        private void ConstructorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (SubItem_ConstructorMenu.IsChecked)
            {
                ((CanvasItem)data.items[data.GetItemByID(data.menuCreationCanvasID)]).item.Visibility = Visibility.Visible;
            }
            else
            {
                ((CanvasItem)data.items[data.GetItemByID(data.menuCreationCanvasID)]).item.Visibility = Visibility.Hidden;
            }
        }
    }
}
