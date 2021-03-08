using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dialog
{
    public class TileDialogButtonClickedEventArgs : DialogButtonClickedEventArgs
    {
        private Tile tile;

        public Tile Tile => tile;

        public TileDialogButtonClickedEventArgs(Tile tile, DialogResult dialogResult) : base(dialogResult)
        {
            this.tile = tile;
        }
    }
}
