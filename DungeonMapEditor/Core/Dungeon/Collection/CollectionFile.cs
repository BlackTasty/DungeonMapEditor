using DungeonMapEditor.Core.Observer;
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
    public class CollectionFile<T> : JsonFile<CollectionFile<T>>
    {
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        private VeryObservableCollection<T> data;
        private CollectionType collectionType;

        public VeryObservableCollection<T> Data => data;

        [JsonIgnore]
        public CollectionType CollectionType => collectionType;

        [JsonIgnore]
        public bool HasData => data?.Count > 0;

        /// <summary>
        /// Only required by JSON parser!
        /// </summary>
        [JsonConstructor]
        public CollectionFile(List<T> data)
        {
            InitializeLists();
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

        /// <summary>
        /// Used to create a new <see cref="CollectionFile{T}"/> with the specified collection type.
        /// </summary>
        /// <param name="collectionType">What type is this collection?</param>
        public CollectionFile(CollectionType collectionType)
        {
            InitializeLists();
            this.collectionType = collectionType;
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

        /// <summary>
        /// Loads an existing <see cref="CollectionFile{T}"/> from a json file.
        /// </summary>
        /// <param name="fi">A <see cref="FileInfo"/> object containing the path to the collection file</param>
        public CollectionFile(FileInfo fi) : base(fi)
        {
            InitializeLists();
            Load();
            changeManager.ResetObservers();
        }

        private void InitializeLists()
        {
            data = new VeryObservableCollection<T>("Data", changeManager);
            changeManager.ChangeObserved += ChangeManager_ChangeObserved;
        }

        private void ChangeManager_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
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

                SaveFile(Path.Combine(parentPath, subDirName), this);
            }
            else
            {
                SaveFile(JsonConvert.SerializeObject(this));
            }
        }

        public void Load()
        {
            CollectionFile<T> fileData = LoadFile();

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

            data.Add(fileData.Data);
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
