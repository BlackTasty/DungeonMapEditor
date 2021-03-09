using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.ViewModel;
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
    /// Interaction logic for PlaceableControl.xaml
    /// </summary>
    public partial class PlaceableControl : Grid
    {
        public PlaceableControl()
        {
            InitializeComponent();
        }

        public Placeable Placeable
        {
            get => (DataContext as PlaceableControlViewModel).Placeable;
            set
            {
                (DataContext as PlaceableControlViewModel).Placeable = value;
            }
        }
    }
}
