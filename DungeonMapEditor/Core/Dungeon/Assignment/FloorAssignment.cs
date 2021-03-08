using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class FloorAssignment
    {
        private FloorPlan floorPlan;

        public FloorPlan FloorPlan => floorPlan;

        public double PositionX { get; set; }

        public double PositionY { get; set; }

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
