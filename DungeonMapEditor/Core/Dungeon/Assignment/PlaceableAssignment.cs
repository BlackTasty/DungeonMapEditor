using DungeonMapEditor.Controls;
using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonMapEditor.Core.Dungeon.Assignment
{
    public class PlaceableAssignment : Assignment
    {
        private Placeable placeable;
        private PlaceableControl control;

        private double mRotationOverride;
        private double mTileRatioXOverride;
        private double mTileRatioYOverride;

        [JsonIgnore]
        public bool KeepAspectRatio { get; set; }

        [JsonIgnore]
        public Placeable Placeable => placeable;

        [JsonIgnore]
        public PlaceableControl Control => control;

        public string PlaceableGuid => placeable?.Guid;

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public int ZIndex { get; set; } = 1;

        public double RotationOverride
        {
            get => Math.Round(mRotationOverride, 0);
            set
            {
                mRotationOverride = value;
                InvokePropertyChanged();
                InvokePropertyChanged("RealRotation");
            }
        }

        public double TileRatioXOverride
        {
            get => mTileRatioXOverride;
            set
            {
                if (KeepAspectRatio)
                {
                    Size current = new Size(mTileRatioXOverride, mTileRatioYOverride);
                    Size desired = new Size(value, mTileRatioYOverride);
                    mTileRatioYOverride = Helper.ChangeSize_KeepAspectRatio(current, desired).Height;
                    if (control != null)
                    {
                        control.Height = Height;
                    }
                    InvokePropertyChanged("TileRatioYOverride");
                }

                mTileRatioXOverride = value;
                if (control != null)
                {
                    control.Width = Width;
                }
                InvokePropertyChanged();
            }
        }

        public double TileRatioYOverride
        {
            get => mTileRatioYOverride;
            set
            {
                if (KeepAspectRatio)
                {
                    Size current = new Size(mTileRatioXOverride, mTileRatioYOverride);
                    Size desired = new Size(mTileRatioXOverride, value);
                    mTileRatioXOverride = Helper.ChangeSize_KeepAspectRatio(current, desired).Width;
                    if (control != null)
                    {
                        control.Width = Width;
                    }
                    InvokePropertyChanged("TileRatioXOverride");
                }

                mTileRatioYOverride = value;
                if (control != null)
                {
                    control.Height = Height;
                }
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public double RealRotation => Placeable.Rotation + RotationOverride;

        [JsonIgnore]
        public double Width => mTileRatioXOverride > 0 ? App.SnapValue * mTileRatioXOverride : placeable.Width;

        [JsonIgnore]
        public double Height => mTileRatioYOverride > 0 ? App.SnapValue * mTileRatioYOverride : placeable.Height;

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public PlaceableAssignment(string placeableGuid, double posX, double posY, 
            double rotationOverride, double tileRatioXOverride, double tileRatioYOverride, string notes) : base(notes)
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

            RotationOverride = rotationOverride;
            TileRatioXOverride = tileRatioXOverride;
            TileRatioYOverride = tileRatioYOverride;
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

        public bool SetControl(PlaceableControl control)
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
