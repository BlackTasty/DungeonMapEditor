using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
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

        private int tagIndex;
        private ProjectFile projectFile;
        private RoomControl selectedRoomControl;

        public FloorPlanGrid(FloorPlan floorPlan, ProjectFile projectFile)
        {
            InitializeComponent();
            floorPlan.FloorNameChanged += FloorPlan_FloorNameChanged;
            floorPlan.NameChanged += FloorPlan_FloorNameChanged;
            this.projectFile = projectFile;

            FloorPlanViewModel vm = DataContext as FloorPlanViewModel;
            vm.ProjectFile = projectFile;
            floorPlan.Load();
            vm.FloorPlan = floorPlan;
            AddRooms();
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

                Canvas.SetLeft(roomControl, roomAssignment.X - 2);
                Canvas.SetTop(roomControl, roomAssignment.Y - 2);

                grid.Children.Add(roomControl);
            }
        }

        private void FloorPlan_FloorNameChanged(object sender, NameChangedEventArgs e)
        {
            OnFloorNameChanged(e);
        }

        protected virtual void OnFloorNameChanged(NameChangedEventArgs e)
        {
            FloorNameChanged?.Invoke(this, e);
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
    }
}
