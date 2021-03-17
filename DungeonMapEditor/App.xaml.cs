using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Collection;
using DungeonMapEditor.ViewModel;
using DungeonMapEditor.ViewModel.Communication;
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
        private static VeryObservableStackCollection<ProjectFile> mProjectHistory = new VeryObservableStackCollection<ProjectFile>("ProjectHistory", 7);

        public static string CollectionPath => Path.Combine(BasePath, "Collections");

        public static bool IsDesignMode => DesignerProperties.GetIsInDesignMode(new DependencyObject());

        public static double SnapValue = 50;

        public static VeryObservableCollection<CollectionSet> LoadedCollections { get; set; } = 
            new VeryObservableCollection<CollectionSet>("LoadedCollections", null, ViewModelMessage.LoadedCollectionsChanged);

        public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        public static string ProjectsPath => Path.Combine(BasePath, "Projects");

        public static VeryObservableStackCollection<ProjectFile> ProjectHistory => mProjectHistory;

        public static bool IsHistoryEmpty => ProjectHistory.Count == 0;

        [STAThread]
        public static void Main()
        {
            ProjectHistory.TriggerAlso("IsHistoryEmpty");

            LoadCollections();
            LoadHistory();

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
            if (!Directory.Exists(CollectionPath))
            {
                Directory.CreateDirectory(CollectionPath);
            }

            foreach (DirectoryInfo di in new DirectoryInfo(CollectionPath).EnumerateDirectories())
            {
                var validDirs = di.EnumerateDirectories().Where(x => x.Name.StartsWith("placeables", StringComparison.CurrentCultureIgnoreCase) ||
                                                                  x.Name.StartsWith("tiles", StringComparison.CurrentCultureIgnoreCase)).ToList();

                if (validDirs.Count > 0)
                {
                    LoadedCollections.Add(new CollectionSet(di));
                }
            }
        }

        public static void LoadHistory()
        {
            ProjectHistory.Clear();

            if (!Directory.Exists(ProjectsPath))
            {
                return;
            }

            List<ProjectFile> history = new List<ProjectFile>();
            foreach (DirectoryInfo di in new DirectoryInfo(ProjectsPath).EnumerateDirectories())
            {
                if (di.EnumerateFiles(di.Name + ".dm").Count() > 0)
                {
                    history.Add(new ProjectFile(di));
                }
            }

            ProjectHistory.Add(history.OrderBy(x => x.LastModifyDate).Reverse().Take(ProjectHistory.Limit));
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

        public static Tile GetTileByGuid(string guid)
        {
            Tile target = GetLoadedTiles().FirstOrDefault(x => x.Guid == guid);
            return target != null ? target : new Tile(false);
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

        public static Placeable GetPlaceableByGuid(string guid)
        {
            Placeable target = GetLoadedPlaceables().FirstOrDefault(x => x.Guid == guid);
            return target != null ? target : new Placeable(false);
        }
    }
}
