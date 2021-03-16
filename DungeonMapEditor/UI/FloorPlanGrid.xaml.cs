using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.FileSystem;
using DungeonMapEditor.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for FloorPlanGrid.xaml
    /// </summary>
    public partial class FloorPlanGrid : DockPanel
    {
        public event EventHandler<NameChangedEventArgs> FloorNameChanged;
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private int tagIndex;
        private ProjectFile projectFile;
        private RoomControl selectedRoomControl;
        private bool roomControlClicked;

        public FloorPlanGrid(FloorPlan floorPlan, ProjectFile projectFile)
        {
            InitializeComponent();
            floorPlan.FloorNameChanged += FloorPlan_FloorNameChanged;
            floorPlan.NameChanged += FloorPlan_FloorNameChanged;
            floorPlan.ChangeManager.ChangeObserved += ChangeManager_ChangeObserved;
            this.projectFile = projectFile;

            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;
            vm.ProjectFile = projectFile;
            floorPlan.Load();
            vm.FloorPlan = floorPlan;
            AddRooms();
        }

        private void ChangeManager_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            if (e.Observer.PropertyName == "Name")
            {
                OnFloorNameChanged(new NameChangedEventArgs("", e.UnsavedChanges ? e.NewValue + "*" : e.NewValue));
            }
            else
            {
                FloorPlanViewModel vm = DataContext as FloorPlanViewModel;
                OnChangeObserved(new ChangeObservedEventArgs(vm.FloorPlan?.UnsavedChanges ?? false, vm.FloorPlan?.Name, e.Observer));
            }
        }

        private void AddRooms()
        {
            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;

            foreach (RoomAssignment roomAssignment in vm.FloorPlan.RoomAssignments)
            {
                RoomControl roomControl = new RoomControl(roomAssignment)
                {
                    Width = roomAssignment.RoomPlan.TilesX * 25,
                    Height = roomAssignment.RoomPlan.TilesY * 25,
                    Tag = tagIndex
                };
                tagIndex++;

                roomControl.MouseLeftButtonDown += RoomControl_MouseLeftButtonDown;

                Canvas.SetLeft(roomControl, roomAssignment.X);
                Canvas.SetTop(roomControl, roomAssignment.Y);

                grid.Children.Add(roomControl);
            }
        }

        private void FloorPlan_FloorNameChanged(object sender, NameChangedEventArgs e)
        {
            OnFloorNameChanged(e);
        }

        public string GetFloorPlanGuid()
        {
            return (DataContext as FloorPlanViewModel).FloorPlan?.Guid;
        }

        private void RoomPlan_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow clickedRow && clickedRow.DataContext is RoomPlan roomPlan)
            {
                RoomControl roomControl = new RoomControl(roomPlan, projectFile)
                {
                    Width = roomPlan.TilesX * 25,
                    Height = roomPlan.TilesY * 25,
                    Tag = tagIndex
                };
                tagIndex++;

                roomControl.MouseLeftButtonDown += RoomControl_MouseLeftButtonDown;

                Canvas.SetLeft(roomControl, roomControl.RoomAssignment.X - 2);
                Canvas.SetTop(roomControl, roomControl.RoomAssignment.Y - 2);

                grid.Children.Add(roomControl);

                (DataContext as FloorPlanViewModel).FloorPlan.RoomAssignments.Add(roomControl.RoomAssignment);
            }
        }

        private void RoomControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            roomControlClicked = true;
            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;
            if (selectedRoomControl != null)
            {
                selectedRoomControl.Background = Brushes.Transparent;
            }
            RoomControl roomControl = sender as RoomControl;

            RoomAssignment roomAssignment = vm.FloorPlan.RoomAssignments.FirstOrDefault(x => x.Control.Tag == roomControl.Tag);
            vm.SelectedRoomAssignment = roomAssignment;

            roomControl.Background = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
            selectedRoomControl = roomControl;
            vm.SelectedTabIndex = 1;
        }

        private void grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (roomControlClicked)
            {
                roomControlClicked = false;
                return;
            }
            if (selectedRoomControl != null)
            {
                selectedRoomControl.Background = Brushes.Transparent;
            }
            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;
            vm.SelectedRoomAssignment = null;
            selectedRoomControl = null;
            vm.SelectedTabIndex = 0;
        }

        private void DockPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && selectedRoomControl != null)
            {
                RemoveRoom(selectedRoomControl);
            }
        }

        private void RemoveRoom(RoomControl roomControl)
        {
            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;
            vm.FloorPlan.RoomAssignments.Remove(roomControl.RoomAssignment);
            grid.Children.Remove(roomControl);
            vm.SelectedRoomAssignment = null;
            selectedRoomControl = null;
        }

        private void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;

            if (!vm.FloorPlan.FromFile)
            {
                OnChangeObserved(new ChangeObservedEventArgs(vm.FloorPlan.UnsavedChanges, vm.FloorPlan.Name,
                    vm.FloorPlan.ChangeManager.GetObserverByName("Name")));
            }
        }

        protected virtual void OnFloorNameChanged(NameChangedEventArgs e)
        {
            FloorNameChanged?.Invoke(this, e);
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
