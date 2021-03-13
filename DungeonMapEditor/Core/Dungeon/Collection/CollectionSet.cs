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
        private string guid;
        private string mName;

        public string Guid => guid;

        public string Name
        {
            get => mName;
            set
            {
                mName = value;
                InvokePropertyChanged();
            }
        }

        [JsonIgnore]
        public CollectionFile<Tile> TileFile { get; set; }

        [JsonIgnore]
        public int TileCount => TileFile?.Data?.Count ?? 0;

        [JsonIgnore]
        public CollectionFile<Placeable> PlaceableFile { get; set; }

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
        }

        private void Initialize()
        {
            TileFile = new CollectionFile<Tile>(CollectionType.Tiles);
            PlaceableFile = new CollectionFile<Placeable>(CollectionType.Placeables);
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
    }
}
