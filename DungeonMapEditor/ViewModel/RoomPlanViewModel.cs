using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.ViewModel.Communication;
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
        private PlaceableAssignment mSelectedPlaceableAssignment;
        private VeryObservableCollection<Tile> mAvailableTiles = new VeryObservableCollection<Tile>("Tiles");
        private VeryObservableCollection<Placeable> mAvailablePlaceables = new VeryObservableCollection<Placeable>("Placeables");
        private Tile mSelectedAvailableTile;
        private bool mKeepAspectRatio;
        private int mSelectedTabIndex;

        public RoomPlanViewModel()
        {
            LoadAvailableTilesAndObjects();

            Mediator.Instance.Register(o =>
            {
                LoadAvailableTilesAndObjects();
            }, ViewModelMessage.LoadedCollectionsChanged);
        }

        private void LoadAvailableTilesAndObjects()
        {
            mAvailableTiles.Clear();
            mAvailablePlaceables.Clear();

            mAvailableTiles.Add(new Tile(false));
            mAvailableTiles.Add(App.GetLoadedTiles());
            mAvailablePlaceables.Add(App.GetLoadedPlaceables());
        }

        public bool KeepAspectRatio
        {
            get => mKeepAspectRatio;
            set
            {
                mKeepAspectRatio = value;
                InvokePropertyChanged();
                if (SelectedPlaceableAssignment != null)
                {
                    SelectedPlaceableAssignment.KeepAspectRatio = value;
                }
            }
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

        public int SelectedTabIndex
        {
            get => mSelectedTabIndex;
            set
            {
                mSelectedTabIndex = value;
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
                InvokePropertyChanged("IsTileAssignmentSelected");
            }
        }

        public PlaceableAssignment SelectedPlaceableAssignment
        {
            get => mSelectedPlaceableAssignment;
            set
            {
                mSelectedPlaceableAssignment = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsPlaceableAssignmentSelected");
                if (SelectedPlaceableAssignment != null)
                {
                    SelectedPlaceableAssignment.KeepAspectRatio = mKeepAspectRatio;
                }
            }
        }

        public bool IsPlaceableAssignmentSelected => mSelectedPlaceableAssignment != null;

        public bool IsTileAssignmentSelected => mSelectedTileAssignment != null;

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
