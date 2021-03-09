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

        public string PlaceableGuid => placeable?.Guid;

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public PlaceableAssignment(string placeableGuid, double posX, double posY)
        {
            if (!string.IsNullOrWhiteSpace(placeableGuid))
            {
                placeable = App.GetPlaceableByGuid(placeableGuid);
            }
            else
            {
                placeable = new Placeable(false);
            }

            PositionX = posX;
            PositionY = posY;
        }

        /// <summary>
        /// Used to create a new position assignment for a placeable object.
        /// </summary>
        /// <param name="placeable">The placeable object to assign</param>
        /// <param name="posX">The exact X position on the grid.</param>
        /// <param name="posY">The exact Y position on the grid.</param>
        public PlaceableAssignment(Placeable placeable, double posX, double posY) : this(placeable)
        {
            PositionX = posX;
            PositionY = posY;
        }

        /// <summary>
        /// Used to create a new position assignment for a placeable object. Default position: X = 0; Y = 0
        /// </summary>
        /// <param name="placeable">The placeable object to assign</param>
        public PlaceableAssignment(Placeable placeable)
        {
            this.placeable = placeable;
        }
    }
}
