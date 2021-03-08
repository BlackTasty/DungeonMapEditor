using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class PlaceableAssignment
    {
        private Placeable placeable;

        public Placeable Placeable => placeable;

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public PlaceableAssignment(Placeable placeable, double posX, double posY) : this(placeable)
        {
            PositionX = posX;
            PositionY = posY;
        }

        public PlaceableAssignment(Placeable placeable)
        {
            this.placeable = placeable;
        }
    }
}
