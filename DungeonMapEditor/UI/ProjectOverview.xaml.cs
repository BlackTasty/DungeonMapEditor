using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.FileSystem;
using DungeonMapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
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

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for ProjectOverview.xaml
    /// </summary>
    public partial class ProjectOverview : DockPanel
    {
        private Style roomTabStyle;
        private Style floorTabStyle;
        private Style documentLayoutTabStyle;
        private TabItem selectedTabItem;

        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;
        public event EventHandler<OpenDialogEventArgs> OpenDialog;
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        public ProjectOverview() : this(new ProjectFile("Untitled dungeon"))
        {
        }

        public ProjectOverview(ProjectFile projectFile)
        {
            InitializeComponent();
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
            projectFile.Load();
            vm.ProjectFile = projectFile;
            //vm.ProjectNameChanged += ProjectOverview_ProjectNameChanged;
            vm.ChangeObserved += ProjectOverview_ChangeObserved;
            roomTabStyle = Application.Current.Resources.MergedDictionaries[1]["RoomTabItem"] as Style;
            floorTabStyle = Application.Current.Resources.MergedDictionaries[1]["FloorTabItem"] as Style;
            documentLayoutTabStyle = Application.Current.Resources.MergedDictionaries[1]["LayoutTabItem"] as Style;
        }

        private void ProjectOverview_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
        }

        public string GetProjectName()
        {
            return (DataContext as ProjectOverviewViewModel).ProjectFile?.Name;
        }

        public bool GetUnsavedChanges()
        {
            return (DataContext as ProjectOverviewViewModel).ProjectFile?.UnsavedChanges ?? false;
        }

        public string GetProjectGuid()
        {
            return (DataContext as ProjectOverviewViewModel).ProjectFile?.Guid;
        }

        private void ProjectOverview_ProjectNameChanged(object sender, NameChangedEventArgs e)
        {
            OnProjectNameChanged(e);
        }

        private void AddFloor_Click(object sender, RoutedEventArgs e)
        {
            DialogCreateFloor dialog = new DialogCreateFloor((DataContext as ProjectOverviewViewModel).ProjectFile);
            dialog.DialogCompleted += FloorPlanDialog_DialogCompleted;
            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void AddRoom_Click(object sender, RoutedEventArgs e)
        {
            DialogCreateRoom dialog = new DialogCreateRoom((DataContext as ProjectOverviewViewModel).ProjectFile);
            dialog.DialogCompleted += RoomPlanDialog_DialogCompleted;
            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void RoomPlanDialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs<RoomPlan> e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                vm.ProjectFile.RoomPlans.Add(new RoomAssignment(e.ResultObject, vm.ProjectFile));
                OpenRoomPlan(e.ResultObject);
            }
        }

        private void FloorPlanDialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs<FloorPlan> e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                vm.ProjectFile.FloorPlans.Add(new FloorAssignment(e.ResultObject, vm.ProjectFile));
                OpenFloorPlan(e.ResultObject);
            }
        }

        private void FloorDataGridRow_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                if (row.DataContext is FloorAssignment floorAssignment)
                {
                    OpenFloorPlan(floorAssignment.FloorPlan);
                }
            }
        }

        private void RoomDataGridRow_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow row)
            {
                if (row.DataContext is RoomAssignment roomAssignment)
                {
                    OpenRoomPlan(roomAssignment.RoomPlan);
                }
            }
        }

        public void OpenFloorPlan(FloorPlan floorPlan)
        {
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;

            FloorPlanGrid floorPlanGrid = new FloorPlanGrid(floorPlan, vm.ProjectFile);
            floorPlanGrid.ChangeObserved += FloorPlanGrid_ChangeObserved;
            //floorPlanGrid.FloorNameChanged += PlanGrid_NameChanged;

            tabControl.SelectedIndex = AddTab(floorPlanGrid, floorPlan.Name, floorTabStyle, Guid.NewGuid().ToString());
            SetCurrentTabItemBold(tabControl.Items[tabControl.SelectedIndex] as TabItem);
        }

        private void FloorPlanGrid_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            if (sender is FloorPlanGrid target)
            {
                TabItem targetTabItem = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is FloorPlanGrid tab &&
                                            tab.GetFloorPlanGuid() == target.GetFloorPlanGuid());
                if (targetTabItem != null && targetTabItem.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = !e.UnsavedChanges ? e.NewValue : e.NewValue + "*";
                        headerText.FontStyle = !e.UnsavedChanges ? FontStyles.Normal : FontStyles.Italic;
                    }
                }

                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                OnChangeObserved(new ChangeObservedEventArgs(e.UnsavedChanges, vm.ProjectFile.Name, e.Observer));
            }
        }

        public void OpenRoomPlan(RoomPlan roomPlan)
        {
            RoomPlanGrid roomPlanGrid = new RoomPlanGrid(roomPlan);
            roomPlanGrid.ChangeObserved += RoomPlanGrid_ChangeObserved;
            roomPlanGrid.RoomNameChanged += PlanGrid_NameChanged;

            tabControl.SelectedIndex = AddTab(roomPlanGrid, roomPlan.Name, roomTabStyle, Guid.NewGuid().ToString());
            SetCurrentTabItemBold(tabControl.Items[tabControl.SelectedIndex] as TabItem);
        }

        private void RoomPlanGrid_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            if (sender is RoomPlanGrid target)
            {
                TabItem targetTabItem = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is RoomPlanGrid tab && 
                                            tab.GetRoomPlanGuid() == target.GetRoomPlanGuid());
                if (targetTabItem != null && targetTabItem.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = !e.UnsavedChanges ? e.NewValue : e.NewValue + "*";
                        headerText.FontStyle = !e.UnsavedChanges ? FontStyles.Normal : FontStyles.Italic;
                    }
                }

                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                OnChangeObserved(new ChangeObservedEventArgs(e.UnsavedChanges, vm.ProjectFile.Name, e.Observer));
            }
        }

        private void PlanGrid_NameChanged(object sender, NameChangedEventArgs e)
        {
            if (sender is FloorPlanGrid floorPlanGrid)
            {
                TabItem targetTabItem = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is FloorPlanGrid tab && 
                                            tab.GetFloorPlanGuid() == floorPlanGrid.GetFloorPlanGuid());
                if (targetTabItem != null && targetTabItem.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = e.NewName;
                    }
                }
            }
            else if(sender is RoomPlanGrid roomPlanGrid)
            {
                TabItem targetTabItem = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is RoomPlanGrid tab &&
                                            tab.GetRoomPlanGuid() == roomPlanGrid.GetRoomPlanGuid());
                if (targetTabItem != null && targetTabItem.Header is StackPanel headerStack)
                {
                    TextBlock headerText = headerStack.Children.OfType<TextBlock>().FirstOrDefault();

                    if (headerText != null)
                    {
                        headerText.Text = e.NewName;
                    }
                }
            }
        }

        private int AddTab(FrameworkElement element, string headerString, Style tabStyle, object tabItemTag = null)
        {
            var existingTabs = tabControl.Items.OfType<TabItem>();

            foreach (var tab in existingTabs)
            {
                if (element is FloorPlanGrid elementFloorPlan && tab.Content is FloorPlanGrid tabFloorPlan)
                {
                    if (elementFloorPlan.GetFloorPlanGuid() == tabFloorPlan.GetFloorPlanGuid())
                    {
                        return tabControl.Items.IndexOf(tab);
                    }
                }
                else if (element is RoomPlanGrid elementRoomPlan && tab.Content is RoomPlanGrid tabRoomPlan)
                {
                    if (elementRoomPlan.GetRoomPlanGuid() == tabRoomPlan.GetRoomPlanGuid())
                    {
                        return tabControl.Items.IndexOf(tab);
                    }
                }
                else if (element is ProjectPlanGrid elementProjectPlan && tab.Content is ProjectPlanGrid tabProjectPlan)
                {
                    if (elementProjectPlan.GetProjectPlanGuid() == tabProjectPlan.GetProjectPlanGuid())
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
                Style = tabStyle
            };
            tabCloseButton.Tag = tabItem;

            tabControl.Items.Add(tabItem);

            return tabControl.Items.Count - 1;
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TabItem targetTab)
            {
                if (targetTab.Content is FloorPlanGrid floorPlanGrid)
                {
                    var floorPlanVm = floorPlanGrid.DataContext as FloorPlanViewModel;
                    if (floorPlanVm.FloorPlan.UnsavedChanges)
                    {
                        ShowUnsavedDialog(floorPlanGrid, targetTab);
                    }
                    else
                    {
                        floorPlanGrid.FloorNameChanged -= PlanGrid_NameChanged;
                        floorPlanGrid.ChangeObserved -= FloorPlanGrid_ChangeObserved;
                        tabControl.Items.Remove(targetTab);
                    }
                }
                else if (targetTab.Content is RoomPlanGrid roomPlanGrid)
                {
                    var roomPlanVm = roomPlanGrid.DataContext as RoomPlanViewModel;
                    if (roomPlanVm.RoomPlan.UnsavedChanges)
                    {
                        ShowUnsavedDialog(roomPlanGrid, targetTab);
                    }
                    else
                    {
                        roomPlanGrid.UnloadGrid();
                        roomPlanGrid.ChangeObserved -= RoomPlanGrid_ChangeObserved;
                        roomPlanGrid.RoomNameChanged -= PlanGrid_NameChanged;
                        tabControl.Items.Remove(targetTab);
                    }
                }
                else
                {
                    tabControl.Items.Remove(targetTab);
                }
            }
        }

        private void ShowUnsavedDialog<T>(T unsavedObject, TabItem targetTab)
        {
            DialogClosingUnsaved dialog = new DialogClosingUnsaved(targetTab);
            dialog.SetObjectValues(unsavedObject);
            dialog.DialogCompleted += DialogClosingUnsaved_DialogCompleted;

            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void DialogClosingUnsaved_DialogCompleted(object sender, ClosingUnsavedDialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.Abort)
            {
                return;
            }

            if (e.Target is RoomPlanGrid roomPlanGrid)
            {
                if (e.DialogResult == DialogResult.Yes)
                {
                    var roomPlanVm = roomPlanGrid.DataContext as RoomPlanViewModel;
                    roomPlanVm.RoomPlan.Save(System.IO.Path.Combine((DataContext as ProjectOverviewViewModel).ProjectFile.FilePath, "rooms"));
                }
                else
                {
                    (roomPlanGrid.DataContext as RoomPlanViewModel).RoomPlan.Load();
                }
                roomPlanGrid.UnloadGrid();
                roomPlanGrid.ChangeObserved -= RoomPlanGrid_ChangeObserved;
                roomPlanGrid.RoomNameChanged -= PlanGrid_NameChanged;
            }
            else if (e.Target is FloorPlanGrid floorPlanGrid)
            {
                if (e.DialogResult == DialogResult.Yes)
                {
                    var floorPlanVm = floorPlanGrid.DataContext as FloorPlanViewModel;
                    floorPlanVm.FloorPlan.Save(System.IO.Path.Combine((DataContext as ProjectOverviewViewModel).ProjectFile.FilePath, "floors"));
                }
                else
                {
                    (floorPlanGrid.DataContext as FloorPlanViewModel).FloorPlan.Load();
                }
                floorPlanGrid.FloorNameChanged -= PlanGrid_NameChanged;
                floorPlanGrid.ChangeObserved -= FloorPlanGrid_ChangeObserved;
            }

            tabControl.Items.Remove(e.TargetTab);
        }

        private void SaveProject_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as ProjectOverviewViewModel).ProjectFile.Save();
        }

        private void ExportAs_Click(object sender, RoutedEventArgs e)
        {
            DialogExportProject dialog = new DialogExportProject((DataContext as ProjectOverviewViewModel).ProjectFile);
            dialog.DialogCompleted += Dialog_DialogCompleted;
            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void Dialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs<ProjectExport> e)
        {
            if (e.DialogResult == Core.Dialog.DialogResult.OK)
            {
                string exportDir = e.ResultObject.ExportProject();
                Process.Start("explorer.exe", string.Format("/select,\"{0}\"", exportDir));
            }
        }

        private void ConfigureLayout_Click(object sender, RoutedEventArgs e)
        {
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;

            ProjectPlanGrid projectPlanGrid = new ProjectPlanGrid(vm.ProjectFile);

            tabControl.SelectedIndex = AddTab(projectPlanGrid, "Document layout", documentLayoutTabStyle, Guid.NewGuid().ToString());
            SetCurrentTabItemBold(tabControl.Items[tabControl.SelectedIndex] as TabItem);
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

        private void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            //selectedTabItem = tabControl.Items[0] as TabItem;
            //selectedTabItem.FontWeight = FontWeights.Bold;
        }

        private void RemoveRoom_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedRoom();
        }

        private void RemoveSelectedRoom()
        {
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
            DialogRemoveObject dialog = new DialogRemoveObject(vm.SelectedRoomAssignment.RoomPlan.Name);
            dialog.DialogCompleted += DialogRemoveRoom_DialogCompleted;

            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void DialogRemoveRoom_DialogCompleted(object sender, DialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                TabItem openTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is RoomPlanGrid roomPlan &&
                                        roomPlan.GetRoomPlanGuid() == vm.SelectedRoomAssignment.RoomPlan.Guid);

                if (openTab != null)
                {
                    tabControl.Items.Remove(openTab);
                }

                vm.SelectedRoomAssignment.RoomPlan.Delete();
                vm.ProjectFile.RoomPlans.Remove(vm.SelectedRoomAssignment);
                vm.ProjectFile.SaveOnlyRooms();
            }
        }

        private void RemoveFloor_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelectedFloor();
        }

        private void RemoveSelectedFloor()
        {
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
            TabItem openTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is FloorPlanGrid floorPlan &&
                                    floorPlan.GetFloorPlanGuid() == vm.SelectedFloorAssignment.FloorPlan.Guid);

            if (openTab != null)
            {
                tabControl.Items.Remove(openTab);
            }

            DialogRemoveObject dialog = new DialogRemoveObject(vm.SelectedFloorAssignment.FloorPlan.Name);
            dialog.DialogCompleted += DialogRemoveFloor_DialogCompleted;

            OnOpenDialog(new OpenDialogEventArgs(dialog));
        }

        private void DialogRemoveFloor_DialogCompleted(object sender, DialogButtonClickedEventArgs e)
        {
            if (e.DialogResult == DialogResult.OK)
            {
                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                TabItem openTab = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Content is FloorPlanGrid roomPlan &&
                                        roomPlan.GetFloorPlanGuid() == vm.SelectedFloorAssignment.FloorPlan.Guid);

                if (openTab != null)
                {
                    tabControl.Items.Remove(openTab);
                }

                vm.SelectedFloorAssignment.FloorPlan.Delete();
                vm.ProjectFile.FloorPlans.Remove(vm.SelectedFloorAssignment);
                vm.ProjectFile.SaveOnlyFloors();
            }
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }

        protected virtual void OnOpenDialog(OpenDialogEventArgs e)
        {
            OpenDialog?.Invoke(this, e);
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            bool unsavedRoomPlan = tabControl.Items.OfType<TabItem>().Any(x => x.Content is RoomPlanGrid roomPlan &&
                                    (roomPlan.DataContext as RoomPlanViewModel).RoomPlan.UnsavedChanges);
            bool unsavedFloorPlan = tabControl.Items.OfType<TabItem>().Any(x => x.Content is FloorPlanGrid floorPlan &&
                                    (floorPlan.DataContext as FloorPlanViewModel).FloorPlan.UnsavedChanges);

            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;

            ChangeObserved?.Invoke(this, new ChangeObservedEventArgs(vm.ProjectFile.UnsavedChanges || unsavedRoomPlan || unsavedFloorPlan, 
                e.NewValue, e.Observer));
        }
    }
}
