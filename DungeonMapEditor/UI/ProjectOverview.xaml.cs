﻿using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
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
        private SolidColorBrush roomTabColor = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
        private SolidColorBrush floorTabColor = new SolidColorBrush(Color.FromArgb(64, 0, 128, 255));
        private SolidColorBrush documentLayoutTabColor = new SolidColorBrush(Color.FromArgb(64, 32, 255, 0));

        public event EventHandler<NameChangedEventArgs> ProjectNameChanged;
        public event EventHandler<OpenDialogEventArgs> OpenDialog;

        public ProjectOverview() : this(new ProjectFile("Untitled dungeon"))
        {
        }

        public ProjectOverview(ProjectFile projectFile)
        {
            InitializeComponent();
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
            projectFile.Load();
            vm.ProjectFile = projectFile;
            vm.ProjectNameChanged += ProjectOverview_ProjectNameChanged;
        }

        public string GetProjectName()
        {
            return (DataContext as ProjectOverviewViewModel).ProjectFile?.Name;
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
            if (e.DialogResult == Core.Dialog.DialogResult.OK)
            {
                ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;
                vm.ProjectFile.RoomPlans.Add(new RoomAssignment(e.ResultObject, vm.ProjectFile));
                OpenRoomPlan(e.ResultObject);
            }
        }

        private void FloorPlanDialog_DialogCompleted(object sender, CreateDialogCompletedEventArgs<FloorPlan> e)
        {
            if (e.DialogResult == Core.Dialog.DialogResult.OK)
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
            floorPlanGrid.FloorNameChanged += PlanGrid_NameChanged;

            tabControl.SelectedIndex = AddTab(floorPlanGrid, floorPlan.Name, floorTabColor, Guid.NewGuid().ToString());
        }

        public void OpenRoomPlan(RoomPlan roomPlan)
        {
            RoomPlanGrid roomPlanGrid = new RoomPlanGrid(roomPlan);
            roomPlanGrid.RoomNameChanged += PlanGrid_NameChanged;

            tabControl.SelectedIndex = AddTab(roomPlanGrid, roomPlan.Name, roomTabColor, Guid.NewGuid().ToString());
        }

        private void PlanGrid_NameChanged(object sender, NameChangedEventArgs e)
        {
            if (sender is FrameworkElement target)
            {
                TabItem targetTabItem = tabControl.Items.OfType<TabItem>().FirstOrDefault(x => x.Tag == target.Tag);
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

        private int AddTab(FrameworkElement element, string headerString, object tabItemTag = null)
        {
            return AddTab(element, headerString, Brushes.White, tabItemTag);
        }

        private int AddTab(FrameworkElement element, string headerString, SolidColorBrush background, object tabItemTag = null)
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
                Content = "x",
                Margin = new Thickness(8, 0, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                Padding = new Thickness(4, 0, 4, 0)
            };
            tabCloseButton.Click += TabCloseButton_Click;

            header.Children.Add(new TextBlock() { Text = headerString });
            header.Children.Add(tabCloseButton);

            TabItem tabItem = new TabItem
            {
                Header = header,
                Content = element,
                Tag = tabItemTag,
                Background = background
            };
            tabCloseButton.Tag = tabItem;

            tabControl.Items.Add(tabItem);

            return tabControl.Items.Count - 1;
        }

        protected virtual void OnProjectNameChanged(NameChangedEventArgs e)
        {
            ProjectNameChanged?.Invoke(this, e);
        }

        protected virtual void OnOpenDialog(OpenDialogEventArgs e)
        {
            OpenDialog?.Invoke(this, e);
        }

        private void TabCloseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag is TabItem targetTab)
            {
                if (targetTab.Content is FloorPlanGrid floorPlanGrid)
                {
                    floorPlanGrid.FloorNameChanged -= PlanGrid_NameChanged;
                }

                tabControl.Items.Remove(targetTab);
            }
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
            string exportDir = e.ResultObject.ExportProject();
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", exportDir));
        }

        private void ConfigureLayout_Click(object sender, RoutedEventArgs e)
        {
            ProjectOverviewViewModel vm = DataContext as ProjectOverviewViewModel;

            ProjectPlanGrid projectPlanGrid = new ProjectPlanGrid(vm.ProjectFile);

            tabControl.SelectedIndex = AddTab(projectPlanGrid, "Document layout", documentLayoutTabColor, Guid.NewGuid().ToString());
        }
    }
}
