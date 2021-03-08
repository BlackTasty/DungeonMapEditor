using DungeonMapEditor.Core.Dungeon.Assignment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon
{
    public class RoomPlan : BaseData
    {
        public List<TileAssignment> TileAssignments { get; set; }

        public List<PlaceableAssignment> PlaceableAssignments { get; set; }

        public List<FloorChangeTile> FloorChangeTiles { get; set; }

        /// <summary>
        /// Amount of tiles in X coordinate
        /// </summary>
        public int TilesX { get; set; }

        /// <summary>
        /// Amount of tiles in Y coordinate
        /// </summary>
        public int TilesY { get; set; }

        [JsonConstructor]
        public RoomPlan(string name, string description, double rotation, List<TileAssignment> tileAssignments,
            List<PlaceableAssignment> placeableAssignments, List<FloorChangeTile> floorChangeTiles, int tilesX, int tilesY) :
            base(name, description, rotation)
        {
            TileAssignments = tileAssignments;
            PlaceableAssignments = placeableAssignments;
            FloorChangeTiles = floorChangeTiles;
            TilesX = tilesX;
            TilesY = tilesY;
        }

        public RoomPlan(int tilesX, int tilesY)
        {
            TilesX = tilesX;
            TilesY = tilesY;

            TileAssignments = new List<TileAssignment>();
            PlaceableAssignments = new List<PlaceableAssignment>();
            FloorChangeTiles = new List<FloorChangeTile>();
            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    TileAssignments.Add(new TileAssignment(new Tile(), x, y, App.SnapValue));
                }
            }
        }

        private RoomPlan(FileInfo fi) : base(fi)
        {
            string json = LoadFile();
            RoomPlan roomPlan = JsonConvert.DeserializeObject<RoomPlan>(json);

            TileAssignments = roomPlan.TileAssignments;
            PlaceableAssignments = roomPlan.PlaceableAssignments;
            FloorChangeTiles = roomPlan.FloorChangeTiles;
            TilesX = roomPlan.TilesX;
            TilesY = roomPlan.TilesY;
        }

        public new RoomPlan FromJsonFile(FileInfo fi)
        {
            return new RoomPlan(fi);
        }
    }
}
