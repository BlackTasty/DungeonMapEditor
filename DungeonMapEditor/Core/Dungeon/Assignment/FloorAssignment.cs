using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class FloorAssignment
    {
        private FloorPlan floorPlan;

        private ProjectFile assignedProject;

        public string AssignedProjectPath => assignedProject.FilePath;

        [JsonIgnore]
        public FloorPlan FloorPlan => floorPlan;

        public string FloorPlanFile => !string.IsNullOrWhiteSpace(floorPlan.Name) ? floorPlan.Name + ".json" : null;

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        [JsonConstructor]
        public FloorAssignment(string assignedProjectPath, string floorPlanFile, double positionX, double positionY)
        {
            floorPlan = new FloorPlan(new FileInfo(Path.Combine(assignedProjectPath, floorPlanFile)));
            PositionX = positionX;
            PositionY = positionY;
        }

        public FloorAssignment(FloorPlan floorPlan, double positionX, double positionY) : this(floorPlan)
        {
            PositionX = positionX;
            PositionY = positionY;
        }

        public FloorAssignment(FloorPlan floorPlan)
        {
            this.floorPlan = floorPlan;
        }
    }
}
