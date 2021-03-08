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

        public string RoomPlanFile => !string.IsNullOrWhiteSpace(roomPlan.Name) ? roomPlan.Name + ".json" : null;

        public int X { get; set; }

        public int Y { get; set; }

        [JsonConstructor]
        public RoomAssignment(string roomPlanFile, int x, int y)
        {
            if (!string.IsNullOrWhiteSpace(roomPlanFile))
            {
                roomPlan = new RoomPlan(new FileInfo(roomPlanFile));
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
