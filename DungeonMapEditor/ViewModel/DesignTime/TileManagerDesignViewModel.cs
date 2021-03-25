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
            SelectedCollection.TileFile.Data.Add(new List<Tile>());
        }
    }
}
