using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DungeonMapEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly string collectionPath = Path.Combine(BasePath, "Collections");

        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public static double SnapValue = 50;

        public static VeryObservableCollection<CollectionSet> LoadedCollections { get; set; } = 
            new VeryObservableCollection<CollectionSet>("LoadedCollections");

        public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        public static string ProjectsPath => Path.Combine(BasePath, "Projects");


        [STAThread]
        public static void Main()
        {
            LoadCollections();

            App app = new App()
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose
            };
            app.InitializeComponent();
            app.Run();
        }

        public static void LoadCollections()
        {
            LoadedCollections.Clear();

            foreach (DirectoryInfo di in new DirectoryInfo(collectionPath).EnumerateDirectories())
            {
                var validDirs = di.EnumerateDirectories().Where(x => x.Name.StartsWith("placeables", StringComparison.CurrentCultureIgnoreCase) ||
                                                                  x.Name.StartsWith("tiles", StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (validDirs.Count > 0)
                {
                    foreach (var dir in validDirs)
                    {
                        LoadedCollections.Add(new CollectionSet(dir));

                        FileInfo fi = dir.EnumerateFiles(dir.Name + ".json", SearchOption.TopDirectoryOnly).FirstOrDefault();

                        if (fi != null)
                        {
                        }
                    }
                }

            }
        }

        public static List<Tile> GetLoadedTiles()
        {
            List<Tile> tiles = new List<Tile>();

            foreach (var collectionSet in LoadedCollections)
            {
                if (collectionSet.TileFile.HasData)
                {
                    foreach (var tile in collectionSet.TileFile.Data)
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }

        public static List<Placeable> GetLoadedPlaceables()
        {
            List<Placeable> placeables = new List<Placeable>();

            foreach (var collectionSet in LoadedCollections)
            {
                if (collectionSet.PlaceableFile.HasData)
                {
                    foreach (var placeable in collectionSet.PlaceableFile.Data)
                    {
                        placeables.Add(placeable);
                    }
                }
            }

            return placeables;
        }
    }
}
