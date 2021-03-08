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
        private CollectionFile<Tile> mTileFile;
        private int mSelectedTileIndex = -1;
        private bool mIsConfiguratorOpen;
        private bool mIsEditTile;

        public CollectionFile<Tile> TileFile
        {
            get => mTileFile;
            set
            {
                mTileFile = value;
                InvokePropertyChanged();
            }
        }

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

        public bool IsConfiguratorOpen
        {
            get => mIsConfiguratorOpen;
            set
            {
                mIsConfiguratorOpen = value;
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

        public TileManagerViewModel()
        {
            if (!App.IsDesignMode)
            {
                TileFile = new CollectionFile<Tile>(CollectionType.Tiles);

                string wallCornerPath = Path.Combine(App.BasePath, "Resources", "Demo", "Dungeon_Corner.png");
                string wallInnerCornerPath = Path.Combine(App.BasePath, "Resources", "Demo", "Dungeon_InnerCorner.png");
                string wallPath = Path.Combine(App.BasePath, "Resources", "Demo", "Dungeon_Wall.png");
                TileFile.Data.Add(new List<Tile>()
            {
                new Tile()
                {
                    Name = "Wall Horizontal Top",
                    Description = "A horizontal wall",
                    ImagePath = wallPath
                },
                new Tile()
                {
                    Name = "Wall Vertical Right",
                    Description = "A vertical wall",
                    ImagePath = wallPath,
                    Rotation = 90
                },
                new Tile()
                {
                    Name = "Wall Horizontal Bottom",
                    Description = "A horizontal wall",
                    ImagePath = wallPath,
                    Rotation = 180
                },
                new Tile()
                {
                    Name = "Wall Vertical Left",
                    Description = "A vertical wall",
                    ImagePath = wallPath,
                    Rotation = 270
                },
                new Tile()
                {
                    Name = "Wall Corner (Top Left)",
                    Description = "A corner wall",
                    ImagePath = wallCornerPath
                },
                new Tile()
                {
                    Name = "Wall Corner (Top Right)",
                    Description = "A corner wall",
                    ImagePath = wallCornerPath,
                    Rotation = 90
                },
                new Tile()
                {
                    Name = "Wall Corner (Bottom Right)",
                    Description = "A corner wall",
                    ImagePath = wallCornerPath,
                    Rotation = 180
                },
                new Tile()
                {
                    Name = "Wall Corner (Bottom Left)",
                    Description = "A corner wall",
                    ImagePath = wallCornerPath,
                    Rotation = 270
                },
                new Tile()
                {
                    Name = "Wall Inner Corner (Top Left)",
                    Description = "An inner corner wall",
                    ImagePath = wallInnerCornerPath
                },
                new Tile()
                {
                    Name = "Wall Inner Corner (Top Right)",
                    Description = "An inner corner wall",
                    ImagePath = wallInnerCornerPath,
                    Rotation = 90
                },
                new Tile()
                {
                    Name = "Wall Inner Corner (Bottom Right)",
                    Description = "An inner corner wall",
                    ImagePath = wallInnerCornerPath,
                    Rotation = 180
                },
                new Tile()
                {
                    Name = "Wall Inner Corner (Bottom Left)",
                    Description = "An inner corner wall",
                    ImagePath = wallInnerCornerPath,
                    Rotation = 270
                }
            });
            }
        }

        public Tile GetSelectedTile()
        {
            return mSelectedTileIndex > -1 ? TileFile.Data[mSelectedTileIndex] : null;
        }
    }
}
