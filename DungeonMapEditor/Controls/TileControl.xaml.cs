using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.ViewModel;
using DungeonMapEditor.ViewModel.Communication;
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

namespace DungeonMapEditor.Controls
{
    /// <summary>
    /// Interaction logic for MapTile.xaml
    /// </summary>
    public partial class TileControl : Border
    {
        public TileControl()
        {
            InitializeComponent();
        }

        public Tile Tile
        {
            get => (DataContext as TileControlViewModel).Tile;
            set
            {
                (DataContext as TileControlViewModel).Tile = value;
            }
        }
    }
}
