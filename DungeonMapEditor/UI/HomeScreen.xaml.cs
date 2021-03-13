using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for HomeScreen.xaml
    /// </summary>
    public partial class HomeScreen : DockPanel
    {
        public event EventHandler<HomeScreenSelectionMadeEventArgs> SelectionMade;

        public HomeScreen()
        {
            InitializeComponent();
        }

        private void CreateMap_Click(object sender, RoutedEventArgs e)
        {
            CreateMap();
        }

        public void CreateMap()
        {
            OnSelectionMade(new HomeScreenSelectionMadeEventArgs(HomeScreenSelectionType.NewProject));
        }

        private void LoadMapFromFile_Click(object sender, RoutedEventArgs e)
        {
            LoadMapFromFile();
        }

        public void LoadMapFromFile()
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Open a project",
                Filter = "Dungeon map file (*.dm)|*.dm"
            };

            if (dialog.ShowDialog() == true)
            {
                OnSelectionMade(new HomeScreenSelectionMadeEventArgs(new ProjectFile(new FileInfo(dialog.FileName).Directory)));
            }
        }

        private void ManageTiles_Click(object sender, RoutedEventArgs e)
        {
            OnSelectionMade(new HomeScreenSelectionMadeEventArgs(HomeScreenSelectionType.OpenTileManager));
        }

        protected virtual void OnSelectionMade(HomeScreenSelectionMadeEventArgs e)
        {
            SelectionMade?.Invoke(this, e);
        }

        private void ProjectHistoryItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListBoxItem).DataContext is ProjectFile projectFile)
            {
                OnSelectionMade(new HomeScreenSelectionMadeEventArgs(projectFile));
            }
        }
    }
}
