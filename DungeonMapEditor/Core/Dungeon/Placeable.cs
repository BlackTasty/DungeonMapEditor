using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class Placeable : BaseData
    {
        private BitmapImage mImage;
        private string mImagePath;
        private double mTileRatioX = 1;
        private double mTileRatioY = 1;

        [JsonIgnore]
        public BitmapImage Image => mImage;

        public string ImagePath
        {
            get => mImagePath;
            set
            {
                mImagePath = value;
                mImage = Helper.FileToBitmapImage(value);
                InvokePropertyChanged();
                InvokePropertyChanged("Image");
            }
        }

        /// <summary>
        /// Placeable-to-Tile ratio in X coordinate.
        /// </summary>
        public double TileRatioX
        {
            get => mTileRatioX;
            set
            {
                mTileRatioX = value;
                InvokePropertyChanged();
                InvokePropertyChanged("Width");
            }
        }


        /// <summary>
        /// Placeable-to-Tile ratio in Y coordinate.
        /// </summary>
        public double TileRatioY
        {
            get => mTileRatioY;
            set
            {
                mTileRatioY = value;
                InvokePropertyChanged();
                InvokePropertyChanged("Height");
            }
        }

        [JsonIgnore]
        public double Width => TileRatioX * App.SnapValue;

        [JsonIgnore]
        public double Height => TileRatioY * App.SnapValue;

        [JsonConstructor]
        public Placeable(string name, string description, double rotation, string imagePath, double tileRatioX, double tileRatioY) :
            base(name, description, rotation)
        {
            ImagePath = imagePath;
            mTileRatioX = tileRatioX;
            mTileRatioY = tileRatioY;
        }
    }
}
