using DungeonMapEditor.Core.Dialog;
using DungeonMapEditor.Core.Dungeon;
using DungeonMapEditor.ViewModel;
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

namespace DungeonMapEditor.UI
{
    /// <summary>
    /// Interaction logic for TileManager.xaml
    /// </summary>
    public partial class TileManager : Grid
    {
        public event EventHandler<TileManagerDialogButtonClickedEventArgs> DialogButtonClicked;

        public TileManager()
        {
            InitializeComponent();
        }

        private void TileConfigurator_DialogButtonClicked(object sender, TileDialogButtonClickedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            if (e.DialogResult == DialogResult.OK)
            {
                if (!vm.IsEditTile)
                {
                    vm.TileFile.Data.Add(e.Tile);
                }
            }

            vm.IsConfiguratorOpen = false;
        }

        private void AddNewTile_Click(object sender, RoutedEventArgs e)
        {
            ShowConfigurator(false);
        }

        private void EditTile_Click(object sender, RoutedEventArgs e)
        {
            ShowConfigurator(true);
        }

        private void RemoveTile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ShowConfigurator(bool isEdit)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;

            vm.IsConfiguratorOpen = true;
            vm.IsEditTile = isEdit;
            if (isEdit)
            {
                tileConfigurator.SetTile(vm.GetSelectedTile());
            }
            else
            {
                tileConfigurator.SetTile(new Tile());
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            TileManagerViewModel vm = DataContext as TileManagerViewModel;
            string collectionDir = Path.Combine(App.BasePath, "Collections", "Default");
            Directory.CreateDirectory(collectionDir);

            vm.TileFile.Save(collectionDir);

            OnDialogButtonClicked(new TileManagerDialogButtonClickedEventArgs(vm.TileFile, DialogResult.OK));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            OnDialogButtonClicked(new TileManagerDialogButtonClickedEventArgs((DataContext as TileManagerViewModel).TileFile, 
                DialogResult.Abort));
        }

        protected virtual void OnDialogButtonClicked(TileManagerDialogButtonClickedEventArgs e)
        {
            DialogButtonClicked?.Invoke(this, e);
        }
    }
}
