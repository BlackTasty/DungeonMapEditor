using DungeonMapEditor.Core;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.Core.Dungeon.Assignment;
using DungeonMapEditor.Core.FileSystem;
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
        public event EventHandler<ChangeObservedEventArgs> ChangeObserved;

        public TileControl() : this(new TileAssignment(new Tile(false)))
        {

        }

        public TileControl(TileAssignment tileAssignment, bool showNoteIcon  = true)
        {
            InitializeComponent();
            TileAssignment = tileAssignment;
            if (!showNoteIcon)
            {
                noteIcon.Source = null;
            }
        }

        private void TileAssignment_ChangeObserved(object sender, ChangeObservedEventArgs e)
        {
            OnChangeObserved(e);
        }

        public TileAssignment TileAssignment
        {
            get => (DataContext as TileControlViewModel).TileAssignment;
            set
            {
                (DataContext as TileControlViewModel).TileAssignment = value;
                value.ChangeObserved += TileAssignment_ChangeObserved;
            }
        }

        public override string ToString()
        {
            return TileAssignment?.ToString();
        }

        protected virtual void OnChangeObserved(ChangeObservedEventArgs e)
        {
            ChangeObserved?.Invoke(this, e);
        }
    }
}
