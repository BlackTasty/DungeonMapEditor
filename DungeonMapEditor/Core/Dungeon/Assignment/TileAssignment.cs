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

        public string TileFilePath => mTile?.GetFullPath();

        [JsonIgnore]
        public TileControl Control => control;

        public int X { get; set; }

        public int Y { get; set; }

        [JsonIgnore]
        public double CanvasX => canvasX;

        [JsonIgnore]
        public double CanvasY => canvasY;

        [JsonConstructor]
        public TileAssignment(string tileFilePath, int x, int y) : this(new Tile(new FileInfo(tileFilePath)), x, y)
        {
        }


        public TileAssignment(Tile tile, int x, int y) : this(tile)
        {
            X = x;
            Y = y;
        }

        public TileAssignment(Tile tile)
        {
            mTile = tile;
            canvasX = X * App.SnapValue;
            canvasY = Y * App.SnapValue;
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
