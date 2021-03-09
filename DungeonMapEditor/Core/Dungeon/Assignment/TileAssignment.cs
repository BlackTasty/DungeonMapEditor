using DungeonMapEditor.Controls;
using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class TileAssignment : ViewModelBase
    {
        private Tile mTile;
        private TileControl control;

        private double canvasX;
        private double canvasY;

        [JsonIgnore]
        public Tile Tile
        {
            get => mTile;
            set
            {
                mTile = value;
                InvokePropertyChanged();
            }
        }

        public string TileGuid => mTile?.Guid;

        [JsonIgnore]
        public TileControl Control => control;

        public int X { get; set; }

        public int Y { get; set; }

        [JsonIgnore]
        public double CanvasX => canvasX;

        [JsonIgnore]
        public double CanvasY => canvasY;

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public TileAssignment(string tileGuid, int x, int y)
        {
            if (!string.IsNullOrWhiteSpace(tileGuid))
            {
                mTile = App.GetTileByGuid(tileGuid);
            }
            else
            {
                mTile = new Tile(false);
            }
            X = x;
            Y = y;

            canvasX = X * App.SnapValue - 50;
            canvasY = Y * App.SnapValue - 50;
        }

        /// <summary>
        /// Used to create a new position assignment for a tile.
        /// </summary>
        /// <param name="tile">The tile to assign</param>
        /// <param name="x">The X position</param>
        /// <param name="y">The Y position</param>
        public TileAssignment(Tile tile, int x, int y) : this(tile)
        {
            X = x;
            Y = y;

            canvasX = X * App.SnapValue - 50;
            canvasY = Y * App.SnapValue - 50;
        }

        /// <summary>
        /// Used to create a new position assignment for a tile. Default position: X = 0; Y = 0
        /// </summary>
        /// <param name="tile">The tile to assign</param>
        public TileAssignment(Tile tile)
        {
            mTile = tile;
        }

        public bool SetControl(TileControl control)
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
