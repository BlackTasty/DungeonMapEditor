using DungeonMapEditor.Core.Dungeon.Assignment;
using Newtonsoft.Json;
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

        [JsonIgnore]
        public FloorPlan TargetFloor { get; set; }

        public string TargetFloorFile => !string.IsNullOrWhiteSpace(TargetFloor?.Name) ? TargetFloor.Name + ".json" : null;
    }
}
