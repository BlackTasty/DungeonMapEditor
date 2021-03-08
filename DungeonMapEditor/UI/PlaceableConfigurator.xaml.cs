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
    /// Interaction logic for PlaceableConfigurator.xaml
    /// </summary>
    public partial class PlaceableConfigurator : DockPanel
    {
        public event EventHandler<PlaceableDialogButtonClickedEventArgs> DialogButtonClicked;

        public PlaceableConfigurator()
        {
            InitializeComponent();
        }

        public void SetPlaceable(Placeable placeable)
        {
            (DataContext as PlaceableConfiguratorViewModel).Placeable = placeable;
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
                (DataContext as PlaceableConfiguratorViewModel).Placeable.ImagePath = dialog.FileName;
            }
        }

        protected virtual void OnDialogButtonClicked(PlaceableDialogButtonClickedEventArgs e)
        {
            DialogButtonClicked?.Invoke(this, e);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            OnDialogButtonClicked(new PlaceableDialogButtonClickedEventArgs((DataContext as PlaceableConfiguratorViewModel).Placeable, 
                DialogResult.OK));
        }

        private void Abort_Click(object sender, RoutedEventArgs e)
        {
            OnDialogButtonClicked(new PlaceableDialogButtonClickedEventArgs((DataContext as PlaceableConfiguratorViewModel).Placeable, 
                DialogResult.Abort));
        }
    }
}
