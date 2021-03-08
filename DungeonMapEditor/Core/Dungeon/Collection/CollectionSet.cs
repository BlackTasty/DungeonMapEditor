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
        public string Name { get; set; }

        [JsonIgnore]
        public CollectionFile<Tile> TileFile { get; set; }

        [JsonIgnore]
        public int TileCount => TileFile?.Data?.Count ?? 0;

        [JsonIgnore]
        public CollectionFile<Placeable> PlaceableFile { get; set; }

        [JsonIgnore]
        public int PlaceableCount => PlaceableFile?.Data?.Count ?? 0;

        public CollectionSet(string name)
        {
            fileName = name;
            Initialize();
        }

        public CollectionSet(FileInfo fi) : base(fi)
        {
            Initialize();
            Load();
        }

        public CollectionSet(DirectoryInfo di) : base(di)
        {
            Initialize();
            Load();
        }

        private void Initialize()
        {
            TileFile = new CollectionFile<Tile>(CollectionType.Tiles);
            PlaceableFile = new CollectionFile<Placeable>(CollectionType.Placeables);
            Name = fileName;
        }

        public void Save(string parentPath = null)
        {
            if (!fromFile)
            {
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    throw new Exception("ParentPath needs to have a value if CollectionSet file is being created!");
                }

                SaveFile(Path.Combine(parentPath, Name), this);
            }
            else
            {
                //SaveFile(JsonConvert.SerializeObject(this));
            }

            if (TileFile.HasData)
            {
                //TODO: Copy images of tiles into resource folder of this collection
                /*foreach (Tile tile in TileFile.Data)
                {
                    // Copy target image to collection resource folder
                    if (!string.IsNullOrWhiteSpace(tile.ImagePath))
                    {
                        string resourceDirectory = Path.Combine(TileFile.FilePath, "Resources");

                    }
                }*/
                TileFile.Save();
            }

            if (PlaceableFile.HasData)
            {
                PlaceableFile.Save();
            }
        }

        public void Load()
        {
            foreach (DirectoryInfo di in new DirectoryInfo(Path.Combine(filePath, Name)).EnumerateDirectories())
            {
                FileInfo jsonFile = di.EnumerateFiles(di.Name + ".json").FirstOrDefault();
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
