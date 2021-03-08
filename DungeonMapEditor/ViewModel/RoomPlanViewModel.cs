using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class RoomPlanViewModel : ViewModelBase
    {
        private RoomPlan mRoomPlan;
        private TileAssignment mSelectedTileAssignment;
        private VeryObservableCollection<Tile> mAvailableTiles = new VeryObservableCollection<Tile>("Tiles");
        private Tile mSelectedAvailableTile;

        public RoomPlanViewModel()
        {
            mAvailableTiles.Add(App.GetLoadedTiles());
        }

        public RoomPlan RoomPlan
        {
            get => mRoomPlan;
            set
            {
                mRoomPlan = value;
                InvokePropertyChanged();
            }
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
    }
}
