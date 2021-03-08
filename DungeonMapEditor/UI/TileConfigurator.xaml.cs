using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.ViewModel;
using Microsoft.Win32;
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
    /// Interaction logic for TileConfigurator.xaml
    /// </summary>
    public partial class TileConfigurator : DockPanel
    {
        public event EventHandler<TileDialogButtonClickedEventArgs> DialogButtonClicked;

        public TileConfigurator()
        {
            InitializeComponent();
        }

        public void SetTile(Tile tile)
        {
            (DataContext as TileConfiguratorViewModel).Tile = tile;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Title = "Select file",
                Filter = "Image files (*.png, *.jpg)|*.jpg;*.jpeg;*.png"
            };

            if (dialog.ShowDialog() == true)
            {
                (DataContext as TileConfiguratorViewModel).Tile.ImagePath = dialog.FileName;
            }
        }

        protected virtual void OnDialogButtonClicked(TileDialogButtonClickedEventArgs e)
        {
            DialogButtonClicked?.Invoke(this, e);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            OnDialogButtonClicked(new TileDialogButtonClickedEventArgs((DataContext as TileConfiguratorViewModel).Tile, DialogResult.OK));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogButtonClicked(new TileDialogButtonClickedEventArgs((DataContext as TileConfiguratorViewModel).Tile, DialogResult.Abort));
        }
    }
}
