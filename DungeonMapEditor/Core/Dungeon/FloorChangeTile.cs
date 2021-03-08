using DungeonMapEditor.Core.Dungeon.Assignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public class FloorChangeTile
    {
        public TileAssignment TileAssignment { get; set; }

        public FloorPlan TargetFloor { get; set; }
    }
}
