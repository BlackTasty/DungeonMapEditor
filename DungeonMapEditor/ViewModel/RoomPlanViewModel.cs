using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.Dungeon.Collection;
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
        private Tile mSelectedAvailableTile;
        private bool mKeepAspectRatio;
        private int mSelectedTabIndex;
        private CollectionSet mSelectedCollection;

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

        public VeryObservableCollection<CollectionSet> LoadedCollections => App.LoadedCollections;

        public CollectionSet SelectedCollectionSet
        {
            get => mSelectedCollection;
            set
            {
                mSelectedCollection = value;
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
    }
}
