using DungeonMapEditor.Core.Dungeon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.ViewModel
{
    class TileConfiguratorViewModel : TileControlViewModel
    {
        public TileConfiguratorViewModel()
        {
            TileAssignment = new Core.Dungeon.Assignment.TileAssignment(new Tile(false));
        }
    }
}
