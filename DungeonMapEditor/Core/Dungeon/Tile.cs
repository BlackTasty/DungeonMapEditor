using DungeonMapEditor.Core.Dungeon.Collection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

        public string TileText
        {
            get => mTileText;
            set
            {
                mTileText = value;
                InvokePropertyChanged();
            }
        }

        public double TextFontSize
        {
            get => mTextFontSize;
            set
            {
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
        public Tile(string name, string description, double rotation, string guid, string imagePath, string tileText, double textFontSize) : 
            base(name, description, rotation, guid)
        {
            ImagePath = imagePath;
            TileText = tileText;
            TextFontSize = textFontSize;
        }

        /// <summary>
        /// Loads an existing <see cref="Tile"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the <see cref="Tile"/> file.</param>
        public Tile(FileInfo fi) : base(fi)
        {
            Load();
        }

        /// <summary>
        /// Used to create an empty tile.
        /// </summary>
        public Tile() { }

        public void SetAssignedCollection(CollectionSet assignedCollection)
        {
            mAssignedCollection = assignedCollection;
        }

        public void Load()
        {
            Tile tile = LoadFile();

            ImagePath = tile.ImagePath;
        }

        public override string GetFullPath()
        {
            return !IsEmpty ? base.GetFullPath() : null;
        }
    }
}
