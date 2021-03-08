using DungeonMapEditor.Core.Enum;
using DungeonMapEditor.Core.Events;
using System;
using System.Collections.Generic;
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
            OnSelectionMade(new HomeScreenSelectionMadeEventArgs(HomeScreenSelectionType.NewProject));
        }

        private void LoadMapFromFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManageTiles_Click(object sender, RoutedEventArgs e)
        {
            OnSelectionMade(new HomeScreenSelectionMadeEventArgs(HomeScreenSelectionType.OpenTileManager));
        }

        protected virtual void OnSelectionMade(HomeScreenSelectionMadeEventArgs e)
        {
            SelectionMade?.Invoke(this, e);
        }
    }
}
