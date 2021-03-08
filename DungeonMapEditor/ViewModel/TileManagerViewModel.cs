using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.ViewModel
{
    class TileManagerViewModel : ViewModelBase
    {
        private CollectionSet mSelectedCollection;
        private int mSelectedTileIndex = -1;
        private int mSelectedPlaceableIndex = -1;
        private bool mIsTileConfiguratorOpen;
        private bool mIsPlaceableConfiguratorOpen;
        private bool mIsEditTile;
        private bool mIsEditPlaceable;

        public CollectionSet SelectedCollection
        {
            get => mSelectedCollection;
            set
            {
                mSelectedCollection = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsCollectionSelected");
                SelectedTileIndex = -1;
            }
        }

        public bool IsCollectionSelected => SelectedCollection != null;

        public VeryObservableCollection<CollectionSet> LoadedCollections => App.LoadedCollections;

        public int SelectedTileIndex
        {
            get => mSelectedTileIndex;
            set
            {
                mSelectedTileIndex = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsTileSelected");
            }
        }

        public bool IsTileSelected => mSelectedTileIndex > -1;

        public int SelectedPlaceableIndex
        {
            get => mSelectedPlaceableIndex;
            set
            {
                mSelectedPlaceableIndex = value;
                InvokePropertyChanged();
                InvokePropertyChanged("IsPlaceableSelected");
            }
        }

        public bool IsPlaceableSelected => mSelectedPlaceableIndex > -1;

        public bool IsTileConfiguratorOpen
        {
            get => mIsTileConfiguratorOpen;
            set
            {
                mIsTileConfiguratorOpen = value;
                InvokePropertyChanged();
            }
        }

        public bool IsPlaceableConfiguratorOpen
        {
            get => mIsPlaceableConfiguratorOpen;
            set
            {
                mIsPlaceableConfiguratorOpen = value;
                InvokePropertyChanged();
            }
        }

        public bool IsEditTile
        {
            get => mIsEditTile;
            set
            {
                mIsEditTile = value;
                InvokePropertyChanged();
            }
        }

        public bool IsEditPlaceable
        {
            get => mIsEditPlaceable;
            set
            {
                mIsEditPlaceable = value;
                InvokePropertyChanged();
            }
        }

        public TileManagerViewModel()
        {
        }

        public Tile GetSelectedTile()
        {
            return mSelectedTileIndex > -1 ? SelectedCollection.TileFile.Data[mSelectedTileIndex] : null;
        }

        public Placeable GetSelectedPlaceable()
        {
            return mSelectedPlaceableIndex > -1 ? SelectedCollection.PlaceableFile.Data[mSelectedPlaceableIndex] : null;
        }
    }
}
