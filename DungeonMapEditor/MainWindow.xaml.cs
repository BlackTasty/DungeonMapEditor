using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
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
using Xceed.Wpf.AvalonDock.Controls;

namespace DungeonMapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TabItem selectedTabItem;
        private HomeScreen homeInstance;

        public MainWindow()
        {
            InitializeComponent();
            homeInstance = new HomeScreen();
            homeInstance.SelectionMade += HomeScreen_SelectionMade;
            AddTab(homeInstance, "Home", true);
        }

        private int AddTab(FrameworkElement element, string headerString, bool isSelected = false, object tabItemTag = null)
        {
            var existingTabs = tabControl.Items.OfType<TabItem>();

            foreach (var tab in existingTabs)
            {
                if (element is HomeScreen elementHome && tab.Content is HomeScreen tabHome)
                {
                    return tabControl.Items.IndexOf(tab);
                }
                else if (element is ProjectOverview elementProjectOverview && tab.Content is ProjectOverview tabProjectOverview)
                {
                    if (elementProjectOverview.GetProjectGuid() == tabProjectOverview.GetProjectGuid())
                    {
                        return tabControl.Items.IndexOf(tab);
                    }
                }
            }

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
            projectOverview.ChangeObserved += ProjectOverview_ChangeObserved;
            tabControl.SelectedIndex = AddTab(projectOverview, projectOverview.GetProjectName(), true, projectOverview.Tag);
            App.LoadHistory();
        }

        private void ProjectOverview_ChangeObserved(object sender, Core.FileSystem.ChangeObservedEventArgs e)
        {
            if (sender is ProjectOverview projectOverview)
            {
                TabItem target = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Tag == projectOverview.Tag);
                if (target != null && target.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = !e.UnsavedChanges ? e.NewValue : e.NewValue + "*";
                        headerText.FontStyle = !e.UnsavedChanges ? FontStyles.Normal : FontStyles.Italic;
                    }
                }
            }
        }

        private void Dialog_OpenDialog(object sender, OpenDialogEventArgs e)
        {
            if (e.Dialog is DialogCreateFloor createFloor)
            {
                createFloor.DialogCompleted += Dialog_DialogCompleted;
            }
            else if (e.Dialog is DialogCreateRoom createRoom)
            {
                createRoom.DialogCompleted += Dialog_DialogCompleted;
            }
            else if (e.Dialog is DialogCreateCollection createCollection)
            {
                createCollection.DialogCompleted += Dialog_DialogCompleted;
            }
            else if (e.Dialog is DialogExportProject exportProject)
            {
                exportProject.DialogCompleted += Dialog_DialogCompleted;
            }
            else if (e.Dialog is DialogRemoveObject removeObject)
            {
                removeObject.DialogCompleted += Dialog_DialogCompleted;
            }
            else if (e.Dialog is DialogClosingUnsaved closingUnsaved)
            {
                closingUnsaved.DialogCompleted += Dialog_DialogCompleted;
            }
            (DataContext as MainViewModel).Dialog = e.Dialog;
        }

        private void Dialog_DialogCompleted(object sender, Core.Dialog.DialogButtonClickedEventArgs e)
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
                        headerText.Text = !projectOverview.GetUnsavedChanges() ? e.NewName : e.NewName + "*";
                        headerText.FontStyle = !projectOverview.GetUnsavedChanges() ? FontStyles.Normal : FontStyles.Italic;
                    }
                }
            }
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TabItem targetTab)
            {
                if (targetTab.Content is ProjectOverview projectOverview)
                {
                    var projectVm = projectOverview.DataContext as ProjectOverviewViewModel;
                    if (projectVm.ProjectFile.UnsavedChanges)
                    {
                        DialogClosingUnsaved dialog = new DialogClosingUnsaved(targetTab);
                        dialog.SetObjectValues(projectOverview);
                        dialog.DialogCompleted += DialogClosingUnsaved_DialogCompleted;

                        (DataContext as MainViewModel).Dialog = dialog;
                    }
                    else
                    {
                        projectOverview.ProjectNameChanged -= ProjectOverview_ProjectNameChanged;
                        projectOverview.OpenDialog -= Dialog_OpenDialog;
                        projectOverview.ChangeObserved -= ProjectOverview_ChangeObserved;
                        tabControl.Items.Remove(targetTab);
                    }
                }
            }
        }

        private void DialogClosingUnsaved_DialogCompleted(object sender, Core.Dialog.ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == Core.Dialog.DialogResult.Abort)
            {
                return;
            }

            if (e.Target is ProjectOverview projectOverview)
            {
                if (e.DialogResult == Core.Dialog.DialogResult.Yes)
                {
                    (projectOverview.DataContext as ProjectOverviewViewModel).ProjectFile.Save();
                }
                else
                {
                    (projectOverview.DataContext as ProjectOverviewViewModel).ProjectFile.Load();
                }
                projectOverview.ProjectNameChanged -= ProjectOverview_ProjectNameChanged;
                projectOverview.OpenDialog -= Dialog_OpenDialog;
                projectOverview.ChangeObserved -= ProjectOverview_ChangeObserved;
            }

            tabControl.Items.Remove(e.TargetTab);
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

        private void OpenHomeScreen_Click(object sender, RoutedEventArgs e)
        {
            tabControl.SelectedIndex = AddTab(homeInstance, "Home", true);
        }

        private void CreateProject_Click(object sender, RoutedEventArgs e)
        {
            homeInstance.CreateMap();
        }

        private void LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            homeInstance.LoadMapFromFile();
        }
    }
}
