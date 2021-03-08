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
    /// Interaction logic for RoomPlanGrid.xaml
    /// </summary>
    public partial class RoomPlanGrid : DockPanel
    {
        private int tagIndex = 0;
        private TileControl selectedTileControl;
        private bool updateTile = true;

        public RoomPlanGrid() : this(new RoomPlan(10, 5))
        {
        }

        public RoomPlanGrid(RoomPlan roomPlan)
        {
            InitializeComponent();
            (DataContext as RoomPlanViewModel).RoomPlan = roomPlan;
            AddTiles();
        }

        private void AddTiles()
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            var lastAssignment = vm.RoomPlan.TileAssignments.LastOrDefault();
            var brushColor = Brushes.Gray.Color;

            Border roomBorder = new Border()
            {
                Width = lastAssignment.CanvasX + 50,
                Height = lastAssignment.CanvasY + 50,
                BorderBrush = new SolidColorBrush(Color.FromArgb(128, brushColor.R, brushColor.G, brushColor.B)),
                BorderThickness = new Thickness(1),
                Background = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255))
            };

            grid.Children.Add(roomBorder);

            foreach (TileAssignment tileAssignment in vm.RoomPlan.TileAssignments)
            {
                if (tileAssignment.Control == null)
                {
                    Border tileBorder = new Border()
                    {
                        BorderBrush = new SolidColorBrush(Color.FromArgb(128, brushColor.R, brushColor.G, brushColor.B)),
                        BorderThickness = new Thickness(0),
                        Height = 50,
                        Width = 50
                    };
                    TileControl tileControl = new TileControl();
                    tileControl.MouseLeftButtonDown += TileControl_MouseLeftButtonDown;
                    tileControl.MouseLeftButtonUp += TileControl_MouseLeftButtonUp;
                    tileControl.Tag = tagIndex;
                    tagIndex++;

                    Canvas.SetLeft(tileBorder, tileAssignment.CanvasX - 2);
                    Canvas.SetTop(tileBorder, tileAssignment.CanvasY - 2);

                    tileBorder.Child = tileControl;
                    grid.Children.Add(tileBorder);
                    (tileControl.DataContext as TileControlViewModel).Tile = tileAssignment.Tile;
                    tileAssignment.SetControl(tileControl);
                }
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
            if (updateTile)
            {
                RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
                vm.SelectedTileAssignment.Tile = vm.SelectedAvailableTile;
                vm.SelectedTileAssignment.Control.Tile = vm.SelectedAvailableTile;

                selectedTileControl.Tile = vm.SelectedAvailableTile;
            }
        }
    }
}
