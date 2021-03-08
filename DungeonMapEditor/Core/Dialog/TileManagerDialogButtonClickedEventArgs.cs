using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dialog
{
    public class TileManagerDialogButtonClickedEventArgs  : DialogButtonClickedEventArgs
    {
        private CollectionFile<Tile> tileFile;

        public CollectionFile<Tile> TileFile => tileFile;

        public TileManagerDialogButtonClickedEventArgs(CollectionFile<Tile> tileFile, DialogResult dialogResult) : base(dialogResult)
        {
            this.tileFile = tileFile;
        }
    }
}
