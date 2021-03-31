using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.Core.Enum;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class Tile : BaseData<Tile>
    {
        private BitmapImage mImage;
        private string mImagePath;
        private CollectionSet mAssignedCollection;
        private string mTileText;
        private double mTextFontSize = 16;
        private TileRotation mTileRotation = TileRotation.Degrees_0;
        private TileType mTileType = TileType.Wall;

        [JsonIgnore]
        public BitmapImage Image => mImage;

        public string ImagePath
        {
            get => mImagePath;
            set
            {
                changeManager.ObserveProperty(value);
                mImagePath = value;
                mImage = Helper.FileToBitmapImage(value);
                InvokePropertyChanged();
                InvokePropertyChanged("Image");
            }
        }

        public TileRotation TileRotation
        {
            get => mTileRotation;
            set
            {
                changeManager.ObserveProperty(value);
                mTileRotation = value;
                switch (value)
                {
                    case TileRotation.Degrees_0:
                        Rotation = 0;
                        break;
                    case TileRotation.Degrees_90:
                        Rotation = 90;
                        break;
                    case TileRotation.Degrees_180:
                        Rotation = 180;
                        break;
                    case TileRotation.Degrees_270:
                        Rotation = 270;
                        break;
                }
                InvokePropertyChanged();
            }
        }

        public TileType TileType
        {
            get => mTileType;
            set
            {
                changeManager.ObserveProperty(value);
                mTileType = value;
                InvokePropertyChanged();
            }
        }

        public string TileText
        {
            get => mTileText;
            set
            {
                changeManager.ObserveProperty(value);
                mTileText = value;
                InvokePropertyChanged();
            }
        }

        public double TextFontSize
        {
            get => mTextFontSize;
            set
            {
                changeManager.ObserveProperty(value);
                mTextFontSize = value;
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public BitmapImage ImageDesignMode
        {
            set
            {
                if (App.IsDesignMode)
                {
                    mImage = value;
                }
            }
        }

        [JsonIgnore]
        public CollectionSet AssignedCollection => mAssignedCollection;

        [JsonIgnore]
        public bool HasAssignedFile => mAssignedCollection != null;

        [JsonIgnore]
        public bool IsEmpty => string.IsNullOrWhiteSpace(mImagePath);

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public Tile(string name, string description, double rotation, string guid, string imagePath, string tileText, 
            double textFontSize, TileRotation tileRotation, TileType tileType) : 
            base(name, description, rotation, guid)
        {
            ImagePath = imagePath;
            TileText = tileText;
            TextFontSize = textFontSize;
            TileRotation = tileRotation;
            TileType = tileType;
        }

        /// <summary>
        /// Loads an existing <see cref="Tile"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the <see cref="Tile"/> file.</param>
        public Tile(FileInfo fi) : base(fi)
        {
            Load();

            changeManager.ResetObservers();
        }

        /// <summary>
        /// Used to create an empty tile.
        /// </summary>
        public Tile(bool generateGuid)
        {
            if (!generateGuid)
            {
                guid = null;
            }
        }

        public void SetAssignedCollection(CollectionSet assignedCollection)
        {
            mAssignedCollection = assignedCollection;
        }

        public void Load()
        {
            Tile tile = LoadFile();

            ImagePath = tile.ImagePath;
            TileText = tile.TileText;
            TextFontSize = tile.TextFontSize;
            TileRotation = tile.TileRotation;
            TileType = tile.TileType;
        }

        public override string GetFullPath()
        {
            return !IsEmpty ? base.GetFullPath() : null;
        }

        public Tile Duplicate(bool generateGuid)
        {
            Tile duplicate = new Tile(generateGuid)
            {
                Name = Name,
                Description = Description,
                TileRotation = TileRotation,
                TileText = TileText,
                TileType = TileType,
                ImagePath = ImagePath,
                TextFontSize = TextFontSize
            };

            if (!generateGuid)
            {
                duplicate.guid = guid;
            }

            return duplicate;
        }

        public override IBaseData Copy()
        {
            return Duplicate(false);
        }
    }
}
