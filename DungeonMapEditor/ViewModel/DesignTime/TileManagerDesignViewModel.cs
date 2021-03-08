using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel.DesignTime
{
    class TileManagerDesignViewModel : TileManagerViewModel
    {
        public TileManagerDesignViewModel()
        {
            TileFile = new CollectionFile<Tile>(CollectionType.Tiles);
            TileFile.Data.Add(new List<Tile>()
            {
                new Tile()
                {
                    Name = "Wall Horizontal Top",
                    Description = "A horizontal wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall)
                },
                new Tile()
                {
                    Name = "Wall Vertical Right",
                    Description = "A vertical wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall),
                    Rotation = 90
                },
                new Tile()
                {
                    Name = "Wall Horizontal Bottom",
                    Description = "A horizontal wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall),
                    Rotation = 180
                },
                new Tile()
                {
                    Name = "Wall Vertical Left",
                    Description = "A vertical wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall),
                    Rotation = 270
                },
                new Tile()
                {
                    Name = "Wall Corner (Top Left)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner)
                },
                new Tile()
                {
                    Name = "Wall Corner (Top Right)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner),
                    Rotation = 90
                },
                new Tile()
                {
                    Name = "Wall Corner (Bottom Right)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner),
                    Rotation = 180
                },
                new Tile()
                {
                    Name = "Wall Corner (Bottom Left)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner),
                    Rotation = 270
                }
            });
        }
    }
}
