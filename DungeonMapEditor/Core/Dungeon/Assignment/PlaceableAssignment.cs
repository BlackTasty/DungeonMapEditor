using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class PlaceableAssignment
    {
        private Placeable placeable;

        [JsonIgnore]
        public Placeable Placeable => placeable;

        public string PlaceableFilePath => placeable?.GetFullPath();

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        [JsonConstructor]
        public PlaceableAssignment(string placeableFilePath, double posX, double posY) : 
            this(new Placeable(new FileInfo(placeableFilePath)), posX, posY)
        {
        }

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
