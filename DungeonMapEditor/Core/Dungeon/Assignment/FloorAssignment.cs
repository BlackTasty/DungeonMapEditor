using DungeonMapEditor.Controls;
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
        private FloorControl control;

        public string AssignedProjectPath { get; set; }

        [JsonIgnore]
        public FloorPlan FloorPlan => floorPlan;

        [JsonIgnore]
        public FloorControl Control => control;

        public string FloorPlanFile => !string.IsNullOrWhiteSpace(floorPlan.Name) ? floorPlan.Name + ".json" : null;

        public double X { get; set; }

        public double Y { get; set; }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public FloorAssignment(string assignedProjectPath, string floorPlanFile, double x, double y)
        {
            floorPlan = new FloorPlan(new FileInfo(Path.Combine(assignedProjectPath, floorPlanFile)));
            AssignedProjectPath = assignedProjectPath;
            X = x;
            Y = y;
        }

        /// <summary>
        /// Used to create a new position assignment for a floor.
        /// </summary>
        /// <param name="floorPlan">The floor plan to assign</param>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public FloorAssignment(FloorPlan floorPlan, ProjectFile assignedProject, double x, double y) : this(floorPlan, assignedProject)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Used to create a new position assignment for a floor. Default position: X = 0; Y = 0
        /// </summary>
        /// <param name="floorPlan">The floor plan to assign</param>
        public FloorAssignment(FloorPlan floorPlan, ProjectFile assignedProject)
        {
            this.floorPlan = floorPlan;
            AssignedProjectPath = assignedProject.FilePath;
        }

        public bool SetControl(FloorControl control)
        {
            if (this.control != null)
            {
                return false;
            }

            this.control = control;
            return true;
        }
    }
}
