using DungeonMapEditor.Core.Dungeon.Collection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DungeonMapEditor.Core.Dungeon
{
    public class Tile : BaseData
    {
        private BitmapImage mImage;
        private string mImagePath;
        private CollectionSet mAssignedCollection;

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

        [JsonConstructor]
        public Tile(string name, string description, double rotation, string imagePath) : base(name, description, rotation)
        {
            ImagePath = imagePath;
        }

        public Tile() { }

        public void SetAssignedCollection(CollectionSet assignedCollection)
        {
            mAssignedCollection = assignedCollection;
        }
    }
}
