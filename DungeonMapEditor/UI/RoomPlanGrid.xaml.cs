using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
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
    /// Interaction logic for RoomPlanGrid.xaml
    /// </summary>
    public partial class RoomPlanGrid : DockPanel
    {
        public event EventHandler<NameChangedEventArgs> RoomNameChanged;

        private int tagIndex = 0;
        private TileControl selectedTileControl;
        private bool updateTile = true;

        public RoomPlanGrid(RoomPlan roomPlan)
        {
            InitializeComponent();
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            vm.RoomPlan = roomPlan;
            vm.RoomNameChanged += Vm_RoomNameChanged;
            vm.GridSizeChanged += Vm_GridSizeChanged;
            AddTiles();
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
            AddTiles(false);
        }

        private void Vm_RoomNameChanged(object sender, NameChangedEventArgs e)
        {
            OnRoomNameChanged(e);
        }

        private void AddTiles(bool addRoomBorder = true)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;

            if (addRoomBorder)
            {
                var lastAssignment = vm.RoomPlan.TileAssignments.LastOrDefault();
                var brushColor = Brushes.Gray.Color;

                Border roomBorder = new Border()
                {
                    Name = "roomBorder",
                    Width = lastAssignment.CanvasX + 50,
                    Height = lastAssignment.CanvasY + 50,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(128, brushColor.R, brushColor.G, brushColor.B)),
                    BorderThickness = new Thickness(1),
                    Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))
                };

                grid.Children.Add(roomBorder);
            }

            foreach (TileAssignment tileAssignment in vm.RoomPlan.TileAssignments.Where(x => x.Control == null))
            {
                TileControl tileControl = new TileControl()
                {
                    Width = 54,
                    Height = 54,
                    Tag = tagIndex
                };
                tileControl.MouseLeftButtonDown += TileControl_MouseLeftButtonDown;
                tileControl.MouseLeftButtonUp += TileControl_MouseLeftButtonUp;
                tagIndex++;

                Canvas.SetLeft(tileControl, tileAssignment.CanvasX - 4);
                Canvas.SetTop(tileControl, tileAssignment.CanvasY - 4);

                grid.Children.Add(tileControl);
                (tileControl.DataContext as TileControlViewModel).Tile = tileAssignment.Tile;
                tileAssignment.SetControl(tileControl);
            }
        }

        private void TileControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }

        private void TileControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedTileControl != null)
            {
                selectedTileControl.Background = Brushes.Transparent;
            }
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            TileControl tileControl = sender as TileControl;

            TileAssignment tileAssignment = vm.RoomPlan.TileAssignments.FirstOrDefault(x => x.Control.Tag == tileControl.Tag);
            (DataContext as RoomPlanViewModel).SelectedTileAssignment = tileAssignment;

            tileControl.Background = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
            selectedTileControl = tileControl;
            updateTile = false;
            vm.SelectedAvailableTile = null;
            updateTile = true;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            if (updateTile)
            {
                vm.SelectedTileAssignment.Tile = vm.SelectedAvailableTile;
                vm.SelectedTileAssignment.Control.Tile = vm.SelectedAvailableTile;

                selectedTileControl.Tile = vm.SelectedAvailableTile;
            }
            tilePreview.Tile = vm.SelectedAvailableTile;
        }

        protected virtual void OnRoomNameChanged(NameChangedEventArgs e)
        {
            RoomNameChanged?.Invoke(this, e);
        }

        private void Placeables_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaceableControl placeableControl = new PlaceableControl();
        }
    }
}
