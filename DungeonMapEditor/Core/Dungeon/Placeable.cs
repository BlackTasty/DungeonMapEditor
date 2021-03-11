using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class Placeable : BaseData<Placeable>
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

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public Placeable(string name, string description, double rotation, string guid, string imagePath, 
            double tileRatioX, double tileRatioY) :
            base(name, description, rotation, guid)
        {
            ImagePath = imagePath;
            mTileRatioX = tileRatioX;
            mTileRatioY = tileRatioY;
        }

        /// <summary>
        /// Used to load a <see cref="Placeable"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the <see cref="Placeable"/> file</param>
        public Placeable(FileInfo fi) : base(fi)
        {
            Load();
        }

        public Placeable(bool generateGuid)
        {
            if (!generateGuid)
            {
                guid = null;
            }
        }

        public void Load()
        {
            Placeable placeable = LoadFile();

            ImagePath = placeable.ImagePath;
            TileRatioX = placeable.TileRatioX;
            TileRatioY = placeable.TileRatioY;
        }
    }
}
