using DungeonMapEditor.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonMapEditor.Core.Dungeon.Collection
{
    public class CollectionFile<T> : JsonFile
    {
        private VeryObservableCollection<T> data;
        private CollectionType collectionType;

        public VeryObservableCollection<T> Data => data;

        [JsonIgnore]
        public CollectionType CollectionType => collectionType;

        [JsonIgnore]
        public bool HasData => data?.Count > 0;

        public CollectionFile(CollectionType collectionType)
        {
            this.collectionType = collectionType;
            data = new VeryObservableCollection<T>("Data");
            switch (collectionType)
            {
                case CollectionType.Placeables:
                    fileName = "placeables.json";
                    break;
                case CollectionType.Tiles:
                    fileName = "tiles.json";
                    break;
            }
        }

        [JsonConstructor]
        public CollectionFile(List<T> data)
        {
            this.data = new VeryObservableCollection<T>("Data");
            T dataCheck = data.FirstOrDefault();
            if (dataCheck is Tile)
            {
                fileName = "tiles.json";
                collectionType = CollectionType.Tiles;
            }
            else if (dataCheck is Placeable)
            {
                fileName = "placeables.json";
                collectionType = CollectionType.Placeables;
            }

            this.data.Add(data);
        }

        public CollectionFile(FileInfo fi) : base(fi)
        {
            Load();
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if Collection file is being created!");
                }

                string subDirName;
                switch (collectionType)
                {
                    case CollectionType.Placeables:
                        subDirName = "placeables";
                        break;
                    case CollectionType.Tiles:
                        subDirName = "tiles";
                        break;
                    default:
                        return;
                }

                SaveFile(Path.Combine(parentPath, subDirName), JsonConvert.SerializeObject(this));
            }
            else
            {
                SaveFile(JsonConvert.SerializeObject(this));
            }
        }

        public void Load()
        {
            string json = LoadFile();
            CollectionFile<T> fileData = JsonConvert.DeserializeObject<CollectionFile<T>>(json);

            switch (fileData.FileName)
            {
                case "placeables.json":
                    collectionType = CollectionType.Placeables;
                    break;
                case "tiles.json":
                    collectionType = CollectionType.Tiles;
                    break;
                default:
                    return;
            }

            data = fileData.Data;
        }
    }
}
