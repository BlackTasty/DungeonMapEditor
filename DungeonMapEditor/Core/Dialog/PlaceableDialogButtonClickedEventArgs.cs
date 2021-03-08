using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dialog
{
    public class PlaceableDialogButtonClickedEventArgs : DialogButtonClickedEventArgs
    {
        private Placeable placeable;

        public Placeable Placeable => placeable;

        public PlaceableDialogButtonClickedEventArgs(Placeable placeable, DialogResult dialogResult) : base(dialogResult)
        {
            this.placeable = placeable;
        }
    }
}
