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
            SelectedCollection = new CollectionSet("Test");
            SelectedCollection.TileFile = new CollectionFile<Tile>(CollectionType.Tiles);
            SelectedCollection.TileFile.Data.Add(new List<Tile>()
            {
                new Tile(false)
                {
                    Name = "Wall Horizontal Top",
                    Description = "A horizontal wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall)
                },
                new Tile(false)
                {
                    Name = "Wall Vertical Right",
                    Description = "A vertical wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall),
                    Rotation = 90
                },
                new Tile(false)
                {
                    Name = "Wall Horizontal Bottom",
                    Description = "A horizontal wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall),
                    Rotation = 180
                },
                new Tile(false)
                {
                    Name = "Wall Vertical Left",
                    Description = "A vertical wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Wall),
                    Rotation = 270
                },
                new Tile(false)
                {
                    Name = "Wall Corner (Top Left)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner)
                },
                new Tile(false)
                {
                    Name = "Wall Corner (Top Right)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner),
                    Rotation = 90
                },
                new Tile(false)
                {
                    Name = "Wall Corner (Bottom Right)",
                    Description = "A corner wall",
                    ImageDesignMode = Helper.BitmapToBitmapImage(Properties.Resources.Dungeon_Corner),
                    Rotation = 180
                },
                new Tile(false)
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
