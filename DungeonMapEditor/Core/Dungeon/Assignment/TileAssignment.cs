using DungeonMapEditor.Controls;
using DungeonMapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Tile Tile
        {
            get => mTile;
            set
            {
                mTile = value;
                InvokePropertyChanged();
            }
        }

        public TileControl Control => control;

        public int X { get; set; }

        public int Y { get; set; }

        public double CanvasX => canvasX;

        public double CanvasY => canvasY;

        public TileAssignment(Tile tile, int x, int y, double snapValue) : this(tile)
        {
            X = x;
            Y = y;

            canvasX = x * snapValue;
            canvasY = y * snapValue;
        }

        public TileAssignment(Tile tile)
        {
            this.mTile = tile;
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
