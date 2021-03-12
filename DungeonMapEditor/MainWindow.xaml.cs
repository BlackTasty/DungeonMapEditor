using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.UI;
using DungeonMapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TabItem selectedTabItem;

        public MainWindow()
        {
            InitializeComponent();
            AddTab(new HomeScreen(), "Home", true);
        }

        private int AddTab(FrameworkElement element, string headerString, bool isSelected = false, object tabItemTag = null)
        {
            StackPanel header = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Button tabCloseButton = new Button()
            {
                Content = "X",
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(4, 0, 4, 0),
                Style = Application.Current.Resources.MergedDictionaries[1]["CloseButton"] as Style
            };
            tabCloseButton.Click += TabCloseButton_Click;

            header.Children.Add(new TextBlock() { Text = headerString });
            header.Children.Add(tabCloseButton);

            TabItem tabItem = new TabItem
            {
                Header = header,
                Content = element,
                Tag = tabItemTag,
                Background = Brushes.Transparent
            };
            tabCloseButton.Tag = tabItem;

            tabControl.Items.Add(tabItem);

            if (tabItem.Content is HomeScreen homeScreen)
            {
                homeScreen.SelectionMade += HomeScreen_SelectionMade;
            }

            if (isSelected)
            {
                SetCurrentTabItemBold(tabItem);
            }

            return tabControl.Items.Count - 1;
        }

        private void HomeScreen_SelectionMade(object sender, HomeScreenSelectionMadeEventArgs e)
        {
            int selectedTabIndex = -1;
            switch (e.Selection)
            {
                case HomeScreenSelectionType.NewProject:
                    MainViewModel vm = DataContext as MainViewModel;
                    DialogCreateProject dialog = new DialogCreateProject();
                    dialog.DialogCompleted += Dialog_DialogCompleted;

                    vm.Dialog = dialog;
                    break;
                case HomeScreenSelectionType.LoadProject:
                    OpenProject(e.SelectedProject);
                    break;
                case HomeScreenSelectionType.OpenTileManager:
                    TileManager tileManager = new TileManager();
                    tileManager.OpenDialog += Dialog_OpenDialog;
                    selectedTabIndex = AddTab(tileManager, "Collection manager");
                    break;
            }

            if (selectedTabIndex >= 0)
            {
                tabControl.SelectedIndex = selectedTabIndex;
            }
        }

        private void Dialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs<ProjectFile> e)
        {
            if (e.DialogResult == Core.Dialog.DialogResult.OK)
            {
                e.ResultObject.Save(App.ProjectsPath);
                OpenProject(e.ResultObject);
            }

            (DataContext as MainViewModel).ShowDialog = false;
        }

        private void OpenProject(ProjectFile projectFile)
        {
            ProjectOverview projectOverview = new ProjectOverview(projectFile);
            projectOverview.Tag = Guid.NewGuid().ToString();
            projectOverview.ProjectNameChanged += ProjectOverview_ProjectNameChanged;
            projectOverview.OpenDialog += Dialog_OpenDialog;
            tabControl.SelectedIndex = AddTab(projectOverview, projectOverview.GetProjectName(), true, projectOverview.Tag);
            App.LoadHistory();
        }

        private void Dialog_OpenDialog(object sender, OpenDialogEventArgs e)
        {
            if (e.Dialog is DialogCreateFloor createFloor)
            {
                createFloor.DialogCompleted += CreateFloor_DialogCompleted;
            }
            else if (e.Dialog is DialogCreateRoom createRoom)
            {
                createRoom.DialogCompleted += CreateRoom_DialogCompleted;
            }
            else if (e.Dialog is DialogCreateCollection createCollection)
            {
                createCollection.DialogCompleted += CreateCollection_DialogCompleted;
            }
            else if (e.Dialog is DialogExportProject exportProject)
            {
                exportProject.DialogCompleted += ExportProject_DialogCompleted;
            }
            (DataContext as MainViewModel).Dialog = e.Dialog;
        }

        private void ExportProject_DialogCompleted(object sender, CreateDialogCompletedEventArgs<ProjectExport> e)
        {
            (DataContext as MainViewModel).ShowDialog = false;
        }

        private void CreateCollection_DialogCompleted(object sender, CreateDialogCompletedEventArgs<Core.Dungeon.Collection.CollectionSet> e)
        {
            (DataContext as MainViewModel).ShowDialog = false;
        }

        private void CreateRoom_DialogCompleted(object sender, CreateDialogCompletedEventArgs<RoomPlan> e)
        {
            (DataContext as MainViewModel).ShowDialog = false;
        }

        private void CreateFloor_DialogCompleted(object sender, CreateDialogCompletedEventArgs<FloorPlan> e)
        {
            (DataContext as MainViewModel).ShowDialog = false;
        }

        private void ProjectOverview_ProjectNameChanged(object sender, NameChangedEventArgs e)
        {
            if (sender is ProjectOverview projectOverview)
            {
                TabItem target = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Tag == projectOverview.Tag);
                if (target != null && target.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = e.NewName;
                    }
                }
            }
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TabItem targetTab)
            {
                if (targetTab.Content is HomeScreen homeScreen)
                {
                    homeScreen.SelectionMade -= HomeScreen_SelectionMade;
                }
                else if (targetTab.Content is ProjectOverview projectOverview)
                {
                    projectOverview.ProjectNameChanged -= ProjectOverview_ProjectNameChanged;
                }
                tabControl.Items.Remove(targetTab);
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabItem selected = e.AddedItems.OfType<TabItem>().FirstOrDefault();
            if (selected != null)
            {
                SetCurrentTabItemBold(selected);
            }
        }

        private void SetCurrentTabItemBold(TabItem tabItem)
        {
            if (selectedTabItem != null)
            {
                //selectedTabItem.FontWeight = FontWeights.Normal;
            }

            if (tabItem != null)
            {
                selectedTabItem = tabItem;
                //selectedTabItem.FontWeight = FontWeights.Bold;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //selectedTabItem = tabControl.Items[0] as TabItem;
            //selectedTabItem.FontWeight = FontWeights.Bold;
        }
    }
}
