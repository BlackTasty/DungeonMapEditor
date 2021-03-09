using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class RoomPlanViewModel : ViewModelBase
    {
        public event EventHandler<NameChangedEventArgs> RoomNameChanged;
        public event EventHandler<EventArgs> GridSizeChanged;

        private RoomPlan mRoomPlan;
        private TileAssignment mSelectedTileAssignment;
        private VeryObservableCollection<Tile> mAvailableTiles = new VeryObservableCollection<Tile>("Tiles");
        private VeryObservableCollection<Placeable> mAvailablePlaceables = new VeryObservableCollection<Placeable>("Placeables");
        private Tile mSelectedAvailableTile;

        public RoomPlanViewModel()
        {
            mAvailableTiles.Add(new Tile(false));
            mAvailableTiles.Add(App.GetLoadedTiles());
            mAvailablePlaceables.Add(App.GetLoadedPlaceables());
        }

        public RoomPlan RoomPlan
        {
            get => mRoomPlan;
            set
            {
                if (mRoomPlan != null)
                {
                    mRoomPlan.NameChanged -= RoomPlan_NameChanged;
                    mRoomPlan.GridSizeChanged -= RoomPlan_GridSizeChanged;
                }

                mRoomPlan = value;
                mRoomPlan.NameChanged += RoomPlan_NameChanged;
                mRoomPlan.GridSizeChanged += RoomPlan_GridSizeChanged;
                InvokePropertyChanged();
            }
        }

        private void RoomPlan_GridSizeChanged(object sender, EventArgs e)
        {
            OnGridSizeChanged(e);
        }

        public TileAssignment SelectedTileAssignment
        {
            get => mSelectedTileAssignment;
            set
            {
                mSelectedTileAssignment = value;
                InvokePropertyChanged();
            }
        }

        public VeryObservableCollection<Tile> AvailableTiles
        {
            get => mAvailableTiles;
            set
            {
                mAvailableTiles = value;
                InvokePropertyChanged();
            }
        }

        public VeryObservableCollection<Placeable> AvailablePlaceables
        {
            get => mAvailablePlaceables;
            set
            {
                mAvailablePlaceables = value;
                InvokePropertyChanged();
            }
        }

        public Tile SelectedAvailableTile
        {
            get => mSelectedAvailableTile;
            set
            {
                mSelectedAvailableTile = value;
                InvokePropertyChanged();
            }
        }

        public void UpdateTileOnSelectedTile(Tile newTile)
        {
            TileAssignment target = RoomPlan.TileAssignments.FirstOrDefault(x => x.X == SelectedTileAssignment.X && 
                                                                                 x.Y == SelectedTileAssignment.Y);

        }

        private void RoomPlan_NameChanged(object sender, NameChangedEventArgs e)
        {
            OnRoomNameChanged(e);
        }

        protected virtual void OnRoomNameChanged(NameChangedEventArgs e)
        {
            RoomNameChanged?.Invoke(this, e);
        }

        protected virtual void OnGridSizeChanged(EventArgs e)
        {
            GridSizeChanged?.Invoke(this, e);
        }
    }
}
