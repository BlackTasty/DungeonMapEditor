using DungeonMapEditor.Controls;
using DungeonMapEditor.Core.Observer;
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
        private Placeable mPlaceable;
        private PlaceableControl control;

        private double mRotationOverride;
        private double mTileRatioXOverride;
        private double mTileRatioYOverride;

        private double positionX;
        private double positionY;

        [JsonIgnore]
        public bool AnyUnsavedChanges => UnsavedChanges || (mPlaceable?.UnsavedChanges ?? false);

        [JsonIgnore]
        public bool KeepAspectRatio { get; set; }

        [JsonIgnore]
        public Placeable Placeable => mPlaceable;

        [JsonIgnore]
        public PlaceableControl Control => control;

        public string PlaceableGuid => mPlaceable?.Guid;

        public double PositionX
        { 
            get => positionX;
            set
            {
                changeManager.ObserveProperty(value);
                positionX = value;
            }
        }

        public double PositionY
        {
            get => positionY;
            set
            {
                changeManager.ObserveProperty(value);
                positionY = value;
            }
        }

        public int ZIndex { get; set; } = 1;

        public double RotationOverride
        {
            get => Math.Round(mRotationOverride, 0);
            set
            {
                changeManager.ObserveProperty(value);
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
                    changeManager.ObserveProperty(mTileRatioYOverride, "TileRatioYOverride");
                    if (control != null)
                    {
                        control.Height = Height;
                    }
                    InvokePropertyChanged("TileRatioYOverride");
                }

                changeManager.ObserveProperty(value);
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
                    changeManager.ObserveProperty(mTileRatioXOverride, "TileRatioXOverride");
                    if (control != null)
                    {
                        control.Width = Width;
                    }
                    InvokePropertyChanged("TileRatioXOverride");
                }

                changeManager.ObserveProperty(value);
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
        public double Width => mTileRatioXOverride > 0 ? App.Settings.GridScaling * mTileRatioXOverride : mPlaceable.Width;

        [JsonIgnore]
        public double Height => mTileRatioYOverride > 0 ? App.Settings.GridScaling * mTileRatioYOverride : mPlaceable.Height;

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public PlaceableAssignment(string placeableGuid, double posX, double posY, 
            double rotationOverride, double tileRatioXOverride, double tileRatioYOverride, string notes) : base(notes)
        {
            if (!string.IsNullOrWhiteSpace(placeableGuid))
            {
                mPlaceable = App.GetPlaceableByGuid(placeableGuid);
            }
            else
            {
                mPlaceable = new Placeable(false);
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
            this.mPlaceable = placeable;
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

        public void UnsetControl()
        {
            this.control = null;
        }

        protected override void OnChangeObserved(ChangeObservedEventArgs e)
        {
            base.OnChangeObserved(new ChangeObservedEventArgs(AnyUnsavedChanges, e.NewValue, e.Observer));
        }
    }
}
