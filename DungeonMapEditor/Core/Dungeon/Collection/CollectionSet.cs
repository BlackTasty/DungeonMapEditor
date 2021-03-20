using DungeonMapEditor.Core.Observer;
using DungeonMapEditor.ViewModel.Communication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Collection
{
    public class CollectionSet : JsonFile<CollectionSet>
    {
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private string guid;
        private string mName;
        private bool mAnyUnsavedChanges;

        private CollectionFile<Tile> tileFile;
        private CollectionFile<Placeable> placeableFile;

        public string Guid => guid;

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged();
                changeManager.ObserveProperty(value);
            }
        }

        [JsonIgnore]
        public bool AnyUnsavedChanges
        {
            get => mAnyUnsavedChanges;
            private set
            {
                mAnyUnsavedChanges = value;
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public CollectionFile<Tile> TileFile
        {
            get => tileFile;
            set
            {
                if (tileFile != null)
                {
                    tileFile.ChangeObserved -= Collection_ChangeObserved;
                }
                tileFile = value;
                tileFile.ChangeObserved += Collection_ChangeObserved;
            }
        }

        [JsonIgnore]
        public int TileCount => TileFile?.Data?.Count ?? 0;

        [JsonIgnore]
        public CollectionFile<Placeable> PlaceableFile
        {
            get => placeableFile;
            set
            {
                if (placeableFile != null)
                {
                    placeableFile.ChangeObserved -= Collection_ChangeObserved;
                }
                placeableFile = value;
                placeableFile.ChangeObserved += Collection_ChangeObserved;
            }
        }

        [JsonIgnore]
        public int PlaceableCount => PlaceableFile?.Data?.Count ?? 0;

        [JsonConstructor]
        public CollectionSet(string name, string guid)
        {
            Name = name;
            this.guid = guid;
        }

        public CollectionSet(string name)
        {
            fileName = "collection.json";
            Name = name;
            guid = System.Guid.NewGuid().ToString();
            Initialize();
        }

        public CollectionSet(DirectoryInfo di) : base(di.EnumerateFiles("collection.json").FirstOrDefault())
        {
            Initialize();
            Load();
            changeManager.ResetObservers();
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if CollectionSet file is being created!");
                }

                Directory.CreateDirectory(Path.Combine(parentPath, "tiles"));
                Directory.CreateDirectory(Path.Combine(parentPath, "placeables"));
                filePath = parentPath;
                SaveFile(parentPath, this);
            }
            else
            {
                SaveFile(JsonConvert.SerializeObject(this));
            }

            if (TileFile.HasData)
            {
                #region TODO
                //TODO: Copy images of tiles into resource folder of this collection
                /*foreach (Tile tile in TileFile.Data)
                {
                    // Copy target image to collection resource folder
                    if (!string.IsNullOrWhiteSpace(tile.ImagePath))
                    {
                        string resourceDirectory = Path.Combine(TileFile.FilePath, "Resources");

                    }
                }*/
                #endregion
                if (TileFile.FromFile)
                {
                    TileFile.Save();
                }
                else
                {
                    TileFile.Save(parentPath);
                }
            }

            if (PlaceableFile.HasData)
            {
                if (PlaceableFile.FromFile)
                {
                    PlaceableFile.Save();
                }
                else
                {
                    PlaceableFile.Save(parentPath);
                }
            }
            Mediator.Instance.NotifyColleagues(ViewModelMessage.LoadedCollectionsChanged, this);
        }

        public void Load()
        {
            CollectionSet collectionSet = LoadFile();

            Name = collectionSet.Name;
            guid = collectionSet.guid;

            foreach (DirectoryInfo di in new DirectoryInfo(Path.Combine(filePath)).EnumerateDirectories())
            {
                FileInfo jsonFile = di.EnumerateFiles(di.Name + ".json").FirstOrDefault();
                if (jsonFile == null)
                {
                    continue;
                }
                switch (jsonFile.Name)
                {
                    case "tiles.json":
                        TileFile = new CollectionFile<Tile>(jsonFile);
                        break;
                    case "placeables.json":
                        PlaceableFile = new CollectionFile<Placeable>(jsonFile);
                        break;
                }
            }
        }

        private void Initialize()
        {
            TileFile = new CollectionFile<Tile>(CollectionType.Tiles);
            PlaceableFile = new CollectionFile<Placeable>(CollectionType.Placeables);
        }

        private void Collection_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(new ChangeObservedEventArgs(tileFile.UnsavedChanges || placeableFile.UnsavedChanges ||
                UnsavedChanges, e.NewValue, e.Observer));
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            AnyUnsavedChanges = e.UnsavedChanges;
            ChangeObserved?.Invoke(this, e);
        }
    }
}
