using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
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
    /// Interaction logic for RoomPlanGrid.xaml
    /// </summary>
    public partial class RoomPlanGrid : DockPanel
    {
        public event EventHandler<NameChangedEventArgs> RoomNameChanged;
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private int tileTagIndex = 0;
        private int placeableTagIndex = 0;
        private TileControl selectedTileControl;
        private PlaceableControl selectedPlaceableControl;
        private bool updateTile = true;
        private bool isInit = true;

        private Size canvasBounds = new Size();

        public RoomPlanGrid(RoomPlan roomPlan)
        {
            InitializeComponent();
            isInit = false;
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            roomPlan.Load();
            vm.RoomPlan = roomPlan;
            roomPlan.ChangeManager.ChangeObserved += ChangeManager_ChangeObserved;
            vm.RoomNameChanged += Vm_RoomNameChanged;
            vm.GridSizeChanged += Vm_GridSizeChanged;
            AddTilesAndPlaceables();
        }

        private void ChangeManager_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            if (e.Observer.PropertyName == "Name")
            {
                OnRoomNameChanged(new NameChangedEventArgs("", e.UnsavedChanges ? e.NewValue + "*" : e.NewValue));
            }

            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            OnChangeObserved(new ChangeObservedEventArgs(vm.RoomPlan.UnsavedChanges, vm.RoomPlan.Name, e.Observer));
        }

        public string GetRoomPlanGuid()
        {
            return (DataContext as RoomPlanViewModel).RoomPlan?.Guid;
        }

        private void Vm_GridSizeChanged(object sender, EventArgs e)
        {
            Border roomBorder = grid.Children.OfType<Border>().FirstOrDefault();
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;

            roomBorder.Width = vm.RoomPlan.TilesX * 50;
            roomBorder.Height = vm.RoomPlan.TilesY * 50;
            AddTilesAndPlaceables(false);
        }

        private void Vm_RoomNameChanged(object sender, NameChangedEventArgs e)
        {
            OnRoomNameChanged(e);
        }

        private void AddTilesAndPlaceables(bool addRoomBorder = true)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            canvasBounds = new Size(vm.RoomPlan.TilesX * 50, vm.RoomPlan.TilesY * 50);
            grid.Width = canvasBounds.Width;
            grid.Height = canvasBounds.Height;

            if (addRoomBorder)
            {
                var lastAssignment = vm.RoomPlan.TileAssignments.LastOrDefault();
                var brushColor = Brushes.Gray.Color;

                Border roomBorder = new Border()
                {
                    Name = "roomBorder",
                    Width = canvasBounds.Width,
                    Height = canvasBounds.Height,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0)),
                    BorderThickness = new Thickness(2)
                };

                grid.Children.Add(roomBorder);
            }

            foreach (TileAssignment tileAssignment in vm.RoomPlan.TileAssignments.Where(x => x.Control == null))
            {
                TileControl tileControl = new TileControl(tileAssignment)
                {
                    Width = 54,
                    Height = 54,
                    Tag = tileTagIndex
                };
                tileControl.MouseLeftButtonDown += TileControl_MouseLeftButtonDown;
                tileTagIndex++;

                Canvas.SetLeft(tileControl, tileAssignment.CanvasX - 4);
                Canvas.SetTop(tileControl, tileAssignment.CanvasY - 4);

                tileAssignment.SetControl(tileControl);
                grid.Children.Add(tileControl);
                Console.WriteLine("TileControl generated for position: X={0}; Y={1}", tileAssignment.X, tileAssignment.Y);
            }

            foreach (PlaceableAssignment placeableAssignment in vm.RoomPlan.PlaceableAssignments.Where(x => x.Control == null))
            {
                PlaceableControl placeableControl = new PlaceableControl(placeableAssignment, canvasBounds)
                {
                    Width = placeableAssignment.Width,
                    Height = placeableAssignment.Height,
                    Tag = placeableTagIndex
                };
                placeableTagIndex++;

                placeableControl.MouseLeftButtonDown += PlaceableControl_MouseLeftButtonDown;

                Canvas.SetLeft(placeableControl, placeableControl.PlaceableAssignment.PositionX);
                Canvas.SetTop(placeableControl, placeableControl.PlaceableAssignment.PositionY);

                grid.Children.Add(placeableControl);
            }
        }

        private void TileControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            if (selectedPlaceableControl != null)
            {
                selectedPlaceableControl.Background = Brushes.Transparent;
                selectedPlaceableControl = null;
                vm.SelectedPlaceableAssignment = null;
            }
            if (selectedTileControl != null)
            {
                selectedTileControl.Background = Brushes.Transparent;
            }
            TileControl tileControl = sender as TileControl;

            TileAssignment tileAssignment = tileControl.TileAssignment;
            vm.SelectedTileAssignment = tileAssignment;

            tileControl.Background = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
            selectedTileControl = tileControl;
            updateTile = false;
            vm.SelectedAvailableTile = null;
            updateTile = true;
            vm.SelectedTabIndex = 1;
            if (tileAssignment.TileGuid != null)
            {
                vm.SelectedAvailableTile = tileAssignment.Tile;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInit || selectedTileControl == null)
            {
                return;
            }

            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            if (updateTile)
            {
                vm.SelectedTileAssignment.Tile = vm.SelectedAvailableTile;
                //vm.SelectedTileAssignment.Control.TileAssignment.Tile = vm.SelectedAvailableTile;

                selectedTileControl.TileAssignment.Tile = vm.SelectedAvailableTile;
            }
            tilePreview.TileAssignment.Tile = vm.SelectedAvailableTile;
        }

        private void Placeables_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow clickedRow && clickedRow.DataContext is Placeable placeable)
            {
                PlaceableControl placeableControl = new PlaceableControl(placeable, canvasBounds)
                {
                    Width = placeable.Width,
                    Height = placeable.Height,
                    Tag = placeableTagIndex
                };
                placeableTagIndex++;

                placeableControl.MouseLeftButtonDown += PlaceableControl_MouseLeftButtonDown;

                Canvas.SetLeft(placeableControl, placeableControl.PlaceableAssignment.PositionX);
                Canvas.SetTop(placeableControl, placeableControl.PlaceableAssignment.PositionY);

                grid.Children.Add(placeableControl);

                (DataContext as RoomPlanViewModel).RoomPlan.PlaceableAssignments.Add(placeableControl.PlaceableAssignment);
            }

        }

        private void RemovePlaceable(PlaceableControl placeableControl)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            vm.RoomPlan.PlaceableAssignments.Remove(placeableControl.PlaceableAssignment);
            grid.Children.Remove(placeableControl);
            vm.SelectedPlaceableAssignment = null;
            selectedPlaceableControl = null;
        }

        private void PlaceableControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            if (selectedTileControl != null)
            {
                updateTile = false;
                selectedTileControl.Background = Brushes.Transparent;
                selectedTileControl = null;
                vm.SelectedTileAssignment = null;
                updateTile = true;
            }
            if (selectedPlaceableControl != null)
            {
                selectedPlaceableControl.Background = Brushes.Transparent;
            }
            PlaceableControl placeableControl = sender as PlaceableControl;

            PlaceableAssignment placeableAssignment = vm.RoomPlan.PlaceableAssignments.FirstOrDefault(x => x.Control.Tag == placeableControl.Tag);
            vm.SelectedPlaceableAssignment = placeableAssignment;

            placeableControl.Background = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
            selectedPlaceableControl = placeableControl;
            updateTile = false;
            vm.SelectedAvailableTile = null;
            updateTile = true;
            vm.SelectedTabIndex = 2;
        }

        private void DockPanel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && selectedPlaceableControl != null)
            {
                RemovePlaceable(selectedPlaceableControl);
            }
        }

        private void DockPanel_Loaded(object sender, RoutedEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;

            if (!vm.RoomPlan.FromFile)
            {
                OnChangeObserved(new ChangeObservedEventArgs(vm.RoomPlan.UnsavedChanges, vm.RoomPlan.Name,
                    vm.RoomPlan.ChangeManager.GetObserverByName("Name")));
            }
        }

        protected virtual void OnRoomNameChanged(NameChangedEventArgs e)
        {
            RoomNameChanged?.Invoke(this, e);
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
