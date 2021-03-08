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

        public string AssignedProjectPath { get; set; }

        [JsonIgnore]
        public RoomPlan RoomPlan => roomPlan;

        public string RoomPlanFile => !string.IsNullOrWhiteSpace(roomPlan.Name) ? roomPlan.Name + ".json" : null;

        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public RoomAssignment(string assignedProjectPath, string roomPlanFile, int x, int y)
        {
            roomPlan = new RoomPlan(new FileInfo(Path.Combine(assignedProjectPath, roomPlanFile)));
            AssignedProjectPath = assignedProjectPath;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Used to create a new position assignment for a room plan.
        /// </summary>
        /// <param name="roomPlan">The room plan to assign</param>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public RoomAssignment(RoomPlan roomPlan, ProjectFile assignedProject, int x, int y) : this(roomPlan, assignedProject)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Used to create a new position assignment for a room plan. Default position: X = 0; Y = 0
        /// </summary>
        /// <param name="roomPlan">The room plan to assign</param>
        public RoomAssignment(RoomPlan roomPlan, ProjectFile assignedProject)
        {
            this.roomPlan = roomPlan;
            AssignedProjectPath = assignedProject.FilePath;
        }
    }
}
