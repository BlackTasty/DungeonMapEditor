using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
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
    /// Interaction logic for ProjectPlanGrid.xaml
    /// </summary>
    public partial class ProjectPlanGrid : DockPanel
    {
        private FloorControl selectedFloorControl;
        private int tagIndex;
        private double planOffset = 8;
        private bool floorControlClicked;

        public ProjectPlanGrid(ProjectFile projectFile)
        {
            InitializeComponent();

            ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;
            projectFile.Load();
            vm.ProjectFile = projectFile;
            AddFloors();
        }

        public string GetProjectPlanGuid()
        {
            return (DataContext as ProjectPlanViewModel).ProjectFile?.FilePath;
        }

        private void AddFloors()
        {
            ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;

            foreach (FloorAssignment floorAssignment in vm.ProjectFile.FloorPlans)
            {
                FloorControl floorControl = new FloorControl(floorAssignment, vm.ProjectFile, planOffset);
                GenerateFloorControl(floorControl);
            }
        }

        private void RemoveFloor(FloorControl floorControl)
        {
            ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;
            vm.ProjectFile.FloorPlans.Remove(floorControl.FloorAssignment);
            grid.Children.Remove(floorControl);
            vm.SelectedFloorAssignment = null;
            selectedFloorControl = null;
        }

        private void FloorPlan_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow clickedRow && clickedRow.DataContext is FloorPlan floorPlan)
            {
                ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;

                FloorControl floorControl = new FloorControl(floorPlan, vm.ProjectFile, planOffset);
                GenerateFloorControl(floorControl);

                vm.ProjectFile.FloorPlans.Add(floorControl.FloorAssignment);
            }
        }

        private void GenerateFloorControl(FloorControl floorControl)
        {
            floorControl.Tag = tagIndex;
            tagIndex++;

            floorControl.MouseLeftButtonDown += FloorControl_MouseLeftButtonDown;

            Canvas.SetLeft(floorControl, floorControl.FloorAssignment.X);
            Canvas.SetTop(floorControl, floorControl.FloorAssignment.Y);
            grid.Children.Add(floorControl);
        }

        private void FloorControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            floorControlClicked = true;
            ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;
            if (selectedFloorControl != null)
            {
                selectedFloorControl.Background = Brushes.Transparent;
            }
            FloorControl floorControl = sender as FloorControl;

            FloorAssignment roomAssignment = vm.ProjectFile.FloorPlans.FirstOrDefault(x => x.Control.Tag == floorControl.Tag);
            vm.SelectedFloorAssignment = roomAssignment;

            floorControl.Background = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
            selectedFloorControl = floorControl;
            selectedFloorControl.Focus();
            Keyboard.Focus(selectedFloorControl);
            vm.SelectedTabIndex = 1;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;
            Size size = Helper.GetDocumentSize(vm.ProjectFile.DocumentSizeType);

            if (vm.ProjectFile.DocumentOrientation == Orientation.Vertical)
            {
                size = new Size(size.Height, size.Width);
            }*/
        }

        private void grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (floorControlClicked)
            {
                floorControlClicked = false;
                return;
            }
            if (selectedFloorControl != null)
            {
                selectedFloorControl.Background = Brushes.Transparent;
            }
            ProjectPlanViewModel vm = DataContext as ProjectPlanViewModel;
            vm.SelectedFloorAssignment = null;
            selectedFloorControl = null;
        }

        private void DockPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && selectedFloorControl != null)
            {
                RemoveFloor(selectedFloorControl);
            }
        }
    }
}
