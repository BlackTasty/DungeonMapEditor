using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class RoomAssignment
    {
        private RoomPlan roomPlan;

        [JsonIgnore]
        public RoomPlan RoomPlan => roomPlan;

        public string RoomPlanPath => roomPlan?.GetFullPath();

        public int X { get; set; }

        public int Y { get; set; }

        [JsonConstructor]
        public RoomAssignment(string roomPlanPath, int x, int y)
        {
            if (!string.IsNullOrWhiteSpace(roomPlanPath))
            {
                roomPlan = RoomPlan.FromJsonFile(new FileInfo(roomPlanPath));
            }
            X = x;
            Y = y;
        }

        public RoomAssignment(RoomPlan roomPlan)
        {
            this.roomPlan = roomPlan;
        }
    }
}
