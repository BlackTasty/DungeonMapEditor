using DungeonMapEditor.Controls;
using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.Observer;
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
using System.Windows.Threading;

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

        private DispatcherTimer predictInnerRoomTimer;
        private List<TileControl> drawRoomSelectedTiles = new List<TileControl>();
        private List<TileAssignment> inRoomTiles = new List<TileAssignment>();

        private Size canvasBounds = new Size();
        private SolidColorBrush highlightColor = new SolidColorBrush(Color.FromArgb(64, 255, 128, 0));
        private SolidColorBrush drawRoomColor = new SolidColorBrush(Color.FromArgb(64, 128, 255, 0));
        private SolidColorBrush innerRoomPredictColor = new SolidColorBrush(Color.FromArgb(64, 64, 64, 64));

        private bool isClosing;

        public RoomPlanGrid(RoomPlan roomPlan)
        {
            InitializeComponent();
            predictInnerRoomTimer = new DispatcherTimer();
            predictInnerRoomTimer.Interval = TimeSpan.FromMilliseconds(500);
            isInit = false;
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            roomPlan.Load();
            vm.RoomPlan = roomPlan;
            roomPlan.ChangeManager.ChangeObserved += ChangeManager_ChangeObserved;
            vm.RoomNameChanged += Vm_RoomNameChanged;
            vm.GridSizeChanged += Vm_GridSizeChanged;
            vm.ChangeObserved += ChangeManager_ChangeObserved;
            predictInnerRoomTimer.Tick += PredictInnerRoomTimer_Tick;
            AddTilesAndPlaceables();
        }

        private void PredictInnerRoomTimer_Tick(object sender, EventArgs e)
        {
            PredictInnerRoom();
        }

        private void ChangeManager_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            if (e.Observer.PropertyName == "Name")
            {
                OnRoomNameChanged(new NameChangedEventArgs("", vm.RoomPlan.AnyUnsavedChanges ? e.NewValue + "*" : e.NewValue));
            }

            OnChangeObserved(new ChangeObservedEventArgs(vm.RoomPlan.AnyUnsavedChanges, vm.RoomPlan.Name, e.Observer));
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
            canvasBounds = new Size(vm.RoomPlan.TilesX * 50 + 2, vm.RoomPlan.TilesY * 50 + 2);
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

            // Generate tile control for every assignment without control
            foreach (TileAssignment tileAssignment in vm.RoomPlan.TileAssignments.Where(x => x.Control == null))
            {
                if (grid.Children.OfType<TileControl>().Any(t => t.TileAssignment.X == tileAssignment.X && t.TileAssignment.Y == tileAssignment.Y))
                {
                    continue;
                }

                TileControl tileControl = new TileControl(tileAssignment)
                {
                    Width = 54,
                    Height = 54,
                    Tag = tileTagIndex
                };
                tileControl.MouseLeftButtonDown += TileControl_MouseLeftButtonDown;
                tileControl.MouseRightButtonDown += TileControl_MouseRightButtonDown;
                tileControl.MouseEnter += TileControl_MouseEnter;
                tileControl.ChangeObserved += ChangeManager_ChangeObserved;

                tileTagIndex++;

                Canvas.SetLeft(tileControl, tileAssignment.CanvasX);
                Canvas.SetTop(tileControl, tileAssignment.CanvasY);

                tileAssignment.SetControl(tileControl);
                grid.Children.Add(tileControl);
            }

            // Generate placeable control for every assignment without control
            foreach (PlaceableAssignment placeableAssignment in vm.RoomPlan.PlaceableAssignments.Where(x => x.Control == null))
            {
                PlaceableControl placeableControl = new PlaceableControl(placeableAssignment, canvasBounds)
                {
                    Width = placeableAssignment.Width,
                    Height = placeableAssignment.Height,
                    Tag = placeableTagIndex,
                    IsHitTestVisible = !vm.IsRoomDrawEnabled
                };
                placeableTagIndex++;

                placeableControl.MouseLeftButtonDown += PlaceableControl_MouseLeftButtonDown;
                placeableControl.ChangeObserved += ChangeManager_ChangeObserved;

                Canvas.SetLeft(placeableControl, placeableControl.PlaceableAssignment.PositionX);
                Canvas.SetTop(placeableControl, placeableControl.PlaceableAssignment.PositionY);
                grid.Children.Add(placeableControl);
            }
        }

        private void TileControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((DataContext as RoomPlanViewModel).IsRoomDrawEnabled && sender is TileControl tileControl)
            {
                if (drawRoomSelectedTiles.Any(x => x.Tag.ToString() == tileControl.Tag.ToString()))
                {
                    tileControl.Background = Brushes.Transparent;
                    drawRoomSelectedTiles.Remove(tileControl);
                    if (drawRoomSelectedTiles.Count == 0)
                    {
                        confirmDrawRoom.IsEnabled = false;
                    }
                }
                predictInnerRoomTimer.Stop();
                predictInnerRoomTimer.Start();
            }
        }

        private void TileControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if ((DataContext as RoomPlanViewModel).IsRoomDrawEnabled && sender is TileControl tileControl)
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    if (!drawRoomSelectedTiles.Any(x => x.Tag.ToString() == tileControl.Tag.ToString()))
                    {
                        tileControl.Background = drawRoomColor;
                        drawRoomSelectedTiles.Add(tileControl);
                        confirmDrawRoom.IsEnabled = true;
                    }

                    predictInnerRoomTimer.Stop();
                    predictInnerRoomTimer.Start();
                }
                else if (Mouse.RightButton == MouseButtonState.Pressed)
                {
                    if (drawRoomSelectedTiles.Any(x => x.Tag.ToString() == tileControl.Tag.ToString()))
                    {
                        tileControl.Background = Brushes.Transparent;
                        drawRoomSelectedTiles.Remove(tileControl);
                        if (drawRoomSelectedTiles.Count == 0)
                        {
                            confirmDrawRoom.IsEnabled = false;
                        }
                    }

                    predictInnerRoomTimer.Stop();
                    predictInnerRoomTimer.Start();
                }
            }
        }

        private void TileControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;

            if (sender is TileControl tileControl)
            {
                if (!vm.IsRoomDrawEnabled)
                {
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

                    TileAssignment tileAssignment = tileControl.TileAssignment;
                    vm.SelectedTileAssignment = tileAssignment;

                    tileControl.Background = highlightColor;
                    selectedTileControl = tileControl;
                    updateTile = false;
                    updateTile = true;
                    vm.SelectedTabIndex = 1;
                    if (tileAssignment.TileGuid != null)
                    {
                        string tileGuid = tileAssignment.TileGuid;
                        vm.SelectedCollectionSet = App.LoadedCollections.FirstOrDefault(x => x.TileFile.Data.Any(y => y.Guid == tileGuid));
                        vm.SelectedAvailableTile = vm.CollectionTiles.FirstOrDefault(x => x.Guid == tileGuid);
                    }
                    else
                    {
                        vm.SelectedAvailableTile = vm.CollectionTiles.FirstOrDefault();
                    }
                }
                else
                {
                    predictInnerRoomTimer.Stop();
                    predictInnerRoomTimer.Start();
                    if (!drawRoomSelectedTiles.Any(x => x.Tag.ToString() == tileControl.Tag.ToString()))
                    {
                        tileControl.Background = drawRoomColor;
                        drawRoomSelectedTiles.Add(tileControl);
                        confirmDrawRoom.IsEnabled = true;
                    }
                }
            }
        }

        private void PredictInnerRoom()
        {
            if (isClosing)
            {
                return;
            }

            foreach (TileAssignment oldInnerRoom in inRoomTiles)
            {
                if (!drawRoomSelectedTiles.Any(x => x.TileAssignment.X == oldInnerRoom.X && x.TileAssignment.Y == oldInnerRoom.Y))
                {
                    oldInnerRoom.Control.Background = Brushes.Transparent;
                }
            }

            inRoomTiles.Clear();

            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            for (int y = 1; y <= vm.RoomPlan.TilesY; y++)
            {
                for (int x = 1; x <= vm.RoomPlan.TilesX; x++)
                {
                    if (!drawRoomSelectedTiles.Any(t => t.TileAssignment.X == x && t.TileAssignment.Y == y))
                    {
                        TileAssignment inRoomTile = vm.RoomPlan.TileAssignments.FirstOrDefault(t => t.X == x && t.Y == y);
                        bool hasNeighbours = vm.RoomPlan.TileAssignments.Any(t => t.X == x - 1 && t.Y == y) &&
                                                vm.RoomPlan.TileAssignments.Any(t => t.X == x && t.Y == y - 1) &&
                                                vm.RoomPlan.TileAssignments.Any(t => t.X == x + 1 && t.Y == y) &&
                                                vm.RoomPlan.TileAssignments.Any(t => t.X == x && t.Y == y + 1);
                        if (inRoomTile != null)
                        {
                            int wallCount = 0;
                            #region Check if enclosed by top wall
                            for (int yCheck = y - 1; yCheck >= 1; yCheck--)
                            {
                                if (drawRoomSelectedTiles.Any(t => t.TileAssignment.X == x && t.TileAssignment.Y == yCheck))
                                {
                                    wallCount++;
                                    break;
                                }
                            }
                            #endregion
                            #region Check if enclosed by bottom wall
                            for (int yCheck = y + 1; yCheck <= vm.RoomPlan.TilesY; yCheck++)
                            {
                                if (drawRoomSelectedTiles.Any(t => t.TileAssignment.X == x && t.TileAssignment.Y == yCheck))
                                {
                                    wallCount++;
                                    break;
                                }
                            }
                            #endregion
                            #region Check if enclosed by left wall
                            for (int xCheck = x - 1; xCheck >= 1; xCheck--)
                            {
                                if (drawRoomSelectedTiles.Any(t => t.TileAssignment.X == xCheck && t.TileAssignment.Y == y))
                                {
                                    wallCount++;
                                    break;
                                }
                            }
                            #endregion
                            #region Check if enclosed by right wall
                            for (int xCheck = x + 1; xCheck <= vm.RoomPlan.TilesX; xCheck++)
                            {
                                if (drawRoomSelectedTiles.Any(t => t.TileAssignment.X == xCheck && t.TileAssignment.Y == y))
                                {
                                    wallCount++;
                                    break;
                                }
                            }
                            #endregion

                            if (wallCount >= 4)
                            {
                                inRoomTiles.Add(inRoomTile);
                                inRoomTile.Control.Background = innerRoomPredictColor;
                            }
                        }
                    }
                }
            }

            noRoomError.Visibility = inRoomTiles.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
            predictInnerRoomTimer.Stop();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isInit || selectedTileControl == null)
            {
                return;
            }

            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            if (vm.SelectedAvailableTile != null)
            {
                if (updateTile)
                {
                    vm.SelectedTileAssignment.Tile = vm.SelectedAvailableTile;
                    //vm.SelectedTileAssignment.Control.TileAssignment.Tile = vm.SelectedAvailableTile;

                    selectedTileControl.TileAssignment.Tile = vm.SelectedAvailableTile;
                }
                tilePreview.TileAssignment.Tile = vm.SelectedAvailableTile;
            }
        }

        private void Placeables_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow clickedRow && clickedRow.DataContext is Placeable placeable)
            {
                RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
                PlaceableControl placeableControl = new PlaceableControl(placeable, canvasBounds)
                {
                    Width = placeable.Width,
                    Height = placeable.Height,
                    Tag = placeableTagIndex,
                    IsHitTestVisible = !vm.IsRoomDrawEnabled
                };
                placeableTagIndex++;

                placeableControl.MouseLeftButtonDown += PlaceableControl_MouseLeftButtonDown;

                Canvas.SetLeft(placeableControl, placeableControl.PlaceableAssignment.PositionX);
                Canvas.SetTop(placeableControl, placeableControl.PlaceableAssignment.PositionY);

                grid.Children.Add(placeableControl);

                vm.RoomPlan.PlaceableAssignments.Add(placeableControl.PlaceableAssignment);
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

            placeableControl.Background = highlightColor;
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

        private void RoomDraw_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            bool roomDrawEnabled = vm.IsRoomDrawEnabled;
            
            foreach (PlaceableAssignment assignment in vm.RoomPlan.PlaceableAssignments)
            {
                assignment.Control.IsHitTestVisible = !roomDrawEnabled;
            }

            if (roomDrawEnabled)
            {
                if (selectedPlaceableControl != null)
                {
                    selectedPlaceableControl.Background = Brushes.Transparent;
                    selectedPlaceableControl = null;
                    vm.SelectedPlaceableAssignment = null;
                }
                if (selectedTileControl != null)
                {
                    updateTile = false;
                    selectedTileControl.Background = Brushes.Transparent;
                    selectedTileControl = null;
                    vm.SelectedTileAssignment = null;
                    updateTile = true;
                }
            }
            else
            {
                foreach (TileControl tileControl in drawRoomSelectedTiles)
                {
                    tileControl.Background = Brushes.Transparent;
                }
                drawRoomSelectedTiles.Clear();
                foreach (TileAssignment tileassignment in inRoomTiles)
                {
                    tileassignment.Control.Background = Brushes.Transparent;
                }
                inRoomTiles.Clear();
                confirmDrawRoom.IsEnabled = false;
                noRoomError.Visibility = Visibility.Visible;
            }
        }

        private void ResetDrawRoom_Click(object sender, RoutedEventArgs e)
        {
            foreach (TileControl tileControl in drawRoomSelectedTiles)
            {
                tileControl.Background = Brushes.Transparent;
            }
            drawRoomSelectedTiles.Clear();
            foreach (TileAssignment tileassignment in inRoomTiles)
            {
                tileassignment.Control.Background = Brushes.Transparent;
            }
            inRoomTiles.Clear();
            confirmDrawRoom.IsEnabled = false;
            noRoomError.Visibility = Visibility.Visible;
        }

        private void ConfirmDrawRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;

            foreach (TileControl selectedTile in drawRoomSelectedTiles)
            {
                TileControl leftTile = drawRoomSelectedTiles.FirstOrDefault(x => x.TileAssignment.X == selectedTile.TileAssignment.X - 1 && x.TileAssignment.Y == selectedTile.TileAssignment.Y);
                TileControl topTile = drawRoomSelectedTiles.FirstOrDefault(x => x.TileAssignment.X == selectedTile.TileAssignment.X && x.TileAssignment.Y == selectedTile.TileAssignment.Y - 1);
                TileControl rightTile = drawRoomSelectedTiles.FirstOrDefault(x => x.TileAssignment.X == selectedTile.TileAssignment.X + 1 && x.TileAssignment.Y == selectedTile.TileAssignment.Y);
                TileControl bottomTile = drawRoomSelectedTiles.FirstOrDefault(x => x.TileAssignment.X == selectedTile.TileAssignment.X && x.TileAssignment.Y == selectedTile.TileAssignment.Y + 1);

                TileAssignment leftInRoom = inRoomTiles.FirstOrDefault(x => x.X == selectedTile.TileAssignment.X - 1 && x.Y == selectedTile.TileAssignment.Y);
                TileAssignment topInRoom = inRoomTiles.FirstOrDefault(x => x.X == selectedTile.TileAssignment.X && x.Y == selectedTile.TileAssignment.Y - 1);
                TileAssignment rightInRoom = inRoomTiles.FirstOrDefault(x => x.X == selectedTile.TileAssignment.X + 1 && x.Y == selectedTile.TileAssignment.Y);
                TileAssignment bottomInRoom= inRoomTiles.FirstOrDefault(x => x.X == selectedTile.TileAssignment.X && x.Y == selectedTile.TileAssignment.Y + 1);
                if ((topTile != null || bottomTile != null) && leftInRoom != null && rightInRoom != null) // Vertical center wall
                {
                    selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                        .FirstOrDefault(x => x.TileType == TileType.Wall_Centered && (x.TileRotation == TileRotation.Degrees_90 | 
                        x.TileRotation == TileRotation.Degrees_270));
                }
                if ((leftTile != null || rightTile != null) && topInRoom != null && bottomInRoom != null) // Horizontal center wall
                {
                    selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                        .FirstOrDefault(x => x.TileType == TileType.Wall_Centered && (x.TileRotation == TileRotation.Degrees_0 |
                        x.TileRotation == TileRotation.Degrees_180));
                }
                if (leftTile != null && rightTile != null && (topTile == null && bottomTile == null || 
                    topTile != null && bottomTile == null || topTile == null && bottomTile != null)) // Horizonzal wall
                {
                    TileRotation rotation = inRoomTiles.Any(t => t.Y == selectedTile.TileAssignment.Y + 1) ? 
                        TileRotation.Degrees_0 : TileRotation.Degrees_180;

                    selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                        .FirstOrDefault(x => x.TileType == TileType.Wall && x.TileRotation == rotation);
                }
                else if (topTile != null && bottomTile != null && (leftTile == null && rightTile == null ||
                    leftTile != null && rightTile == null || leftTile == null && rightTile != null)) // Vertical wall
                {
                    TileRotation rotation = inRoomTiles.Any(t => t.X == selectedTile.TileAssignment.X + 1) ?
                        TileRotation.Degrees_270 : TileRotation.Degrees_90;
                    selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                        .FirstOrDefault(x => x.TileType == TileType.Wall && x.TileRotation == rotation);
                }
                else if (leftTile != null && bottomTile != null && rightTile == null && topTile == null) // Top right corner
                {
                    if (inRoomTiles.Any(t => t.X == selectedTile.TileAssignment.X + 1) && 
                        inRoomTiles.Any(t => t.Y == selectedTile.TileAssignment.Y - 1)) // Corner is inner corner
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner_Inner && x.TileRotation == TileRotation.Degrees_270);
                    }
                    else
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner && x.TileRotation == TileRotation.Degrees_90);
                    }
                }
                else if (rightTile != null && bottomTile != null && leftTile == null && topTile == null) // Top left corner
                {
                    if (inRoomTiles.Any(t => t.X == selectedTile.TileAssignment.X - 1) &&
                        inRoomTiles.Any(t => t.Y == selectedTile.TileAssignment.Y - 1)) // Corner is inner corner
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner_Inner && x.TileRotation == TileRotation.Degrees_180);
                    }
                    else
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner && x.TileRotation == TileRotation.Degrees_0);
                    }
                }
                else if (leftTile != null && topTile != null && rightTile == null && bottomTile == null) // Bottom right corner
                {
                    if (inRoomTiles.Any(t => t.X == selectedTile.TileAssignment.X + 1) &&
                        inRoomTiles.Any(t => t.Y == selectedTile.TileAssignment.Y + 1)) // Corner is inner corner
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner_Inner && x.TileRotation == TileRotation.Degrees_0);
                    }
                    else
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner && x.TileRotation == TileRotation.Degrees_180);
                    }
                }
                else if (rightTile != null && topTile != null && leftTile == null && bottomTile == null) // Bottom left corner
                {
                    if (inRoomTiles.Any(t => t.X == selectedTile.TileAssignment.X - 1) &&
                        inRoomTiles.Any(t => t.Y == selectedTile.TileAssignment.Y + 1)) // Corner is inner corner
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner_Inner && x.TileRotation == TileRotation.Degrees_90);
                    }
                    else
                    {
                        selectedTile.TileAssignment.Tile = vm.SelectedCollectionSet.TileFile.Data
                            .FirstOrDefault(x => x.TileType == TileType.Corner && x.TileRotation == TileRotation.Degrees_270);
                    }
                }
            }

            vm.IsRoomDrawEnabled = false;
        }

        public void UnloadGrid()
        {
            isClosing = true;
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;

            foreach (TileAssignment tileAssignment in vm.RoomPlan.TileAssignments.Where(x => x.Control != null))
            {
                tileAssignment.Control.MouseLeftButtonDown -= TileControl_MouseLeftButtonDown;
                tileAssignment.Control.MouseRightButtonDown -= TileControl_MouseRightButtonDown;
                tileAssignment.Control.MouseEnter -= TileControl_MouseEnter;
                tileAssignment.Control.ChangeObserved -= ChangeManager_ChangeObserved;
                grid.Children.Remove(tileAssignment.Control);
                tileAssignment.UnsetControl();
            }

            foreach (PlaceableAssignment placeableAssignment in vm.RoomPlan.PlaceableAssignments.Where(x => x.Control != null))
            {
                placeableAssignment.Control.MouseLeftButtonDown -= PlaceableControl_MouseLeftButtonDown;
                placeableAssignment.Control.ChangeObserved -= ChangeManager_ChangeObserved;
                grid.Children.Remove(placeableAssignment.Control);
                placeableAssignment.UnsetControl();
            }

            vm.RoomPlan.ChangeManager.ChangeObserved -= ChangeManager_ChangeObserved;
            vm.RoomNameChanged -= Vm_RoomNameChanged;
            vm.GridSizeChanged -= Vm_GridSizeChanged;
            vm.ChangeObserved -= ChangeManager_ChangeObserved;
            predictInnerRoomTimer.Tick -= PredictInnerRoomTimer_Tick;
        }

        private void DockPanel_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        private void ClearGrid_Click(object sender, RoutedEventArgs e)
        {
            RoomPlanViewModel vm = DataContext as RoomPlanViewModel;
            foreach (TileAssignment tileAssignment in vm.RoomPlan.TileAssignments)
            {
                tileAssignment.Tile = new Tile(false);
            }
        }
    }
}
