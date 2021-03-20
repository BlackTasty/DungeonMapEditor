using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Observer;
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
    public class TileAssignment : Assignment
    {
        private Tile mTile;
        private TileControl control;

        private double canvasX;
        private double canvasY;

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || (mTile?.UnsavedChanges ?? false);

        [JsonIgnore]
        public Tile Tile
        {
            get => mTile;
            set
            {
                if (mTile != null)
                {
                    mTile.ChangeObserved -= Tile_ChangeObserved;
                }
                mTile = value;
                mTile.ChangeObserved += Tile_ChangeObserved;
                InvokePropertyChanged();
            }
        }

        private void Tile_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
        }

        [JsonIgnore]
        public TileControl Control => control;

        public string TileGuid => mTile?.Guid;

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
        public TileAssignment(string tileGuid, int x, int y, string notes) : base(notes)
        {
            if (!string.IsNullOrWhiteSpace(tileGuid))
            {
                Tile = App.GetTileByGuid(tileGuid);
            }
            else
            {
                Tile = new Tile(false);
            }
            X = x;
            Y = y;

            canvasX = X * App.SnapValue - 50;
            canvasY = Y * App.SnapValue - 50;
            changeManager.ResetObservers();
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
            Tile = tile;
        }

        public bool SetControl(TileControl control)
        {
            if (this.control != null)
            {
                return false;
            }

            this.control = control;
            control.ChangeObserved += Control_ChangeObserved;
            return true;
        }

        private void Control_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
        }

        public void UnsetControl()
        {
            control.ChangeObserved -= Control_ChangeObserved;
            this.control = null;
        }

        public override string ToString()
        {
            return "X: " + X + "; Y: " + Y;
        }

        protected override void OnChangeObserved(ChangeObservedEventArgs e)
        {
            base.OnChangeObserved(new ChangeObservedEventArgs(AnyUnsavedChanges, e.NewValue, e.Observer));
        }
    }
}
