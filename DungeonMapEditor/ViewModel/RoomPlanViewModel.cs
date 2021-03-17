using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Events;
using DungeonMapEditor.Core.FileSystem;
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
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private RoomPlan mRoomPlan;
        private TileAssignment mSelectedTileAssignment;
        private PlaceableAssignment mSelectedPlaceableAssignment;
        private Tile mSelectedAvailableTile;
        private bool mKeepAspectRatio;
        private int mSelectedTabIndex;
        private CollectionSet mSelectedCollection;
        private VeryObservableCollection<Tile> mCollectionTiles = new VeryObservableCollection<Tile>("CollectionTiles");
        private bool mIsRoomDrawEnabled;

        public RoomPlanViewModel()
        {
            LoadAvailableTilesAndObjects();
            mSelectedCollection = LoadedCollections.FirstOrDefault();

            Mediator.Instance.Register(o =>
            {
                LoadAvailableTilesAndObjects();
            }, ViewModelMessage.LoadedCollectionsChanged);
        }

        private void LoadAvailableTilesAndObjects()
        {
            SelectedCollectionSet = SelectedCollectionSet;
        }

        public bool IsRoomDrawEnabled
        {
            get => mIsRoomDrawEnabled;
            set
            {
                mIsRoomDrawEnabled = value;
                InvokePropertyChanged();
                if (value)
                {
                    SelectedAvailableTile = null;
                    SelectedPlaceableAssignment = null;
                }
            }
        }

        public VeryObservableCollection<Tile> CollectionTiles
        {
            get => mCollectionTiles;
            private set
            {
                mCollectionTiles.Clear();
                mCollectionTiles.Add(new Tile(false));
                if (value != null && value.Count > 0)
                {
                    mCollectionTiles.Add(value);
                }
                InvokePropertyChanged();
            }
        }

        public VeryObservableCollection<CollectionSet> LoadedCollections => App.LoadedCollections;

        public CollectionSet SelectedCollectionSet
        {
            get => mSelectedCollection;
            set
            {
                mSelectedCollection = value;
                CollectionTiles = value?.TileFile?.Data;
                InvokePropertyChanged();
            }
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
                    mRoomPlan.ChangeObserved -= RoomPlan_ChangeObserved;
                }

                mRoomPlan = value;
                mRoomPlan.NameChanged += RoomPlan_NameChanged;
                mRoomPlan.GridSizeChanged += RoomPlan_GridSizeChanged;
                mRoomPlan.ChangeObserved += RoomPlan_ChangeObserved;
                InvokePropertyChanged();
            }
        }

        private void RoomPlan_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
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
                else
                {
                    SelectedTabIndex = 0;
                }
            }
        }

        public bool IsPlaceableAssignmentSelected => mSelectedPlaceableAssignment != null;

        public bool IsTileAssignmentSelected => mSelectedTileAssignment != null;

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

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
